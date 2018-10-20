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
        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.Unloaded += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            kinect.CloseKinect();
        }

        private void Start_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.SkeletonInitialized += Kinect_SkeletonFrameReady;
            kinect.Run();
            kinect.SkeletonReady += Kinect_SkeletonReady;
        }

        private void Up_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.UpdateAngle(10);
        }

        private void Hori_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.UpdateAngle(0);
        }

        private void Down_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.UpdateAngle(-10);
        }


        private void End_Button_Click(object sender, RoutedEventArgs e)
        {
            kinect.CloseKinect();
        }

        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            //connect to Arduino.
        }

        private void Kinect_SkeletonFrameReady(object sender, Kinect.SkeletonInitializedEventArgs e)
        {
            imageSource = new DrawingImage(kinect.DrawingGroup);
            jointIamge.Source = imageSource;
        }


        private void Kinect_SkeletonReady(object sender, Kinect.SkeletonEventArgs e)
        {
            head_x.Text = "" + e.HeadHandPoint.X;
            head_y.Text = "" + e.HeadHandPoint.Y;
            handleft_x.Text = "" + e.LeftHandPoint.X;
            handleft_y.Text = "" + e.LeftHandPoint.Y;
            handright_x.Text = "" + e.RightHandPoint.X;
            handright_y.Text = "" + e.RightHandPoint.Y;
        }
    }
}
