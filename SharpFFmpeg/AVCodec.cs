using System;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpFFmpeg
{
    public partial class FFmpeg
    {
        private const string avcodecDllName = "avcodec-54.dll";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output_channels"></param>
        /// <param name="input_channels"></param>
        /// <param name="output_rate"></param>
        /// <param name="input_rate"></param>
        /// <returns>ReSampleContext pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr audio_resample_init(int output_channels, int input_channels,
                                                    int output_rate, int input_rate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pResampleContext"></param>
        /// <param name="output"></param>
        /// <param name="intput"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int audio_resample(IntPtr pResampleContext, IntPtr output, IntPtr intput, int nb_samples);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pResampleContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void audio_resample_close(IntPtr pResampleContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="out_rate"></param>
        /// <param name="in_rate"></param>
        /// <param name="filter_length"></param>
        /// <param name="log2_phase_count"></param>
        /// <param name="linear"></param>
        /// <param name="cutoff"></param>
        /// <returns>AVResampleContext pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_resample_init(int out_rate, int in_rate, int filter_length, int log2_phase_count, int linear, double cutoff);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVResampleContext"></param>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        /// <param name="consumed"></param>
        /// <param name="src_size"></param>
        /// <param name="udpate_ctx"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_resample(IntPtr pAVResampleContext, IntPtr dst, IntPtr src, IntPtr consumed, int src_size, int udpate_ctx);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVResampleContext"></param>
        /// <param name="sample_delta"></param>
        /// <param name="compensation_distance"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_resample_compensate(IntPtr pAVResampleContext, int sample_delta, int compensation_distance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVResampleContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_resample_close(IntPtr pAVResampleContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output_width"></param>
        /// <param name="output_height"></param>
        /// <param name="input_width"></param>
        /// <param name="input_height"></param>
        /// <returns>ImgReSampleContext pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr img_resample_init(int output_width, int output_height,
                                      int input_width, int input_height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owidth"></param>
        /// <param name="oheight"></param>
        /// <param name="iwidth"></param>
        /// <param name="iheight"></param>
        /// <param name="topBand"></param>
        /// <param name="bottomBand"></param>
        /// <param name="leftBand"></param>
        /// <param name="rightBand"></param>
        /// <param name="padtop"></param>
        /// <param name="padbottom"></param>
        /// <param name="padleft"></param>
        /// <param name="padright"></param>
        /// <returns>ImgReSampleContext pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr img_resample_full_init(int owidth, int oheight,
                                      int iwidth, int iheight,
                                      int topBand, int bottomBand,
                                      int leftBand, int rightBand,
                                      int padtop, int padbottom,
                                      int padleft, int padright);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pImgReSampleContext"></param>
        /// <param name="p_output_AVPicture"></param>
        /// <param name="p_input_AVPicture"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void img_resample(IntPtr pImgReSampleContext, IntPtr p_output_AVPicture, IntPtr p_input_AVPicture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pImgReSampleContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void ImgReSampleContext(IntPtr pImgReSampleContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPicture"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>        
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avpicture_alloc(IntPtr pAVPicture, int pix_fmt, int width, int height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPicture"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avpicture_free(IntPtr pAVPicture);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPicture"></param>
        /// <param name="ptr"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avpicture_fill(IntPtr pAVPicture, IntPtr ptr, AVPixelFormat pix_fmt, int width, int height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_src_AVPicture"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dest"></param>
        /// <param name="dest_size"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avpicture_layout(IntPtr p_src_AVPicture, int pix_fmt, int width, int height,
                                           IntPtr dest, int dest_size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pix_fmt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avpicture_get_size(int pix_fmt, int width, int height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pix_fmt"></param>
        /// <param name="h_shift"></param>
        /// <param name="v_shift"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_get_chroma_sub_sample(int pix_fmt, IntPtr h_shift, IntPtr v_shift);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pix_fmt"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern String avcodec_get_pix_fmt_name(int pix_fmt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_set_dimensions(IntPtr pAVCodecContext, int width, int height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern AVPixelFormat avcodec_get_pix_fmt([MarshalAs(UnmanagedType.LPStr)]String name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern uint avcodec_pix_fmt_to_codec_tag(AVPixelFormat p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dst_pix_fmt"></param>
        /// <param name="src_pix_fmt"></param>
        /// <param name="has_alpha"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_get_pix_fmt_loss(int dst_pix_fmt, int src_pix_fmt, int has_alpha);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pix_fmt_mask"></param>
        /// <param name="src_pix_fmt"></param>
        /// <param name="has_alpha"></param>
        /// <param name="loss_ptr"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_find_best_pix_fmt(int pix_fmt_mask, int src_pix_fmt, int has_alpha, IntPtr loss_ptr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPicture"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int img_get_alpha_info(IntPtr pAVPicture, int pix_fmt, int width, int height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_dst_AVPicture"></param>
        /// <param name="dst_pix_fmt"></param>
        /// <param name="p_src_AVPicture"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int img_convert(IntPtr p_dst_AVPicture, int dst_pix_fmt,
                            IntPtr p_src_AVPicture, int pix_fmt, int width, int height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_dst_AVPicture"></param>
        /// <param name="p_src_AVPicture"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avpicture_deinterlace(IntPtr p_dst_AVPicture, IntPtr p_src_AVPicture,
                            int pix_fmt, int width, int height);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern uint avcodec_version();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern uint avcodec_build();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern uint avcodec_init();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodec"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void register_avcodec(IntPtr pAVCodec);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AVCodec pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr avcodec_find_encoder(CodecID id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mame"></param>
        /// <returns>AVCodec pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr avcodec_find_encoder_by_name(
                    [MarshalAs(UnmanagedType.LPStr)]String mame);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>AVCodec pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr avcodec_find_decoder(CodecID id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mame"></param>
        /// <returns>AVCodec pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr avcodec_find_decoder_by_name(
                    [MarshalAs(UnmanagedType.LPStr)]String mame);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mam"></param>
        /// <param name="buf_size"></param>
        /// <param name="pAVCodeContext"></param>
        /// <param name="encode"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_string(
                    [MarshalAs(UnmanagedType.LPStr)]String mam, int buf_size,
                    IntPtr pAVCodeContext, int encode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_get_context_defaults(IntPtr pAVCodecContext);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>AVCodecContext pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr avcodec_alloc_context();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVFrame"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_get_frame_defaults(IntPtr pAVFrame);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>AVFrame pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr avcodec_alloc_frame();

        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_free_frame(ref IntPtr ptr);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="pAVFrame"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_default_get_buffer(IntPtr pAVCodecContext, IntPtr pAVFrame);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="pAVFrame"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_default_release_buffer(IntPtr pAVCodecContext, IntPtr pAVFrame);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="pAVFrame"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_default_reget_buffer(IntPtr pAVCodecContext, IntPtr pAVFrame);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_align_dimensions(IntPtr pAVCodecContext, ref int width, ref int height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="av_log_ctx"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_check_dimensions(IntPtr av_log_ctx, ref uint width, ref uint height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="fmt"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern AVPixelFormat avcodec_default_get_format(IntPtr pAVCodecContext, ref AVPixelFormat fmt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="thread_count"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_thread_init(IntPtr pAVCodecContext, int thread_count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_thread_free(IntPtr pAVCodecContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="func"></param>
        /// <param name="arg"></param>
        /// <param name="ret"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_thread_execute(IntPtr pAVCodecContext,
                                [MarshalAs(UnmanagedType.FunctionPtr)]FuncCallback func,
                                [MarshalAs(UnmanagedType.LPArray)]IntPtr[] arg, ref int ret, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="func"></param>
        /// <param name="arg"></param>
        /// <param name="ret"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_default_execute(IntPtr pAVCodecContext,
                               [MarshalAs(UnmanagedType.FunctionPtr)]FuncCallback func,
                               [MarshalAs(UnmanagedType.LPArray)]IntPtr[] arg, ref int ret, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="pAVCodec"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_open(IntPtr pAVCodecContext, IntPtr pAVCodec);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="samples"></param>
        /// <param name="frame_size_ptr"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_decode_audio(IntPtr pAVCodecContext,
                                            IntPtr samples, out int frame_size_ptr,
                                            IntPtr buf, int buf_size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="pAVFrame"></param>
        /// <param name="got_picture_ptr"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <returns></returns>
        //[DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        //public static extern int avcodec_decode_video(IntPtr pAVCodecContext, IntPtr pAVFrame,
        //                                    ref int got_picture_ptr, IntPtr buf, int buf_size);

        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_decode_video2(IntPtr pAVCodecContext, IntPtr pAVFrame,
                                            ref int got_picture_ptr, IntPtr avpkt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="pAVSubtitle"></param>
        /// <param name="got_sub_ptr"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_decode_subtitle(IntPtr pAVCodecContext, IntPtr pAVSubtitle,
                                           ref int got_sub_ptr, IntPtr buf, int buf_size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="pdata"></param>
        /// <param name="data_size_ptr"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_parse_frame(IntPtr pAVCodecContext,
                                            [MarshalAs(UnmanagedType.LPArray)]IntPtr[] pdata,
                                            IntPtr data_size_ptr, IntPtr buf, int buf_size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <param name="samples"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_encode_audio(IntPtr pAVCodecContext, IntPtr buf, int buf_size,
                                            IntPtr samples);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <param name="pAVFrame"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_encode_video(IntPtr pAVCodecContext, IntPtr buf, int buf_size,
                                            IntPtr pAVFrame);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <param name="pAVSubtitle"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_encode_subtitle(IntPtr pAVCodecContext, IntPtr buf, int buf_size,
                                            IntPtr pAVSubtitle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int avcodec_close(IntPtr pAVCodecContext);

        /// <summary>
        /// 
        /// </summary>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_register_all();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_flush_buffers(IntPtr pAVCodecContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void avcodec_default_free_buffers(IntPtr pAVCodecContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pict_type"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern byte av_get_pict_type_char(int pict_type);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec_id"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_get_bits_per_sample(CodecID codec_id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVcodecParser"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_register_codec_parser(IntPtr pAVcodecParser);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codec_id"></param>
        /// <returns>AVCodecParserContext pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_parser_init(int codec_id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecParserContext"></param>
        /// <param name="pAVCodecContext"></param>
        /// <param name="poutbuf"></param>
        /// <param name="poutbuf_size"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <param name="pts"></param>
        /// <param name="dts"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_parser_parse(IntPtr pAVCodecParserContext,
                                                IntPtr pAVCodecContext,
                            [MarshalAs(UnmanagedType.LPArray)]IntPtr[] poutbuf, ref int poutbuf_size,
                            IntPtr buf, int buf_size, Int64 pts, Int64 dts);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecParserContext"></param>
        /// <param name="pAVCodecContext"></param>
        /// <param name="poutbuf"></param>
        /// <param name="poutbuf_size"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <param name="keyframe"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_parser_change(IntPtr pAVCodecParserContext,
                                                IntPtr pAVCodecContext,
                            [MarshalAs(UnmanagedType.LPArray)]IntPtr[] poutbuf, ref int poutbuf_size,
                            IntPtr buf, int buf_size, int keyframe);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVCodecParserContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_parser_close(IntPtr pAVCodecParserContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVBitStreamFilter"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_register_bitstream_filter(IntPtr pAVBitStreamFilter);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>AVBitStreamFilterContext pointer</returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_bitstream_filter_init([MarshalAs(UnmanagedType.LPStr)]
                                        String name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVBitStreamFilterContext"></param>
        /// <param name="pAVCodecContext"></param>
        /// <param name="args"></param>
        /// <param name="poutbuf"></param>
        /// <param name="poutbuf_size"></param>
        /// <param name="buf"></param>
        /// <param name="buf_size"></param>
        /// <param name="keyframe"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int av_bitstream_filter_filter(IntPtr pAVBitStreamFilterContext,
                                        IntPtr pAVCodecContext,
                                        [MarshalAs(UnmanagedType.LPStr)]String args,
                                        [MarshalAs(UnmanagedType.LPArray)]IntPtr[] poutbuf,
                                        ref  int poutbuf_size, IntPtr buf, int buf_size, int keyframe);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVBitStreamFilterContext"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_bitstream_filter_close(IntPtr pAVBitStreamFilterContext);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_mallocz(uint size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr av_strdup([MarshalAs(UnmanagedType.LPStr)]String s);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        [DllImport(avutilDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_freep(IntPtr ptr);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="size"></param>
        /// <param name="min_size"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_fast_realloc(IntPtr ptr, ref uint size, ref uint min_size);

        /// <summary>
        /// 
        /// </summary>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_free_static();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_mallocz_static(uint size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="size"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void av_realloc_static(IntPtr ptr, uint size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAVPicture"></param>
        /// <param name="p_src_AVPicture"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern void img_copy(IntPtr pAVPicture, IntPtr p_src_AVPicture,
                            int pix_fmt, int width, int height);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_dst_pAVPicture"></param>
        /// <param name="p_src_pAVPicture"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="top_band"></param>
        /// <param name="left_band"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int img_crop(IntPtr p_dst_pAVPicture, IntPtr p_src_pAVPicture,
                            int pix_fmt, int top_band, int left_band);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_dst_pAVPicture"></param>
        /// <param name="p_src_pAVPicture"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="pix_fmt"></param>
        /// <param name="padtop"></param>
        /// <param name="padbottom"></param>
        /// <param name="padleft"></param>
        /// <param name="padright"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        [DllImport(avcodecDllName), SuppressUnmanagedCodeSecurity]
        public static extern int img_pad(IntPtr p_dst_pAVPicture, IntPtr p_src_pAVPicture,
                            int height, int width, int pix_fmt, int padtop, int padbottom,
                            int padleft, int padright, ref int color);

        // *********************************************************************************
        // Constants
        // *********************************************************************************

        // 1 second of 48khz 32bit audio in bytes
        public const int AVCODEC_MAX_AUDIO_FRAME_SIZE = 192000;

        /**
      * Required number of additionally allocated bytes at the end of the input bitstream for decoding.
      * this is mainly needed because some optimized bitstream readers read
      * 32 or 64 bit at once and could read over the end<br>
      * Note, if the first 23 bits of the additional bytes are not 0 then damaged
      * MPEG bitstreams could cause overread and segfault
      */
        public const int FF_INPUT_BUFFER_PADDING_SIZE = 8;

        /**
         * minimum encoding buffer size.
         * used to avoid some checks during header writing
         */
        public const int FF_MIN_BUFFER_SIZE = 16384;

        public const int FF_MAX_B_FRAMES = 16;

        /* encoding support
           these flags can be passed in AVCodecContext.flags before initing
           Note: not everything is supported yet.
        */

        public const int CODEC_FLAG_QSCALE = 0x0002;  // use fixed qscale
        public const int CODEC_FLAG_4MV = 0x0004;  // 4 MV per MB allowed / Advanced prediction for H263
        public const int CODEC_FLAG_QPEL = 0x0010;  // use qpel MC
        public const int CODEC_FLAG_GMC = 0x0020;  // use GMC
        public const int CODEC_FLAG_MV0 = 0x0040;  // always try a MB with MV=<0,0>
        public const int CODEC_FLAG_PART = 0x0080;  // use data partitioning
        /* parent program gurantees that the input for b-frame containing streams is not written to
           for at least s->max_b_frames+1 frames, if this is not set than the input will be copied */
        public const int CODEC_FLAG_INPUT_PRESERVED = 0x0100;
        public const int CODEC_FLAG_PASS1 = 0x0200;   // use internal 2pass ratecontrol in first  pass mode
        public const int CODEC_FLAG_PASS2 = 0x0400;   // use internal 2pass ratecontrol in second pass mode
        public const int CODEC_FLAG_EXTERN_HUFF = 0x1000; // use external huffman table (for mjpeg)
        public const int CODEC_FLAG_GRAY = 0x2000;   // only decode/encode grayscale
        public const int CODEC_FLAG_EMU_EDGE = 0x4000;// don't draw edges
        public const int CODEC_FLAG_PSNR = 0x8000; // error[?] variables will be set during encoding
        public const int CODEC_FLAG_TRUNCATED = 0x00010000; /** input bitstream might be truncated at a random location instead
                                            of only at frame boundaries */
        public const int CODEC_FLAG_NORMALIZE_AQP = 0x00020000; // normalize adaptive quantization
        public const int CODEC_FLAG_INTERLACED_DCT = 0x00040000; // use interlaced dct
        public const int CODEC_FLAG_LOW_DELAY = 0x00080000; // force low delay
        public const int CODEC_FLAG_ALT_SCAN = 0x00100000; // use alternate scan
        public const int CODEC_FLAG_TRELLIS_QUANT = 0x00200000; // use trellis quantization
        public const int CODEC_FLAG_GLOBAL_HEADER = 0x00400000; // place global headers in extradata instead of every keyframe
        public const int CODEC_FLAG_BITEXACT = 0x00800000; // use only bitexact stuff (except (i)dct)
        /* Fx : Flag for h263+ extra options */
        public const int CODEC_FLAG_H263P_AIC = 0x01000000; // H263 Advanced intra coding / MPEG4 AC prediction (remove this)
        public const int CODEC_FLAG_AC_PRED = 0x01000000; // H263 Advanced intra coding / MPEG4 AC prediction
        public const int CODEC_FLAG_H263P_UMV = 0x02000000; // Unlimited motion vector
        public const int CODEC_FLAG_CBP_RD = 0x04000000; // use rate distortion optimization for cbp
        public const int CODEC_FLAG_QP_RD = 0x08000000; // use rate distortion optimization for qp selectioon
        public const int CODEC_FLAG_H263P_AIV = 0x00000008; // H263 Alternative inter vlc
        public const int CODEC_FLAG_OBMC = 0x00000001; // OBMC
        public const int CODEC_FLAG_LOOP_FILTER = 0x00000800; // loop filter
        public const int CODEC_FLAG_H263P_SLICE_STRUCT = 0x10000000;
        public const int CODEC_FLAG_INTERLACED_ME = 0x20000000; // interlaced motion estimation
        public const int CODEC_FLAG_SVCD_SCAN_OFFSET = 0x40000000; // will reserve space for SVCD scan offset user data
        public const uint CODEC_FLAG_CLOSED_GOP = ((uint)0x80000000);
        public const int CODEC_FLAG2_FAST = 0x00000001; // allow non spec compliant speedup tricks
        public const int CODEC_FLAG2_STRICT_GOP = 0x00000002; // strictly enforce GOP size
        public const int CODEC_FLAG2_NO_OUTPUT = 0x00000004; // skip bitstream encoding
        public const int CODEC_FLAG2_LOCAL_HEADER = 0x00000008; // place global headers at every keyframe instead of in extradata
        public const int CODEC_FLAG2_BPYRAMID = 0x00000010; // H.264 allow b-frames to be used as references
        public const int CODEC_FLAG2_WPRED = 0x00000020; // H.264 weighted biprediction for b-frames
        public const int CODEC_FLAG2_MIXED_REFS = 0x00000040; // H.264 multiple references per partition
        public const int CODEC_FLAG2_8X8DCT = 0x00000080; // H.264 high profile 8x8 transform
        public const int CODEC_FLAG2_FASTPSKIP = 0x00000100; // H.264 fast pskip
        public const int CODEC_FLAG2_AUD = 0x00000200; // H.264 access unit delimiters
        public const int CODEC_FLAG2_BRDO = 0x00000400; // b-frame rate-distortion optimization
        public const int CODEC_FLAG2_INTRA_VLC = 0x00000800; // use MPEG-2 intra VLC table
        public const int CODEC_FLAG2_MEMC_ONLY = 0x00001000; // only do ME/MC (I frames -> ref, P frame -> ME+MC)

        /* Unsupported options :
         * Syntax Arithmetic coding (SAC)
         * Reference Picture Selection
         * Independant Segment Decoding */
        /* /Fx */
        /* codec capabilities */
        public const int CODEC_CAP_DRAW_HORIZ_BAND = 0x0001; // decoder can use draw_horiz_band callback

        /**
         * Codec uses get_buffer() for allocating buffers.
         * direct rendering method 1
         */
        public const int CODEC_CAP_DR1 = 0x0002;

        /* if 'parse_only' field is true, then avcodec_parse_frame() can be
            used */
        public const int CODEC_CAP_PARSE_ONLY = 0x0004;
        public const int CODEC_CAP_TRUNCATED = 0x0008;

        /* codec can export data for HW decoding (XvMC) */
        public const int CODEC_CAP_HWACCEL = 0x0010;

        /**
         * codec has a non zero delay and needs to be feeded with NULL at the end to get the delayed data.
         * if this is not set, the codec is guranteed to never be feeded with NULL data
         */
        public const int CODEC_CAP_DELAY = 0x0020;

        /**
         * Codec can be fed a final frame with a smaller size.
         * This can be used to prevent truncation of the last audio samples.
        */
        public const int CODEC_CAP_SMALL_LAST_FRAME = 0x0040;

        //the following defines may change, don't expect compatibility if you use them
        public const int MB_TYPE_INTRA4x4 = 0x001;
        public const int MB_TYPE_INTRA16x16 = 0x0002; //FIXME h264 specific
        public const int MB_TYPE_INTRA_PCM = 0x0004; //FIXME h264 specific
        public const int MB_TYPE_16x16 = 0x0008;
        public const int MB_TYPE_16x8 = 0x0010;
        public const int MB_TYPE_8x16 = 0x0020;
        public const int MB_TYPE_8x8 = 0x0040;
        public const int MB_TYPE_INTERLACED = 0x0080;
        public const int MB_TYPE_DIRECT2 = 0x0100; //FIXME
        public const int MB_TYPE_ACPRED = 0x0200;
        public const int MB_TYPE_GMC = 0x0400;
        public const int MB_TYPE_SKIP = 0x0800;
        public const int MB_TYPE_P0L0 = 0x1000;
        public const int MB_TYPE_P1L0 = 0x2000;
        public const int MB_TYPE_P0L1 = 0x4000;
        public const int MB_TYPE_P1L1 = 0x8000;
        public const int MB_TYPE_L0 = (MB_TYPE_P0L0 | MB_TYPE_P1L0);
        public const int MB_TYPE_L1 = (MB_TYPE_P0L1 | MB_TYPE_P1L1);
        public const int MB_TYPE_L0L1 = (MB_TYPE_L0 | MB_TYPE_L1);
        public const int MB_TYPE_QUANT = 0x00010000;
        public const int MB_TYPE_CBP = 0x00020000;
        //Note bits 24-31 are reserved for codec specific use (h264 ref0, mpeg1 0mv, ...)

        public const int FF_QSCALE_TYPE_MPEG1 = 0;
        public const int FF_QSCALE_TYPE_MPEG2 = 1;
        public const int FF_QSCALE_TYPE_H264 = 2;

        public const int FF_BUFFER_TYPE_INTERNAL = 1;
        public const int FF_BUFFER_TYPE_USER = 2; // Direct rendering buffers (image is (de)allocated by user)
        public const int FF_BUFFER_TYPE_SHARED = 4; // buffer from somewhere else, don't dealloc image (data/base), all other tables are not shared
        public const int FF_BUFFER_TYPE_COPY = 8; // just a (modified) copy of some other buffer, don't dealloc anything

        public const int FF_I_TYPE = 1; // Intra
        public const int FF_P_TYPE = 2; // Predicted
        public const int FF_B_TYPE = 3; // Bi-dir predicted
        public const int FF_S_TYPE = 4; // S(GMC)-VOP MPEG4
        public const int FF_SI_TYPE = 5;
        public const int FF_SP_TYPE = 6;

        public const int FF_BUFFER_HINTS_VALID = 0x01; // Buffer hints value is meaningful (if 0 ignore)
        public const int FF_BUFFER_HINTS_READABLE = 0x02; // Codec will read from buffer
        public const int FF_BUFFER_HINTS_PRESERVE = 0x04; // User must not alter buffer content
        public const int FF_BUFFER_HINTS_REUSABLE = 0x08; // Codec will reuse the buffer (update)

        public const int DEFAULT_FRAME_RATE_BASE = 1001000;

        public const int FF_ASPECT_EXTENDED = 15;

        public const int FF_RC_STRATEGY_XVID = 1;

        public const int FF_BUG_AUTODETECT = 1;  // autodetection
        public const int FF_BUG_OLD_MSMPEG4 = 2;
        public const int FF_BUG_XVID_ILACE = 4;
        public const int FF_BUG_UMP4 = 8;
        public const int FF_BUG_NO_PADDING = 16;
        public const int FF_BUG_AMV = 32;
        public const int FF_BUG_AC_VLC = 0;  // will be removed, libavcodec can now handle these non compliant files by default
        public const int FF_BUG_QPEL_CHROMA = 64;
        public const int FF_BUG_STD_QPEL = 128;
        public const int FF_BUG_QPEL_CHROMA2 = 256;
        public const int FF_BUG_DIRECT_BLOCKSIZE = 512;
        public const int FF_BUG_EDGE = 1024;
        public const int FF_BUG_HPEL_CHROMA = 2048;
        public const int FF_BUG_DC_CLIP = 4096;
        public const int FF_BUG_MS = 8192; // workaround various bugs in microsofts broken decoders
        // public const int FF_BUG_FAKE_SCALABILITY =16; //autodetection should work 100%

        public const int FF_COMPLIANCE_VERY_STRICT = 2; // strictly conform to a older more strict version of the spec or reference software
        public const int FF_COMPLIANCE_STRICT = 1; // strictly conform to all the things in the spec no matter what consequences
        public const int FF_COMPLIANCE_NORMAL = 0;
        public const int FF_COMPLIANCE_INOFFICIAL = -1; // allow inofficial extensions
        public const int FF_COMPLIANCE_EXPERIMENTAL = -2; // allow non standarized experimental things

        public const int FF_ER_CAREFUL = 1;
        public const int FF_ER_COMPLIANT = 2;
        public const int FF_ER_AGGRESSIVE = 3;
        public const int FF_ER_VERY_AGGRESSIVE = 4;

        public const int FF_DCT_AUTO = 0;
        public const int FF_DCT_FASTINT = 1;
        public const int FF_DCT_INT = 2;
        public const int FF_DCT_MMX = 3;
        public const int FF_DCT_MLIB = 4;
        public const int FF_DCT_ALTIVEC = 5;
        public const int FF_DCT_FAAN = 6;

        public const int FF_IDCT_AUTO = 0;
        public const int FF_IDCT_INT = 1;
        public const int FF_IDCT_SIMPLE = 2;
        public const int FF_IDCT_SIMPLEMMX = 3;
        public const int FF_IDCT_LIBMPEG2MMX = 4;
        public const int FF_IDCT_PS2 = 5;
        public const int FF_IDCT_MLIB = 6;
        public const int FF_IDCT_ARM = 7;
        public const int FF_IDCT_ALTIVEC = 8;
        public const int FF_IDCT_SH4 = 9;
        public const int FF_IDCT_SIMPLEARM = 10;
        public const int FF_IDCT_H264 = 11;
        public const int FF_IDCT_VP3 = 12;
        public const int FF_IDCT_IPP = 13;
        public const int FF_IDCT_XVIDMMX = 14;
        public const int FF_IDCT_CAVS = 15;

        public const int FF_EC_GUESS_MVS = 1;
        public const int FF_EC_DEBLOCK = 2;

        public const uint FF_MM_FORCE = 0x80000000; /* force usage of selected flags (OR) */
        //    /* lower 16 bits - CPU features */

        public const int FF_MM_MMX = 0x0001; /* standard MMX */
        public const int FF_MM_3DNOW = 0x0004; /* AMD 3DNOW */
        public const int FF_MM_MMXEXT = 0x0002;/* SSE integer functions or AMD MMX ext */
        public const int FF_MM_SSE = 0x0008; /* SSE functions */
        public const int FF_MM_SSE2 = 0x0010;/* PIV SSE2 functions */
        public const int FF_MM_3DNOWEXT = 0x0020;/* AMD 3DNowExt */

        public const int FF_MM_IWMMXT = 0x0100; /* XScale IWMMXT */

        public const int FF_PRED_LEFT = 0;
        public const int FF_PRED_PLANE = 1;
        public const int FF_PRED_MEDIAN = 2;

        public const int FF_DEBUG_PICT_INFO = 1;
        public const int FF_DEBUG_RC = 2;
        public const int FF_DEBUG_BITSTREAM = 4;
        public const int FF_DEBUG_MB_TYPE = 8;
        public const int FF_DEBUG_QP = 16;
        public const int FF_DEBUG_MV = 32;
        public const int FF_DEBUG_DCT_COEFF = 0x00000040;
        public const int FF_DEBUG_SKIP = 0x00000080;
        public const int FF_DEBUG_STARTCODE = 0x00000100;
        public const int FF_DEBUG_PTS = 0x00000200;
        public const int FF_DEBUG_ER = 0x00000400;
        public const int FF_DEBUG_MMCO = 0x00000800;
        public const int FF_DEBUG_BUGS = 0x00001000;
        public const int FF_DEBUG_VIS_QP = 0x00002000;
        public const int FF_DEBUG_VIS_MB_TYPE = 0x00004000;

        public const int FF_DEBUG_VIS_MV_P_FOR = 0x00000001; //visualize forward predicted MVs of P frames
        public const int FF_DEBUG_VIS_MV_B_FOR = 0x00000002; //visualize forward predicted MVs of B frames
        public const int FF_DEBUG_VIS_MV_B_BACK = 0x00000004; //visualize backward predicted MVs of B frames

        public const int FF_CMP_SAD = 0;
        public const int FF_CMP_SSE = 1;
        public const int FF_CMP_SATD = 2;
        public const int FF_CMP_DCT = 3;
        public const int FF_CMP_PSNR = 4;
        public const int FF_CMP_BIT = 5;
        public const int FF_CMP_RD = 6;
        public const int FF_CMP_ZERO = 7;
        public const int FF_CMP_VSAD = 8;
        public const int FF_CMP_VSSE = 9;
        public const int FF_CMP_NSSE = 10;
        public const int FF_CMP_W53 = 11;
        public const int FF_CMP_W97 = 12;
        public const int FF_CMP_DCTMAX = 13;
        public const int FF_CMP_DCT264 = 14;
        public const int FF_CMP_CHROMA = 256;

        public const int FF_DTG_AFD_SAME = 8;
        public const int FF_DTG_AFD_4_3 = 9;
        public const int FF_DTG_AFD_16_9 = 10;
        public const int FF_DTG_AFD_14_9 = 11;
        public const int FF_DTG_AFD_4_3_SP_14_9 = 13;
        public const int FF_DTG_AFD_16_9_SP_14_9 = 14;
        public const int FF_DTG_AFD_SP_4_3 = 15;

        public const int FF_DEFAULT_QUANT_BIAS = 999999;

        public const int FF_LAMBDA_SHIFT = 7;
        public const int FF_LAMBDA_SCALE = (1 << FF_LAMBDA_SHIFT);
        public const int FF_QP2LAMBDA = 118; // factor to convert from H.263 QP to lambda
        public const int FF_LAMBDA_MAX = (256 * 128 - 1);

        public const int FF_CODER_TYPE_VLC = 0;
        public const int FF_CODER_TYPE_AC = 1;

        public const int SLICE_FLAG_CODED_ORDER = 0x0001; // draw_horiz_band() is called in coded order instead of display
        public const int SLICE_FLAG_ALLOW_FIELD = 0x0002; // allow draw_horiz_band() with field slices (MPEG2 field pics)
        public const int SLICE_FLAG_ALLOW_PLANE = 0x0004; // allow draw_horiz_band() with 1 component at a time (SVQ1)


        public const int FF_MB_DECISION_SIMPLE = 0; // uses mb_cmp
        public const int FF_MB_DECISION_BITS = 1; // chooses the one which needs the fewest bits
        public const int FF_MB_DECISION_RD = 2; // rate distoration

        public const int FF_AA_AUTO = 0;
        public const int FF_AA_FASTINT = 1; //not implemented yet
        public const int FF_AA_INT = 2;
        public const int FF_AA_FLOAT = 3;

        public const int FF_PROFILE_UNKNOWN = -99;

        public const int FF_LEVEL_UNKNOWN = -99;

        public const int X264_PART_I4X4 = 0x001; /* Analyse i4x4 */
        public const int X264_PART_I8X8 = 0x002; /* Analyse i8x8 (requires 8x8 transform) */
        public const int X264_PART_P8X8 = 0x010; /* Analyse p16x8, p8x16 and p8x8 */
        public const int X264_PART_P4X4 = 0x020; /* Analyse p8x4, p4x8, p4x4 */
        public const int X264_PART_B8X8 = 0x100; /* Analyse b16x8, b8x16 and b8x8 */

        public const int FF_COMPRESSION_DEFAULT = -1;

        public const int AVPALETTE_SIZE = 1024;
        public const int AVPALETTE_COUNT = 256;

        public const int FF_LOSS_RESOLUTION = 0x0001; /* loss due to resolution change */
        public const int FF_LOSS_DEPTH = 0x0002; /* loss due to color depth change */
        public const int FF_LOSS_COLORSPACE = 0x0004; /* loss due to color space conversion */
        public const int FF_LOSS_ALPHA = 0x0008; /* loss of alpha bits */
        public const int FF_LOSS_COLORQUANT = 0x0010; /* loss due to color quantization */
        public const int FF_LOSS_CHROMA = 0x0020; /* loss of chroma (e.g. rgb to gray conversion) */

        public const int FF_ALPHA_TRANSP = 0x0001; // image has some totally transparent pixels 
        public const int FF_ALPHA_SEMI_TRANSP = 0x0002; // image has some transparent pixels 

        public const int AV_PARSER_PTS_NB = 4;

        public const int PARSER_FLAG_COMPLETE_FRAMES = 0x0001;


        // *********************************************************************************
        // Enums
        // *********************************************************************************

        public enum CodecID
        {
            CODEC_ID_NONE,
            CODEC_ID_MPEG1VIDEO,
            CODEC_ID_MPEG2VIDEO, /* prefered ID for MPEG Video 1 or 2 decoding */
            CODEC_ID_MPEG2VIDEO_XVMC,
            CODEC_ID_H261,
            CODEC_ID_H263,
            CODEC_ID_RV10,
            CODEC_ID_RV20,
            CODEC_ID_MJPEG,
            CODEC_ID_MJPEGB,
            CODEC_ID_LJPEG,
            CODEC_ID_SP5X,
            CODEC_ID_JPEGLS,
            CODEC_ID_MPEG4,
            CODEC_ID_RAWVIDEO,
            CODEC_ID_MSMPEG4V1,
            CODEC_ID_MSMPEG4V2,
            CODEC_ID_MSMPEG4V3,
            CODEC_ID_WMV1,
            CODEC_ID_WMV2,
            CODEC_ID_H263P,
            CODEC_ID_H263I,
            CODEC_ID_FLV1,
            CODEC_ID_SVQ1,
            CODEC_ID_SVQ3,
            CODEC_ID_DVVIDEO,
            CODEC_ID_HUFFYUV,
            CODEC_ID_CYUV,
            CODEC_ID_H264,
            CODEC_ID_INDEO3,
            CODEC_ID_VP3,
            CODEC_ID_THEORA,
            CODEC_ID_ASV1,
            CODEC_ID_ASV2,
            CODEC_ID_FFV1,
            CODEC_ID_4XM,
            CODEC_ID_VCR1,
            CODEC_ID_CLJR,
            CODEC_ID_MDEC,
            CODEC_ID_ROQ,
            CODEC_ID_INTERPLAY_VIDEO,
            CODEC_ID_XAN_WC3,
            CODEC_ID_XAN_WC4,
            CODEC_ID_RPZA,
            CODEC_ID_CINEPAK,
            CODEC_ID_WS_VQA,
            CODEC_ID_MSRLE,
            CODEC_ID_MSVIDEO1,
            CODEC_ID_IDCIN,
            CODEC_ID_8BPS,
            CODEC_ID_SMC,
            CODEC_ID_FLIC,
            CODEC_ID_TRUEMOTION1,
            CODEC_ID_VMDVIDEO,
            CODEC_ID_MSZH,
            CODEC_ID_ZLIB,
            CODEC_ID_QTRLE,
            CODEC_ID_SNOW,
            CODEC_ID_TSCC,
            CODEC_ID_ULTI,
            CODEC_ID_QDRAW,
            CODEC_ID_VIXL,
            CODEC_ID_QPEG,
            CODEC_ID_XVID,
            CODEC_ID_PNG,
            CODEC_ID_PPM,
            CODEC_ID_PBM,
            CODEC_ID_PGM,
            CODEC_ID_PGMYUV,
            CODEC_ID_PAM,
            CODEC_ID_FFVHUFF,
            CODEC_ID_RV30,
            CODEC_ID_RV40,
            CODEC_ID_VC1,
            CODEC_ID_WMV3,
            CODEC_ID_LOCO,
            CODEC_ID_WNV1,
            CODEC_ID_AASC,
            CODEC_ID_INDEO2,
            CODEC_ID_FRAPS,
            CODEC_ID_TRUEMOTION2,
            CODEC_ID_BMP,
            CODEC_ID_CSCD,
            CODEC_ID_MMVIDEO,
            CODEC_ID_ZMBV,
            CODEC_ID_AVS,
            CODEC_ID_SMACKVIDEO,
            CODEC_ID_NUV,
            CODEC_ID_KMVC,
            CODEC_ID_FLASHSV,
            CODEC_ID_CAVS,
            CODEC_ID_JPEG2000,
            CODEC_ID_VMNC,
            CODEC_ID_VP5,
            CODEC_ID_VP6,
            CODEC_ID_VP6F,

            /* various pcm "codecs" */
            CODEC_ID_PCM_S16LE = 0x10000,
            CODEC_ID_PCM_S16BE,
            CODEC_ID_PCM_U16LE,
            CODEC_ID_PCM_U16BE,
            CODEC_ID_PCM_S8,
            CODEC_ID_PCM_U8,
            CODEC_ID_PCM_MULAW,
            CODEC_ID_PCM_ALAW,
            CODEC_ID_PCM_S32LE,
            CODEC_ID_PCM_S32BE,
            CODEC_ID_PCM_U32LE,
            CODEC_ID_PCM_U32BE,
            CODEC_ID_PCM_S24LE,
            CODEC_ID_PCM_S24BE,
            CODEC_ID_PCM_U24LE,
            CODEC_ID_PCM_U24BE,
            CODEC_ID_PCM_S24DAUD,

            /* various adpcm codecs */
            CODEC_ID_ADPCM_IMA_QT = 0x11000,
            CODEC_ID_ADPCM_IMA_WAV,
            CODEC_ID_ADPCM_IMA_DK3,
            CODEC_ID_ADPCM_IMA_DK4,
            CODEC_ID_ADPCM_IMA_WS,
            CODEC_ID_ADPCM_IMA_SMJPEG,
            CODEC_ID_ADPCM_MS,
            CODEC_ID_ADPCM_4XM,
            CODEC_ID_ADPCM_XA,
            CODEC_ID_ADPCM_ADX,
            CODEC_ID_ADPCM_EA,
            CODEC_ID_ADPCM_G726,
            CODEC_ID_ADPCM_CT,
            CODEC_ID_ADPCM_SWF,
            CODEC_ID_ADPCM_YAMAHA,
            CODEC_ID_ADPCM_SBPRO_4,
            CODEC_ID_ADPCM_SBPRO_3,
            CODEC_ID_ADPCM_SBPRO_2,

            /* AMR */
            CODEC_ID_AMR_NB = 0x12000,
            CODEC_ID_AMR_WB,

            /* RealAudio codecs*/
            CODEC_ID_RA_144 = 0x13000,
            CODEC_ID_RA_288,

            /* various DPCM codecs */
            CODEC_ID_ROQ_DPCM = 0x14000,
            CODEC_ID_INTERPLAY_DPCM,
            CODEC_ID_XAN_DPCM,
            CODEC_ID_SOL_DPCM,

            CODEC_ID_MP2 = 0x15000,
            CODEC_ID_MP3, /* prefered ID for MPEG Audio layer 1, 2 or3 decoding */
            CODEC_ID_AAC,
            CODEC_ID_MPEG4AAC,
            CODEC_ID_AC3,
            CODEC_ID_DTS,
            CODEC_ID_VORBIS,
            CODEC_ID_DVAUDIO,
            CODEC_ID_WMAV1,
            CODEC_ID_WMAV2,
            CODEC_ID_MACE3,
            CODEC_ID_MACE6,
            CODEC_ID_VMDAUDIO,
            CODEC_ID_SONIC,
            CODEC_ID_SONIC_LS,
            CODEC_ID_FLAC,
            CODEC_ID_MP3ADU,
            CODEC_ID_MP3ON4,
            CODEC_ID_SHORTEN,
            CODEC_ID_ALAC,
            CODEC_ID_WESTWOOD_SND1,
            CODEC_ID_GSM,
            CODEC_ID_QDM2,
            CODEC_ID_COOK,
            CODEC_ID_TRUESPEECH,
            CODEC_ID_TTA,
            CODEC_ID_SMACKAUDIO,
            CODEC_ID_QCELP,

            /* subtitle codecs */
            CODEC_ID_DVD_SUBTITLE = 0x17000,
            CODEC_ID_DVB_SUBTITLE,

            CODEC_ID_MPEG2TS = 0x20000, /* _FAKE_ codec to indicate a raw MPEG2 transport
                         stream (only used by libavformat) */
        };

        public enum CodecType
        {
            CODEC_TYPE_UNKNOWN = -1,
            CODEC_TYPE_VIDEO,
            CODEC_TYPE_AUDIO,
            CODEC_TYPE_DATA,
            CODEC_TYPE_SUBTITLE,
        };

        /* currently unused, may be used if 24/32 bits samples ever supported */
        /* all in native endian */
        public enum SampleFormat
        {
            SAMPLE_FMT_NONE = -1,
            SAMPLE_FMT_U8,              ///< unsigned 8 bits
            SAMPLE_FMT_S16,             ///< signed 16 bits
            SAMPLE_FMT_S24,             ///< signed 24 bits
            SAMPLE_FMT_S32,             ///< signed 32 bits
            SAMPLE_FMT_FLT,             ///< float
        };

        public enum Motion_Est_ID
        {
            ME_ZERO = 1,
            ME_FULL,
            ME_LOG,
            ME_PHODS,
            ME_EPZS,
            ME_X1,
            ME_HEX,
            ME_UMH,
            ME_ITER,
        };

        public enum AVDiscard
        {
            //we leave some space between them for extensions (drop some keyframes for intra only or drop just some bidir frames)
            AVDISCARD_NONE = -16, ///< discard nothing
            AVDISCARD_DEFAULT = 0, ///< discard useless packets like 0 size packets in avi
            AVDISCARD_NONREF = 8, ///< discard all non reference
            AVDISCARD_BIDIR = 16, ///< discard all bidirectional frames
            AVDISCARD_NONKEY = 32, ///< discard all frames except keyframes
            AVDISCARD_ALL = 48, ///< discard all
        };

        public enum FF_DECODE_ERROR : int
        {
            FF_DECODE_ERROR_NONE = 0,
            FF_DECODE_ERROR_INVALID_BITSTREAM = 1,
            FF_DECODE_ERROR_MISSING_REFERENCE = 2
        };

        // *********************************************************************************
        // Structs
        // *********************************************************************************

        [StructLayout(LayoutKind.Sequential)]
        public struct RcOverride
        {
            public int start_frame;

            public int end_frame;

            public int qscale;

            [MarshalAs(UnmanagedType.R4)]
            public float quality_factor;
        };

        public struct AVPanScan
        {
            /**
             * id.
             * - encoding: set by user.
             * - decoding: set by lavc
             */
            public int id;
            /**
             * width and height in 1/16 pel
             * - encoding: set by user.
             * - decoding: set by lavc
             */
            public int width;
            public int height;
            /**
             * position of the top left corner in 1/16 pel for up to 3 fields/frames.
             * - encoding: set by user.
             * - decoding: set by lavc
             */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            // [3][2] = 3 x 2 = 6
            public short[] position;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVFrame
        {
            /**
             * pointer to the picture/channel planes.
             * This might be different from the first allocated byte
             * - encoding: Set by user
             * - decoding: set by AVCodecContext.get_buffer()
             */
            //public IntPtr data;
            //public IntPtr data2;
            //public IntPtr data3;
            //public IntPtr data4;
            //public IntPtr data5;
            //public IntPtr data6;
            //public IntPtr data7;
            //public IntPtr data8;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType=UnmanagedType.U4, SizeConst=8)]
            public IntPtr[] data;

            /**
             * Size, in bytes, of the data for each picture/channel plane.
             *
             * For audio, only linesize[0] may be set. For planar audio, each channel
             * plane must be the same size.
             *
             * - encoding: Set by user
             * - decoding: set by AVCodecContext.get_buffer()
             */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
            public int[] linesize;

            /**
             * pointers to the data planes/channels.
             *
             * For video, this should simply point to data[].
             *
             * For planar audio, each channel has a separate data pointer, and
             * linesize[0] contains the size of each channel buffer.
             * For packed audio, there is just one data pointer, and linesize[0]
             * contains the total size of the buffer for all channels.
             *
             * Note: Both data and extended_data will always be set by get_buffer(),
             * but for planar audio with more channels that can fit in data,
             * extended_data must be used by the decoder in order to access all
             * channels.
             *
             * encoding: set by user
             * decoding: set by AVCodecContext.get_buffer()
             */
            public IntPtr extended_data;

            /**
             * width and height of the video frame
             * - encoding: unused
             * - decoding: Read by user.
             */
            public int width, height;

            /**
             * number of audio samples (per channel) described by this frame
             * - encoding: Set by user
             * - decoding: Set by libavcodec
             */
            public int nb_samples;

            /**
             * format of the frame, -1 if unknown or unset
             * Values correspond to enum AVPixelFormat for video frames,
             * enum AVSampleFormat for audio)
             * - encoding: unused
             * - decoding: Read by user.
             */
            public int format;

            /**
             * 1 -> keyframe, 0-> not
             * - encoding: Set by libavcodec.
             * - decoding: Set by libavcodec.
             */
            public int key_frame;

            /**
             * Picture type of the frame, see ?_TYPE below.
             * - encoding: Set by libavcodec. for coded_picture (and set by user for input).
             * - decoding: Set by libavcodec.
             */
            public AVPictureType pict_type;

            /**
             * pointer to the first allocated byte of the picture. Can be used in get_buffer/release_buffer.
             * This isn't used by libavcodec unless the default get/release_buffer() is used.
             * - encoding:
             * - decoding:
             */
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 8)]
            public IntPtr[] @base;

            /**
             * sample aspect ratio for the video frame, 0/1 if unknown/unspecified
             * - encoding: unused
             * - decoding: Read by user.
             */
            public AVRational sample_aspect_ratio;

            /**
             * presentation timestamp in time_base units (time when frame should be shown to user)
             * If AV_NOPTS_VALUE then frame_rate = 1/time_base will be assumed.
             * - encoding: MUST be set by user.
             * - decoding: Set by libavcodec.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pts;

            /**
             * reordered pts from the last AVPacket that has been input into the decoder
             * - encoding: unused
             * - decoding: Read by user.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pkt_pts;

            /**
             * dts from the last AVPacket that has been input into the decoder
             * - encoding: unused
             * - decoding: Read by user.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pkt_dts;

            /**
             * picture number in bitstream order
             * - encoding: set by
             * - decoding: Set by libavcodec.
             */
            public int coded_picture_number;

            /**
             * picture number in display order
             * - encoding: set by
             * - decoding: Set by libavcodec.
             */
            public int display_picture_number;

            /**
             * quality (between 1 (good) and FF_LAMBDA_MAX (bad))
             * - encoding: Set by libavcodec. for coded_picture (and set by user for input).
             * - decoding: Set by libavcodec.
             */
            public int quality;

            /**
             * is this picture used as reference
             * The values for this are the same as the MpegEncContext.picture_structure
             * variable, that is 1->top field, 2->bottom field, 3->frame/both fields.
             * Set to 4 for delayed, non-reference frames.
             * - encoding: unused
             * - decoding: Set by libavcodec. (before get_buffer() call)).
             */
            public int reference;

            /**
             * QP table
             * - encoding: unused
             * - decoding: Set by libavcodec.
             */
            public IntPtr qscale_table;
            
            /**
             * QP store stride
             * - encoding: unused
             * - decoding: Set by libavcodec.
             */
            public int qstride;

            /**
             *
             */
            public int qscale_type;

            /**
             * mbskip_table[mb]>=1 if MB didn't change
             * stride= mb_width = (width+15)>>4
             * - encoding: unused
             * - decoding: Set by libavcodec.
             */
            public IntPtr mbskip_table;

            /**
             * motion vector table
             * @code
             * example:
             * int mv_sample_log2= 4 - motion_subsample_log2;
             * int mb_width= (width+15)>>4;
             * int mv_stride= (mb_width << mv_sample_log2) + 1;
             * motion_val[direction][x + y*mv_stride][0->mv_x, 1->mv_y];
             * @endcode
             * - encoding: Set by user.
             * - decoding: Set by libavcodec.
             */
            //public int16_t (*motion_val[2])[2];
            public IntPtr motion_val;

            /**
             * macroblock type table
             * mb_type_base + mb_width + 2
             * - encoding: Set by user.
             * - decoding: Set by libavcodec.
             */
            public IntPtr mb_type;

            /**
             * DCT coefficients
             * - encoding: unused
             * - decoding: Set by libavcodec.
             */
            public IntPtr dct_coeff;

            /**
             * motion reference frame index
             * the order in which these are stored can depend on the codec.
             * - encoding: Set by user.
             * - decoding: Set by libavcodec.
             */
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U4, SizeConst = 2)]
            public IntPtr[] ref_index;

            /**
             * for some private data of the user
             * - encoding: unused
             * - decoding: Set by user.
             */
            public IntPtr opaque;

            /**
             * error
             * - encoding: Set by libavcodec. if flags&CODEC_FLAG_PSNR.
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=8, ArraySubType=UnmanagedType.U8)]
            public UInt64[] error;

            /**
             * type of the buffer (to keep track of who has to deallocate data[*])
             * - encoding: Set by the one who allocates it.
             * - decoding: Set by the one who allocates it.
             * Note: User allocated (direct rendering) & internal buffers cannot coexist currently.
             */
            public int type;

            /**
             * When decoding, this signals how much the picture must be delayed.
             * extra_delay = repeat_pict / (2*fps)
             * - encoding: unused
             * - decoding: Set by libavcodec.
             */
            public int repeat_pict;

            /**
             * The content of the picture is interlaced.
             * - encoding: Set by user.
             * - decoding: Set by libavcodec. (default 0)
             */
            public int interlaced_frame;

            /**
             * If the content is interlaced, is top field displayed first.
             * - encoding: Set by user.
             * - decoding: Set by libavcodec.
             */
            public int top_field_first;

            /**
             * Tell user application that palette has changed from previous frame.
             * - encoding: ??? (no palette-enabled encoder yet)
             * - decoding: Set by libavcodec. (default 0).
             */
            public int palette_has_changed;

            /**
             * codec suggestion on buffer type if != 0
             * - encoding: unused
             * - decoding: Set by libavcodec. (before get_buffer() call)).
             */
            public int buffer_hints;

            /**
             * Pan scan.
             * - encoding: Set by user.
             * - decoding: Set by libavcodec.
             */
            public IntPtr pan_scan;

            /**
             * reordered opaque 64bit (generally an integer or a double precision float
             * PTS but can be anything).
             * The user sets AVCodecContext.reordered_opaque to represent the input at
             * that time,
             * the decoder reorders values as needed and sets AVFrame.reordered_opaque
             * to exactly one of the values provided by the user through AVCodecContext.reordered_opaque
             * @deprecated in favor of pkt_pts
             * - encoding: unused
             * - decoding: Read by user.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 reordered_opaque;

            /**
             * hardware accelerator private data (FFmpeg-allocated)
             * - encoding: unused
             * - decoding: Set by libavcodec
             */
            public IntPtr hwaccel_picture_private;

            /**
             * the AVCodecContext which ff_thread_get_buffer() was last called on
             * - encoding: Set by libavcodec.
             * - decoding: Set by libavcodec.
             */
            public IntPtr owner;

            /**
             * used by multithreading to store frame-specific info
             * - encoding: Set by libavcodec.
             * - decoding: Set by libavcodec.
             */
            public IntPtr thread_opaque;

            /**
             * log2 of the size of the block which a single vector in motion_val represents:
             * (4->16x16, 3->8x8, 2-> 4x4, 1-> 2x2)
             * - encoding: unused
             * - decoding: Set by libavcodec.
             */
            public char motion_subsample_log2;

            /**
             * Sample rate of the audio data.
             *
             * - encoding: unused
             * - decoding: read by user
             */
            public int sample_rate;

            /**
             * Channel layout of the audio data.
             *
             * - encoding: unused
             * - decoding: read by user.
             */
            [MarshalAs(UnmanagedType.U8)]
            public UInt64 channel_layout;

            //NATE: I've manually added this to correct the offsets of fields below this. It's possible it needs to be higher in the struct.
            public int reserved;

            /**
             * frame timestamp estimated using various heuristics, in stream time base
             * Code outside libavcodec should access this field using:
             * av_frame_get_best_effort_timestamp(frame)
             * - encoding: unused
             * - decoding: set by libavcodec, read by user.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 best_effort_timestamp;

            /**
             * reordered pos from the last AVPacket that has been input into the decoder
             * Code outside libavcodec should access this field using:
             * av_frame_get_pkt_pos(frame)
             * - encoding: unused
             * - decoding: Read by user.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pkt_pos;

            /**
             * duration of the corresponding packet, expressed in
             * AVStream->time_base units, 0 if unknown.
             * Code outside libavcodec should access this field using:
             * av_frame_get_pkt_duration(frame)
             * - encoding: unused
             * - decoding: Read by user.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pkt_duration;

            /**
             * metadata.
             * Code outside libavcodec should access this field using:
             * av_frame_get_metadata(frame)
             * - encoding: Set by user.
             * - decoding: Set by libavcodec.
             */
            public IntPtr metadata;

            /**
             * decode error flags of the frame, set to a combination of
             * FF_DECODE_ERROR_xxx flags if the decoder produced a frame, but there
             * were errors during the decoding.
             * Code outside libavcodec should access this field using:
             * av_frame_get_decode_error_flags(frame)
             * - encoding: unused
             * - decoding: set by libavcodec, read by user.
             */
            public FF_DECODE_ERROR decode_error_flags;

            /**
             * number of audio channels, only used for audio.
             * Code outside libavcodec should access this field using:
             * av_frame_get_channels(frame)
             * - encoding: unused
             * - decoding: Read by user.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 channels;

            /**
             * size of the corresponding packet containing the compressed
             * frame. It must be accessed using av_frame_get_pkt_size() and
             * av_frame_set_pkt_size().
             * It is set to a negative value if unknown.
             * - encoding: unused
             * - decoding: set by libavcodec, read by user.
             */
            public int pkt_size;

        };

        public delegate void DrawhorizBandCallback(IntPtr pAVCodecContext, IntPtr pAVFrame,
                                            [MarshalAs(UnmanagedType.LPArray, SizeConst = 8)]int[] offset,
                                            int y, int type, int height);

        public delegate void RtpCallback(IntPtr pAVCodecContext, IntPtr pdata, int size, int mb_nb);

        public delegate int GetBufferCallback(IntPtr pAVCodecContext, IntPtr pAVFrame);

        public delegate void ReleaseBufferCallback(IntPtr pAVCodecContext, IntPtr pAVFrame);

        public delegate AVPixelFormat GetFormatCallback(IntPtr pAVCodecContext, IntPtr pPixelFormat);

        public delegate int RegetBufferCallback(IntPtr pAVCodecContext, IntPtr pAVFrame);

        public delegate int FuncCallback(IntPtr pAVCodecContext, IntPtr parg);

        public delegate int ExecuteCallback(IntPtr pAVCodecContext,
                                            [MarshalAs(UnmanagedType.FunctionPtr)]FuncCallback func,
                                            [MarshalAs(UnmanagedType.LPArray)]IntPtr[] arg2, ref int ret, int count, int size);

        public delegate int FuncCallback2(IntPtr pAVCodecContext, IntPtr parg, int jobnr, int threadnr);

        public delegate int ExecuteCallback2(IntPtr pAVCodecContext,
                                            [MarshalAs(UnmanagedType.FunctionPtr)]FuncCallback2 func,
                                            [MarshalAs(UnmanagedType.LPArray)]IntPtr[] arg2, ref int ret, int count);


        public struct AVCodecContext
        {
            /**
             * Info on struct for av_log
             * - set by avcodec_alloc_context
             */
            public IntPtr av_class; //  AVClass *av_class;

            public int log_level_offset;

            public CodecType codec_type; /* see CODEC_TYPE_xxx */

            public IntPtr codec;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] codec_name;

            public CodecID codec_id; /* see CODEC_ID_xxx */

            /**
             * fourcc (LSB first, so "ABCD" -> ('D'<<24) + ('C'<<16) + ('B'<<8) + 'A').
             * this is used to workaround some encoder bugs
             * - encoding: set by user, if not then the default based on codec_id will be used
             * - decoding: set by user, will be converted to upper case by lavc during init
             */
            public uint codec_tag;

            /**
             * fourcc from the AVI stream header (LSB first, so "ABCD" -> ('D'<<24) + ('C'<<16) + ('B'<<8) + 'A').
             * this is used to workaround some encoder bugs
             * - encoding: unused
             * - decoding: set by user, will be converted to upper case by lavc during init
             */
            public uint stream_codec_tag;

            /**
             * some codecs needs additionnal format info. It is stored here
             * - encoding: set by user.
             * - decoding: set by lavc. (FIXME is this ok?)
             */
            public int sub_id;

            public IntPtr priv_data;

            public IntPtr @internal;

            /**
             * private data of the user, can be used to carry app specific stuff.
             * - encoding: set by user
             * - decoding: set by user
             */
            public IntPtr opaque;

            /**
             * the average bitrate.
             * - encoding: set by user. unused for constant quantizer encoding
             * - decoding: set by lavc. 0 or some bitrate if this info is available in the stream
             */
            public int bit_rate;

            /**
             * number of bits the bitstream is allowed to diverge from the reference.
             *           the reference can be CBR (for CBR pass1) or VBR (for pass2)
             * - encoding: set by user. unused for constant quantizer encoding
             * - decoding: unused
             */
            public int bit_rate_tolerance;

            /**
             * global quality for codecs which cannot change it per frame.
             * this should be proportional to MPEG1/2/4 qscale.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int global_quality;

            /**
             * - encoding: set by user.
             * - decoding: unused
             */
            public int compression_level;

            /**
             * CODEC_FLAG_*.
             * - encoding: set by user.
             * - decoding: set by user.
             */
            public int flags;

            /**
             * CODEC_FLAG2_*.
             * - encoding: set by user.
             * - decoding: set by user.
             */
            public int flags2;

            /**
             * some codecs need / can use extra-data like huffman tables.
             * mjpeg: huffman tables
             * rv10: additional flags
             * mpeg4: global headers (they can be in the bitstream or here)
             * the allocated memory should be FF_INPUT_BUFFER_PADDING_SIZE bytes larger
             * then extradata_size to avoid prolems if its read with the bitstream reader
             * the bytewise contents of extradata must not depend on the architecture or cpu endianness
             * - encoding: set/allocated/freed by lavc.
             * - decoding: set/allocated/freed by user.
             */
            public IntPtr extradata; // void* extradata;

            public int extradata_size;

            /**
             * this is the fundamental unit of time (in seconds) in terms
             * of which frame timestamps are represented. for fixed-fps content,
             * timebase should be 1/framerate and timestamp increments should be
             * identically 1.
             * - encoding: MUST be set by user
             * - decoding: set by lavc.
             */
            public AVRational time_base;

            /**
             * For some codecs, the time base is closer to the field rate than the frame rate.
             * Most notably, H.264 and MPEG-2 specify time_base as half of frame duration
             * if no telecine is used ...
             *
             * Set to time_base ticks per frame. Default 1, e.g., H.264/MPEG-2 set it to 2.
             */
            public int ticks_per_frame;

            /**
             * number of frames the decoded output will be delayed relative to
             * the encoded input.
             * - encoding: set by lavc.
             * - decoding: unused
             */
            public int delay;

            /* video only */
            /**
             * picture width / height.
             * - encoding: MUST be set by user.
             * - decoding: set by lavc.
             * Note, for compatibility its possible to set this instead of
             * coded_width/height before decoding
             */
            public int width;

            public int height;

            /**
             * bitsream width / height. may be different from width/height if lowres
             * or other things are used
             * - encoding: unused
             * - decoding: set by user before init if known, codec should override / dynamically change if needed
             */
            public int coded_width;

            public int coded_height;

            /**
             * the number of pictures in a group of pitures, or 0 for intra_only.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int gop_size;

            /**
             * pixel format, see PIX_FMT_xxx.
             * - encoding: set by user.
             * - decoding: set by lavc.
             */
            public AVPixelFormat pix_fmt;

            /**
             * motion estimation algorithm used for video coding.
             * 1 (zero), 2 (full), 3 (log), 4 (phods), 5 (epzs), 6 (x1), 7 (hex),
             * 8 (umh), 9 (iter) [7, 8 are x264 specific, 9 is snow specific]
             * - encoding: MUST be set by user.
             * - decoding: unused
             */
            public int me_method;

            /**
             * if non NULL, 'draw_horiz_band' is called by the libavcodec
             * decoder to draw an horizontal band. It improve cache usage. Not
             * all codecs can do that. You must check the codec capabilities
             * before
             * - encoding: unused
             * - decoding: set by user.
             * @param height the height of the slice
             * @param y the y position of the slice
             * @param type 1->top field, 2->bottom field, 3->frame
             * @param offset offset into the AVFrame.data from which the slice should be read
             */
            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public DrawhorizBandCallback draw_horiz_band;
            public IntPtr draw_horiz_band;

            /**
             * callback to negotiate the pixelFormat.
             * @param fmt is the list of formats which are supported by the codec,
             * its terminated by -1 as 0 is a valid format, the formats are ordered by quality
             * the first is allways the native one
             * @return the choosen format
             * - encoding: unused
             * - decoding: set by user, if not set then the native format will always be choosen
             */
            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public GetFormatCallback get_format;
            public IntPtr get_format;

            /**
             * maximum number of b frames between non b frames.
             * note: the output will be delayed by max_b_frames+1 relative to the input
             * - encoding: set by user.
             * - decoding: unused
             */
            public int max_b_frames;

            /**
             * qscale factor between ip and b frames.
             * - encoding: set by user.
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float b_quant_factor;

            /** obsolete FIXME remove */
            public int rc_strategy;

            public int b_frame_strategy;

            /**
             * luma single coeff elimination threshold.
             * - encoding: set by user
             * - decoding: unused
             */
            public int luma_elim_threshold;

            /**
             * chroma single coeff elimination threshold.
             * - encoding: set by user
             * - decoding: unused
             */
            public int chroma_elim_threshold;

            /**
             * qscale offset between ip and b frames.
             * if > 0 then the last p frame quantizer will be used (q= lastp_q*factor+offset)
             * if < 0 then normal ratecontrol will be done (q= -normal_q*factor+offset)
             * - encoding: set by user.
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float b_quant_offset;

            /**
             * if 1 the stream has a 1 frame delay during decoding.
             * - encoding: set by lavc
             * - decoding: set by lavc
             */
            public int has_b_frames;

            /**
             * 0-> h263 quant 1-> mpeg quant.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int mpeg_quant;

            /**
             * qscale factor between p and i frames.
             * if > 0 then the last p frame quantizer will be used (q= lastp_q*factor+offset)
             * if < 0 then normal ratecontrol will be done (q= -normal_q*factor+offset)
             * - encoding: set by user.
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float i_quant_factor;

            /**
             * qscale offset between p and i frames.
             * - encoding: set by user.
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float i_quant_offset;

            /**
             * luminance masking (0-> disabled).
             * - encoding: set by user
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float lumi_masking;

            /**
             * temporary complexity masking (0-> disabled).
             * - encoding: set by user
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float temporal_cplx_masking;

            /**
             * spatial complexity masking (0-> disabled).
             * - encoding: set by user
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float spatial_cplx_masking;

            /**
             * p block masking (0-> disabled).
             * - encoding: set by user
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float p_masking;

            /**
             * darkness masking (0-> disabled).
             * - encoding: set by user
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float dark_masking;

            /**
             * slice count.
             * - encoding: set by lavc
             * - decoding: set by user (or 0)
             */
            public int slice_count;

            /**
             * prediction method (needed for huffyuv).
             * - encoding: set by user
             * - decoding: unused
             */
            public int prediction_method;

            /**
             * slice offsets in the frame in bytes.
             * - encoding: set/allocated by lavc
             * - decoding: set/allocated by user (or NULL)
             */
            public IntPtr slice_offset; // int *slice_offset

            /**
             * sample aspect ratio (0 if unknown).
             * numerator and denominator must be relative prime and smaller then 256 for some video standards
             * - encoding: set by user.
             * - decoding: set by lavc.
             */
            public AVRational sample_aspect_ratio;

            /**
             * motion estimation compare function.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int me_cmp;


            /**
             * subpixel motion estimation compare function.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int me_sub_cmp;

            /**
             * macroblock compare function (not supported yet).
             * - encoding: set by user.
             * - decoding: unused
             */
            public int mb_cmp;

            /**
             * interlaced dct compare function
             * - encoding: set by user.
             * - decoding: unused
             */
            public int ildct_cmp;

            /**
             * ME diamond size & shape.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int dia_size;

            /**
             * amount of previous MV predictors (2a+1 x 2a+1 square).
             * - encoding: set by user.
             * - decoding: unused
             */
            public int last_predictor_count;

            /**
             * pre pass for motion estimation.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int pre_me;

            /**
             * motion estimation pre pass compare function.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int me_pre_cmp;

            /**
             * ME pre pass diamond size & shape.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int pre_dia_size;

            /**
             * subpel ME quality.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int me_subpel_quality;

            /**
             * DTG active format information (additionnal aspect ratio
             * information only used in DVB MPEG2 transport streams). 0 if
             * not set.
             *
             * - encoding: unused.
             * - decoding: set by decoder
             */
            public int dtg_active_format;

            /**
             * Maximum motion estimation search range in subpel units.
             * if 0 then no limit
             *
             * - encoding: set by user.
             * - decoding: unused.
             */
            public int me_range;

            /**
             * intra quantizer bias.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int intra_quant_bias;

            /**
             * inter quantizer bias.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int inter_quant_bias;

            /**
             * color table ID.
             * - encoding: unused.
             * - decoding: which clrtable should be used for 8bit RGB images
             *             table have to be stored somewhere FIXME
             */
            public int color_table_id;

            /**
              * slice flags
              * - encoding: unused
              * - decoding: set by user.
              */
            public int slice_flags;

            /**
             * XVideo Motion Acceleration
             * - encoding: forbidden
             * - decoding: set by decoder
             */
            public int xvmc_acceleration;

            /**
             * macroblock decision mode
             * - encoding: set by user.
             * - decoding: unused
             */
            public int mb_decision;

            /**
             * custom intra quantization matrix
             * - encoding: set by user, can be NULL
             * - decoding: set by lavc
             */
            public IntPtr intra_matrix; // uint16_t* intra_matrix;

            /**
             * custom inter quantization matrix
             * - encoding: set by user, can be NULL
             * - decoding: set by lavc
             */
            public IntPtr inter_matrix; // uint16_t* inter_matrix;

            /**
             * scene change detection threshold.
             * 0 is default, larger means fewer detected scene changes
             * - encoding: set by user.
             * - decoding: unused
             */
            public int scenechange_threshold;

            /**
             * noise reduction strength
             * - encoding: set by user.
             * - decoding: unused
             */
            public int noise_reduction;

            /**
             *
             * - encoding: set by user.
             * - decoding: unused
             */
            public int inter_threshold;

            /**
             * Quantizer noise shaping.
             * - encoding: set by user
             * - decoding: unused
             */
            public int quantizer_noise_shaping;

            /**
             * Motion estimation threshold. under which no motion estimation is
             * performed, but instead the user specified motion vectors are used
             *
             * - encoding: set by user
             * - decoding: unused
             */
            public int me_threshold;

            /**
             * Macroblock threshold. under which the user specified macroblock types will be used
             * - encoding: set by user
             * - decoding: unused
             */
            public int mb_threshold;

            /**
             * precision of the intra dc coefficient - 8.
             * - encoding: set by user
             * - decoding: unused
             */
            public int intra_dc_precision;

            /**
             * number of macroblock rows at the top which are skipped.
             * - encoding: unused
             * - decoding: set by user
             */
            public int skip_top;

            /**
             * number of macroblock rows at the bottom which are skipped.
             * - encoding: unused
             * - decoding: set by user
             */
            public int skip_bottom;

            /**
             * border processing masking. raises the quantizer for mbs on the borders
             * of the picture.
             * - encoding: set by user
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float border_masking;

            /**
             * minimum MB lagrange multipler.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int mb_lmin;

            /**
             * maximum MB lagrange multipler.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int mb_lmax;

            /**
             *
             * - encoding: set by user.
             * - decoding: unused
             */
            public int me_penalty_compensation;

            /**
             *
             * - encoding: set by user.
             * - decoding: unused
             */
            public int bidir_refine;

            /**
             *
             * - encoding: set by user.
             * - decoding: unused
             */
            public int brd_scale;

            /**
             * minimum gop size
             * - encoding: set by user.
             * - decoding: unused
             */
            public int keyint_min;

            /**
             * number of reference frames
             * - encoding: set by user.
             * - decoding: unused
             */
            public int refs;

            /**
             * chroma qp offset from luma
             * - encoding: set by user.
             * - decoding: unused
             */
            public int chromaoffset;

            /**
             * multiplied by qscale for each frame and added to scene_change_score
             * - encoding: set by user.
             * - decoding: unused
             */
            public int scenechange_factor;

            /**
             *
             * note: value depends upon the compare functin used for fullpel ME
             * - encoding: set by user.
             * - decoding: unused
             */
            public int mv0_threshold;

            /**
             * adjusts sensitivity of b_frame_strategy 1
             * - encoding: set by user.
             * - decoding: unused
             */
            public int b_sensitivity;

            public AVColorPrimaries color_primaries;

            public AVColorTransferCharacteristic color_trc;

            public AVColorSpace colorspace;

            public AVColorRange color_range;

            public AVChromaLocation chroma_sample_location;

            public int slices;

            public AVFieldOrder field_order;

            /* audio only */
            public int sample_rate; // samples per sec

            public int channels;

            /**
             * audio sample format.
             * - encoding: set by user.
             * - decoding: set by lavc.
             */
            public SampleFormat sample_fmt;  // sample format, currenly unused

            public int frame_size;

            public int frame_number;   // audio or video frame number

            /**
             * number of bytes per packet if constant and known or 0
             * used by some WAV based audio codecs
             */
            public int block_align;

            /**
             * audio cutoff bandwidth (0 means "automatic") . Currently used only by FAAC
             * - encoding: set by user.
             * - decoding: unused
             */
            public int cutoff;

            public int request_channels;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 channel_layout;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 request_channel_layout;

            public AVAudioServiceType audio_service_type;

            public AVSampleFormat request_sample_fmt;

            /**
              * called at the beginning of each frame to get a buffer for it.
              * if pic.reference is set then the frame will be read later by lavc
              * avcodec_align_dimensions() should be used to find the required width and
              * height, as they normally need to be rounded up to the next multiple of 16
              * - encoding: unused
              * - decoding: set by lavc, user can override
              */
            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public GetBufferCallback get_buffer;
            public IntPtr get_buffer;

            /**
             * called to release buffers which where allocated with get_buffer.
             * a released buffer can be reused in get_buffer()
             * pic.data[*] must be set to NULL
             * - encoding: unused
             * - decoding: set by lavc, user can override
             */
            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public ReleaseBufferCallback release_buffer;
            public IntPtr release_buffer;

            /**
             * called at the beginning of a frame to get cr buffer for it.
             * buffer type (size, hints) must be the same. lavc won't check it.
             * lavc will pass previous buffer in pic, function should return
             * same buffer or new buffer with old frame "painted" into it.
             * if pic.data[0] == NULL must behave like get_buffer().
             * - encoding: unused
             * - decoding: set by lavc, user can override
             */
            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public RegetBufferCallback reget_buffer;
            public IntPtr reget_buffer;

            [MarshalAs(UnmanagedType.R4)]
            public float qcompress;  // amount of qscale change between easy & hard scenes (0.0-1.0)

            [MarshalAs(UnmanagedType.R4)]
            public float qblur;      // amount of qscale smoothing over time (0.0-1.0)

            /**
             * minimum quantizer.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int qmin;

            /**
             * maximum quantizer.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int qmax;

            /**
             * maximum quantizer difference etween frames.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int max_qdiff;

            /**
             * ratecontrol qmin qmax limiting method.
             * 0-> clipping, 1-> use a nice continous function to limit qscale wthin qmin/qmax
             * - encoding: set by user.
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float rc_qsquish;

            [MarshalAs(UnmanagedType.R4)]
            float rc_qmod_amp;

            public int rc_qmod_freq;

            /**
             * decoder bitstream buffer size.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int rc_buffer_size;

            public int rc_override_count;

            /**
             * ratecontrol override, see RcOverride.
             * - encoding: allocated/set/freed by user.
             * - decoding: unused
             */
            public IntPtr rc_override; // RcOverride* rc_override;

            /**
             * rate control equation.
             * - encoding: set by user
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.LPStr)]
            public String rc_eq; // char* rc_eq;

            /**
             * maximum bitrate.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int rc_max_rate;

            /**
             * minimum bitrate.
             * - encoding: set by user.
             * - decoding: unused
             */
            public int rc_min_rate;

            [MarshalAs(UnmanagedType.R4)]
            public float rc_buffer_aggressivity;

            /**
             * initial complexity for pass1 ratecontrol.
             * - encoding: set by user.
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.R4)]
            public float rc_initial_cplx;

            [MarshalAs(UnmanagedType.R4)]
            public float rc_max_available_vbv_use;

            [MarshalAs(UnmanagedType.R4)]
            public float rc_min_vbv_overflow_use;

            /**
             * number of bits which should be loaded into the rc buffer before decoding starts
             * - encoding: set by user.
             * - decoding: unused
             */
            public int rc_initial_buffer_occupancy;

            /**
             * coder type
             * - encoding: set by user.
             * - decoding: unused
             */
            public int coder_type;

            /**
             * context model
             * - encoding: set by user.
             * - decoding: unused
             */
            public int context_model;

            /**
             * minimum lagrange multipler
             * - encoding: set by user.
             * - decoding: unused
             */
            public int lmin;

            /**
             * maximum lagrange multipler
             * - encoding: set by user.
             * - decoding: unused
             */
            public int lmax;

            /**
             * frame skip threshold
             * - encoding: set by user
             * - decoding: unused
             */
            public int frame_skip_threshold;

            /**
             * frame skip factor
             * - encoding: set by user
             * - decoding: unused
             */
            public int frame_skip_factor;

            /**
             * frame skip exponent
             * - encoding: set by user
             * - decoding: unused
             */
            public int frame_skip_exp;

            /**
             * frame skip comparission function
             * - encoding: set by user.
             * - decoding: unused
             */
            public int frame_skip_cmp;

            /**
             * trellis RD quantization
             * - encoding: set by user.
             * - decoding: unused
             */
            public int trellis;

            /**
             * - encoding: set by user.
             * - decoding: unused.
             */
            public int min_prediction_order;

            /**
             * - encoding: set by user.
             * - decoding: unused.
             */
            public int max_prediction_order;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 timecode_frame_start;

            /* The RTP callback: This function is called   */
            /* every time the encoder has a packet to send */
            /* Depends on the encoder if the data starts   */
            /* with a Start Code (it should) H.263 does.   */
            /* mb_nb contains the number of macroblocks    */
            /* encoded in the RTP payload                  */
            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public RtpCallback rtp_callback;
            public IntPtr rtp_callback;

            /* The size of the RTP payload: the coder will  */
            /* do it's best to deliver a chunk with size    */
            /* below rtp_payload_size, the chunk will start */
            /* with a start code on some codecs like H.263  */
            /* This doesn't take account of any particular  */
            /* headers inside the transmited RTP payload    */
            public int rtp_payload_size;

            public int mv_bits;

            public int header_bits;

            public int i_tex_bits;

            public int p_tex_bits;

            public int i_count;

            public int p_count;

            public int skip_count;

            public int misc_bits;

            /**
             * number of bits used for the previously encoded frame.
             * - encoding: set by lavc
             * - decoding: unused
             */
            public int frame_bits;

            /**
             * pass1 encoding statistics output buffer.
             * - encoding: set by lavc
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.LPStr)]
            public String stats_out; // char* stats_out

            /**
             * pass2 encoding statistics input buffer.
             * concatenated stuff from stats_out of pass1 should be placed here
             * - encoding: allocated/set/freed by user
             * - decoding: unused
             */
            public String stats_in;// char *stats_in

            /**
             * workaround bugs in encoders which sometimes cannot be detected automatically.
             * - encoding: set by user
             * - decoding: set by user
             */
            public int workaround_bugs;

            /**
             * strictly follow the std (MPEG4, ...).
             * - encoding: set by user
             * - decoding: unused
             */
            public int strict_std_compliance;

            /**
             * error resilience higher values will detect more errors but may missdetect
             * some more or less valid parts as errors.
             * - encoding: unused
             * - decoding: set by user
             */
            public int error_resilience;

            /**
             * debug.
             * - encoding: set by user.
             * - decoding: set by user.
             */
            public int debug;

            /**
             * debug.
             * - encoding: set by user.
             * - decoding: set by user.
             */
            public int debug_mv;

            public int err_recognition;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 reordered_opaque;

            public IntPtr hwaccel;

            public IntPtr hwaccel_context;

            /**
             * error.
             * - encoding: set by lavc if flags&CODEC_FLAG_PSNR
             * - decoding: unused
             */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Int64[] error;

            /**
             * dct algorithm, see FF_DCT_* below.
             * - encoding: set by user
             * - decoding: unused
             */
            public int dct_algo;

            /**
             * idct algorithm, see FF_IDCT_* below.
             * - encoding: set by user
             * - decoding: set by user
             */
            public int idct_algo;

            /**
             * dsp_mask could be add used to disable unwanted CPU features
             * CPU features (i.e. MMX, SSE. ...)
             *
             * with FORCE flag you may instead enable given CPU features
             * (Dangerous: usable in case of misdetection, improper usage however will
             * result into program crash)
             */
            public uint dsp_mask;

            /**
             * bits per sample/pixel from the demuxer (needed for huffyuv).
             * - encoding: set by lavc
             * - decoding: set by user
             */
            public int bits_per_coded_sample;


            /**
             * bits per sample/pixel from the demuxer (needed for huffyuv).
             * - encoding: set by lavc
             * - decoding: set by user
             */
            public int bits_per_raw_sample;

            /**
             * low resolution decoding. 1-> 1/2 size, 2->1/4 size
             * - encoding: unused
             * - decoding: set by user
             */
            public int lowres;

            /**
             * the picture in the bitstream.
             * - encoding: set by lavc
             * - decoding: set by lavc
             */
            public IntPtr coded_frame; // AVFrame* coded_frame;

            /**
             * Thread count.
             * is used to decide how many independant tasks should be passed to execute()
             * - encoding: set by user
             * - decoding: set by user
             */
            public int thread_count;

            /**
             * Which multithreading methods to use.
             * Use of FF_THREAD_FRAME will increase decoding delay by one frame per thread,
             * so clients which cannot provide future frames should not use it.
             *
             * - encoding: Set by user, otherwise the default is used.
             * - decoding: Set by user, otherwise the default is used.
             */
            public int thread_type;

            public int active_thread_type;

            public int thread_safe_callbacks;

            /**
             * the codec may call this to execute several independant things. it will return only after
             * finishing all tasks, the user may replace this with some multithreaded implementation, the
             * default implementation will execute the parts serially
             * @param count the number of things to execute
             * - encoding: set by lavc, user can override
             * - decoding: set by lavc, user can override
             */
            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public ExecuteCallback execute;
            public IntPtr execute;

            /**
             * the codec may call this to execute several independant things. it will return only after
             * finishing all tasks, the user may replace this with some multithreaded implementation, the
             * default implementation will execute the parts serially
             * @param count the number of things to execute
             * - encoding: set by lavc, user can override
             * - decoding: set by lavc, user can override
             */
            //[MarshalAs(UnmanagedType.FunctionPtr)]
            //public ExecuteCallback2 execute2;
            public IntPtr execute2;

            /**
             * Thread opaque.
             * can be used by execute() to store some per AVCodecContext stuff.
             * - encoding: set by execute()
             * - decoding: set by execute()
             */
            public IntPtr thread_opaque; // void* thread_opaque;

            /**
             * noise vs. sse weight for the nsse comparsion function.
             * - encoding: set by user
             * - decoding: unused
             */
            public int nsse_weight;

            /**
             * profile
             * - encoding: set by user
             * - decoding: set by lavc
             */
            public int profile;

            /**
             * level
             * - encoding: set by user
             * - decoding: set by lavc
             */
            
            public int level;

            /**
             *
             * - encoding: unused
             * - decoding: set by user.
             */
            public AVDiscard skip_loop_filter;

            /**
             *
             * - encoding: unused
             * - decoding: set by user.
             */
            public AVDiscard skip_idct;

            /**
             *
             * - encoding: unused
             * - decoding: set by user.
             */
            public AVDiscard skip_frame;

            /**
             * Header containing style information for text subtitles.
             * For SUBTITLE_ASS subtitle type, it should contain the whole ASS
             * [Script Info] and [V4+ Styles] section, plus the [Events] line and
             * the Format line following. It shouldn't include any Dialogue line.
             * - encoding: Set/allocated/freed by user (before avcodec_open2())
             * - decoding: Set/allocated/freed by libavcodec (by avcodec_open2())
             */
            public IntPtr subtitle_header;

            public int subtitle_header_size;

            /**
             * simulates errors in the bitstream to test error concealment.
             * - encoding: set by user.
             * - decoding: unused.
             */
            public int error_rate;

            /**
             * Current packet as passed into the decoder, to avoid having
             * to pass the packet into every function. Currently only valid
             * inside lavc and get/release_buffer callbacks.
             * - decoding: set by avcodec_decode_*, read by get_buffer() for setting pkt_pts
             * - encoding: unused
             */
            public IntPtr pkt;

            /**
             * VBV delay coded in the last frame (in periods of a 27 MHz clock).
             * Used for compliant TS muxing.
             * - encoding: Set by libavcodec.
             * - decoding: unused.
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 vbv_delay;

            /**
             * Timebase in which pkt_dts/pts and AVPacket.dts/pts are.
             * Code outside libavcodec should access this field using:
             * avcodec_set_pkt_timebase(avctx)
             * - encoding unused.
             * - decodimg set by user
             */
            public AVRational pkt_timebase;

            public IntPtr codec_descriptor;

            /**
             * Current statistics for PTS correction.
             * - decoding: maintained and used by libavcodec, not intended to be used by user apps
             * - encoding: unused
             */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pts_correction_num_faulty_pts;
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pts_correction_num_faulty_dts;
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pts_correction_last_pts;
            [MarshalAs(UnmanagedType.I8)]
            public Int64 pts_correction_last_dts;

            public IntPtr metadata;
            
            /**
             * hurry up amount.
             * deprecated in favor of skip_idct and skip_frame
             * - encoding: unused
             * - decoding: set by user. 1-> skip b frames, 2-> skip idct/dequant too, 5-> skip everything except header
             */
        };

        public delegate int InitCallback(IntPtr pAVCodecContext);

        public delegate int EncodeCallback(IntPtr pAVCodecContext, IntPtr buf, int buf_size, IntPtr data);

        public delegate int CloseCallback(IntPtr pAVCodecContext);

        public delegate int DecodeCallback(IntPtr pAVCodecContext, IntPtr outdata, ref int outdata_size,
                                            IntPtr buf, int buf_size);

        public delegate int FlushCallback(IntPtr pAVCodecContext);

        [StructLayout(LayoutKind.Sequential)]
        public struct AVCodec
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public String name;

            public CodecType type;

            public CodecID id;

            public int priv_data_size;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public InitCallback init;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public EncodeCallback encode;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public CloseCallback close;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public DecodeCallback decode;

            public int capabilities;

            //#if LIBAVCODEC_VERSION_INT < ((50<<16)+(0<<8)+0)
            //    IntPtr dummy; // FIXME remove next time we break binary compatibility
            //#endif
            public IntPtr next; // AVCodec *next

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public FlushCallback flush;

            // array of supported framerates, or NULL if any, array is terminated by {0,0}
            public IntPtr supported_framerates;

            // array of supported pixel formats, or NULL if unknown, array is terminanted by -1
            public IntPtr pix_fmts; // enum PixelFormat *pix_fmts
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVPicture
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public IntPtr[] data; // IntPtr data[4]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] linesize;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVPaletteControl
        {
            /* demuxer sets this to 1 to indicate the palette has changed;
             * decoder resets to 0 */
            public int palette_changed;

            /* 4-byte ARGB palette entries, stored in native byte order; note that
             * the individual palette components should be on a 8-bit scale; if
             * the palette data comes from a IBM VGA native format, the component
             * data is probably 6 bits in size and needs to be scaled */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AVPALETTE_COUNT)]
            public uint[] palette;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVSubtitleRect
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort x;

            [MarshalAs(UnmanagedType.U2)]
            public ushort y;

            [MarshalAs(UnmanagedType.U2)]
            public ushort w;

            [MarshalAs(UnmanagedType.U2)]
            public ushort h;

            [MarshalAs(UnmanagedType.U2)]
            public ushort nb_colors;

            public int linesize;

            public IntPtr bitmap; // IntPtr bitmap;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVSubtitle
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort format;  /* 0 = graphics */

            public uint start_display_time; /* relative to packet pts, in ms */

            public uint end_display_time; /* relative to packet pts, in ms */

            public uint num_rects;

            public IntPtr rects; // AVSubtitleRect *rects;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVCodecParserContext
        {
            public IntPtr priv_data;

            public IntPtr parser; // AVCodecParser *parser

            [MarshalAs(UnmanagedType.I8)]
            public Int64 frame_offset; // offset of the current frame 

            [MarshalAs(UnmanagedType.I8)]
            public Int64 cur_offset; // current offset incremented by each av_parser_parse()

            [MarshalAs(UnmanagedType.I8)]
            public Int64 last_frame_offset; // offset of the last frame

            /* video info */
            public int pict_type; /* XXX: put it back in AVCodecContext */

            public int repeat_pict; /* XXX: put it back in AVCodecContext */

            [MarshalAs(UnmanagedType.I8)]
            public Int64 pts;     /* pts of the current frame */

            [MarshalAs(UnmanagedType.I8)]
            public Int64 dts;     /* dts of the current frame */

            /* private data */
            [MarshalAs(UnmanagedType.I8)]
            public Int64 last_pts;

            [MarshalAs(UnmanagedType.I8)]
            public Int64 last_dts;

            public int fetch_timestamp;

            public int cur_frame_start_index;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AV_PARSER_PTS_NB)]
            public Int64[] cur_frame_offset;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AV_PARSER_PTS_NB)]
            public Int64[] cur_frame_pts;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = AV_PARSER_PTS_NB)]
            public Int64[] cur_frame_dts;

            public int flags;
        };

        public delegate int ParaerInitCallback(IntPtr pAVCodecParserContext);

        public delegate int ParserParseCallback(IntPtr pAVCodecParserContext,
                                                IntPtr pAVCodecContext,
                                                [MarshalAs(UnmanagedType.LPArray)]IntPtr[] poutbuf,
                                                ref int poutbuf_size,
                                                IntPtr buf, int buf_size);

        public delegate void ParserCloseCallback(IntPtr pAVcodecParserContext);

        public delegate int SplitCallback(IntPtr pAVCodecContext, IntPtr buf, int buf_size);

        [StructLayout(LayoutKind.Sequential)]
        public struct AVCodecParser
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public int[] codec_ids; /* several codec IDs are permitted */

            public int priv_data_size;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ParaerInitCallback parser_init;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ParserParseCallback parser_parse;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public ParserCloseCallback parser_close;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public SplitCallback split;

            public IntPtr next;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct AVBitStreamFilterContext
        {
            public IntPtr priv_data;
            public IntPtr filter; // AVBitStreamFilter *filter
            public IntPtr parser; // AVCodecParserContext *parser
            public IntPtr next; // AVBitStreamFilterContext *next
        };

        public delegate int FilterCallback(IntPtr pAVBitStreamFilterContext,
                                        IntPtr pAVCodecContext,
                                        [MarshalAs(UnmanagedType.LPStr)]String args,
                                        [MarshalAs(UnmanagedType.LPArray)]IntPtr[] poutbuf, ref int poutbuf_size,
                                        IntPtr buf, int buf_size, int keyframe);

        [StructLayout(LayoutKind.Sequential)]
        public struct AVBitStreamFilter
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public String name;

            public int priv_data_size;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public FilterCallback filter;

            public IntPtr next; // AVBitStreamFilter *next
        };
    }
}
