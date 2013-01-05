using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SharpFFmpeg;
using System.Drawing;
using System.Threading.Tasks;

namespace ComskipToCuttermaran
{
    public partial class MediaFile : IDisposable
    {
        public class SwsScaler : IDisposable
        {
            IntPtr _pSwsContext = IntPtr.Zero;

            // Source
            FFmpeg.AVCodecContext _srcCodecContext;
            IntPtr _pSrcFrameInfo;
            FFmpeg.AVFrame _srcFrameInfo;

            // Destination
            int _dstFrameBytesLength;
            IntPtr _pDstFrameBytes;

            IntPtr _pDstFrameInfo;
            FFmpeg.AVFrame _dstFrameInfo;

            Size _dstSize;
            PixelFormat _dstPixelFormat;

            Dictionary<int, byte[]> _yDataBytes = new Dictionary<int, byte[]>();

            // Marshal Offsets for AVFrame
            int _marshalOffset_AVFrame_width;
            int _marshalOffset_AVFrame_height;
            int _marshalOffset_AVFrame_interlaced_frame;
            int _marshalOffset_AVFrame_data;
            int _marshalOffset_AVFrame_linesize;

            // Public properties
            public PixelFormat DstPixelFormat
            {
                get { return _dstPixelFormat; }
                set
                {
                    _dstPixelFormat = value;
                    Reset();
                }
            }
            public Size DstSize
            {
                get { return _dstSize; }
                set
                {
                    _dstSize = value;
                    Reset();
                }
            }
            public FFmpeg.SwScale.SWS_FLAGS ResizeMethod { get; set; }

            public enum PixelFormat : int
            {
                RGB24 = 0,
                Y = 1,
            }
            FFmpeg.AVPixelFormat[] _dstAVPixelFormat =
            {
                FFmpeg.AVPixelFormat.PIX_FMT_BGR24,
                FFmpeg.AVPixelFormat.PIX_FMT_YUV420P
            };

            public SwsScaler(FFmpeg.AVCodecContext srcCodecContext)
            {
                ResizeMethod = FFmpeg.SwScale.SWS_FLAGS.SWS_FAST_BILINEAR;

                _srcCodecContext = srcCodecContext;
                DstSize = new Size(srcCodecContext.width, srcCodecContext.height);
                DstPixelFormat = PixelFormat.RGB24;

                _marshalOffset_AVFrame_width = Marshal.OffsetOf(typeof(FFmpeg.AVFrame), "width").ToInt32();
                _marshalOffset_AVFrame_height = Marshal.OffsetOf(typeof(FFmpeg.AVFrame), "height").ToInt32();
                _marshalOffset_AVFrame_interlaced_frame = Marshal.OffsetOf(typeof(FFmpeg.AVFrame), "interlaced_frame").ToInt32();
                _marshalOffset_AVFrame_data = Marshal.OffsetOf(typeof(FFmpeg.AVFrame), "data").ToInt32();
                _marshalOffset_AVFrame_linesize = Marshal.OffsetOf(typeof(FFmpeg.AVFrame), "linesize").ToInt32();
            }

            public void Dispose()
            {
                Reset();
            }

            public void Reset()
            {
                FreeHGlobal(ref _pDstFrameBytes);

                if (_pSwsContext != IntPtr.Zero)
                {
                    FFmpeg.SwScale.sws_freeContext(_pSwsContext);
                    _pSwsContext = IntPtr.Zero;
                }

                if (_pDstFrameInfo != IntPtr.Zero)
                {
                    FFmpeg.avcodec_free_frame(ref _pDstFrameInfo);
                    _pDstFrameInfo = IntPtr.Zero;
                }
            }

