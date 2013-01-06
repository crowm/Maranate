using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpFFmpeg;

namespace ComskipToCuttermaran
{
    public partial class MediaFile : IDisposable
    {
        string _filename;

        IntPtr _pFormatContext;
        FFmpeg.AVFormatContext _formatContext;

        // Video
        int _videoStreamIndex;
        IntPtr _pVideoStream;
        FFmpeg.AVStream _videoStream;
        IntPtr _pVideoCodecContext;
        FFmpeg.AVCodecContext _videoCodecContext;
        IntPtr _pVideoCodecDecoder;
        IntPtr _pFrameOrig;
        FFmpeg.AVFrame _frameOrig;

        SwsScaler _scalerY;
        SwsScaler _scalerRGB;

        IntPtr _pPacket;
        FFmpeg.AVPacket _lastPacket;

        long? _videoFrameFirstDTS;
        long? _videoFieldDTSDuration;

        PendingFrame _pendingFrame = null;
        int _lastFieldNumber = -1;

        // Audio
        //int _audioStreamIndex;
        //IntPtr _pAudioStream;
        //FFmpeg.AVStream _audioStream;
        //IntPtr _pAudioCodec;
        //FFmpeg.AVCodecContext _audioCodecContext;
        //IntPtr _pAudioCodecDecoder;

        // Settings
        public ResolutionOption Resolution { get; set; }
        public bool OutputRGBImage { get; set; }
        public bool OutputYData { get; set; }
        public bool OutputYImage { get; set; }

        private class PendingFrame : IDisposable
        {
            public FrameField[] Fields { get; set; }
            public int CurrentIndex { get; set; }

            public bool IsKeyFrame
            {
                get
                {
                    return (Fields != null) ? ((CurrentIndex == 0) && (Fields[0].AVFrame.key_frame != 0)) : false;
                }
            }

            public PendingFrame()
            {
                CurrentIndex = 0;
            }

            public void Dispose()
            {
                if (Fields != null)
                {
                    foreach (var field in Fields)
                    {
                        if (field != null)
                        {
                            field.Dispose();
                        }
                    }
                    Fields = null;
                }
            }
        }
        public class FrameField : IDisposable
        {
            public Image Image { get; set; }
            public ImageProcessing.YData YData { get; set; }
            public int FrameNumber { get; set; }
            public int FieldNumber { get; set; }
            public int FieldIndex { get; set; }
            public long PTS { get; set; }
            public double Seconds { get; set; }
            public long FilePosition { get; set; }

            public FFmpeg.AVFrame AVFrame { get; set; }

            public void Dispose()
            {
                if (Image != null)
                {
                    Image.Dispose();
                    Image = null;
                }
            }
        }

        public enum ResolutionOption : int
        {
            Full = 0,
            Half = 1,
            Quarter = 2
        }

        public enum OutputOptions
        {
            RGBImage,
            YImage,
            YData
        }

        public enum SeekModes
        {
            Accurate,
            PreviousKeyFrame,
            NextKeyFrame
        }

        static MediaFile()
        {
            FFmpeg.av_register_all();
        }

        public MediaFile()
        {
            Resolution = ResolutionOption.Full;
            OutputRGBImage = true;
            OutputYData = false;
            OutputYImage = false;
        }

        public MediaFile Clone()
        {
            return Clone(Resolution);
        }
        public MediaFile Clone(ResolutionOption resolution)
        {
            var clone = new MediaFile();
            clone.Resolution = resolution;
            clone.OutputRGBImage = OutputRGBImage;
            clone.OutputYData = OutputYData;
            clone.OutputYImage = OutputYImage;
            clone._lastFieldNumber = _lastFieldNumber;
            clone.Open(_filename);
            return clone;
        }

        public void Dispose()
        {
            Reset();

            //FFmpeg.av_free_static();
        }

