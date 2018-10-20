using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BarkLight
{

    class Kinect
    {
        private KinectSensor sensor;

        public KinectSensor Sensor
        {
            get { return sensor; }
            set { sensor = value; }
        }

        private DepthImagePixel[] depthPool;//all pixels
        #region interfaces
        private byte[] colorBuffer;
        public byte[] ColorBuffer
        {
            get { return colorBuffer; }
            set { colorBuffer = value; }
        }
        private DepthImagePoint[] depthBuffer;//only pixels in the viewbox;
        public DepthImagePoint[] DepthBuffer
        {
            get { return depthBuffer; }
            set { depthBuffer = value; }
        }

        private byte[] depthImage;
        public byte[] DepthImage
        {
            get { return depthImage; }
            set { depthImage = value; }
        }

        private int depthWidth;

        public int DepthWidth
        {
            get { return depthWidth; }
            set { depthWidth = value; }
        }
        private int depthHeight;

        public int DepthHeight
        {
            get { return depthHeight; }
            set { depthHeight = value; }
        }
        private int colorWidth;

        public int ColorWidth
        {
            get { return colorWidth; }
            set { colorWidth = value; }
        }
        private int colorHeight;

        public int ColorHeight
        {
            get { return colorHeight; }
            set { colorHeight = value; }
        }

        #endregion
        
        public static readonly float[] CameraPosition = new float[] { 0, 0, -30 };
        public static int RotationDegree = 0;
        public static int FocalLength = 0;
        public static float VAngle = 47f;
        public static float HAngle = 57f;


        public void InitializeKinect()
        {
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }
            if (null != this.sensor)
            {
                //Initialize the depth sensor
                this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                this.depthPool = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
                this.sensor.DepthFrameReady += DepthFrameReady;
                //this.sensor.SkeletonFrameReady += Sensor_SkeletonFrameReady;

                Kinect.FocalLength = sensor.DepthStream.MinDepth;

                //Initialize the RGB camera
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                this.sensor.ColorFrameReady += this.ColorImageReady;
                
            }
            if (null == this.sensor)
            {
                //Initialize helpers
                Kinect.RotationDegree = (int)0;
                Kinect.FocalLength = 400;
                Console.WriteLine("Kinect not connected");
            }
        }

        //private void Sensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        //{
        //    //declare an array of Skeletons
        //    Skeleton[] skeletons = new Skeleton[1];

        //    //Opens a SkeletonFrame object, which contains one frame of skeleton data.
        //    using (SkeletonFrame skeletonframe = e.OpenSkeletonFrame())
        //    {
        //        //Check if the Frame is Indeed open 
        //        if (skeletonframe != null)
        //        {

        //            skeletons = new Skeleton[skeletonframe.SkeletonArrayLength];

        //            // Copies skeleton data to an array of Skeletons, where each Skeleton contains a collection of the joints.
        //            skeletonframe.CopySkeletonDataTo(skeletons);

        //            //draw the Skeleton based on the Default Mode(Standing), "Seated"
        //            if (sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Default)
        //            {
        //                //Draw standing Skeleton
        //                DrawStandingSkeletons(skeletons);
        //            }
        //            else if (sensor.SkeletonStream.TrackingMode == SkeletonTrackingMode.Seated)
        //            {
        //                //Draw a Seated Skeleton with 10 joints
        //                DrawSeatedSkeletons(skeletons);
        //            }
        //        }
        //    }
        //}

        public void Run()
        {
            try
            {
                if (this.sensor == null) {
                    InitializeKinect();
                }
                this.sensor.Start();
                this.sensor.ElevationAngle = Kinect.RotationDegree;
            }
            catch (IOException)
            {
                this.sensor = null;
            }
            Console.WriteLine("Kinect started");
        }
        public void CloseKinect()
        {
            if (null != this.sensor && sensor.IsRunning)
            {
                this.sensor.ElevationAngle = 0;
                sensor.Stop();
                sensor.Dispose();
            }
        }

        internal void UpdateAngle(int v)
        {

            Kinect.RotationDegree = v;
            System.Diagnostics.Debug.WriteLine(Kinect.RotationDegree);
            this.sensor.ElevationAngle = Kinect.RotationDegree;
        }

        private void ColorImageReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {
                    if (this.colorBuffer == null)
                    {
                        colorWidth = colorFrame.Width;
                        colorHeight = colorFrame.Height;
                        this.colorBuffer = new byte[this.sensor.ColorStream.FramePixelDataLength];
                    }
                    colorFrame.CopyPixelDataTo(this.colorBuffer);
                }
            }
        }

        public event EventHandler<DepthCameraReadyEventArgs> DepthCameraReady;

        public class DepthCameraReadyEventArgs : EventArgs
        {
            public Bitmap Bitmap { get; set; }
        }

        public event EventHandler<DepthCameraReadyEventArgs> SkeletonFrameReady;

        public class SkeletonFrameReadyEventArgs : EventArgs
        {
        }

        private void DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {

                    depthFrame.CopyDepthImagePixelDataTo(this.depthPool);
                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;
                    if (this.depthImage == null)
                    {
                        this.depthImage = new byte[this.sensor.DepthStream.FramePixelDataLength * sizeof(int)];
                        this.depthBuffer = new DepthImagePoint[this.sensor.DepthStream.FramePixelDataLength];
                        this.depthWidth = depthFrame.Width;
                        this.depthHeight = depthFrame.Height;
                    }
                    int pixelIndex = 0;
                    for (int i = 0; i < this.depthPool.Length; ++i)
                    {
                        short depth = depthPool[i].Depth;
                        if (depth<1500)
                        {
                            byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);
                            this.depthImage[pixelIndex++] = intensity;
                            this.depthImage[pixelIndex++] = intensity;
                            this.depthImage[pixelIndex++] = intensity;
                            ++pixelIndex;

                            depthBuffer[i] = new DepthImagePoint();
                            depthBuffer[i].X = i % depthWidth;
                            depthBuffer[i].Y = i / depthWidth;
                            depthBuffer[i].Depth = depth;
                        }
                        else
                        {
                            depthBuffer[i].Depth = -1;
                            this.depthImage[pixelIndex++] = 0;
                            this.depthImage[pixelIndex++] = 0;
                            this.depthImage[pixelIndex++] = 0;
                            ++pixelIndex;
                        }
                    }
                    //display image
                    DepthCameraReady?.Invoke(this, new DepthCameraReadyEventArgs
                    {
                        Bitmap = GetDepthImage()
                    });
                }
            }
        }

        internal Bitmap GetDepthImage()
        {
            Bitmap bitmapImage = null;
            if (DepthImage != null && null != Sensor)
            {
                bitmapImage = new Bitmap(DepthWidth, DepthHeight, PixelFormat.Format32bppRgb);
                lock (bitmapImage)
                {
                    var g = Graphics.FromImage(bitmapImage);
                    g.Clear(System.Drawing.Color.FromArgb(0, 0, 0));

                    //Copy the depth frame data onto the bitmap  
                    BitmapData bmapdata = bitmapImage.LockBits(
                        new System.Drawing.Rectangle(0, 0, DepthWidth, DepthHeight),
                        ImageLockMode.WriteOnly, bitmapImage.PixelFormat);
                    IntPtr ptr = bmapdata.Scan0;
                    Marshal.Copy(DepthImage, 0, ptr, DepthWidth * DepthHeight * sizeof(int));
                    bitmapImage.UnlockBits(bmapdata);
                }
            }
            return bitmapImage;
        }


    }
}
