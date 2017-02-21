using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using System.Windows.Media.Imaging;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Cvb;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;

namespace EyeDetector
{
    public class Core : INotifyPropertyChanged
    {
        //filds
        private static Image<Gray, Byte> imgThresh;
        private static Image<Bgr, Byte> imgRezult;

        private static Mat matOriginal = new Mat();
        private static Mat matThresh  = new Mat(0, 0, DepthType.Cv8U, 1);
        private static Mat matGray = new Mat();
        private static Mat matRezult = new Mat();
        private static Mat h_plane = new Mat(0, 0, DepthType.Cv8U, 1);
        private static Mat s_plane = new Mat(0, 0, DepthType.Cv8U, 1);
        private static Mat v_plane = new Mat(0, 0, DepthType.Cv8U, 1);
        private static Mat h_mask = new Mat(0, 0, DepthType.Cv8U, 1);
        private static Mat s_mask = new Mat(0, 0, DepthType.Cv8U, 1);
        private static Mat v_mask = new Mat(0, 0, DepthType.Cv8U, 1);
        private static Mat hsv_mask = new Mat(0, 0, DepthType.Cv8U, 3);
        private static Mat[] src_channels;

        //Events
        public event PropertyChangedEventHandler PropertyChanged; // Событие, которое нужно вызывать при изменении
        public void RaisePropertyChanged(string propertyName)
        {
            //Если кто-то на него подписан, то вызывем его
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //metods: get-set;
        //main
        public static void SetImgOriginal(string fileName)
        {
            matOriginal = new Mat(fileName, LoadImageType.AnyColor);
        }
        public static BitmapSource ImgOriginal
        {
            get { return ToBitmapSource(matOriginal); }
        }
        public static BitmapSource ImgThresh
        {
            get { return ToBitmapSource(imgThresh); }
        }
        public static BitmapSource ImgGrey
        {
            get { return ToBitmapSource(matGray); }
        }
        public static BitmapSource ImgRezult
        {
            get { return ToBitmapSource(imgRezult); }
        }
        //additional//
        public static BitmapSource ImgH_plane
        {
            get { return ToBitmapSource(h_plane); }
        }
        public static BitmapSource ImgS_plane
        {
            get { return ToBitmapSource(s_plane); }
        }
        public static BitmapSource ImgV_plane
        {
            get { return ToBitmapSource(v_plane); }
        }
        public static BitmapSource ImgH_mask
        {
            get { return ToBitmapSource(h_mask); }
        }
        public static BitmapSource ImgS_mask
        {
            get { return ToBitmapSource(s_mask); }
        }
        public static BitmapSource ImgV_mask
        {
            get { return ToBitmapSource(v_mask); }
        }
        public static BitmapSource ImgHSV_mask
        {
            get { return ToBitmapSource(hsv_mask); }
        }
        public static Mat MatThresh
        {
            get { return matThresh; }
        }

        //metods: operations
        public DtoEllipsLimits FndChannels()
        {
            //конвертируем в HSV 
            CvInvoke.CvtColor(matOriginal, hsv_mask, ColorConversion.Bgr2Hsv);
            //разбиваем на отельные каналы
            src_channels = hsv_mask.Split();
            h_plane = src_channels[0];
            s_plane = src_channels[1];
            v_plane = src_channels[2];
            // определяем минимальное и максимальное значение
            // у каналов HSV
            double framemin = 0;
            double framemax = 0;
            var limits = new DtoEllipsLimits();
            System.Drawing.Point p = new System.Drawing.Point();
            CvInvoke.MinMaxLoc(h_plane, ref framemin, ref framemax, ref p, ref p);
            limits.hmin = (byte)framemin;
            limits.hmax = (byte)framemax;
            CvInvoke.MinMaxLoc(s_plane, ref framemin, ref framemax, ref p, ref p);
            limits.smin = (byte)framemin;
            limits.smax = (byte)framemax;
            CvInvoke.MinMaxLoc(v_plane, ref framemin, ref framemax, ref p, ref p);
            limits.vmin = (byte)framemin;
            limits.vmax = (byte)framemax;
            CvInvoke.CvtColor(matOriginal, matGray, ColorConversion.Bgr2Gray);
            return limits;
        }
        public static void PostProses(Ellips ellipse, bool isRepaint = true)
        {
            CvInvoke.InRange(h_plane, new ScalarArray(ellipse.Hmin), new ScalarArray(ellipse.Hmax), h_mask);
            CvInvoke.InRange(s_plane, new ScalarArray(ellipse.Smin), new ScalarArray(ellipse.Smax), s_mask);
            CvInvoke.InRange(v_plane, new ScalarArray(ellipse.Vmin), new ScalarArray(ellipse.Vmax), v_mask);

            CvInvoke.BitwiseAnd(h_mask, s_mask, matThresh);
            CvInvoke.BitwiseAnd(matThresh, v_mask, matThresh);

            Mat st1 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(21, 21), new System.Drawing.Point(10, 10));
            Mat st2 = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(11, 11), new System.Drawing.Point(5, 5));
            CvInvoke.MorphologyEx(matThresh, matThresh, MorphOp.Close, st1, new System.Drawing.Point(10, 10), 1, BorderType.Default, new MCvScalar());
            CvInvoke.MorphologyEx(matThresh, matThresh, MorphOp.Open, st2, new System.Drawing.Point(5, 5), 1, BorderType.Default, new MCvScalar());
            CvInvoke.GaussianBlur(matThresh, matThresh, new System.Drawing.Size(5, 5), 2);