        private void Reset()
        {
            if (_pFormatContext != IntPtr.Zero)
            {
                IntPtr ppFormatContext = Marshal.AllocHGlobal(4);
                Marshal.WriteInt32(ppFormatContext, _pFormatContext.ToInt32());
                FFmpeg.avformat_close_input(ppFormatContext);
                Marshal.FreeHGlobal(ppFormatContext);
            }

            DisposeFrame(ref _pendingFrame);

            FreeHGlobal(ref _pPacket);

            if (_scalerY != null)
            {
                _scalerY.Dispose();
                _scalerY = null;
            }
            if (_scalerRGB != null)
            {
                _scalerRGB.Dispose();
                _scalerRGB = null;
            }

            if (_pFrameOrig != IntPtr.Zero)
            {
                FFmpeg.avcodec_free_frame(ref _pFrameOrig);
                _pFrameOrig = IntPtr.Zero;
            }

            if (_pFormatContext != IntPtr.Zero)
            {
                _pFormatContext = IntPtr.Zero;
            }
        }

        private static void DisposeFrame(ref PendingFrame frame)
        {
            if (frame != null)
            {
                frame.Dispose();
                frame = null;
            }
        }
        private static void FreeHGlobal(ref IntPtr p)
        {
            if (p != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(p);
                p = IntPtr.Zero;
            }
        }



        public void Open(string filename)
        {
            int ret;

            Reset();

            _filename = filename;

            var dictionary = new FFmpeg.AVDictionary();
            dictionary.count = 0;
            ret = FFmpeg.avformat_open_input(out _pFormatContext, filename, IntPtr.Zero, dictionary);
            if (ret < 0)
                throw new Exception("Failed to open input file: " + ret.ToString());

            ret = FFmpeg.av_find_stream_info(_pFormatContext);
            if (ret < 0)
                throw new Exception("Failed to find stream information: " + ret.ToString());

            _formatContext = (FFmpeg.AVFormatContext) Marshal.PtrToStructure(_pFormatContext, typeof(FFmpeg.AVFormatContext));

            for (int streamIndex = 0; streamIndex < _formatContext.nb_streams; ++streamIndex)
            {
                //IntPtr pStream = _formatContext.streams[streamIndex];
                IntPtr pStream = Marshal.ReadIntPtr(_formatContext.streams, streamIndex * 4);
                var stream = (FFmpeg.AVStream)Marshal.PtrToStructure(pStream, typeof(FFmpeg.AVStream));
                var codecContext = (FFmpeg.AVCodecContext)Marshal.PtrToStructure(stream.codec, typeof(FFmpeg.AVCodecContext));

                if (codecContext.codec_type == FFmpeg.CodecType.CODEC_TYPE_VIDEO)
                {
                    _videoStreamIndex = streamIndex;
                    _pVideoStream = pStream;
                    _videoStream = stream;
                    _pVideoCodecContext = stream.codec;
                    _videoCodecContext = codecContext;

                    if (Resolution != ResolutionOption.Full)
                    {
                        Marshal.WriteInt32(_pVideoCodecContext, Marshal.OffsetOf(typeof(FFmpeg.AVCodecContext), "lowres").ToInt32(), (int)Resolution);
                    }

                    _pVideoCodecDecoder = FFmpeg.avcodec_find_decoder(_videoCodecContext.codec_id);
                    if (_pVideoCodecDecoder == IntPtr.Zero)
                        throw new Exception("Failed to find decoder");

                    ret = FFmpeg.avcodec_open(stream.codec, _pVideoCodecDecoder);
                    if (ret < 0)
                        throw new Exception("Failed to open codec: " + ret.ToString());

                    _videoCodecContext = (FFmpeg.AVCodecContext)Marshal.PtrToStructure(stream.codec, typeof(FFmpeg.AVCodecContext));

                    //Allocate buffers for original frame
                    _pFrameOrig = FFmpeg.avcodec_alloc_frame();

                    //Allocate buffers for RGB frame
                    _scalerY = new SwsScaler(_videoCodecContext);
                    _scalerY.DstPixelFormat = SwsScaler.PixelFormat.Y;

                    //Allocate buffers for RGB frame
                    _scalerRGB = new SwsScaler(_videoCodecContext);

                    // Allocate packet memory
                    int sizeOfPacket = Marshal.SizeOf(typeof(FFmpeg.AVPacket));
                    _pPacket = Marshal.AllocHGlobal(sizeOfPacket);
                    RtlZeroMemory(_pPacket, sizeOfPacket);

                }
                //else if (codecContext.codec_type == FFmpeg.CodecType.CODEC_TYPE_AUDIO)
                //{
                //    _audioStreamIndex = i;
                //    _pAudioStream = _formatContext.streams[i];
                //    _audioStream = stream;
                //    _pAudioCodec = stream.codec;
                //    _audioCodecContext = codecContext;
                //}
            }

            // Seek to the start of the file to reset dts values.
            ret = FFmpeg.avformat_seek_file(_pFormatContext, _videoStreamIndex, 0, 0, Int64.MaxValue, FFmpeg.AVSEEK_FLAG.AVSEEK_FLAG_NONE);
            if (ret < 0)
                throw new Exception("Failed to seek to first frame: " + ret.ToString());
            FFmpeg.avcodec_flush_buffers(_pVideoCodecContext);

            // Read the first frame to set initial dts values
            ReadVideoFrame();

            if (_lastFieldNumber == -1)
            {
                if (_filename == @"F:\Convert\ComskipTest\Chuck - 4x02 - Retune\(2012-09-10 03-55) Chuck - 4x02 - Chuck Versus the Suitcase.m2v")
                {
                    _lastFieldNumber = 160295;
                }
                else
                {
                    SeekToPTS(Int64.MaxValue, false);
                    _lastFieldNumber = _pendingFrame.Fields.Last().FieldNumber;
                }
            }

            SeekToPTS(0, false);
            return;
        }

