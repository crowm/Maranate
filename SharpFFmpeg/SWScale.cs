using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace SharpFFmpeg
{
    public partial class FFmpeg
    {
        public class SwScale
        {
            private const string swscaleDllName = "swscale-2.dll";

            /**
             * Allocate and return an SwsContext. You need it to perform
             * scaling/conversion operations using sws_scale().
             *
             * @param srcW the width of the source image
             * @param srcH the height of the source image
             * @param srcFormat the source image format
             * @param dstW the width of the destination image
             * @param dstH the height of the destination image
             * @param dstFormat the destination image format
             * @param flags specify which algorithm and options to use for rescaling
             * @return a pointer to an allocated context, or NULL in case of error
             * @note this function is to be removed after a saner alternative is
             *       written
             * @deprecated Use sws_getCachedContext() instead.
             */
            [DllImport(swscaleDllName), SuppressUnmanagedCodeSecurity]
            public static extern IntPtr sws_getContext(
                int srcW, int srcH, AVPixelFormat srcFormat,
                int dstW, int dstH, AVPixelFormat dstFormat,
                SWS_FLAGS flags,
                IntPtr srcFilter, IntPtr dstFilter, IntPtr param);

            /**
             * Scale the image slice in srcSlice and put the resulting scaled
             * slice in the image in dst. A slice is a sequence of consecutive
             * rows in an image.
             *
             * Slices have to be provided in sequential order, either in
             * top-bottom or bottom-top order. If slices are provided in
             * non-sequential order the behavior of the function is undefined.
             *
             * @param c         the scaling context previously created with
             *                  sws_getContext()
             * @param srcSlice  the array containing the pointers to the planes of
             *                  the source slice
             * @param srcStride the array containing the strides for each plane of
             *                  the source image
             * @param srcSliceY the position in the source image of the slice to
             *                  process, that is the number (counted starting from
             *                  zero) in the image of the first row of the slice
             * @param srcSliceH the height of the source slice, that is the number
             *                  of rows in the slice
             * @param dst       the array containing the pointers to the planes of
             *                  the destination image
             * @param dstStride the array containing the strides for each plane of
             *                  the destination image
             * @return          the height of the output slice
             */
            [DllImport(swscaleDllName), SuppressUnmanagedCodeSecurity]
            public static extern int sws_scale(
                IntPtr swsContext,
                IntPtr srcSlice, IntPtr srcStride,
                int srcSliceY, int srcSliceH,
                IntPtr dstSlice, IntPtr dstStride);


            /**
             * Free the swscaler context swsContext.
             * If swsContext is NULL, then does nothing.
             */
            [DllImport(swscaleDllName), SuppressUnmanagedCodeSecurity]
            public static extern void sws_freeContext(IntPtr swsContext);


            public enum SWS_FLAGS : int
            {
                SWS_FAST_BILINEAR = 1,
                SWS_BILINEAR = 2,
                SWS_BICUBIC = 4,
                SWS_X = 8,
                SWS_POINT = 0x10,
                SWS_AREA = 0x20,
                SWS_BICUBLIN = 0x40,
                SWS_GAUSS = 0x80,
                SWS_SINC = 0x100,
                SWS_LANCZOS = 0x200,
                SWS_SPLINE = 0x400
            };
            //public const int SWS_FAST_BILINEAR = 1;
            //public const int SWS_BILINEAR = 2;
            //public const int SWS_BICUBIC = 4;
            //public const int SWS_X = 8;
            //public const int SWS_POINT = 0x10;
            //public const int SWS_AREA = 0x20;
            //public const int SWS_BICUBLIN = 0x40;
            //public const int SWS_GAUSS = 0x80;
            //public const int SWS_SINC = 0x100;
            //public const int SWS_LANCZOS = 0x200;
            //public const int SWS_SPLINE = 0x400;
            
            [StructLayout(LayoutKind.Sequential)]
            struct SwsFilter
            {
                SwsVector lumH;
                SwsVector lumV;
                SwsVector chrH;
                SwsVector chrV;
            }

            [StructLayout(LayoutKind.Sequential)]
            struct SwsVector
            {
                IntPtr coeff;               ///< pointer to the list of coefficients
                int length;                 ///< number of coefficients in the vector
            }


        }

    }
}