            Image<Gray, Byte> src = new Image<Gray, Byte>(matThresh.Width, matThresh.Height);
            if (isRepaint)
                imgThresh = new Image<Gray, Byte>(matThresh.Width, matThresh.Height);
            src = new Mat(matThresh, ellipse.Rect).ToImage<Gray, Byte>();
            CvInvoke.cvSetImageROI(imgThresh, ellipse.Rect);
            CvInvoke.cvCopy(src, imgThresh, new IntPtr(0));
            CvInvoke.cvResetImageROI(imgThresh);
        }
        public static void PaintRezultImage(double x, double y, double radius,
                                            Rectangle rect, bool isRepaint = true)
        {
            matRezult = matOriginal;

            Image<Bgr, Byte> src = new Image<Bgr, Byte>(matThresh.Width, matThresh.Height);
            try
            {
                if (isRepaint)
                {
                    imgRezult = new Image<Bgr, Byte>(matThresh.Width, matThresh.Height);
                    imgRezult = matOriginal.ToImage<Bgr, Byte>();
                }
                src = new Mat(matGray, rect).ToImage<Bgr, Byte>();

                CvInvoke.cvSetImageROI(imgRezult, rect);
                CvInvoke.cvCopy(src, imgRezult, new IntPtr(0));
                CvInvoke.cvResetImageROI(imgRezult);

                CvInvoke.Circle(imgRezult, new System.Drawing.Point((int)x, (int)y), (int)radius, new MCvScalar(0, 0, 255), 2);
                CvInvoke.Circle(imgRezult, new System.Drawing.Point((int)x, (int)y), 3, new MCvScalar(0, 255, 0), -1);
            }
            catch { }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

        public static BitmapSource ToBitmapSource(IImage image)
        {
            using (System.Drawing.Bitmap source = image.Bitmap)
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                DeleteObject(ptr); //release the HBitmap
                return bs;
            }
        }

        public static Mat ToMat(BitmapSource source)
        {

            if (source.Format == PixelFormats.Bgra32)
            {
                Mat result = new Mat();
                result.Create(source.PixelHeight, source.PixelWidth, DepthType.Cv8U, 4);
                source.CopyPixels(Int32Rect.Empty, result.DataPointer, result.Step * result.Rows, result.Step);
                return result;
            }
            else if (source.Format == PixelFormats.Bgr24)
            {
                Mat result = new Mat();
                result.Create(source.PixelHeight, source.PixelWidth, DepthType.Cv8U, 3);
                source.CopyPixels(Int32Rect.Empty, result.DataPointer, result.Step * result.Rows, result.Step);
                return result;
            }
            else
            {
                throw new Exception(String.Format("Convertion from BitmapSource of format {0} is not supported.", source.Format));
            }
        }
    }
}