        public int Width
        {
            get { return _videoCodecContext.width; }
        }

        public int Height
        {
            get { return _videoCodecContext.height; }
        }

        public int FieldsPerFrame
        {
            get
            {
                return _videoCodecContext.ticks_per_frame;
            }
        }

        public int FieldsPerSecond
        {
            get
            {
                var fields = (int)(_videoCodecContext.time_base.den / (double)_videoCodecContext.time_base.num);
                fields -= (fields % _videoCodecContext.ticks_per_frame);
                return fields;
            }
        }

        public int TotalFields
        {
            get
            {
                return _lastFieldNumber + 1;
            }
        }

        public double FieldDuration
        {
            get
            {
                return _videoFieldDTSDuration.Value * _videoCodecContext.pkt_timebase.num / (double)_videoCodecContext.pkt_timebase.den;
            }
        }

        private long PTSPerField
        {
            get
            {
                return (_videoCodecContext.pkt_timebase.den * _videoCodecContext.pkt_timebase.num) / (_videoCodecContext.time_base.den * _videoCodecContext.time_base.num);
            }
        }

        private void SeekToField(int fieldNumber, SeekModes seekMode)
        {
            if ((_pendingFrame != null) && (_pendingFrame.Fields != null))
            {
                if ((seekMode == SeekModes.Accurate) || (_pendingFrame.IsKeyFrame))
                {
                    for (int fieldIndex = 0; fieldIndex < _pendingFrame.Fields.Length; fieldIndex++)
                    {
                        var field = _pendingFrame.Fields[fieldIndex];
                        if ((field.FieldNumber == fieldNumber))
                        {
                            _pendingFrame.CurrentIndex = fieldIndex;
                            return;
                        }
                    }
                }
            }

            long pts = PTSPerField * fieldNumber;

            SeekToPTS(pts, seekMode);
        }

        private void SeekToTime(double seconds, SeekModes seekMode)
        {
            if ((_pendingFrame != null) && (_pendingFrame.Fields != null))
            {
                if (seekMode == SeekModes.Accurate)
                {
                    for (int fieldIndex = 0; fieldIndex < _pendingFrame.Fields.Length; fieldIndex++)
                    {
                        var field = _pendingFrame.Fields[fieldIndex];
                        if ((field.Seconds == seconds))
                        {
                            _pendingFrame.CurrentIndex = fieldIndex;
                            return;
                        }
                    }
                }
                else
                {
                    if (_pendingFrame.IsKeyFrame)
                        return;
                }
            }

            long pts = (long)(seconds * _videoCodecContext.pkt_timebase.den / (double)_videoCodecContext.pkt_timebase.num);

            SeekToPTS(pts, seekMode);
        }

