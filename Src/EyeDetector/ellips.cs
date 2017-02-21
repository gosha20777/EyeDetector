using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace EyeDetector
{
    public class Ellips : INotifyPropertyChanged
    {
        //filds
        private double x = -1.0, y = -1.0, radious = -1.0;
        private int x0, y0, width, height;

        private byte hmin = 0;
        private byte hmax = 255;
        private byte smin = 0;
        private byte smax = 255;
        private byte vmin = 0;
        private byte vmax = 255;

        //Constructor
        public Ellips()
        {
            x0 = 0;
            y0 = 0;
            width = 0;
            height = 0;
        }

        //Events
        public event PropertyChangedEventHandler PropertyChanged;// Событие, которое нужно вызывать при изменении
        public void RaisePropertyChanged(string propertyName)
        {
            //Если кто-то на него подписан, то вызывем его
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Set-Get metods
        public double X { get { return x; } }
        public double Y { get { return y; } }
        public double Radius { get { return radious; } }
        public int X0 { get { return x0; } set { x0 = value; RaisePropertyChanged("X0"); } }
        public int Y0 { get { return y0; } set { y0 = value; RaisePropertyChanged("Y0"); } }
        public int Width { get { return width; } set { width = value; RaisePropertyChanged("Width"); } }
        public int Height { get { return height; } set { height = value; RaisePropertyChanged("Height"); } }
        public Rectangle Rect
        {
            get { return new Rectangle(x0, y0, width, height); }
            set { x0 = value.X; y0 = value.Y; width = value.Width; height = value.Height; }
        }

        public byte Hmin
        {
            get { return hmin; }
            set { if (value <= 255 && value >= 0) hmin = value; RaisePropertyChanged("Hmin"); }
        }
        public byte Hmax
        {
            get { return hmax; }
            set { if (value <= 255 && value >= 0) hmax = value; RaisePropertyChanged("Hmax"); }
        }
        public byte Smin
        {
            get { return smin; }
            set { if (value <= 255 && value >= 0) smin = value; RaisePropertyChanged("Smin"); }
        }
        public byte Smax
        {
            get { return smax; }
            set { if (value <= 255 && value >= 0) smax = value; RaisePropertyChanged("Smax"); }
        }
        public byte Vmin
        {
            get { return vmin; }
            set { if (value <= 255 && value >= 0) vmin = value; RaisePropertyChanged("Vmin"); }
        }
        public byte Vmax
        {
            get { return vmax; }
            set { if (value <= 255 && value >= 0) vmax = value; RaisePropertyChanged("Vmax"); }
        }
        
        //Metods
        public void DetectEllips(Mat matThreshRect, out bool err, out string errStr)
        {
            err = false;
            errStr = "OK!";
            byte k = 0;
            try
            {
                matThreshRect = new Mat(matThreshRect, Rect);
                CircleF[] circles = CvInvoke.HoughCircles(matThreshRect,
                                    HoughType.Gradient, 2.0, matThreshRect.Rows
                                    / 4, 100, 50, 10, 400);

                foreach (CircleF circle in circles)
                {
                    x = circle.Center.X + x0;
                    y = circle.Center.Y + y0;
                    radious = circle.Radius;
                    k++;
                }
                circles = null;
            }
            catch { };
            if (k == 0 || k > 1) { err = true; errStr = ("Эллипсов найдено: " + Convert.ToString(k)); }
        }

        public bool DetectEllips(Mat matThreshRect, out string errStr)
        {

            bool err = false;
            errStr = "OK!";
            byte k = 0;
            try
            {
                matThreshRect = new Mat(matThreshRect, Rect);
                CircleF[] circles = CvInvoke.HoughCircles(matThreshRect,
                                    HoughType.Gradient, 2.0, matThreshRect.Rows
                                    / 4, 100, 50, 10, 400);

                foreach (CircleF circle in circles)
                {
                    x = circle.Center.X + x0;
                    y = circle.Center.Y + y0;
                    radious = circle.Radius;
                    k++;
                }
                circles = null;
            }
            catch { };
            if (k == 0 || k > 1) { err = true; errStr = ("Эллипсов найдено: " + Convert.ToString(k)); }
            return err;
        }

        public bool DetectEllips(Mat matThreshRect)
        {

            bool err = false;
            byte k = 0;
            try
            {
                matThreshRect = new Mat(matThreshRect, Rect);
                CircleF[] circles = CvInvoke.HoughCircles(matThreshRect,
                                    HoughType.Gradient, 2.0, matThreshRect.Rows
                                    / 4, 100, 50, 10, 400);

                foreach (CircleF circle in circles)
                {
                    x = circle.Center.X + x0;
                    //Console.WriteLine(circle.Center.X);
                    y = circle.Center.Y + y0;
                    radious = circle.Radius;
                    k++;
                }
                circles = null;
            }
            catch { };
            if (k == 0 || k > 1)
                err = true;
            return err;
        }
    }
}