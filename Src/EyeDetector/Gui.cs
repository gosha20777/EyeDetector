using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;
using System.Diagnostics;

namespace EyeDetector
{
    public partial class MainWindow : Window
    {
        //Fildes
        static byte vp_numb = 4;
        public enum eMode { eye, point, full }
        private Polygon[] eyeRect = new Polygon[vp_numb];
        private Polygon[] pointRect = new Polygon[vp_numb];

        private bool isEyeRectDrown = false;
        private bool isPointRectDrown = false;

        private byte hmin = 0;
        private byte hmax = 255;
        private byte smin = 0;
        private byte smax = 255;
        private byte vmin = 0;
        private byte vmax = 255;

        public void Gui_Initialize()
        {
            for (byte i = 0; i < vp_numb; i++)
            {
                eyeRect[i] = new Polygon();
                pointRect[i] = new Polygon();
                switch (i)
                {
                    case 0:
                        imgOriginal.Children.Add(eyeRect[i]);
                        imgOriginal.Children.Add(pointRect[i]);
                        break;
                    case 1:
                        imgResult.Children.Add(eyeRect[i]);
                        imgResult.Children.Add(pointRect[i]);
                        break;
                    case 2:
                        imgVChanel.Children.Add(eyeRect[i]);
                        imgVChanel.Children.Add(pointRect[i]);
                        break;
                    case 3:
                        imgMasked.Children.Add(eyeRect[i]);
                        imgMasked.Children.Add(pointRect[i]);
                        break;
                    default:
                        Console.WriteLine("ERROR: Invalid Value of View ports!");
                        break;
                }
            }
            Console.WriteLine("EyeDetector ver 0.1.3a");
        }
        public void Gui_LoadImages(string[] FileNames, int ItemsCount)
        {
            Array.Resize(ref eye, ItemsCount);
            Array.Resize(ref point, ItemsCount);

            //Initialize 1st frame
            Core.SetImgOriginal(FileNames[0]);
            var core = new Core();
            var limits = core.FndChannels();
            Core.PostProses(new Ellips());
            Core.PaintRezultImage(0, 0, 0, new System.Drawing.Rectangle());
            //Initialize Ellipses
            for (int i = 0; i < eye.Count(); i++)
            {
                eye[i] = new Ellips();
                point[i] = new Ellips();

                eye[i].Hmin = hmin;
                eye[i].Hmax = hmax;
                eye[i].Smin = smin;
                eye[i].Smax = smax;
                eye[i].Vmin = vmin;
                eye[i].Vmax = vmax;

                point[i].Hmin = hmin;
                point[i].Hmax = hmax;
                point[i].Smin = smin;
                point[i].Smax = smax;
                point[i].Vmin = vmin;
                point[i].Vmax = vmax;
            }

            //Set form params
            slFrmControl.Maximum = ItemsCount;
            slFrmControl.SelectionEnd = ItemsCount;

            imgOriginal.Width = Core.ImgOriginal.Width;
            imgOriginal.Height = Core.ImgOriginal.Height;
            imgOriginal.Background = new ImageBrush(Core.ImgOriginal) { Stretch = Stretch.Uniform };

            imgResult.Width = Core.ImgOriginal.Width;
            imgResult.Height = Core.ImgOriginal.Height;
            imgResult.Background = new ImageBrush(Core.ImgRezult) { Stretch = Stretch.Uniform };

            imgVChanel.Width = Core.ImgOriginal.Width;
            imgVChanel.Height = Core.ImgOriginal.Height;
            imgVChanel.Background = new ImageBrush(Core.ImgGrey) { Stretch = Stretch.Uniform };

            imgMasked.Width = Core.ImgOriginal.Width;
            imgMasked.Height = Core.ImgOriginal.Height;
            imgMasked.Background = new ImageBrush(Core.ImgThresh) { Stretch = Stretch.Uniform };

            slZHmax.Value = eye[0].Hmax;
            slZHmin.Value = eye[0].Hmin;
            slZSmax.Value = eye[0].Smax;
            slZSmin.Value = eye[0].Smin;
            slZVmax.Value = eye[0].Vmin;
            slZVmin.Value = eye[0].Vmin;
            slPHmax.Value = point[0].Hmax;
            slPHmin.Value = point[0].Hmin;
            slPSmax.Value = point[0].Smax;
            slPSmin.Value = point[0].Smin;
            slPVmax.Value = point[0].Vmin;
            slPVmin.Value = point[0].Vmin;
            Console.WriteLine("Inage loaded.");
        }
        public void Gui_RepaintImage(eMode mode)
        {
            if(mode == eMode.eye)
            {
                Core.PostProses(eye[surIndex]);
                eye[surIndex].DetectEllips(Core.MatThresh);
                Core.PaintRezultImage(eye[surIndex].X, eye[surIndex].Y, eye[surIndex].Radius, eye[surIndex].Rect);
                Core.PostProses(point[surIndex], false);
                Core.PaintRezultImage(point[surIndex].X, point[surIndex].Y, point[surIndex].Radius, point[surIndex].Rect, false);
            }
            else if (mode == eMode.point)
            {
                Core.PostProses(eye[surIndex]);
                Core.PaintRezultImage(eye[surIndex].X, eye[surIndex].Y, eye[surIndex].Radius, eye[surIndex].Rect);
                Core.PostProses(point[surIndex], false);
                point[surIndex].DetectEllips(Core.MatThresh);
                Core.PaintRezultImage(point[surIndex].X, point[surIndex].Y, point[surIndex].Radius, point[surIndex].Rect, false);
            }
            else if (mode == eMode.full)
            {
                Core.PostProses(eye[surIndex]);
                eye[surIndex].DetectEllips(Core.MatThresh);
                Core.PaintRezultImage(eye[surIndex].X, eye[surIndex].Y, eye[surIndex].Radius, eye[surIndex].Rect);
                Core.PostProses(point[surIndex], false);
                point[surIndex].DetectEllips(Core.MatThresh);
                Core.PaintRezultImage(point[surIndex].X, point[surIndex].Y, point[surIndex].Radius, point[surIndex].Rect, false);
            }
            else
                Console.WriteLine("Error: Invalod value of mode in {Gui.RepaintImage} ");
            imgMasked.Background = new ImageBrush(Core.ImgThresh) { Stretch = Stretch.Uniform };
            imgResult.Background = new ImageBrush(Core.ImgRezult) { Stretch = Stretch.Uniform };
        }
        public void Gui_AddDrowbleRect(eMode mode, ref Canvas img)
        {
            if(mode == eMode.eye)
            {
                for (byte i = 0; i < vp_numb; i++)
                {
                    eyeRect[i].Stroke = Brushes.Red;
                    eyeRect[i].StrokeThickness = 3;
                    eyeRect[i].Points = new PointCollection();
                    eyeRect[i].Points.Add(Mouse.GetPosition(img));
                }
            }
            else if (mode == eMode.point)
            {
                for (byte i = 0; i < vp_numb; i++)
                {
                    pointRect[i].Stroke = Brushes.Yellow;
                    pointRect[i].StrokeThickness = 3;
                    pointRect[i].Points = new PointCollection();
                    pointRect[i].Points.Add(Mouse.GetPosition(img));
                }
            }
            else
                Console.WriteLine("Error: Invalod value of mode in {Gui.AddDrowbleRect} ");
        }
        public void Gui_ResizeDrowbleRect(eMode mode, ref Canvas img)
        {
            if (mode == eMode.eye)
            {
                this.Cursor = Cursors.Cross;
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    for (byte i = 0; i < vp_numb; i++)
                    {
                        Point p0 = eyeRect[i].Points[0];
                        eyeRect[i].Points.Clear();
                        Point p1 = new Point(p0.X, Mouse.GetPosition(img).Y);
                        Point p2 = new Point(Mouse.GetPosition(img).X, p0.Y);
                        Point p3 = new Point(Mouse.GetPosition(img).X, Mouse.GetPosition(img).Y);
                        eyeRect[i].Points.Add(p0);
                        eyeRect[i].Points.Add(p1);
                        eyeRect[i].Points.Add(p3);
                        eyeRect[i].Points.Add(p2);
                    }
                    isEyeRectDrown = true;
                }
            }
            else if (mode == eMode.point)
            {
                this.Cursor = Cursors.Cross;
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    for (byte i = 0; i < vp_numb; i++)
                    {
                        Point p0 = pointRect[i].Points[0];
                        pointRect[i].Points.Clear();
                        Point p1 = new Point(p0.X, Mouse.GetPosition(img).Y);
                        Point p2 = new Point(Mouse.GetPosition(img).X, p0.Y);
                        Point p3 = new Point(Mouse.GetPosition(img).X, Mouse.GetPosition(img).Y);
                        pointRect[i].Points.Add(p0);
                        pointRect[i].Points.Add(p1);
                        pointRect[i].Points.Add(p3);
                        pointRect[i].Points.Add(p2);
                    }
                    isPointRectDrown = true;
                }
            }
            else
                Console.WriteLine("Error: Invalod value of mode in {Gui.ResizeDrowbleRect} ");
        }
        public void Gui_AplayDrowbleRect(eMode mode)
        {
            if (mode == eMode.eye)
            {
                this.Cursor = Cursors.Arrow;
                if (isEyeRectDrown)
                {
                    for (byte i = 0; i < vp_numb; i++)
                    {
                        eye[surIndex].X0 = (int)eyeRect[i].Points[0].X;
                        eye[surIndex].Y0 = (int)eyeRect[i].Points[0].Y;
                        eye[surIndex].Width = (int)(eyeRect[i].Points[2].X - eyeRect[i].Points[0].X);
                        eye[surIndex].Height = (int)(eyeRect[i].Points[2].Y - eyeRect[i].Points[0].Y);
                    }
                    isEyeRectDrown = false;
                    Console.WriteLine("Eye selected.");
                }
            }
            else if (mode == eMode.point)
            {
                this.Cursor = Cursors.Arrow;
                if (isPointRectDrown)
                {
                    for (byte i = 0; i < vp_numb; i++)
                    {
                        point[surIndex].X0 = (int)pointRect[i].Points[0].X;
                        point[surIndex].Y0 = (int)pointRect[i].Points[0].Y;
                        point[surIndex].Width = (int)(pointRect[i].Points[2].X - pointRect[i].Points[0].X);
                        point[surIndex].Height = (int)(pointRect[i].Points[2].Y - pointRect[i].Points[0].Y);
                    }
                    isPointRectDrown = false;
                    Console.WriteLine("Point selected");
                }
            }
            else
                Console.WriteLine("Error: Invalod value of mode in {Gui.AplayDrowbleRect} ");
        }
        public void Gui_SetCorrectFrame(string[] FileNames)
        {
            eye[surIndex].Hmax = (byte)slZHmax.Value;
            eye[surIndex].Hmin = (byte)slZHmin.Value;
            eye[surIndex].Smax = (byte)slZSmax.Value;
            eye[surIndex].Smin = (byte)slZSmin.Value;
            eye[surIndex].Vmax = (byte)slZVmax.Value;
            eye[surIndex].Vmin = (byte)slZVmin.Value;
            point[surIndex].Hmax = (byte)slPHmax.Value;
            point[surIndex].Hmin = (byte)slPHmin.Value;
            point[surIndex].Smax = (byte)slPSmax.Value;
            point[surIndex].Smin = (byte)slPSmin.Value;
            point[surIndex].Vmax = (byte)slPVmax.Value;
            point[surIndex].Vmin = (byte)slPVmin.Value;

            eye[surIndex].X0 = (int)eyeRect[0].Points[0].X;
            eye[surIndex].Y0 = (int)eyeRect[0].Points[0].Y;
            eye[surIndex].Width = (int)(eyeRect[0].Points[2].X - eyeRect[0].Points[0].X);
            eye[surIndex].Height = (int)(eyeRect[0].Points[2].Y - eyeRect[0].Points[0].Y);

            point[surIndex].X0 = (int)pointRect[0].Points[0].X;
            point[surIndex].Y0 = (int)pointRect[0].Points[0].Y;
            point[surIndex].Width = (int)(pointRect[0].Points[2].X - pointRect[0].Points[0].X);
            point[surIndex].Height = (int)(pointRect[0].Points[2].Y - pointRect[0].Points[0].Y);

            Core.SetImgOriginal(FileNames[surIndex]);

            var core = new Core();
            var limits = core.FndChannels();
            Core.PostProses(eye[surIndex]);
            eye[surIndex].DetectEllips(Core.MatThresh);
            Core.PaintRezultImage(eye[surIndex].X, eye[surIndex].Y, eye[surIndex].Radius, eye[surIndex].Rect);
            Core.PostProses(point[surIndex], false);
            point[surIndex].DetectEllips(Core.MatThresh);
            Core.PaintRezultImage(point[surIndex].X, point[surIndex].Y, point[surIndex].Radius, point[surIndex].Rect, false);

            imgOriginal.Background = new ImageBrush(Core.ImgOriginal) { Stretch = Stretch.Uniform };
            imgVChanel.Background = new ImageBrush(Core.ImgGrey) { Stretch = Stretch.Uniform };
            imgMasked.Background = new ImageBrush(Core.ImgThresh) { Stretch = Stretch.Uniform };
            imgResult.Background = new ImageBrush(Core.ImgRezult) { Stretch = Stretch.Uniform };
        }
        public void Gui_RunProgram(string[] FileNames)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Start detecting...");
            Console.WriteLine("Frames: {0}.", eye.Count());
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int begVal = (int)slFrmControl.SelectionStart - 1;
            int endVal = (int)slFrmControl.SelectionEnd;
            for (int i = begVal; i < endVal; i++)
            {
                bool errE, errP;
                eye[i].X0 = (int)eyeRect[0].Points[0].X;
                eye[i].Y0 = (int)eyeRect[0].Points[0].Y;
                eye[i].Width = (int)(eyeRect[0].Points[2].X - eyeRect[0].Points[0].X);
                eye[i].Height = (int)(eyeRect[0].Points[2].Y - eyeRect[0].Points[0].Y);

                point[i].X0 = (int)pointRect[0].Points[0].X;
                point[i].Y0 = (int)pointRect[0].Points[0].Y;
                point[i].Width = (int)(pointRect[0].Points[2].X - pointRect[0].Points[0].X);
                point[i].Height = (int)(pointRect[0].Points[2].Y - pointRect[0].Points[0].Y);

                Core.SetImgOriginal(FileNames[i]);
                var core = new Core();
                var limits = core.FndChannels();
                Core.PostProses(eye[0]);
                errE = eye[i].DetectEllips(Core.MatThresh);
                Core.PostProses(point[surIndex], false);
                errP = point[i].DetectEllips(Core.MatThresh);
                if (errE || errP)
                    Console.ForegroundColor = ConsoleColor.Red;
                else
                    Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("frane {0,4} | eye X {1,3} Y {2,3} | point X {3,3} Y {4,3}", i, eye[i].X, eye[i].Y, point[i].X, point[i].Y);
            }
            stopWatch.Stop();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Detect ented. Time: {0:00}:{1:00} sec", stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds / 10);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        public void Gui_Output(string patch)
        {
                using (System.IO.StreamWriter file =
                   new System.IO.StreamWriter(patch))
            {
                int badcount = 0;
                bool hasValue = false;
                double dx = 0.0, dy = 0.0;
                double lx = 0.0, ly = 0.0;
                double[] resultX = new double[eye.Count()];
                double[] resultY = new double[eye.Count()];
                int added = 0;

                for (int i = 0; i < eye.Count(); i++)
                {
                    bool bad1 = false, bad2 = false;
                    double x, y;

                    bad1 = eye[i].X * eye[i].X + eye[i].Y + eye[i].Y < 0.0001;
                    bad2 = point[i].X * point[i].X + point[i].Y + point[i].Y < 0.0001;

                    if (bad1 || bad2)
                    {
                        badcount++;
                        continue;
                    }

                    //Good Value

                    if (!hasValue)
                    {
                        dx = point[i].X;
                        dy = point[i].Y;
                    }

                    x = eye[i].X - point[i].X + dx;
                    y = eye[i].Y - point[i].Y + dy;

                    for (int j = 0; j < badcount; j++)
                    {
                        resultX[added] = (hasValue ? (lx + (x - lx) / (badcount + 1) * (j + 1)) : x);
                        resultY[added] = (hasValue ? (ly + (y - ly) / (badcount + 1) * (j + 1)) : y);
                        added++;
                    }

                    badcount = 0;
                    lx = x;
                    ly = y;

                    hasValue = true;

                    resultX[added] = x;
                    resultY[added] = y;
                    added++;
                }

                for (int j = 0; j < badcount; j++)
                {
                    resultX[added] = (hasValue ? lx : 0);
                    resultY[added] = (hasValue ? ly : 0);
                    added++;
                }

                double minX, maxX, minY, maxY;
                minX = maxX = resultX[0];
                minY = maxY = resultY[0];
                for (int i = 1; i < added; i++)
                {
                    if (resultX[i] < minX)
                        minX = resultX[i];
                    if (resultX[i] > maxX)
                        maxX = resultX[i];
                    if (resultY[i] < minY)
                        minY = resultY[i];
                    if (resultY[i] > maxY)
                        maxY = resultY[i];
                }
                int w = (int)(maxX - minX);
                int h = (int)(maxY - minY);


                for (int i = 0; i < added; i++)
                {
                    resultX[i] -= minX;
                    resultX[i] += 10.0;
                    resultY[i] -= minY;
                    resultY[i] += 10.0;
                }

                for (int i = 0; i < added; i++)
                {
                    file.WriteLine(string.Format("{0:f3} {1:f3}", resultX[i], resultY[i]));
                }
            }
            Console.WriteLine("file was saved to: " + patch);
        }
    }
}