        private void SeekToPTS(long pts, SeekModes seekMode)
        {
            if (seekMode == SeekModes.PreviousKeyFrame)
            {
                SeekToPTS(pts, true);
            }
            else if (seekMode == SeekModes.NextKeyFrame)
            {
                SeekToPTS(pts, false);
                while (!_pendingFrame.IsKeyFrame)
                {
                    var previousFrame = _pendingFrame;
                    _pendingFrame = ReadVideoFrame();
                    if (_pendingFrame == null)
                    {
                        _pendingFrame = previousFrame;
                        return;
                    }
                    DisposeFrame(ref previousFrame);
                }
            }
            else
            {
                SeekToPTS(pts, false);
            }
        }

        private void SeekToPTS(long pts, bool stopAtKeyframe, bool forceSeek = false)
        {
            if (forceSeek == false)
            {
                if ((_pendingFrame != null) && (_pendingFrame.Fields != null))
                {
                    for (int fieldIndex = 0; fieldIndex < _pendingFrame.Fields.Length; fieldIndex++)
                    {
                        var field = _pendingFrame.Fields[fieldIndex];
                        if ((field.PTS == pts))
                        {
                            _pendingFrame.CurrentIndex = fieldIndex;
                            return;
                        }
                    }
                }
            }

            long seekPTS = pts;
            if (seekPTS < long.MaxValue)
            {
                seekPTS = seekPTS / PTSPerField / _videoCodecContext.ticks_per_frame * _videoCodecContext.ticks_per_frame * PTSPerField;
                if (long.MaxValue - seekPTS > _videoFrameFirstDTS.Value)
                    seekPTS += _videoFrameFirstDTS.Value;
            }

            var originalPendingFrame = _pendingFrame;
            long lastSeekFilePosition = -1L;

            while (true)
            {
                if (seekPTS < 0)
                    seekPTS = 0;

                int ret = FFmpeg.avformat_seek_file(_pFormatContext, _videoStreamIndex, 0, seekPTS, seekPTS, FFmpeg.AVSEEK_FLAG.AVSEEK_FLAG_NONE);
                if (ret < 0)
                    throw new Exception("Failed to seek to first frame: " + ret.ToString());
                FFmpeg.avcodec_flush_buffers(_pVideoCodecContext);

                var pb = (FFmpeg.ByteIOContext)Marshal.PtrToStructure(_formatContext.pb, typeof(FFmpeg.ByteIOContext));
                if (lastSeekFilePosition != -1L)
                {
                    if ((lastSeekFilePosition == pb.pos) && (seekPTS != 0))
                    {
                        seekPTS -= (PTSPerField * _videoCodecContext.ticks_per_frame);
                        continue;
                    }
                }
                lastSeekFilePosition = pb.pos;

                if (_pendingFrame != originalPendingFrame)
                    DisposeFrame(ref _pendingFrame);

                _pendingFrame = ReadVideoFrame();

                if (_pendingFrame != null)
                {
                    if (_pendingFrame.Fields.First().PTS == pts)
                        return;

                    if (_pendingFrame.Fields.First().PTS > pts)
                    {
                        if (seekPTS == 0)
                            return;

                        seekPTS -= PTSPerField * _videoCodecContext.ticks_per_frame;

                        continue;
                    }

                    while (true)
                    {
                        if (stopAtKeyframe && _pendingFrame.IsKeyFrame)
                        {
                            _pendingFrame.CurrentIndex = 0;
                            return;
                        }

                        for (int fieldIndex = 0; fieldIndex < _pendingFrame.Fields.Length; fieldIndex++)
                        {
                            var field = _pendingFrame.Fields[fieldIndex];

                            if (field.PTS >= pts)
                            {
                                _pendingFrame.CurrentIndex = fieldIndex;
                                return;
                            }
                        }

                        var previousFrame = _pendingFrame;
                        _pendingFrame = ReadVideoFrame();
                        if (_pendingFrame == null)
                        {
                            _pendingFrame = previousFrame;
                            _pendingFrame.CurrentIndex = _pendingFrame.Fields.Length - 1;
                            return;
                        }
                        if (previousFrame != originalPendingFrame)
                            DisposeFrame(ref previousFrame);
                    }
                }

            }
        }

