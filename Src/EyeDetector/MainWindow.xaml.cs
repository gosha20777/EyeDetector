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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Ellips[] eye = new Ellips[1];
        private Ellips[] point = new Ellips[1];
        //GUI
        private OpenFileDialog openDig = new OpenFileDialog();
        private SaveFileDialog saveDig = new SaveFileDialog();

        private int surIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            Gui_Initialize();
        }
        private void mFileOpen_Click(object sender, RoutedEventArgs e)
        {
            openDig.Title = "Выбирете файл...";
            openDig.Filter = "Картинки (*.bmp; *.img; *.jpg;)|*.bmp;*.img;*.jpg;";
            openDig.Multiselect = true;
            if (openDig.ShowDialog() == true && openDig.CheckFileExists)
            {
                Gui_LoadImages(openDig.FileNames, openDig.FileNames.Count());
            }         
        }
        
        //eye
        private void slZHmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                eye[surIndex].Hmax = (byte)slZHmax.Value;
                Gui_RepaintImage(eMode.eye);
            }
            catch { }     
        }
        private void slZHmin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                eye[surIndex].Hmin = (byte)slZHmin.Value;
                Gui_RepaintImage(eMode.eye);
            }
            catch { }
        }
        private void slZSmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                eye[surIndex].Smax = (byte)slZSmax.Value;
                Gui_RepaintImage(eMode.eye);
            }
            catch { }         
        }
        private void slZSmin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                eye[surIndex].Smin = (byte)slZSmin.Value;
                Gui_RepaintImage(eMode.eye);
            }
            catch { }
        }
        private void slZVmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                eye[surIndex].Vmax = (byte)slZVmax.Value;
                Gui_RepaintImage(eMode.eye);
            }
            catch { }
        }       
        private void slZVmin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                eye[surIndex].Vmin = (byte)slZVmin.Value;
                Gui_RepaintImage(eMode.eye);
            }
            catch { }         
        }
        
        //Point
        private void slPHmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                point[surIndex].Hmax = (byte)slPHmax.Value;
                Gui_RepaintImage(eMode.point);
            }
            catch { }
        }
        private void slPHmin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                point[surIndex].Hmin = (byte)slPHmin.Value;
                Gui_RepaintImage(eMode.point);
            }
            catch { }
        }
        private void slPSmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                point[surIndex].Smax = (byte)slPSmax.Value;
                Gui_RepaintImage(eMode.point);
            }
            catch { }
        }
        private void slPSmin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                point[surIndex].Smin = (byte)slPSmin.Value;
                Gui_RepaintImage(eMode.point);
            }
            catch { }
        }
        private void slPVmax_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                point[surIndex].Vmax = (byte)slPVmax.Value;
                Gui_RepaintImage(eMode.point);
            }
            catch { }
        }
        private void slPVmin_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                point[surIndex].Vmin = (byte)slPVmin.Value;
                Gui_RepaintImage(eMode.point);
            }
            catch { }
        }

        private void mRectEye_Click(object sender, RoutedEventArgs e)
        {
            mRectEye.IsChecked = !mRectEye.IsChecked;
            mRectPoint.IsChecked = false;
            typeof(Button).GetMethod("set_IsPressed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(bEye, new object[] { mRectEye.IsChecked });
            typeof(Button).GetMethod("set_IsPressed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(bPoint, new object[] { mRectPoint.IsChecked });
        }
        private void mRectPoint_Click(object sender, RoutedEventArgs e)
        {
            mRectPoint.IsChecked = !mRectPoint.IsChecked;
            mRectEye.IsChecked = false;
            typeof(Button).GetMethod("set_IsPressed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(bEye, new object[] { mRectEye.IsChecked });
            typeof(Button).GetMethod("set_IsPressed", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(bPoint, new object[] { mRectPoint.IsChecked });
        }

        private void imgResult_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_AddDrowbleRect(eMode.eye, ref imgResult);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_AddDrowbleRect(eMode.point, ref imgResult);
            }
        }
        private void imgMasked_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_AddDrowbleRect(eMode.eye, ref imgMasked);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_AddDrowbleRect(eMode.point, ref imgMasked);
            }
        }
        private void imgVChanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_AddDrowbleRect(eMode.eye, ref imgVChanel);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_AddDrowbleRect(eMode.point, ref imgVChanel);
            }
        }
        private void imgOriginal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_AddDrowbleRect(eMode.eye, ref imgOriginal);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_AddDrowbleRect(eMode.point, ref imgOriginal);
            }
        }

        private void imgResult_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mRectPoint.IsChecked || mRectEye.IsChecked)
            {
                this.Cursor = Cursors.Arrow;
            }
        }
        private void imgMasked_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mRectPoint.IsChecked || mRectEye.IsChecked)
            {
                this.Cursor = Cursors.Arrow;
            }
        }
        private void imgVChanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mRectPoint.IsChecked || mRectEye.IsChecked)
            {
                this.Cursor = Cursors.Arrow;
            }
        }
        private void imgOriginal_MouseLeave(object sender, MouseEventArgs e)
        {

            if (mRectPoint.IsChecked || mRectEye.IsChecked)
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        private void imgResult_MouseMove(object sender, MouseEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_ResizeDrowbleRect(eMode.eye, ref imgResult);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_ResizeDrowbleRect(eMode.point, ref imgResult);
            }
        }
        private void imgMasked_MouseMove(object sender, MouseEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_ResizeDrowbleRect(eMode.eye, ref imgMasked);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_ResizeDrowbleRect(eMode.point, ref imgMasked);
            }
        }
        private void imgVChanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_ResizeDrowbleRect(eMode.eye, ref imgVChanel);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_ResizeDrowbleRect(eMode.point, ref imgVChanel);
            }
        }
        private void imgOriginal_MouseMove(object sender, MouseEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_ResizeDrowbleRect(eMode.eye, ref imgOriginal);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_ResizeDrowbleRect(eMode.point, ref imgOriginal);
            }
        }

        private void imgResult_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_AplayDrowbleRect(eMode.eye);
                mRectEye_Click(sender, e);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_AplayDrowbleRect(eMode.point);
                mRectPoint_Click(sender, e);
            }
        }
        private void imgMasked_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_AplayDrowbleRect(eMode.eye);
                mRectEye_Click(sender, e);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_AplayDrowbleRect(eMode.point);
                mRectPoint_Click(sender, e);
            }
        }
        private void imgVChanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_AplayDrowbleRect(eMode.eye);
                mRectEye_Click(sender, e);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_AplayDrowbleRect(eMode.point);
                mRectPoint_Click(sender, e);
            }
        }
        private void imgOriginal_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mRectEye.IsChecked)
            {
                Gui_AplayDrowbleRect(eMode.eye);
                mRectEye_Click(sender, e);
            }
            if (mRectPoint.IsChecked)
            {
                Gui_AplayDrowbleRect(eMode.point);
                mRectPoint_Click(sender, e);
            }
        }

        private void slFrmControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                surIndex = (int)slFrmControl.Value - 1;
                Gui_SetCorrectFrame(openDig.FileNames);
            }
            catch { };   
        }

        private void mStart_Click(object sender, RoutedEventArgs e)
        {
            Gui_RunProgram(openDig.FileNames);
        }

        private void mFileSave_Click(object sender, RoutedEventArgs e)
        {
            saveDig.FileName = "Output";
            saveDig.Title = "Сохранить...";
            saveDig.Filter = "Text files(*.txt)|*.txt";
            if (saveDig.ShowDialog() == true && saveDig.CheckPathExists)
            {
                Gui_Output(saveDig.FileName);
            }
        }

        private void bFrmAddGange_Click(object sender, RoutedEventArgs e)
        {
            if (stGange0.Visibility == Visibility.Hidden)
                stGange0.Visibility = Visibility.Visible;
            else if (stGange1.Visibility == Visibility.Hidden)
                stGange1.Visibility = Visibility.Visible;
            else if (stGange2.Visibility == Visibility.Hidden)
                stGange2.Visibility = Visibility.Visible;
            else if (stGange3.Visibility == Visibility.Hidden)
                stGange3.Visibility = Visibility.Visible;
            else if (stGange4.Visibility == Visibility.Hidden)
                stGange4.Visibility = Visibility.Visible;
        }
    }
}