            public void ProcessImage(IntPtr pSrcFrameInfo, FFmpeg.AVFrame srcFrameInfo)
            {
                int ret;

                _yDataBytes.Clear();
                _pSrcFrameInfo = pSrcFrameInfo;
                _srcFrameInfo = srcFrameInfo;

                FFmpeg.AVPixelFormat dstPixelFormat = _dstAVPixelFormat[(int)DstPixelFormat];
                
                if (_srcCodecContext.pix_fmt == dstPixelFormat)
                    return;

                if (_pSwsContext == IntPtr.Zero)
                {
                    _pSwsContext = FFmpeg.SwScale.sws_getContext(
                        _srcFrameInfo.width, _srcFrameInfo.height, _srcCodecContext.pix_fmt,
                        DstSize.Width, DstSize.Height, dstPixelFormat,
                        ResizeMethod, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

                    if (_pSwsContext == IntPtr.Zero)
                        throw new Exception("Failed to create SwScale context");


                    _dstFrameBytesLength = FFmpeg.avpicture_get_size((int)dstPixelFormat, DstSize.Width, DstSize.Height);
                    _pDstFrameBytes = Marshal.AllocHGlobal(_dstFrameBytesLength);
                    RtlZeroMemory(_pDstFrameBytes, _dstFrameBytesLength);

                    _pDstFrameInfo = FFmpeg.avcodec_alloc_frame();
                    ret = FFmpeg.avpicture_fill(_pDstFrameInfo, _pDstFrameBytes, dstPixelFormat, DstSize.Width, DstSize.Height);
                    if (ret < 0)
                        throw new Exception("Failed to fill picture: " + ret.ToString());
                    Marshal.WriteInt32(_pDstFrameInfo, _marshalOffset_AVFrame_width, DstSize.Width);
                    Marshal.WriteInt32(_pDstFrameInfo, _marshalOffset_AVFrame_height, DstSize.Height);
                    Marshal.WriteInt32(_pDstFrameInfo, _marshalOffset_AVFrame_interlaced_frame, _srcFrameInfo.interlaced_frame);
                    _dstFrameInfo = (FFmpeg.AVFrame)Marshal.PtrToStructure(_pDstFrameInfo, typeof(FFmpeg.AVFrame));
                }

                //FFmpeg.av_log_set_callback(new FFmpeg.AVLogCallback(AVLogCallback));

                IntPtr pScaleSrcData = _pSrcFrameInfo + _marshalOffset_AVFrame_data;
                IntPtr pScaleSrcLineSize = _pSrcFrameInfo + _marshalOffset_AVFrame_linesize;
                IntPtr pScaleDstData = _pDstFrameInfo + _marshalOffset_AVFrame_data;
                IntPtr pScaleDstLineSize = _pDstFrameInfo + _marshalOffset_AVFrame_linesize;

                ret = FFmpeg.SwScale.sws_scale(_pSwsContext, pScaleSrcData, pScaleSrcLineSize, 0, _srcFrameInfo.height, pScaleDstData, pScaleDstLineSize);
                if (ret < 0)
                    throw new Exception("Failed to scale frame: " + ret.ToString());
            }

            // Specify -1 to get full interlaced frame
            public ImageProcessing.YData GetYData(int fieldIndex = -1)
            {
                FFmpeg.AVFrame frameInfo = _dstFrameInfo;

                FFmpeg.AVPixelFormat dstPixelFormat = _dstAVPixelFormat[(int)DstPixelFormat];
                if (_srcCodecContext.pix_fmt == dstPixelFormat)
                {
                    frameInfo = _srcFrameInfo;
                }

                int width = frameInfo.width;
                int height = frameInfo.height;
                int stride = frameInfo.linesize[0];
                byte[] data = null;

                if ((fieldIndex != -1) && (frameInfo.interlaced_frame != 0))
                {
                    height /= _srcCodecContext.ticks_per_frame;
                }

                if (!_yDataBytes.TryGetValue(fieldIndex, out data))
                {
                    if ((fieldIndex != -1) && (frameInfo.interlaced_frame != 0))
                    {
                        int dstSize = stride * height;
                        data = new byte[dstSize];

                        int firstRow = fieldIndex;
                        if (frameInfo.top_field_first != 0)
                            firstRow = _srcCodecContext.ticks_per_frame - fieldIndex;

                        IntPtr pFrameBytes = frameInfo.data[0] + (stride * firstRow);
                        int srcStride = stride * _srcCodecContext.ticks_per_frame;

                        for (int dstOffset = 0; dstOffset < dstSize; dstOffset += stride)
                        {
                            Marshal.Copy(pFrameBytes, data, dstOffset, stride);
                            pFrameBytes += srcStride;
                        }
                    }
                    else
                    {
                        data = new byte[stride * frameInfo.height];

                        IntPtr pFrameBytes = frameInfo.data[0];

                        Marshal.Copy(frameInfo.data[0], data, 0, stride * frameInfo.height);
                    }
                    _yDataBytes.Add(fieldIndex, data);
                }

                return new ImageProcessing.YData(width, height, stride, data);
            }

            // Specify -1 to get full interlaced frame
            public Bitmap GetImage(int fieldIndex = -1)
            {
                FFmpeg.AVFrame frameInfo = _dstFrameInfo;

                FFmpeg.AVPixelFormat dstPixelFormat = _dstAVPixelFormat[(int)DstPixelFormat];
                if (_srcCodecContext.pix_fmt == dstPixelFormat)
                {
                    frameInfo = _srcFrameInfo;
                }

                if (DstPixelFormat == PixelFormat.RGB24)
                {
                    IntPtr pFrameBytes = frameInfo.data[0];
                    int frameBytesLength = frameInfo.linesize[0] * frameInfo.height;

                    if ((fieldIndex != -1) && (frameInfo.interlaced_frame != 0))
                    {
                        var height = DstSize.Height / _srcCodecContext.ticks_per_frame;
                        var image = new System.Drawing.Bitmap(DstSize.Width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, DstSize.Width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, image.PixelFormat);

                        int firstRow = fieldIndex;
                        if (frameInfo.top_field_first != 0)
                            firstRow = _srcCodecContext.ticks_per_frame - fieldIndex;

                        int stride = frameInfo.linesize[0];
                        int srcSride = stride * _srcCodecContext.ticks_per_frame;
                        IntPtr pSrc = pFrameBytes + (stride * firstRow);
                        int pDst = 0;
                        int pDstEnd = pDst + (stride * height);

                        while (pDst < pDstEnd)
                        {
                            memcpy(imageData.Scan0 + pDst, pSrc, frameInfo.linesize[0]);
                            pSrc += srcSride;
                            pDst += stride;
                        }

                        image.UnlockBits(imageData);
                        return image;
                    }
                    else
                    {
                        var image = new System.Drawing.Bitmap(DstSize.Width, DstSize.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        var imageData = image.LockBits(new System.Drawing.Rectangle(0, 0, DstSize.Width, DstSize.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, image.PixelFormat);

                        memcpy(imageData.Scan0, pFrameBytes, frameBytesLength);

                        image.UnlockBits(imageData);
                        return image;
                    }
                }
                else if (DstPixelFormat == PixelFormat.Y)
                {
                    var yData = GetYData(fieldIndex);
                    return yData.GetBitmap();
                }
                return null;
            }

            [DllImport("msvcrt.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            static extern int sprintf(StringBuilder buffer, string format, IntPtr args);

            private static void AVLogCallback(IntPtr avcl, int level, string format, params string[] args)
            {
                Console.WriteLine(format);
            }
        }

    }
}