        public void TestSeekToField(long fieldNumber)
        {
            long pts = fieldNumber * PTSPerField;

            SeekToPTS(pts, true, true);
            
            if (_pendingFrame != null)
            {
                var diff = _pendingFrame.Fields[0].FieldNumber - fieldNumber;
                System.Diagnostics.Debug.WriteLine(fieldNumber.ToString() + ", " + _pendingFrame.Fields[0].FieldNumber.ToString() + ", " + diff.ToString() + ((diff > 0) ? " ++++++" : ""));
            }
        }

        private PendingFrame ReadVideoFrame()
        {
            int ret;

            while (true)
            {
                ret = FFmpeg.av_read_frame(_pFormatContext, _pPacket);
                if (ret < 0)
                {
                    var packet = new FFmpeg.AVPacket();
                    packet.dts = _lastPacket.dts + PTSPerField * _videoCodecContext.ticks_per_frame;
                    packet.pts = _lastPacket.pts + PTSPerField * _videoCodecContext.ticks_per_frame;
                    packet.duration = _lastPacket.duration;
                    int sizeOfPacket = Marshal.SizeOf(packet);
                    var pPacket = Marshal.AllocHGlobal(sizeOfPacket);
                    RtlZeroMemory(pPacket, sizeOfPacket);
                    Marshal.StructureToPtr(packet, pPacket, true);
                    int frameFinished = 0;
                    ret = FFmpeg.avcodec_decode_video2(_pVideoCodecContext, _pFrameOrig, ref frameFinished, pPacket);
                    if (frameFinished != 0)
                    {
                        return ProcessFrame();
                    }
                    break;
                }

                _formatContext = (FFmpeg.AVFormatContext)Marshal.PtrToStructure(_pFormatContext, typeof(FFmpeg.AVFormatContext));

                _lastPacket = (FFmpeg.AVPacket)Marshal.PtrToStructure(_pPacket, typeof(FFmpeg.AVPacket));

                if (_lastPacket.stream_index == _videoStreamIndex)
                {
                    // Decode the video frame
                    int frameFinished = 0;
                    ret = FFmpeg.avcodec_decode_video2(_pVideoCodecContext, _pFrameOrig, ref frameFinished, _pPacket);

                    FFmpeg.av_free_packet(_pPacket);

                    if (frameFinished != 0)
                    {
                        return ProcessFrame();
                    }
                }
            }

            return null;
        }

