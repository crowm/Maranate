using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpFFmpeg
{
    public partial class FFmpeg
    {
        private const string avutilDllName = "avutil-52.dll";

        public delegate int Read_PacketCallback(IntPtr opaque, IntPtr buf, int buf_size);

        public delegate int WritePacketCallback(IntPtr opaque, IntPtr buf, int buf_size);

        public delegate Int64 SeekCallback(IntPtr opaque, Int64 offset, int whence);

        public delegate UInt32 UpdateChecksumCallback(UInt32 checksum, IntPtr buf, UInt32 size);

        public delegate int ReadPauseCallback2(IntPtr opaque, int pause);

        public delegate Int64 ReadSeekCallback2(IntPtr opaque, int stream_index, Int64 timestamp, int flags);

        public delegate void AVLogCallback(IntPtr avcl, int level, [MarshalAs(UnmanagedType.LPStr)]string format, params string[] args);

        [DllImport(avutilDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_log_set_callback(AVLogCallback callback);

        public enum AVPixelFormat
        {
            PIX_FMT_NONE = -1,
            PIX_FMT_YUV420P,   // Planar YUV 4:2:0, 12bpp, (1 Cr & Cb sample per 2x2 Y samples)
            PIX_FMT_YUYV422,   // Packed YUV 4:2:2, 16bpp, Y0 Cb Y1 Cr
            PIX_FMT_RGB24,     // Packed RGB 8:8:8, 24bpp, RGBRGB...
            PIX_FMT_BGR24,     // Packed RGB 8:8:8, 24bpp, BGRBGR...
            PIX_FMT_YUV422P,   // Planar YUV 4:2:2, 16bpp, (1 Cr & Cb sample per 2x1 Y samples)
            PIX_FMT_YUV444P,   // Planar YUV 4:4:4, 24bpp, (1 Cr & Cb sample per 1x1 Y samples)
            PIX_FMT_RGB32,     // Packed RGB 8:8:8, 32bpp, (msb)8A 8R 8G 8B(lsb), in cpu endianness
            PIX_FMT_YUV410P,   // Planar YUV 4:1:0,  9bpp, (1 Cr & Cb sample per 4x4 Y samples)
            PIX_FMT_YUV411P,   // Planar YUV 4:1:1, 12bpp, (1 Cr & Cb sample per 4x1 Y samples)
            PIX_FMT_RGB565,    // Packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), in cpu endianness
            PIX_FMT_RGB555,    // Packed RGB 5:5:5, 16bpp, (msb)1A 5R 5G 5B(lsb), in cpu endianness most significant bit to 1
            PIX_FMT_GRAY8,     //        Y        ,  8bpp
            PIX_FMT_MONOWHITE, //        Y        ,  1bpp, 1 is white
            PIX_FMT_MONOBLACK, //        Y        ,  1bpp, 0 is black
            PIX_FMT_PAL8,      // 8 bit with PIX_FMT_RGB32 palette
            PIX_FMT_YUVJ420P,  // Planar YUV 4:2:0, 12bpp, full scale (jpeg)
            PIX_FMT_YUVJ422P,  // Planar YUV 4:2:2, 16bpp, full scale (jpeg)
            PIX_FMT_YUVJ444P,  // Planar YUV 4:4:4, 24bpp, full scale (jpeg)
            PIX_FMT_XVMC_MPEG2_MC,// XVideo Motion Acceleration via common packet passing(xvmc_render.h)
            PIX_FMT_XVMC_MPEG2_IDCT,
            PIX_FMT_UYVY422,   // Packed YUV 4:2:2, 16bpp, Cb Y0 Cr Y1
            PIX_FMT_UYYVYY411, // Packed YUV 4:1:1, 12bpp, Cb Y0 Y1 Cr Y2 Y3
            PIX_FMT_BGR32,     // Packed RGB 8:8:8, 32bpp, (msb)8A 8B 8G 8R(lsb), in cpu endianness
            PIX_FMT_BGR565,    // Packed RGB 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), in cpu endianness
            PIX_FMT_BGR555,    // Packed RGB 5:5:5, 16bpp, (msb)1A 5B 5G 5R(lsb), in cpu endianness most significant bit to 1
            PIX_FMT_BGR8,      // Packed RGB 3:3:2,  8bpp, (msb)2B 3G 3R(lsb)
            PIX_FMT_BGR4,      // Packed RGB 1:2:1,  4bpp, (msb)1B 2G 1R(lsb)
            PIX_FMT_BGR4_BYTE, // Packed RGB 1:2:1,  8bpp, (msb)1B 2G 1R(lsb)
            PIX_FMT_RGB8,      // Packed RGB 3:3:2,  8bpp, (msb)2R 3G 3B(lsb)
            PIX_FMT_RGB4,      // Packed RGB 1:2:1,  4bpp, (msb)2R 3G 3B(lsb)
            PIX_FMT_RGB4_BYTE, // Packed RGB 1:2:1,  8bpp, (msb)2R 3G 3B(lsb)
            PIX_FMT_NV12,      // Planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 for UV
            PIX_FMT_NV21,      // as above, but U and V bytes are swapped

            PIX_FMT_RGB32_1,   // Packed RGB 8:8:8, 32bpp, (msb)8R 8G 8B 8A(lsb), in cpu endianness
            PIX_FMT_BGR32_1,   // Packed RGB 8:8:8, 32bpp, (msb)8B 8G 8R 8A(lsb), in cpu endianness

            PIX_FMT_NB,        // number of pixel
        };

        public enum AVPictureType
        {
            AV_PICTURE_TYPE_NONE = 0, ///< Undefined
            AV_PICTURE_TYPE_I,     ///< Intra
            AV_PICTURE_TYPE_P,     ///< Predicted
            AV_PICTURE_TYPE_B,     ///< Bi-dir predicted
            AV_PICTURE_TYPE_S,     ///< S(GMC)-VOP MPEG4
            AV_PICTURE_TYPE_SI,    ///< Switching Intra
            AV_PICTURE_TYPE_SP,    ///< Switching Predicted
            AV_PICTURE_TYPE_BI     ///< BI type
        };


        [StructLayout(LayoutKind.Sequential)]
        public class ByteIOContext
        {
            //public AVClass av_class;
            public IntPtr pAVClass;

            public IntPtr buffer;

            [MarshalAs(UnmanagedType.I4)]
            public int buffer_size;

            public IntPtr buf_ptr;

            public IntPtr buf_end;

            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //AnonymousCallback opaque;
            public IntPtr opaque;

            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public Read_PacketCallback read_packet;
            public IntPtr read_packet;

            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public WritePacketCallback write_packet;
            public IntPtr write_packet;

            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public SeekCallback seek;
            public IntPtr seek;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 pos; // position in the file of the current buffer 

            [MarshalAs(UnmanagedType.I4)]
            public int must_flush; // true if the next seek should flush

            [MarshalAs(UnmanagedType.I4)]
            public int eof_reached; // true if eof reached

            [MarshalAs(UnmanagedType.I4)]
            public int write_flag;  // true if open for writing 

            [MarshalAs(UnmanagedType.I4)]
            public int max_packet_size;

            [MarshalAs(UnmanagedType.U4)]
            public uint checksum;

            public IntPtr checksum_ptr;

            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public UpdateChecksumCallback update_checksum;
            public IntPtr update_checksum;

            [MarshalAs(UnmanagedType.I4)]
            public int error; // contains the error code or 0 if no error happened

            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public ReadPauseCallback2 read_pause;
            public IntPtr read_pause;

            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public ReadSeekCallback2 read_seek;
            public IntPtr read_seek;

            [MarshalAs(UnmanagedType.I4)]
            public int seekable;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 maxsize;

            [MarshalAs(UnmanagedType.I4)]
            public int direct;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 bytes_read;

            [MarshalAs(UnmanagedType.I4)]
            public int seek_count;

        };

        public delegate String ItemNameCallback();

        [StructLayout(LayoutKind.Sequential)]
        public struct AVClass
        {
            [MarshalAs(UnmanagedType.LPStr)]
            String class_name;
            ItemNameCallback item_name;
            IntPtr pAVOption;
        };

        public struct AVOption
        {
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVRational
        {
            [MarshalAs(UnmanagedType.I4)]
            public int num;
            [MarshalAs(UnmanagedType.I4)]
            public int den;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVDictionaryEntry
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public String key;
            [MarshalAs(UnmanagedType.LPStr)]
            public String value;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVDictionary
        {
            [MarshalAs(UnmanagedType.I4)]
            public int count;
            
            public AVDictionaryEntry[] elems;
        };

        public enum AVDurationEstimationMethod
        {
            AVFMT_DURATION_FROM_PTS,
            AVFMT_DURATION_FROM_STREAM,
            AVFMT_DURATION_FROM_BITRATE
        }

        public enum AVStreamParseType : uint
        {
            AVSTREAM_PARSE_NONE,
            AVSTREAM_PARSE_FULL,       /**< full parsing and repack */
            AVSTREAM_PARSE_HEADERS,    /**< Only parse headers, do not repack. */
            AVSTREAM_PARSE_TIMESTAMPS, /**< full parsing and interpolation of timestamps for frames not starting on a packet boundary */
            AVSTREAM_PARSE_FULL_ONCE,  /**< full parsing and repack of the first frame only, only implemented for H.264 currently */
            AVSTREAM_PARSE_FULL_RAW = (((uint)'R' << 8) | ((uint)'A' << 16) | ((uint)'W' << 24))
                                       /**< full parsing and repack with timestamp and position generation by parser for raw
                                            this assumes that each packet in the file contains no demuxer level headers and
                                            just codec level data, otherwise position generation would fail */
        };

        public enum AVColorPrimaries
        {
            AVCOL_PRI_BT709 = 1, ///< also ITU-R BT1361 / IEC 61966-2-4 / SMPTE RP177 Annex B
            AVCOL_PRI_UNSPECIFIED = 2,
            AVCOL_PRI_BT470M = 4,
            AVCOL_PRI_BT470BG = 5, ///< also ITU-R BT601-6 625 / ITU-R BT1358 625 / ITU-R BT1700 625 PAL & SECAM
            AVCOL_PRI_SMPTE170M = 6, ///< also ITU-R BT601-6 525 / ITU-R BT1358 525 / ITU-R BT1700 NTSC
            AVCOL_PRI_SMPTE240M = 7, ///< functionally identical to above
            AVCOL_PRI_FILM = 8,
            AVCOL_PRI_NB ///< Not part of ABI
        };

        public enum AVColorTransferCharacteristic
        {
            AVCOL_TRC_BT709 = 1, ///< also ITU-R BT1361
            AVCOL_TRC_UNSPECIFIED = 2,
            AVCOL_TRC_GAMMA22 = 4, ///< also ITU-R BT470M / ITU-R BT1700 625 PAL & SECAM
            AVCOL_TRC_GAMMA28 = 5, ///< also ITU-R BT470BG
            AVCOL_TRC_SMPTE240M = 7,
            AVCOL_TRC_NB ///< Not part of ABI
        };

        public enum AVColorSpace
        {
            AVCOL_SPC_RGB = 0,
            AVCOL_SPC_BT709 = 1, ///< also ITU-R BT1361 / IEC 61966-2-4 xvYCC709 / SMPTE RP177 Annex B
            AVCOL_SPC_UNSPECIFIED = 2,
            AVCOL_SPC_FCC = 4,
            AVCOL_SPC_BT470BG = 5, ///< also ITU-R BT601-6 625 / ITU-R BT1358 625 / ITU-R BT1700 625 PAL & SECAM / IEC 61966-2-4 xvYCC601
            AVCOL_SPC_SMPTE170M = 6, ///< also ITU-R BT601-6 525 / ITU-R BT1358 525 / ITU-R BT1700 NTSC / functionally identical to above
            AVCOL_SPC_SMPTE240M = 7,
            AVCOL_SPC_YCOCG = 8, ///< Used by Dirac / VC-2 and H.264 FRext, see ITU-T SG16
            AVCOL_SPC_NB ///< Not part of ABI
        };

        public enum AVColorRange
        {
            AVCOL_RANGE_UNSPECIFIED = 0,
            AVCOL_RANGE_MPEG = 1, ///< the normal 219*2^(n-8) "MPEG" YUV ranges
            AVCOL_RANGE_JPEG = 2, ///< the normal     2^n-1   "JPEG" YUV ranges
            AVCOL_RANGE_NB ///< Not part of ABI
        };

        public enum AVChromaLocation
        {
            AVCHROMA_LOC_UNSPECIFIED = 0,
            AVCHROMA_LOC_LEFT = 1, ///< mpeg2/4, h264 default
            AVCHROMA_LOC_CENTER = 2, ///< mpeg1, jpeg, h263
            AVCHROMA_LOC_TOPLEFT = 3, ///< DV
            AVCHROMA_LOC_TOP = 4,
            AVCHROMA_LOC_BOTTOMLEFT = 5,
            AVCHROMA_LOC_BOTTOM = 6,
            AVCHROMA_LOC_NB ///< Not part of ABI
        };

        public enum AVFieldOrder
        {
            AV_FIELD_UNKNOWN,
            AV_FIELD_PROGRESSIVE,
            AV_FIELD_TT,          //< Top coded_first, top displayed first
            AV_FIELD_BB,          //< Bottom coded first, bottom displayed first
            AV_FIELD_TB,          //< Top coded first, bottom displayed first
            AV_FIELD_BT           //< Bottom coded first, top displayed first
        };

        public enum AVAudioServiceType
        {
            AV_AUDIO_SERVICE_TYPE_MAIN = 0,
            AV_AUDIO_SERVICE_TYPE_EFFECTS = 1,
            AV_AUDIO_SERVICE_TYPE_VISUALLY_IMPAIRED = 2,
            AV_AUDIO_SERVICE_TYPE_HEARING_IMPAIRED = 3,
            AV_AUDIO_SERVICE_TYPE_DIALOGUE = 4,
            AV_AUDIO_SERVICE_TYPE_COMMENTARY = 5,
            AV_AUDIO_SERVICE_TYPE_EMERGENCY = 6,
            AV_AUDIO_SERVICE_TYPE_VOICE_OVER = 7,
            AV_AUDIO_SERVICE_TYPE_KARAOKE = 8,
            AV_AUDIO_SERVICE_TYPE_NB ///< Not part of ABI
        };

        public enum AVSampleFormat
        {
            AV_SAMPLE_FMT_NONE = -1,
            AV_SAMPLE_FMT_U8,          ///< unsigned 8 bits
            AV_SAMPLE_FMT_S16,         ///< signed 16 bits
            AV_SAMPLE_FMT_S32,         ///< signed 32 bits
            AV_SAMPLE_FMT_FLT,         ///< float
            AV_SAMPLE_FMT_DBL,         ///< double

            AV_SAMPLE_FMT_U8P,         ///< unsigned 8 bits, planar
            AV_SAMPLE_FMT_S16P,        ///< signed 16 bits, planar
            AV_SAMPLE_FMT_S32P,        ///< signed 32 bits, planar
            AV_SAMPLE_FMT_FLTP,        ///< float, planar
            AV_SAMPLE_FMT_DBLP,        ///< double, planar

            AV_SAMPLE_FMT_NB           ///< Number of sample formats. DO NOT USE if linking dynamically
        };


        public enum AVCodecID : uint
        {
            AV_CODEC_ID_NONE,

            /* video codecs */
            AV_CODEC_ID_MPEG1VIDEO,
            AV_CODEC_ID_MPEG2VIDEO, ///< preferred ID for MPEG-1/2 video decoding
            AV_CODEC_ID_MPEG2VIDEO_XVMC,
            AV_CODEC_ID_H261,
            AV_CODEC_ID_H263,
            AV_CODEC_ID_RV10,
            AV_CODEC_ID_RV20,
            AV_CODEC_ID_MJPEG,
            AV_CODEC_ID_MJPEGB,
            AV_CODEC_ID_LJPEG,
            AV_CODEC_ID_SP5X,
            AV_CODEC_ID_JPEGLS,
            AV_CODEC_ID_MPEG4,
            AV_CODEC_ID_RAWVIDEO,
            AV_CODEC_ID_MSMPEG4V1,
            AV_CODEC_ID_MSMPEG4V2,
            AV_CODEC_ID_MSMPEG4V3,
            AV_CODEC_ID_WMV1,
            AV_CODEC_ID_WMV2,
            AV_CODEC_ID_H263P,
            AV_CODEC_ID_H263I,
            AV_CODEC_ID_FLV1,
            AV_CODEC_ID_SVQ1,
            AV_CODEC_ID_SVQ3,
            AV_CODEC_ID_DVVIDEO,
            AV_CODEC_ID_HUFFYUV,
            AV_CODEC_ID_CYUV,
            AV_CODEC_ID_H264,
            AV_CODEC_ID_INDEO3,
            AV_CODEC_ID_VP3,
            AV_CODEC_ID_THEORA,
            AV_CODEC_ID_ASV1,
            AV_CODEC_ID_ASV2,
            AV_CODEC_ID_FFV1,
            AV_CODEC_ID_4XM,
            AV_CODEC_ID_VCR1,
            AV_CODEC_ID_CLJR,
            AV_CODEC_ID_MDEC,
            AV_CODEC_ID_ROQ,
            AV_CODEC_ID_INTERPLAY_VIDEO,
            AV_CODEC_ID_XAN_WC3,
            AV_CODEC_ID_XAN_WC4,
            AV_CODEC_ID_RPZA,
            AV_CODEC_ID_CINEPAK,
            AV_CODEC_ID_WS_VQA,
            AV_CODEC_ID_MSRLE,
            AV_CODEC_ID_MSVIDEO1,
            AV_CODEC_ID_IDCIN,
            AV_CODEC_ID_8BPS,
            AV_CODEC_ID_SMC,
            AV_CODEC_ID_FLIC,
            AV_CODEC_ID_TRUEMOTION1,
            AV_CODEC_ID_VMDVIDEO,
            AV_CODEC_ID_MSZH,
            AV_CODEC_ID_ZLIB,
            AV_CODEC_ID_QTRLE,
            AV_CODEC_ID_SNOW,
            AV_CODEC_ID_TSCC,
            AV_CODEC_ID_ULTI,
            AV_CODEC_ID_QDRAW,
            AV_CODEC_ID_VIXL,
            AV_CODEC_ID_QPEG,
            AV_CODEC_ID_PNG,
            AV_CODEC_ID_PPM,
            AV_CODEC_ID_PBM,
            AV_CODEC_ID_PGM,
            AV_CODEC_ID_PGMYUV,
            AV_CODEC_ID_PAM,
            AV_CODEC_ID_FFVHUFF,
            AV_CODEC_ID_RV30,
            AV_CODEC_ID_RV40,
            AV_CODEC_ID_VC1,
            AV_CODEC_ID_WMV3,
            AV_CODEC_ID_LOCO,
            AV_CODEC_ID_WNV1,
            AV_CODEC_ID_AASC,
            AV_CODEC_ID_INDEO2,
            AV_CODEC_ID_FRAPS,
            AV_CODEC_ID_TRUEMOTION2,
            AV_CODEC_ID_BMP,
            AV_CODEC_ID_CSCD,
            AV_CODEC_ID_MMVIDEO,
            AV_CODEC_ID_ZMBV,
            AV_CODEC_ID_AVS,
            AV_CODEC_ID_SMACKVIDEO,
            AV_CODEC_ID_NUV,
            AV_CODEC_ID_KMVC,
            AV_CODEC_ID_FLASHSV,
            AV_CODEC_ID_CAVS,
            AV_CODEC_ID_JPEG2000,
            AV_CODEC_ID_VMNC,
            AV_CODEC_ID_VP5,
            AV_CODEC_ID_VP6,
            AV_CODEC_ID_VP6F,
            AV_CODEC_ID_TARGA,
            AV_CODEC_ID_DSICINVIDEO,
            AV_CODEC_ID_TIERTEXSEQVIDEO,
            AV_CODEC_ID_TIFF,
            AV_CODEC_ID_GIF,
            AV_CODEC_ID_DXA,
            AV_CODEC_ID_DNXHD,
            AV_CODEC_ID_THP,
            AV_CODEC_ID_SGI,
            AV_CODEC_ID_C93,
            AV_CODEC_ID_BETHSOFTVID,
            AV_CODEC_ID_PTX,
            AV_CODEC_ID_TXD,
            AV_CODEC_ID_VP6A,
            AV_CODEC_ID_AMV,
            AV_CODEC_ID_VB,
            AV_CODEC_ID_PCX,
            AV_CODEC_ID_SUNRAST,
            AV_CODEC_ID_INDEO4,
            AV_CODEC_ID_INDEO5,
            AV_CODEC_ID_MIMIC,
            AV_CODEC_ID_RL2,
            AV_CODEC_ID_ESCAPE124,
            AV_CODEC_ID_DIRAC,
            AV_CODEC_ID_BFI,
            AV_CODEC_ID_CMV,
            AV_CODEC_ID_MOTIONPIXELS,
            AV_CODEC_ID_TGV,
            AV_CODEC_ID_TGQ,
            AV_CODEC_ID_TQI,
            AV_CODEC_ID_AURA,
            AV_CODEC_ID_AURA2,
            AV_CODEC_ID_V210X,
            AV_CODEC_ID_TMV,
            AV_CODEC_ID_V210,
            AV_CODEC_ID_DPX,
            AV_CODEC_ID_MAD,
            AV_CODEC_ID_FRWU,
            AV_CODEC_ID_FLASHSV2,
            AV_CODEC_ID_CDGRAPHICS,
            AV_CODEC_ID_R210,
            AV_CODEC_ID_ANM,
            AV_CODEC_ID_BINKVIDEO,
            AV_CODEC_ID_IFF_ILBM,
            AV_CODEC_ID_IFF_BYTERUN1,
            AV_CODEC_ID_KGV1,
            AV_CODEC_ID_YOP,
            AV_CODEC_ID_VP8,
            AV_CODEC_ID_PICTOR,
            AV_CODEC_ID_ANSI,
            AV_CODEC_ID_A64_MULTI,
            AV_CODEC_ID_A64_MULTI5,
            AV_CODEC_ID_R10K,
            AV_CODEC_ID_MXPEG,
            AV_CODEC_ID_LAGARITH,
            AV_CODEC_ID_PRORES,
            AV_CODEC_ID_JV,
            AV_CODEC_ID_DFA,
            AV_CODEC_ID_WMV3IMAGE,
            AV_CODEC_ID_VC1IMAGE,
            AV_CODEC_ID_UTVIDEO,
            AV_CODEC_ID_BMV_VIDEO,
            AV_CODEC_ID_VBLE,
            AV_CODEC_ID_DXTORY,
            AV_CODEC_ID_V410,
            AV_CODEC_ID_XWD,
            AV_CODEC_ID_CDXL,
            AV_CODEC_ID_XBM,
            AV_CODEC_ID_ZEROCODEC,
            AV_CODEC_ID_MSS1,
            AV_CODEC_ID_MSA1,
            AV_CODEC_ID_TSCC2,
            AV_CODEC_ID_MTS2,
            AV_CODEC_ID_CLLC,
            AV_CODEC_ID_MSS2,

            //AV_CODEC_ID_BRENDER_PIX= MKBETAG('B','P','I','X'),
            //AV_CODEC_ID_Y41P       = MKBETAG('Y','4','1','P'),
            //AV_CODEC_ID_ESCAPE130  = MKBETAG('E','1','3','0'),
            //AV_CODEC_ID_EXR        = MKBETAG('0','E','X','R'),
            //AV_CODEC_ID_AVRP       = MKBETAG('A','V','R','P'),

            //AV_CODEC_ID_G2M        = MKBETAG( 0 ,'G','2','M'),
            //AV_CODEC_ID_AVUI       = MKBETAG('A','V','U','I'),
            //AV_CODEC_ID_AYUV       = MKBETAG('A','Y','U','V'),
            //AV_CODEC_ID_TARGA_Y216 = MKBETAG('T','2','1','6'),
            //AV_CODEC_ID_V308       = MKBETAG('V','3','0','8'),
            //AV_CODEC_ID_V408       = MKBETAG('V','4','0','8'),
            //AV_CODEC_ID_YUV4       = MKBETAG('Y','U','V','4'),
            //AV_CODEC_ID_SANM       = MKBETAG('S','A','N','M'),
            //AV_CODEC_ID_PAF_VIDEO  = MKBETAG('P','A','F','V'),
            //AV_CODEC_ID_AVRN       = MKBETAG('A','V','R','n'),
            //AV_CODEC_ID_CPIA       = MKBETAG('C','P','I','A'),
            //AV_CODEC_ID_XFACE      = MKBETAG('X','F','A','C'),
            //AV_CODEC_ID_SGIRLE     = MKBETAG('S','G','I','R'),
            //AV_CODEC_ID_MVC1       = MKBETAG('M','V','C','1'),
            //AV_CODEC_ID_MVC2       = MKBETAG('M','V','C','2'),

            /* various PCM "codecs" */
            AV_CODEC_ID_FIRST_AUDIO = 0x10000,     ///< A dummy id pointing at the start of audio codecs
            AV_CODEC_ID_PCM_S16LE = 0x10000,
            AV_CODEC_ID_PCM_S16BE,
            AV_CODEC_ID_PCM_U16LE,
            AV_CODEC_ID_PCM_U16BE,
            AV_CODEC_ID_PCM_S8,
            AV_CODEC_ID_PCM_U8,
            AV_CODEC_ID_PCM_MULAW,
            AV_CODEC_ID_PCM_ALAW,
            AV_CODEC_ID_PCM_S32LE,
            AV_CODEC_ID_PCM_S32BE,
            AV_CODEC_ID_PCM_U32LE,
            AV_CODEC_ID_PCM_U32BE,
            AV_CODEC_ID_PCM_S24LE,
            AV_CODEC_ID_PCM_S24BE,
            AV_CODEC_ID_PCM_U24LE,
            AV_CODEC_ID_PCM_U24BE,
            AV_CODEC_ID_PCM_S24DAUD,
            AV_CODEC_ID_PCM_ZORK,
            AV_CODEC_ID_PCM_S16LE_PLANAR,
            AV_CODEC_ID_PCM_DVD,
            AV_CODEC_ID_PCM_F32BE,
            AV_CODEC_ID_PCM_F32LE,
            AV_CODEC_ID_PCM_F64BE,
            AV_CODEC_ID_PCM_F64LE,
            AV_CODEC_ID_PCM_BLURAY,
            AV_CODEC_ID_PCM_LXF,
            AV_CODEC_ID_S302M,
            AV_CODEC_ID_PCM_S8_PLANAR,
            //AV_CODEC_ID_PCM_S24LE_PLANAR = MKBETAG(24,'P','S','P'),
            //AV_CODEC_ID_PCM_S32LE_PLANAR = MKBETAG(32,'P','S','P'),
            //AV_CODEC_ID_PCM_S16BE_PLANAR = MKBETAG('P','S','P',16),

            /* various ADPCM codecs */
            AV_CODEC_ID_ADPCM_IMA_QT = 0x11000,
            AV_CODEC_ID_ADPCM_IMA_WAV,
            AV_CODEC_ID_ADPCM_IMA_DK3,
            AV_CODEC_ID_ADPCM_IMA_DK4,
            AV_CODEC_ID_ADPCM_IMA_WS,
            AV_CODEC_ID_ADPCM_IMA_SMJPEG,
            AV_CODEC_ID_ADPCM_MS,
            AV_CODEC_ID_ADPCM_4XM,
            AV_CODEC_ID_ADPCM_XA,
            AV_CODEC_ID_ADPCM_ADX,
            AV_CODEC_ID_ADPCM_EA,
            AV_CODEC_ID_ADPCM_G726,
            AV_CODEC_ID_ADPCM_CT,
            AV_CODEC_ID_ADPCM_SWF,
            AV_CODEC_ID_ADPCM_YAMAHA,
            AV_CODEC_ID_ADPCM_SBPRO_4,
            AV_CODEC_ID_ADPCM_SBPRO_3,
            AV_CODEC_ID_ADPCM_SBPRO_2,
            AV_CODEC_ID_ADPCM_THP,
            AV_CODEC_ID_ADPCM_IMA_AMV,
            AV_CODEC_ID_ADPCM_EA_R1,
            AV_CODEC_ID_ADPCM_EA_R3,
            AV_CODEC_ID_ADPCM_EA_R2,
            AV_CODEC_ID_ADPCM_IMA_EA_SEAD,
            AV_CODEC_ID_ADPCM_IMA_EA_EACS,
            AV_CODEC_ID_ADPCM_EA_XAS,
            AV_CODEC_ID_ADPCM_EA_MAXIS_XA,
            AV_CODEC_ID_ADPCM_IMA_ISS,
            AV_CODEC_ID_ADPCM_G722,
            AV_CODEC_ID_ADPCM_IMA_APC,
            //AV_CODEC_ID_VIMA       = MKBETAG('V','I','M','A'),
            //AV_CODEC_ID_ADPCM_AFC  = MKBETAG('A','F','C',' '),
            //AV_CODEC_ID_ADPCM_IMA_OKI = MKBETAG('O','K','I',' '),

            /* AMR */
            AV_CODEC_ID_AMR_NB = 0x12000,
            AV_CODEC_ID_AMR_WB,

            /* RealAudio codecs*/
            AV_CODEC_ID_RA_144 = 0x13000,
            AV_CODEC_ID_RA_288,

            /* various DPCM codecs */
            AV_CODEC_ID_ROQ_DPCM = 0x14000,
            AV_CODEC_ID_INTERPLAY_DPCM,
            AV_CODEC_ID_XAN_DPCM,
            AV_CODEC_ID_SOL_DPCM,

            /* audio codecs */
            AV_CODEC_ID_MP2 = 0x15000,
            AV_CODEC_ID_MP3, ///< preferred ID for decoding MPEG audio layer 1, 2 or 3
            AV_CODEC_ID_AAC,
            AV_CODEC_ID_AC3,
            AV_CODEC_ID_DTS,
            AV_CODEC_ID_VORBIS,
            AV_CODEC_ID_DVAUDIO,
            AV_CODEC_ID_WMAV1,
            AV_CODEC_ID_WMAV2,
            AV_CODEC_ID_MACE3,
            AV_CODEC_ID_MACE6,
            AV_CODEC_ID_VMDAUDIO,
            AV_CODEC_ID_FLAC,
            AV_CODEC_ID_MP3ADU,
            AV_CODEC_ID_MP3ON4,
            AV_CODEC_ID_SHORTEN,
            AV_CODEC_ID_ALAC,
            AV_CODEC_ID_WESTWOOD_SND1,
            AV_CODEC_ID_GSM, ///< as in Berlin toast format
            AV_CODEC_ID_QDM2,
            AV_CODEC_ID_COOK,
            AV_CODEC_ID_TRUESPEECH,
            AV_CODEC_ID_TTA,
            AV_CODEC_ID_SMACKAUDIO,
            AV_CODEC_ID_QCELP,
            AV_CODEC_ID_WAVPACK,
            AV_CODEC_ID_DSICINAUDIO,
            AV_CODEC_ID_IMC,
            AV_CODEC_ID_MUSEPACK7,
            AV_CODEC_ID_MLP,
            AV_CODEC_ID_GSM_MS, /* as found in WAV */
            AV_CODEC_ID_ATRAC3,
            AV_CODEC_ID_VOXWARE,
            AV_CODEC_ID_APE,
            AV_CODEC_ID_NELLYMOSER,
            AV_CODEC_ID_MUSEPACK8,
            AV_CODEC_ID_SPEEX,
            AV_CODEC_ID_WMAVOICE,
            AV_CODEC_ID_WMAPRO,
            AV_CODEC_ID_WMALOSSLESS,
            AV_CODEC_ID_ATRAC3P,
            AV_CODEC_ID_EAC3,
            AV_CODEC_ID_SIPR,
            AV_CODEC_ID_MP1,
            AV_CODEC_ID_TWINVQ,
            AV_CODEC_ID_TRUEHD,
            AV_CODEC_ID_MP4ALS,
            AV_CODEC_ID_ATRAC1,
            AV_CODEC_ID_BINKAUDIO_RDFT,
            AV_CODEC_ID_BINKAUDIO_DCT,
            AV_CODEC_ID_AAC_LATM,
            AV_CODEC_ID_QDMC,
            AV_CODEC_ID_CELT,
            AV_CODEC_ID_G723_1,
            AV_CODEC_ID_G729,
            AV_CODEC_ID_8SVX_EXP,
            AV_CODEC_ID_8SVX_FIB,
            AV_CODEC_ID_BMV_AUDIO,
            AV_CODEC_ID_RALF,
            AV_CODEC_ID_IAC,
            AV_CODEC_ID_ILBC,
            AV_CODEC_ID_OPUS_DEPRECATED,
            AV_CODEC_ID_COMFORT_NOISE,
            AV_CODEC_ID_TAK_DEPRECATED,
            //AV_CODEC_ID_FFWAVESYNTH = MKBETAG('F','F','W','S'),
            //AV_CODEC_ID_8SVX_RAW    = MKBETAG('8','S','V','X'),
            //AV_CODEC_ID_SONIC       = MKBETAG('S','O','N','C'),
            //AV_CODEC_ID_SONIC_LS    = MKBETAG('S','O','N','L'),
            //AV_CODEC_ID_PAF_AUDIO   = MKBETAG('P','A','F','A'),
            //AV_CODEC_ID_OPUS        = MKBETAG('O','P','U','S'),
            //AV_CODEC_ID_TAK         = MKBETAG('t','B','a','K'),

            /* subtitle codecs */
            AV_CODEC_ID_FIRST_SUBTITLE = 0x17000,          ///< A dummy ID pointing at the start of subtitle codecs.
            AV_CODEC_ID_DVD_SUBTITLE = 0x17000,
            AV_CODEC_ID_DVB_SUBTITLE,
            AV_CODEC_ID_TEXT,  ///< raw UTF-8 text
            AV_CODEC_ID_XSUB,
            AV_CODEC_ID_SSA,
            AV_CODEC_ID_MOV_TEXT,
            AV_CODEC_ID_HDMV_PGS_SUBTITLE,
            AV_CODEC_ID_DVB_TELETEXT,
            AV_CODEC_ID_SRT,
            //AV_CODEC_ID_MICRODVD   = MKBETAG('m','D','V','D'),
            //AV_CODEC_ID_EIA_608    = MKBETAG('c','6','0','8'),
            //AV_CODEC_ID_JACOSUB    = MKBETAG('J','S','U','B'),
            //AV_CODEC_ID_SAMI       = MKBETAG('S','A','M','I'),
            //AV_CODEC_ID_REALTEXT   = MKBETAG('R','T','X','T'),
            //AV_CODEC_ID_SUBVIEWER  = MKBETAG('S','u','b','V'),
            //AV_CODEC_ID_SUBRIP     = MKBETAG('S','R','i','p'),
            //AV_CODEC_ID_WEBVTT     = MKBETAG('W','V','T','T'),

            /* other specific kind of codecs (generally used for attachments) */
            AV_CODEC_ID_FIRST_UNKNOWN = 0x18000,           ///< A dummy ID pointing at the start of various fake codecs.
            AV_CODEC_ID_TTF = 0x18000,
            //AV_CODEC_ID_BINTEXT    = MKBETAG('B','T','X','T'),
            //AV_CODEC_ID_XBIN       = MKBETAG('X','B','I','N'),
            //AV_CODEC_ID_IDF        = MKBETAG( 0 ,'I','D','F'),
            //AV_CODEC_ID_OTF        = MKBETAG( 0 ,'O','T','F'),
            //AV_CODEC_ID_OTF        = MKBETAG('K','L','V','A'),

            AV_CODEC_ID_PROBE = 0x19000, ///< codec_id is not known (like AV_CODEC_ID_NONE) but lavf should attempt to identify it

            AV_CODEC_ID_MPEG2TS = 0x20000, /**< _FAKE_ codec to indicate a raw MPEG-2 TS
                                        * stream (only used by libavformat) */
            AV_CODEC_ID_MPEG4SYSTEMS = 0x20001, /**< _FAKE_ codec to indicate a MPEG-4 Systems
                                        * stream (only used by libavformat) */
            AV_CODEC_ID_FFMETADATA = 0x21000    ///< Dummy codec for streams containing only metadata information.

        }

        private static uint MKBETAG(char a, char b, char c, char d)
        {
            return MKBETAG((uint)a, (uint)b, (uint)c, (uint)d);
        }
        private static uint MKBETAG(int a, char b, char c, char d)
        {
            return MKBETAG((uint)a, (uint)b, (uint)c, (uint)d);
        }
        private static uint MKBETAG(uint a, uint b, uint c, uint d)
        {
            return (a | (b << 8) | (c << 16) | (d << 24));
        }

    }
}
