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
    public class MediaFile : IDisposable
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
        IntPtr _pFrameRGB;
        FFmpeg.AVFrame _frameOrig;
        FFmpeg.AVFrame _frameRGB;

        const int AV_NUM_DATA_POINTERS = 8;
        IntPtr _pScaleSrc;
        IntPtr _pScaleSrcStride;
        IntPtr _pScaleDst;
        IntPtr _pScaleDstStride;
        IntPtr _pSwsContext = IntPtr.Zero;

        int _frameSizeBytesRGB;
        IntPtr _pFrameBytes;
        IntPtr _pPacket;
        FFmpeg.AVPacket _lastPacket;

        long? _videoFrameFirstDTS;
        long? _videoFrameDTSDuration;

        Frame _firstFrame = null;
        Frame _pendingFrame = null;
        int _lastFrameNumber = -1;

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

        public class Frame : IDisposable
        {
            public Image Image { get; set; }
            public byte[] YData { get; set; }
            public int FrameNumber { get; set; }
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
            clone._lastFrameNumber = _lastFrameNumber;
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

            DisposeFrame(ref _firstFrame);
            DisposeFrame(ref _pendingFrame);

            FreeHGlobal(ref _pScaleSrc);
            FreeHGlobal(ref _pScaleSrcStride);
            FreeHGlobal(ref _pScaleDst);
            FreeHGlobal(ref _pScaleDstStride);

            FreeHGlobal(ref _pFrameBytes);
            FreeHGlobal(ref _pPacket);

            if (_pSwsContext != IntPtr.Zero)
            {
                FFmpeg.SwScale.sws_freeContext(_pSwsContext);
                _pSwsContext = IntPtr.Zero;
            }
            if (_pFrameOrig != IntPtr.Zero)
            {
                FFmpeg.avcodec_free_frame(ref _pFrameOrig);
                _pFrameOrig = IntPtr.Zero;
            }
            if (_pFrameRGB != IntPtr.Zero)
            {
                FFmpeg.avcodec_free_frame(ref _pFrameRGB);
                _pFrameRGB = IntPtr.Zero;
            }

            if (_pFormatContext != IntPtr.Zero)
            {
                _pFormatContext = IntPtr.Zero;
            }
        }

        private void DisposeFrame(ref Frame frame)
        {
            if (frame != null)
            {
                frame.Dispose();
                frame = null;
            }
        }
        private void FreeHGlobal(ref IntPtr p)
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
                        var offset = Marshal.OffsetOf(typeof(FFmpeg.AVCodecContext), "lowres");
                        Marshal.WriteInt32(_pVideoCodecContext, offset.ToInt32(), (int)Resolution);
                    }

                    _pVideoCodecDecoder = FFmpeg.avcodec_find_decoder(_videoCodecContext.codec_id);
                    if (_pVideoCodecDecoder == IntPtr.Zero)
                        throw new Exception("Failed to find decoder");

                    ret = FFmpeg.avcodec_open(stream.codec, _pVideoCodecDecoder);
                    if (ret < 0)
                        throw new Exception("Failed to open codec: " + ret.ToString());

                    _videoCodecContext = (FFmpeg.AVCodecContext)Marshal.PtrToStructure(stream.codec, typeof(FFmpeg.AVCodecContext));

                    //Allocate buffers
                    _frameSizeBytesRGB = FFmpeg.avpicture_get_size((int)FFmpeg.AVPixelFormat.PIX_FMT_BGR24, _videoCodecContext.width, _videoCodecContext.height);
                    _pFrameOrig = FFmpeg.avcodec_alloc_frame();
                    _pFrameRGB = FFmpeg.avcodec_alloc_frame();

                    _pFrameBytes = Marshal.AllocHGlobal(_frameSizeBytesRGB);
                    RtlZeroMemory(_pFrameBytes, _frameSizeBytesRGB);
                    ret = FFmpeg.avpicture_fill(_pFrameRGB, _pFrameBytes, FFmpeg.AVPixelFormat.PIX_FMT_BGR24, _videoCodecContext.width, _videoCodecContext.height);
                    if (ret < 0)
                        throw new Exception("Failed to fill picture: " + ret.ToString());
                    _frameRGB = (FFmpeg.AVFrame)Marshal.PtrToStructure(_pFrameRGB, typeof(FFmpeg.AVFrame));
                    
                    int sizeOfPacket = Marshal.SizeOf(typeof(FFmpeg.AVPacket));
                    _pPacket = Marshal.AllocHGlobal(sizeOfPacket);
                    RtlZeroMemory(_pPacket, sizeOfPacket);

                    int avDataPointersBytes = AV_NUM_DATA_POINTERS * 4;
                    _pScaleSrc = Marshal.AllocHGlobal(avDataPointersBytes);
                    _pScaleSrcStride = Marshal.AllocHGlobal(avDataPointersBytes);
                    _pScaleDst = Marshal.AllocHGlobal(avDataPointersBytes);
                    _pScaleDstStride = Marshal.AllocHGlobal(avDataPointersBytes);
                    RtlZeroMemory(_pScaleSrc, avDataPointersBytes);
                    RtlZeroMemory(_pScaleSrcStride, avDataPointersBytes);
                    RtlZeroMemory(_pScaleDst, avDataPointersBytes);
                    RtlZeroMemory(_pScaleDstStride, avDataPointersBytes);

                    Marshal.WriteIntPtr(_pScaleDst, 0, _pFrameBytes);

                    for (int i = 0; i < AV_NUM_DATA_POINTERS; i++)
                    {
                        Marshal.WriteInt32(_pScaleDstStride, 4 * i, _frameRGB.linesize[i]);
                    }

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

            _firstFrame = ReadVideoFrame();
            _pendingFrame = null;

            if (_lastFrameNumber == -1)
            {
                if (_filename == @"F:\Convert\ComskipTest\Chuck - 4x02 - Retune\(2012-09-10 03-55) Chuck - 4x02 - Chuck Versus the Suitcase.m2v")
                {
                    _lastFrameNumber = 80148;
                }
                else
                {
                    SeekToPTS(Int64.MaxValue, false);
                    _lastFrameNumber = _pendingFrame.FrameNumber;
                }
            }

            SeekToPTS(0, false);
            return;
        }

        public int TotalFrames
        {
            get
            {
                return _lastFrameNumber;
            }
        }

        public double FrameDuration
        {
            get
            {
                return _videoFrameDTSDuration.Value * _videoCodecContext.pkt_timebase.num / (double)_videoCodecContext.pkt_timebase.den;
            }
        }

        private long PTSPerFrame
        {
            get
            {
                return (_videoCodecContext.pkt_timebase.den * _videoCodecContext.pkt_timebase.num * _videoCodecContext.ticks_per_frame) / (_videoCodecContext.time_base.den * _videoCodecContext.time_base.num);
            }
        }

        private void SeekToFrame(int frameNumber, SeekModes seekMode)
        {
            if ((_pendingFrame != null) && (_pendingFrame.FrameNumber == frameNumber))
            {
                if ((seekMode == SeekModes.Accurate) || (_pendingFrame.AVFrame.key_frame != 0))
                    return;
            }

            long pts = PTSPerFrame * frameNumber;

            SeekToPTS(pts, seekMode);
        }

        private void SeekToTime(double seconds, SeekModes seekMode)
        {
            if ((_pendingFrame != null) && (_pendingFrame.Seconds == seconds))
            {
                if ((seekMode == SeekModes.Accurate) || (_pendingFrame.AVFrame.key_frame != 0))
                    return;
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
                while (_pendingFrame.AVFrame.key_frame == 0)
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

        private void SeekToPTS(long pts, bool stopAtKeyframe)
        {
            if ((_pendingFrame != null) && (_pendingFrame.PTS == pts))
                return;

            long seekPTS = pts;

            while (true)
            {
                if (seekPTS < 0)
                    seekPTS = 0;

                int ret = FFmpeg.avformat_seek_file(_pFormatContext, _videoStreamIndex, 0, seekPTS, seekPTS, FFmpeg.AVSEEK_FLAG.AVSEEK_FLAG_NONE);
                if (ret < 0)
                    throw new Exception("Failed to seek to first frame: " + ret.ToString());
                FFmpeg.avcodec_flush_buffers(_pVideoCodecContext);

                DisposeFrame(ref _pendingFrame);

                _pendingFrame = ReadVideoFrame();

                if ((_pendingFrame == null) || (_pendingFrame.PTS <= pts))
                {
                    while (_pendingFrame.PTS < pts)
                    {
                        if (stopAtKeyframe && (_pendingFrame.AVFrame.key_frame != 0))
                            break;

                        var previousFrame = _pendingFrame;
                        _pendingFrame = ReadVideoFrame();
                        if (_pendingFrame == null)
                        {
                            _pendingFrame = previousFrame;
                            return;
                        }
                        DisposeFrame(ref previousFrame);
                    }
                    break;
                }

                if (seekPTS == 0)
                    break;

                long gopPTS = PTSPerFrame * _videoCodecContext.gop_size;
                seekPTS -= gopPTS;
            }
        }

        private Frame ReadVideoFrame()
        {
            int ret;

            while (true)
            {
                ret = FFmpeg.av_read_frame(_pFormatContext, _pPacket);
                if (ret < 0)
                {
                    var packet = new FFmpeg.AVPacket();
                    packet.dts = _lastPacket.dts + PTSPerFrame;
                    packet.pts = _lastPacket.pts + PTSPerFrame;
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

        private Frame ProcessFrame()
        {
            int ret;

            _frameOrig = (FFmpeg.AVFrame)Marshal.PtrToStructure(_pFrameOrig, typeof(FFmpeg.AVFrame));

            IntPtr pStream = Marshal.ReadIntPtr(_formatContext.streams, _videoStreamIndex * 4);
            var stream = (FFmpeg.AVStream)Marshal.PtrToStructure(pStream, typeof(FFmpeg.AVStream));
            _videoCodecContext = (FFmpeg.AVCodecContext)Marshal.PtrToStructure(stream.codec, typeof(FFmpeg.AVCodecContext));

            byte[] yData = null;
            Bitmap image = null;

            //---------- Start Save YUV Image ----------
            if (OutputYData || OutputYImage)
            {
                var YBytes = (_frameOrig.linesize[0]) * _frameOrig.height;
                yData = new byte[YBytes];
                Marshal.Copy(_frameOrig.data[0], yData, 0, YBytes);

                if (OutputYImage)
                {
                    var bitmapDataSize = _frameOrig.width * _frameOrig.height / 2;
                    var bitmapData = new long[bitmapDataSize];

                    for (int y = 0; y < _frameOrig.height; y++)
                    {
                        for (int x = 0; x < _frameOrig.width; x+=2)
                        {
                            int i = x + (y * _frameOrig.linesize[0]);
                            byte color1 = yData[i];
                            byte color2 = yData[i + 1];

                            int dstIndex = (x + (y * _frameOrig.width)) / 2;
                            bitmapData[dstIndex] = (long)color1 |
                                                  ((long)color1 << 8) |
                                                  ((long)color1 << 16) |
                                                          (255L << 24) |
                                                  ((long)color2 << 32) |
                                                  ((long)color2 << 40) |
                                                  ((long)color2 << 48) |
                                                          (255L << 56);
                        }
                    }

                    image = new System.Drawing.Bitmap(_videoCodecContext.width, _videoCodecContext.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, _videoCodecContext.width, _videoCodecContext.height), System.Drawing.Imaging.ImageLockMode.ReadWrite, image.PixelFormat);
                    Marshal.Copy(bitmapData, 0, imageData.Scan0, bitmapDataSize);
                    image.UnlockBits(imageData);
                }
            }
            //---------- End Save YUV Image ----------

            //---------- Start Convert to RGB ----------
            if (OutputRGBImage && !OutputYImage)
            {
                if (_pSwsContext == IntPtr.Zero)
                {
                    _pSwsContext = FFmpeg.SwScale.sws_getContext(
                        _videoCodecContext.width, _videoCodecContext.height, _videoCodecContext.pix_fmt,
                        _videoCodecContext.width, _videoCodecContext.height, FFmpeg.AVPixelFormat.PIX_FMT_BGR24,
                        FFmpeg.SwScale.SWS_FLAGS.SWS_FAST_BILINEAR, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                    if (_pSwsContext == IntPtr.Zero)
                        throw new Exception("Failed to create SwScale context");
                }

                //FFmpeg.av_log_set_callback(new FFmpeg.AVLogCallback(AVLogCallback));

                for (int i = 0; i < AV_NUM_DATA_POINTERS; i++)
                {
                    Marshal.WriteIntPtr(_pScaleSrc, 4 * i, _frameOrig.data[i]);
                }

                for (int i = 0; i < AV_NUM_DATA_POINTERS; i++)
                {
                    Marshal.WriteInt32(_pScaleSrcStride, 4 * i, _frameOrig.linesize[i]);
                }

                ret = FFmpeg.SwScale.sws_scale(_pSwsContext, _pScaleSrc, _pScaleSrcStride, 0, _videoCodecContext.height, _pScaleDst, _pScaleDstStride);
                if (ret < 0)
                    throw new Exception("Failed to scale frame: " + ret.ToString());

                if (image != null)
                    image.Dispose();
                image = new System.Drawing.Bitmap(_videoCodecContext.width, _videoCodecContext.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, _videoCodecContext.width, _videoCodecContext.height), System.Drawing.Imaging.ImageLockMode.WriteOnly, image.PixelFormat);
                memcpy(imageData.Scan0, _pFrameBytes, _frameSizeBytesRGB);
                image.UnlockBits(imageData);
            }

            if (_videoFrameFirstDTS == null)
                _videoFrameFirstDTS = _frameOrig.pkt_dts;
            if (_videoFrameDTSDuration == null)
                _videoFrameDTSDuration = _frameOrig.pkt_duration;

            var frame = new Frame();
            frame.Image = image;
            //frame.Image.Save(@"D:\temp\image.png", System.Drawing.Imaging.ImageFormat.Png);
            frame.YData = yData;
            frame.PTS = (_frameOrig.pkt_dts - _videoFrameFirstDTS.Value);
            frame.Seconds = frame.PTS * _videoCodecContext.pkt_timebase.num / (double)_videoCodecContext.pkt_timebase.den;
            frame.FrameNumber = (int)(frame.PTS / _videoFrameDTSDuration.Value);
            frame.FilePosition = _frameOrig.pkt_pos;
            frame.AVFrame = _frameOrig;

            return frame;
        }

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, int count);

        [DllImport("kernel32.dll")]
        static extern void RtlZeroMemory(IntPtr dst, int length);

        public Frame GetVideoFrame(int frameNumber, SeekModes seekMode = SeekModes.Accurate)
        {
            Frame frame = null;
            SeekToFrame(frameNumber, seekMode);
            if (_pendingFrame != null)
            {
                frame = _pendingFrame;
                //frame.Image.Save(@"D:\temp\image-" + frame.FrameNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);
            }
            _pendingFrame = ReadVideoFrame();
            return frame;
        }

        public List<Frame> GetVideoFrames(List<int> frameNumbers)
        {
            var result = new List<Frame>();

            frameNumbers = (from f in frameNumbers orderby f select f).ToList();
            foreach (var frameNumber in frameNumbers)
            {
                if ((frameNumber < 0) || (frameNumber >= TotalFrames))
                {
                    result.Add(null);
                    continue;
                }

                SeekToFrame(frameNumber, SeekModes.Accurate);
                if (_pendingFrame != null)
                {
                    Frame frame = _pendingFrame;
                    //frame.Image.Save(@"D:\temp\image-" + frame.FrameNumber.ToString("00000") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    result.Add(frame);
                }
                else
                {
                    result.Add(null);
                }
                _pendingFrame = ReadVideoFrame();
            }

            return result;
        }

        public List<Frame> GetVideoFrames(int firstFrameNumber, int lastFrameNumber)
        {
            var frameNumbers = new List<int>();
            for (int frameNumber = firstFrameNumber; frameNumber <= lastFrameNumber; frameNumber++)
            {
                frameNumbers.Add(frameNumber);
            }

            return GetVideoFrames(frameNumbers);
        }

        [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        static extern int sprintf(StringBuilder buffer, string format, IntPtr args);

        private static void AVLogCallback(IntPtr avcl, int level, string format, params string[] args)
        {
            Console.WriteLine(format);
        }


    }
}