        private PendingFrame ProcessFrame()
        {
            _frameOrig = (FFmpeg.AVFrame)Marshal.PtrToStructure(_pFrameOrig, typeof(FFmpeg.AVFrame));

            IntPtr pStream = Marshal.ReadIntPtr(_formatContext.streams, _videoStreamIndex * 4);
            var stream = (FFmpeg.AVStream)Marshal.PtrToStructure(pStream, typeof(FFmpeg.AVStream));
            _videoCodecContext = (FFmpeg.AVCodecContext)Marshal.PtrToStructure(stream.codec, typeof(FFmpeg.AVCodecContext));

            if (_videoFrameFirstDTS == null)
                _videoFrameFirstDTS = _frameOrig.pkt_dts;
            if (_videoFieldDTSDuration == null)
                _videoFieldDTSDuration = _frameOrig.pkt_duration / _videoCodecContext.ticks_per_frame;

            var fieldList = new List<FrameField>();

            //---------- Start YUV Image ----------
            if (OutputYData || OutputYImage)
            {
                _scalerY.ProcessImage(_pFrameOrig, _frameOrig);
            }
            //---------- End YUV Image ----------

            //---------- Start RGB Image ----------
            if (OutputRGBImage && !OutputYImage)
            {
                _scalerRGB.ProcessImage(_pFrameOrig, _frameOrig);
            }
            //---------- End RGB Image ----------

            for (int fieldIndex = 0; fieldIndex < _videoCodecContext.ticks_per_frame; fieldIndex++)
            {
                var field = new FrameField();

                if (OutputYData || OutputYImage)
                {
                    field.YData = _scalerY.GetYData(fieldIndex);
                    if (OutputYImage)
                    {
                        field.Image = _scalerY.GetImage(fieldIndex);
                        //field.Image.Save(@"D:\temp\image.png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                
                if (OutputRGBImage && !OutputYImage)
                {
                    field.Image = _scalerRGB.GetImage(fieldIndex);
                    //field.Image.Save(@"D:\temp\image.png", System.Drawing.Imaging.ImageFormat.Png);
                }

                field.PTS = (_frameOrig.pkt_dts - _videoFrameFirstDTS.Value) + (fieldIndex * _videoFieldDTSDuration.Value);
                field.Seconds = field.PTS * _videoCodecContext.pkt_timebase.num / (double)_videoCodecContext.pkt_timebase.den;
                field.FieldIndex = fieldIndex;
                field.FieldNumber = (int)(field.PTS / _videoFieldDTSDuration.Value);
                field.FrameNumber = (int)(field.FieldNumber / _videoCodecContext.ticks_per_frame);
                field.FilePosition = _frameOrig.pkt_pos;
                field.AVFrame = _frameOrig;

                fieldList.Add(field);
            }

            var frame = new PendingFrame();
            frame.Fields = fieldList.ToArray();

            return frame;
        }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

        [DllImport("kernel32.dll")]
        static extern void RtlZeroMemory(IntPtr dst, int length);

        public FrameField GetVideoFrameField(int fieldNumber, SeekModes seekMode = SeekModes.Accurate)
        {
            FrameField frame = null;
            SeekToField(fieldNumber, seekMode);
            if (_pendingFrame != null)
            {
                frame = _pendingFrame.Fields[_pendingFrame.CurrentIndex];
                //frame.Image.Save(@"D:\temp\image-" + frame.FrameNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            _pendingFrame.CurrentIndex++;
            if (_pendingFrame.CurrentIndex >= _pendingFrame.Fields.Length)
                _pendingFrame = ReadVideoFrame();
            return frame;
        }

        public List<FrameField> GetVideoFrameFields(List<int> fieldNumbers)
        {
            var result = new List<FrameField>();

            fieldNumbers = (from f in fieldNumbers orderby f select f).ToList();
            foreach (var frameNumber in fieldNumbers)
            {
                if ((frameNumber < 0) || (frameNumber >= TotalFields))
                {
                    result.Add(null);
                    continue;
                }

                SeekToField(frameNumber, SeekModes.Accurate);
                if (_pendingFrame != null)
                {
                    FrameField frame = _pendingFrame.Fields[_pendingFrame.CurrentIndex];
                    //frame.Image.Save(@"D:\temp\image-" + frame.FrameNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    result.Add(frame);
                }
                else
                {
                    result.Add(null);
                }
                _pendingFrame.CurrentIndex++;
                if (_pendingFrame.CurrentIndex >= _pendingFrame.Fields.Length)
                    _pendingFrame = ReadVideoFrame();
            }

            return result;
        }

        public List<FrameField> GetVideoFrameFields(int firstFieldNumber, int lastFieldNumber)
        {
            var frameNumbers = new List<int>();
            for (int frameNumber = firstFieldNumber; frameNumber <= lastFieldNumber; frameNumber++)
            {
                frameNumbers.Add(frameNumber);
            }

            return GetVideoFrameFields(frameNumbers);
        }


    }
}
