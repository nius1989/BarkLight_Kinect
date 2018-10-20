using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

namespace BarkLight
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Kinect kinect=new Kinect();
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            kinect.InitializeKinect();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            kinect.CloseKinect();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.Run();
            kinect.DepthCameraReady += Kinect_DepthCameraReady;
        }

        private void Up_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.UpdateAngle(20);
        }

        private void Down_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.UpdateAngle(0);
        }

        private void End_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.CloseKinect();
        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            //connect to Arduino.
        }

        private void Kinect_DepthCameraReady(object sender, Kinect.DepthCameraReadyEventArgs e)
        {
            depthIamge.Source = BitmapToImageSource(kinect.GetDepthImage());
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
