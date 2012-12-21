using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpFFmpeg
{
    public partial class FFmpeg
    {
        private const string avformatDllName = "avformat-54.dll";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPacket"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_destruct_packet_nofree(IntPtr pAVPacket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPacket"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_destruct_packet(IntPtr pAVPacket);

        /// <summary>
        /// Initialize optional fields of a packet.
        /// </summary>
        /// <param name="pAVPacket"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_init_packet(IntPtr pAVPacket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPacket"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_new_packet(IntPtr pAVPacket, int size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pByteIOContext"></param>
        /// <param name="pAVPacket"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_get_packet(IntPtr pByteIOContext, IntPtr pAVPacket, int size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPacket"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_dup_packet(IntPtr pAVPacket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPacket"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_free_packet(IntPtr pAVPacket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVImageFormat"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_register_image_format(IntPtr pAVImageFormat);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVProbeData"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_probe_image_format(IntPtr pAVProbeData);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr guess_image_format([MarshalAs(UnmanagedType.LPTStr)]String filename);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern CodecID av_guess_image2_codec([MarshalAs(UnmanagedType.LPTStr)]
                                                                    String filename);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pByteIOContext"></param>
        /// <param name="filename"></param>
        /// <param name="pAVImageFormat"></param>
        /// <param name="alloc_cb"></param>
        /// <param name="opaque"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_read_image(IntPtr pByteIOContext,
                                            [MarshalAs(UnmanagedType.LPTStr)]String filename,
                                            IntPtr pAVImageFormat,
                                            [MarshalAs(UnmanagedType.FunctionPtr)]
                                            AllocCBCallback alloc_cb,
                                            IntPtr opaque);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pByteIOContext"></param>
        /// <param name="pAVImageFormat"></param>
        /// <param name="pAVImageInfo"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_write_image(IntPtr pByteIOContext, IntPtr pAVImageFormat, IntPtr pAVImageInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVInputFormat"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_register_input_format(IntPtr pAVInputFormat);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVOutputFormat"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_register_output_format(IntPtr pAVOutputFormat);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="short_name"></param>
        /// <param name="filename"></param>
        /// <param name="mime_type"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr guess_stream_format([MarshalAs(UnmanagedType.LPTStr)]
                                                        String short_name,
                                                        [MarshalAs(UnmanagedType.LPTStr)]
                                                        String filename,
                                                        [MarshalAs(UnmanagedType.LPTStr)]
                                                        String mime_type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="short_name"></param>
        /// <param name="filename"></param>
        /// <param name="mime_type"></param>
        /// <returns>AVOutputFormat pointer</returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr guess_format([MarshalAs(UnmanagedType.LPTStr)]
                                                  String short_name,
                                                  [MarshalAs(UnmanagedType.LPTStr)]
                                                  String filename,
                                                  [MarshalAs(UnmanagedType.LPTStr)]
                                                  String mime_type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVOutoutFormat"></param>
        /// <param name="short_name"></param>
        /// <param name="filename"></param>
        /// <param name="mime_type"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern CodecID av_guess_codec(IntPtr pAVOutoutFormat,
                                                            [MarshalAs(UnmanagedType.LPTStr)]
                                                            String short_name,
                                                            [MarshalAs(UnmanagedType.LPTStr)]
                                                            String filename,
                                                            [MarshalAs(UnmanagedType.LPTStr)]
                                                            String mime_type,
                                                            CodecType type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFile"></param>
        /// <param name="buf"></param>
        /// <param name="size"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_hex_dump(IntPtr pFile, IntPtr buf, int size);

        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_pkt_dump(IntPtr pFile, IntPtr pAVPacket, int dump_payload);

        /// <summary>
        /// 
        /// </summary>
        [DllImport(avformatDllName, CallingConvention=CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        public static extern void av_register_all();


        /* media file input */
        /// <summary>
        /// 
        /// </summary>
        /// <param name="?"></param>
        /// <returns>AVInputFormat pointer</returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_find_input_format([MarshalAs(UnmanagedType.LPTStr)]String short_name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVProbeData"></param>
        /// <param name="is_opened"></param>
        /// <returns>AVInputFormat pointer</returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_probe_input_format(IntPtr pAVProbeData, int is_opened);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFormatContext"></param>
        /// <param name="filename"></param>
        /// <param name="pAVInputFormat"></param>
        /// <param name="buf_size"></param>
        /// <param name="pAVFormatParameters"></param>
        /// <returns></returns>
        //[DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        //public static extern int av_open_input_file([Out]out IntPtr pFormatContext,
        //                        [MarshalAs(UnmanagedType.LPStr)]String filename,
        //                        IntPtr pAVInputFormat, int buf_size, IntPtr pAVFormatParameters);

        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avformat_open_input([Out]out IntPtr pFormatContext,
                                [MarshalAs(UnmanagedType.LPStr)]String filename,
                                IntPtr pAVInputFormat,
                                AVDictionary options);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>AVFormatContext pointer</returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_alloc_format_context();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_find_stream_info(IntPtr pAVFormatContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="pAVPacket"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_read_packet(IntPtr pAVFormatContext, IntPtr pAVPacket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="pAVPacket"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_read_frame(IntPtr pAVFormatContext, IntPtr pAVPacket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="stream_index"></param>
        /// <param name="timestamp"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_seek_frame(IntPtr pAVFormatContext, int stream_index, Int64 timestamp, AVSEEK_FLAG flags);

        /**
         * Seek to timestamp ts.
         * Seeking will be done so that the point from which all active streams
         * can be presented successfully will be closest to ts and within min/max_ts.
         * Active streams are all streams that have AVStream.discard < AVDISCARD_ALL.
         *
         * If flags contain AVSEEK_FLAG_BYTE, then all timestamps are in bytes and
         * are the file position (this may not be supported by all demuxers).
         * If flags contain AVSEEK_FLAG_FRAME, then all timestamps are in frames
         * in the stream with stream_index (this may not be supported by all demuxers).
         * Otherwise all timestamps are in units of the stream selected by stream_index
         * or if stream_index is -1, in AV_TIME_BASE units.
         * If flags contain AVSEEK_FLAG_ANY, then non-keyframes are treated as
         * keyframes (this may not be supported by all demuxers).
         *
         * @param stream_index index of the stream which is used as time base reference
         * @param min_ts smallest acceptable timestamp
         * @param ts target timestamp
         * @param max_ts largest acceptable timestamp
         * @param flags flags
         * @return >=0 on success, error code otherwise
         *
         * @note This is part of the new seek API which is still under construction.
         *       Thus do not use this yet. It may change at any time, do not expect
         *       ABI compatibility yet!
         */
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avformat_seek_file(IntPtr pAVFormatContext, int stream_index, Int64 min_ts, Int64 ts, Int64 max_ts, AVSEEK_FLAG flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_read_play(IntPtr pAVFormatContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_read_pause(IntPtr pAVFormatContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        //[DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        //public static extern void av_close_input_file(IntPtr pAVFormatContext);

        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avformat_close_input(IntPtr pAVFormatContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="id"></param>
        /// <returns>AVStream pointer</returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_new_stream(IntPtr pAVFormatContext, int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVStream"></param>
        /// <param name="pts_wrap_bits"></param>
        /// <param name="pts_num"></param>
        /// <param name="pts_den"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_set_pts_info(IntPtr pAVStream, int pts_wrap_bits, int pts_num, int pts_den);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_find_default_stream_index(IntPtr pAVFormatContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVStream"></param>
        /// <param name="timestamp"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_index_search_timestamp(IntPtr pAVStream, Int64 timestamp, int flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVStream"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="timestamp"></param>
        /// <param name="size"></param>
        /// <param name="distance"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_add_index_entry(IntPtr pAVStream, Int64 pos, Int64 timestamp, int size, int distance, int flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="stream_index"></param>
        /// <param name="target_ts"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_seek_frame_binary(IntPtr pAVFormatContext, int stream_index, Int64 target_ts, int flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="pAVStream"></param>
        /// <param name="timestamp"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_update_cur_dts(IntPtr pAVFormatContext, IntPtr pAVStream, Int64 timestamp);

        /* media file output */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="pAVFormatParameters"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_set_parameters(IntPtr pAVFormatContext, IntPtr pAVFormatParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_write_header(IntPtr pAVFormatContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="pAVPacket"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_write_frame(IntPtr pAVFormatContext, IntPtr pAVPacket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="pAVPacket"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_interleaved_write_frame(IntPtr pAVFormatContext, IntPtr pAVPacket);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="p_out_AVPacket"></param>
        /// <param name="pAVPacket"></param>
        /// <param name="flush"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_interleave_packet_per_dts(IntPtr pAVFormatContext, out IntPtr p_out_AVPacket, IntPtr pAVPacket, int flush);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_write_trailer(IntPtr pAVFormatContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="index"></param>
        /// <param name="url"></param>
        /// <param name="is_output"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void dump_format(IntPtr pAVFormatContext, int index,
                                                [MarshalAs(UnmanagedType.LPTStr)]
                                                String url,
                                                int is_output);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width_ptr"></param>
        /// <param name="height_ptr"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int parse_image_size(IntPtr width_ptr, IntPtr height_ptr,
                                                [MarshalAs(UnmanagedType.LPTStr)]String arg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFrame_rate"></param>
        /// <param name="pFrame_rate_base"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int parse_frame_rate(IntPtr pFrame_rate, IntPtr pFrame_rate_base,
                                                [MarshalAs(UnmanagedType.LPTStr)]String arg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="datestr"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern Int64 parse_date([MarshalAs(UnmanagedType.LPTStr)]String datestr, int duration);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern Int64 av_gettime();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern Int64 ffm_read_write_index(int fd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="pos"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void ffm_write_write_index(int fd, Int64 pos);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFormatContext"></param>
        /// <param name="pos"></param>
        /// <param name="file_size"></param>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern void ffm_set_write_index(IntPtr pAVFormatContext, Int64 pos, Int64 file_size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="arg_size"></param>
        /// <param name="tag1"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int find_info_tag([MarshalAs(UnmanagedType.LPTStr)]String arg,
                                                int arg_size,
                                                [MarshalAs(UnmanagedType.LPTStr)]String tag1,
                                                [MarshalAs(UnmanagedType.LPTStr)]String info);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <param name="path"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_get_frame_filename(IntPtr buf, int buf_size,
                                            [MarshalAs(UnmanagedType.LPTStr)]String path, int number);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_filename_number_test([MarshalAs(UnmanagedType.LPTStr)]String filename);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int video_grab_init();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int audio_init();

        /* DV1394 */
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int dv1394_init();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(avformatDllName), SuppressUnmanagedCodeSecurity]
        public static extern int dc1394_init();


        // *********************************************************************************
        // Constants
        // *********************************************************************************

        public const int AV_TIME_BASE = 1000000;

        public const int AVFMT_INFINITEOUTPUTLOOP = 0;

        public const uint AVFMT_FLAG_GENPTS = 0x0001;

        public const uint AVFMT_FLAG_IGNIDX = 0x0002;

        public const int AVFMT_NOOUTPUTLOOP = -1;

        // no file should be opened
        public const uint AVFMT_NOFILE = 0x0001;

        // needs '%d' in filename
        public const uint AVFMT_NEEDNUMBER = 0x0002;

        // show format stream IDs numbers
        public const uint AVFMT_SHOW_IDS = 0x0008;

        // format wants AVPicture structure for raw picture data 
        public const uint AVFMT_RAWPICTURE = 0x0020;

        // format wants global header
        public const uint AVFMT_GLOBALHEADER = 0x0040;

        // format doesnt need / has any timestamps
        public const uint AVFMT_NOTIMESTAMPS = 0x0080;

        // AVImageFormat.flags field constants
        public const uint AVIMAGE_INTERLEAVED = 0x0001;

        public const int AVPROBE_SCORE_MAX = 100;

        public const int PKT_FLAG_KEY = 0x0001;

        public const int AVINDEX_KEYFRAME = 0x001;

        public const int MAX_REORDER_DELAY = 4;

        public const uint AVFMTCTX_NOHEADER = 0x001;

        public const int MAX_STREAMS = 20;

        public const int AVERROR_UNKNOWN = -1;
        public const int AVERROR_IO = -2;
        public const int AVERROR_NUMEXPECTED = -3;
        public const int AVERROR_INVALIDDATA = -4;
        public const int AVERROR_NOMEM = -5;
        public const int AVERROR_NOFMT = -6;
        public const int AVERROR_NOTSUPP = -7;

        public enum AVSEEK_FLAG : int
        {
            AVSEEK_FLAG_NONE = 0,
            AVSEEK_FLAG_BACKWARD = 1,
            AVSEEK_FLAG_BYTE = 2,
            AVSEEK_FLAG_ANY = 4,
            AVSEEK_FLAG_FRAME = 8
        }

        public const int FFM_PACKET_SIZE = 4096;

        // *********************************************************************************
        // Constants
        // *********************************************************************************


        public delegate void DestructCallback(IntPtr pAVPacket);

        [StructLayout(LayoutKind.Sequential)]
        public struct AVPacket
        {
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pts; // presentation time stamp in time_base units

            [MarshalAs(UnmanagedType.I8)]
            public Int64 dts; // decompression time stamp in time_base units

            public IntPtr data;

            [MarshalAs(UnmanagedType.I4)]
            public int size;

            [MarshalAs(UnmanagedType.I4)]
            public int stream_index;

            [MarshalAs(UnmanagedType.I4)]
            public int flags;

            public IntPtr side_data;

            [MarshalAs(UnmanagedType.I4)]
            public int side_data_elems;

            [MarshalAs(UnmanagedType.I4)]
            public int duration;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public DestructCallback destruct;

            public IntPtr priv;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 pos;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 convergence_duration;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVFrac
        {
            [MarshalAs(UnmanagedType.I8)]
            Int64 val;
            [MarshalAs(UnmanagedType.I8)]
            Int64 num;
            [MarshalAs(UnmanagedType.I8)]
            Int64 den;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVProbeData
        {
            [MarshalAs(UnmanagedType.LPStr)]
            String filename;
            IntPtr buf;
            int buf_size;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVFormatParameters
        {
            AVRational time_base;
            int sample_rate;
            int channels;
            int width;
            int height;
            AVPixelFormat pix_fmt;
            IntPtr image_format; // AVImageFormat
            int channel;
            [MarshalAs(UnmanagedType.LPStr)]
            String device;
            [MarshalAs(UnmanagedType.LPStr)]
            String standard;
            int mpeg2ts_raw;
            int mpeg2ts_compute_pcr;
            int initial_pause;
            int prealloced_context;
            CodecID video_codec_id;
            CodecID audio_codec_id;
        };

        public delegate int WriteHeader(IntPtr pAVFormatContext);
        public delegate int WritePacket(IntPtr pAVFormatContext, IntPtr pAVPacket);
        public delegate int WriteTrailer(IntPtr pAVFormatContext);
        public delegate int SetParametersCallback(IntPtr pAVFormatContext, IntPtr avFormatParameters);
        public delegate int InterleavePacketCallback(IntPtr pAVFormatContext, IntPtr pOutAVPacket, IntPtr pInAVPacket, int flush);

        [StructLayout(LayoutKind.Sequential)]
        public struct AVOutputFormat
        {
            [MarshalAs(UnmanagedType.LPStr)]
            String name;

            [MarshalAs(UnmanagedType.LPStr)]
            String long_name;

            [MarshalAs(UnmanagedType.LPStr)]
            String mime_type;

            [MarshalAs(UnmanagedType.LPStr)]
            String extensions;

            int priv_data_size;

            CodecID audio_codec;

            CodecID video_codec;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            WriteHeader write_header;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            WritePacket write_packet;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            WriteTrailer write_trailer;

            int flags;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            SetParametersCallback set_parameters;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            InterleavePacketCallback interleave_packet;

            IntPtr nextAVOutputFormat;
        };

        public delegate int ReadProbeCallback(IntPtr pAVProbeData);
        public delegate int ReadHeaderCallback(IntPtr pAVFormatContext, IntPtr pAVFormatParameters);
        public delegate int ReadPacketCallback(IntPtr pAVFormatContext, IntPtr pAVPacket);
        public delegate int ReadCloseCallback(IntPtr pAVFormatContext);
        public delegate int ReadSeekCallback(IntPtr pAVFormatContext, int stream_index, Int64 timestamp, int flags);

        //int64_t (*read_timestamp)(struct AVFormatContext *s, int stream_index,
        //int64_t *pos, int64_t pos_limit);
        public delegate int ReadTimestampCallback(IntPtr pAVFormatContext, int stream_index, IntPtr pos, Int64 pos_limit);
        public delegate int ReadPlayCallback(IntPtr pAVFormatContext);
        public delegate int ReadPauseCallback(IntPtr pAVFormatContext);

        [StructLayout(LayoutKind.Sequential)]
        public struct AVInputFormat
        {
            [MarshalAs(UnmanagedType.LPStr)]
            String name;

            [MarshalAs(UnmanagedType.LPStr)]
            String long_name;

            int priv_data_size;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ReadProbeCallback read_probe;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ReadHeaderCallback read_header;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ReadPacketCallback read_packet;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ReadCloseCallback read_close;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ReadSeekCallback read_seek;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ReadTimestampCallback read_timestamp;

            // can use flags: AVFMT_NOFILE, AVFMT_NEEDNUMBER
            int flags;

            [MarshalAs(UnmanagedType.LPStr)]
            String extensions;

            int value;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ReadPlayCallback read_play;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ReadPauseCallback read_pause;

            IntPtr nextAVInputFormat;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVIndexEntry
        {
            Int64 pos;
            Int64 timestamp;
            int flags;
            int size;
            int min_distance;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVStream
        {
            [MarshalAs(UnmanagedType.I4)]
            public int index; // stream index in AVFormatContext

            [MarshalAs(UnmanagedType.I4)]
            public int id; // format specific stream id

            public IntPtr codec; // AVCodecContext

            /**
             * real base frame rate of the stream.
             * for example if the timebase is 1/90000 and all frames have either
             * approximately 3600 or 1800 timer ticks then r_frame_rate will be 50/1
             */
            public AVRational r_frame_rate;

            public IntPtr priv_data;

            public AVFrac pts; // encoding: PTS generation when outputing stream 

            /**
             * this is the fundamental unit of time (in seconds) in terms
             * of which frame timestamps are represented. for fixed-fps content,
             * timebase should be 1/framerate and timestamp increments should be
             * identically 1.
             */
            public AVRational time_base;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 start_time;

            // decoding: duration of the stream, in AV_TIME_BASE fractional seconds. 
            [MarshalAs(UnmanagedType.I8)]
            public Int64 duration;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 nb_frames; // number of frames in this stream if known or 0

            [MarshalAs(UnmanagedType.I4)]
            public int disposition;

            public AVDiscard discard; // selects which packets can be discarded at will and dont need to be demuxed

            public AVRational sample_aspect_ratio;

            public IntPtr metadata;

            public AVRational avg_frame_rate;

            public AVPacket attached_pic;

            public IntPtr info;

            [MarshalAs(UnmanagedType.I4)]
            public int pts_wrap_bits; // number of bits in pts (used for wrapping control) 
            
            [MarshalAs(UnmanagedType.I8)]
            public Int64 reference_dts;
            
            [MarshalAs(UnmanagedType.I8)]
            public Int64 first_dts;
            
            [MarshalAs(UnmanagedType.I8)]
            public Int64 cur_dts;
            
            [MarshalAs(UnmanagedType.I8)]
            public Int64 last_IP_pts;

            [MarshalAs(UnmanagedType.I4)]
            public int last_IP_duration;

            [MarshalAs(UnmanagedType.I4)]
            public int probe_packets;

            [MarshalAs(UnmanagedType.I4)]
            public int codec_info_nb_frames;

            [MarshalAs(UnmanagedType.I4)]
            public int stream_identifier;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 interleaver_chunk_size;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 interleaver_chunk_duration;

            public AVStreamParseType need_parsing;

            public IntPtr parser;

            public IntPtr last_in_packet_buffer;

            public AVProbeData probe_data;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = (MAX_REORDER_DELAY + 1))]
            public Int64[] pts_buffer; // pts_buffer[MAX_REORDER_DELAY+1]

            public IntPtr index_entries;

            [MarshalAs(UnmanagedType.I4)]
            public int nb_index_entries;

            [MarshalAs(UnmanagedType.U4)]
            public uint index_entries_allocated_size;

            [MarshalAs(UnmanagedType.I4)]
            public int request_probe;

            [MarshalAs(UnmanagedType.I4)]
            public int skip_to_keyframe;

            [MarshalAs(UnmanagedType.I4)]
            public int skip_samples;

            [MarshalAs(UnmanagedType.I4)]
            public int nb_decoded_frames;
            
            [MarshalAs(UnmanagedType.I8)]
            public Int64 mux_ts_offset;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 pts_wrap_reference;
        };

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public struct AVFormatContext
        {
            public IntPtr pAVClass; // set by av_alloc_format_context

            /**
             * Can only be iformat or oformat, not both at the same time.
             *
             * decoding: set by avformat_open_input().
             * encoding: set by the user.
             */
            public IntPtr pAVInputFormat; // can only be iformat or oformat, not both at the same time 
            public IntPtr pAVOutputFormat;

            /**
             * Format private data. This is an AVOptions-enabled struct
             * if and only if iformat/oformat.priv_class is not NULL.
             */
            public IntPtr priv_data;

            /**
             * I/O context.
             *
             * decoding: either set by the user before avformat_open_input() (then
             * the user must close it manually) or set by avformat_open_input().
             * encoding: set by the user.
             *
             * Do NOT set this field if AVFMT_NOFILE flag is set in
             * iformat/oformat.flags. In such a case, the (de)muxer will handle
             * I/O in some other way and this field will be NULL.
             */
            //public ByteIOContext pb;
            public IntPtr pb;

            /* stream info */
            [MarshalAs(UnmanagedType.I4)]
            public int ctx_flags; // format specific flags, see AVFMTCTX_xx

            /**
             * A list of all streams in the file. New streams are created with
             * avformat_new_stream().
             *
             * decoding: streams are created by libavformat in avformat_open_input().
             * If AVFMTCTX_NOHEADER is set in ctx_flags, then new streams may also
             * appear in av_read_frame().
             * encoding: streams are created by the user before avformat_write_header().
             */
            [MarshalAs(UnmanagedType.U4)]
            public uint nb_streams;

            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_STREAMS)]
            //public IntPtr[] streams; // AVStream
            public IntPtr streams;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024, ArraySubType=UnmanagedType.U1)]
            public char[] filename; // input or output filename

            //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            //public string filename;

            /**
             * Decoding: position of the first frame of the component, in
             * AV_TIME_BASE fractional seconds. NEVER set this value directly:
             * It is deduced from the AVStream values.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 start_time;

            /**
             * Decoding: duration of the stream, in AV_TIME_BASE fractional
             * seconds. Only set this value if you know none of the individual stream
             * durations and also do not set any of them. This is deduced from the
             * AVStream values if not set.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 duration;

            /**
             * Decoding: total stream bitrate in bit/s, 0 if not
             * available. Never set it directly if the file_size and the
             * duration are known as FFmpeg can compute it automatically.
             */
            [MarshalAs(UnmanagedType.I4)]
            public int bit_rate;

            [MarshalAs(UnmanagedType.U4)]
            public uint packet_size;

            [MarshalAs(UnmanagedType.I4)]
            public int max_delay;

            [MarshalAs(UnmanagedType.I4)]
            public int flags;

            /**
             * decoding: size of data to probe; encoding: unused.
             */
            [MarshalAs(UnmanagedType.U4)]
            public uint probesize; // decoding: size of data to probe; encoding unused

            /**
             * decoding: maximum time (in AV_TIME_BASE units) during which the input should
             * be analyzed in avformat_find_stream_info().
             */
            [MarshalAs(UnmanagedType.I4)]
            public int max_analyze_duration;

            public IntPtr key;
            [MarshalAs(UnmanagedType.I4)]
            public int keylen;

            [MarshalAs(UnmanagedType.U4)]
            public uint nb_programs;
            public IntPtr programs;

            /**
             * Forced video codec_id.
             * Demuxing: Set by user.
             */
            public AVCodecID video_codec_id;

            /**
             * Forced audio codec_id.
             * Demuxing: Set by user.
             */
            public AVCodecID audio_codec_id;

            /**
             * Forced subtitle codec_id.
             * Demuxing: Set by user.
             */
            public AVCodecID subtitle_codec_id;

            /**
             * Maximum amount of memory in bytes to use for the index of each stream.
             * If the index exceeds this size, entries will be discarded as
             * needed to maintain a smaller size. This can lead to slower or less
             * accurate seeking (depends on demuxer).
             * Demuxers for which a full in-memory index is mandatory will ignore
             * this.
             * muxing  : unused
             * demuxing: set by user
             */
            [MarshalAs(UnmanagedType.U4)]
            public uint max_index_size;

            /**
             * Maximum amount of memory in bytes to use for buffering frames
             * obtained from realtime capture devices.
             */
            [MarshalAs(UnmanagedType.U4)]
            public uint max_picture_buffer;

            [MarshalAs(UnmanagedType.U4)]
            public uint nb_chapters;
            
            public IntPtr chapters;

            public IntPtr metadata;

            /**
             * Start time of the stream in real world time, in microseconds
             * since the unix epoch (00:00 1st January 1970). That is, pts=0
             * in the stream was captured at this real world time.
             * - encoding: Set by user.
             * - decoding: Unused.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 start_time_realtime;

            /**
             * decoding: number of frames used to probe fps
             */
            [MarshalAs(UnmanagedType.I4)]
            public int fps_probe_size;

            /**
             * Error recognition; higher values will detect more errors but may
             * misdetect some more or less valid parts as errors.
             * - encoding: unused
             * - decoding: Set by user.
             */
            [MarshalAs(UnmanagedType.I4)]
            public int error_recognition;

            /**
             * Custom interrupt callbacks for the I/O layer.
             *
             * decoding: set by the user before avformat_open_input().
             * encoding: set by the user before avformat_write_header()
             * (mainly useful for AVFMT_NOFILE formats). The callback
             * should also be passed to avio_open2() if it's used to
             * open the file.
             */
            public IntPtr interrupt_callback;

            /**
             * Flags to enable debugging.
             */
            [MarshalAs(UnmanagedType.I4)]
            public int debug;

            /**
             * Transport stream id.
             * This will be moved into demuxer private options. Thus no API/ABI compatibility
             */
            [MarshalAs(UnmanagedType.I4)]
            public int ts_id;

            /**
             * Audio preload in microseconds.
             * Note, not all formats support this and unpredictable things may happen if it is used when not supported.
             * - encoding: Set by user via AVOptions (NO direct access)
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.I4)]
            public int audio_preload;

            /**
             * Max chunk time in microseconds.
             * Note, not all formats support this and unpredictable things may happen if it is used when not supported.
             * - encoding: Set by user via AVOptions (NO direct access)
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.I4)]
            public int max_chunk_duration;

            /**
             * Max chunk size in bytes
             * Note, not all formats support this and unpredictable things may happen if it is used when not supported.
             * - encoding: Set by user via AVOptions (NO direct access)
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.I4)]
            public int max_chunk_size;

            /**
             * forces the use of wallclock timestamps as pts/dts of packets
             * This has undefined results in the presence of B frames.
             * - encoding: unused
             * - decoding: Set by user via AVOptions (NO direct access)
             */
            [MarshalAs(UnmanagedType.I4)]
            public int use_wallclock_as_timestamps;

            /**
             * Avoids negative timestamps during muxing
             *  0 -> allow negative timestamps
             *  1 -> avoid negative timestamps
             * -1 -> choose automatically (default)
             * Note, this is only works when interleave_packet_per_dts is in use
             * - encoding: Set by user via AVOptions (NO direct access)
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.I4)]
            public int avoid_negative_ts;

            /**
             * avio flags, used to force AVIO_FLAG_DIRECT.
             * - encoding: unused
             * - decoding: Set by user via AVOptions (NO direct access)
             */
            [MarshalAs(UnmanagedType.I4)]
            public int avio_flags;

            /**
             * The duration field can be estimated through various ways, and this field can be used
             * to know how the duration was estimated.
             * - encoding: unused
             * - decoding: Read by user via AVOptions (NO direct access)
             */
            [MarshalAs(UnmanagedType.I4)]
            public AVDurationEstimationMethod duration_estimation_method;

            /**
             * Skip initial bytes when opening stream
             * - encoding: unused
             * - decoding: Set by user via AVOptions (NO direct access)
             */
            [MarshalAs(UnmanagedType.U4)]
            public uint skip_initial_bytes;

            /**
             * Correct single timestamp overflows
             * - encoding: unused
             * - decoding: Set by user via AVOPtions (NO direct access)
             */
            [MarshalAs(UnmanagedType.U4)]
            public uint correct_ts_overflow;

    /*****************************************************************
     * All fields below this line are not part of the public API. They
     * may not be used outside of libavformat and can be changed and
     * removed at will.
     * New public fields should be added right above.
     *****************************************************************
     */

            /* This buffer is only needed when packets were already buffered but
               not decoded, for example to get the codec parameters in mpeg
               streams */

            public IntPtr packet_buffer; // AVPacketList
            public IntPtr packet_buffer_end;

            /* av_seek_frame() support */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 data_offset; // offset of the first packet

            /**
             * Raw packets from the demuxer, prior to parsing and decoding.
             * This buffer is used for buffering packets until the codec can
             * be identified, as parsing cannot be done without knowing the
             * codec.
             */
            public IntPtr raw_packet_buffer;
            public IntPtr raw_packet_buffer_end;

            /**
             * Packets split by the parser get queued here.
             */
            public IntPtr parse_queue;
            public IntPtr parse_queue_end;

            /**
             * Remaining size available for raw_packet_buffer, in bytes.
             */
            [MarshalAs(UnmanagedType.I4)]
            public int raw_packet_buffer_remaining_size;

        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVPacketList
        {
            AVPacket pkt;
            IntPtr next; // AVPacketList
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVImageInfo
        {
            AVPixelFormat pix_fmt; // requested pixel format

            [MarshalAs(UnmanagedType.I4)]
            int width; // requested width

            [MarshalAs(UnmanagedType.I4)]
            int height; // requested height 

            [MarshalAs(UnmanagedType.I4)]
            int interleaved; // image is interleaved (e.g. interleaved GIF) 

            AVPicture pict; // returned allocated image           
        };

        public delegate int AllocCBCallback(IntPtr pVoid, IntPtr pAVImageInfo);

        public delegate int ImgProbeCallback(IntPtr pAVProbeData);
        public delegate int ImgReadCallback(IntPtr pByteIOContext,
                                            [MarshalAs(UnmanagedType.FunctionPtr)]
                                            AllocCBCallback alloc_cb,
                                            IntPtr pVoid);
        public delegate int ImgWriteCallback(IntPtr pByteIOContext, IntPtr pAVImageInfo);

        [StructLayout(LayoutKind.Sequential)]
        public struct AVImageFormat
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            String name;

            [MarshalAs(UnmanagedType.LPTStr)]
            String extensions;

            // tell if a given file has a chance of being parsing by this format
            [MarshalAs(UnmanagedType.FunctionPtr)]
            ImgProbeCallback img_probe;

            /* read a whole image. 'alloc_cb' is called when the image size is
               known so that the caller can allocate the image. If 'allo_cb'
               returns non zero, then the parsing is aborted. Return '0' if
               OK. */
            [MarshalAs(UnmanagedType.FunctionPtr)]
            ImgReadCallback img_read;

            /* write the image */
            int supported_pixel_formats; // mask of supported formats for output

            [MarshalAs(UnmanagedType.FunctionPtr)]
            ImgWriteCallback img_write;

            int flags;

            IntPtr next; // AVImageFormat
        };
    }
}
