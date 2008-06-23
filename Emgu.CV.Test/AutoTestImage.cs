using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Emgu.CV;
using Emgu.UI;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;

namespace Emgu.CV.Test
{
    [TestFixture]
    public static class AutoTestImage
    {
        [Test]
        public static void TestRunningAvg()
        {
            Image<Gray, Single> img1 = new Image<Gray, float>(100, 40, new Gray(100));
            Image<Gray, Single> img2 = new Image<Gray, float>(100, 40, new Gray(50));
            img1.RunningAvg(img2, 0.5);
        }

        [Test]
        public static void TestSetValue()
        {
            Image<Bgr, Single> img1 = new Image<Bgr, float>(50, 20, new Bgr(8.0, 1.0, 2.0));
            for (int i = 0; i < img1.Width; i++)
                for (int j = 0; j < img1.Height; j++)
                {
                    Bgr c = img1[j,i];
                    Assert.IsTrue(c.Equals(new Bgr(8.0, 1.0, 2.0)));
                }
        }

        [Test]
        public static void TestMinMax()
        {
            Image<Gray, Byte> img1 = new Image<Gray, Byte>(50, 60);
            System.Random r = new Random();

            using (Image<Gray, Byte> img2 = img1.Convert<Byte>(delegate(Byte f) { return (Byte)r.Next(255); }))
            {
                img2._Max(120.0);
                for (int i = 0; i < img2.Width; i++)
                    for (int j = 0; j < img2.Height; j++)
                        Assert.GreaterOrEqual(img2[j,i].Intensity, 120.0);
            }

            using (Image<Gray, Byte> img2 = img1.Convert<Byte>(delegate(Byte f) { return (Byte)r.Next(255); }))
            {
                img2._Min(120.0);
                for (int i = 0; i < img2.Width; i++)
                    for (int j = 0; j < img2.Height; j++)
                        Assert.GreaterOrEqual(120.0, img2[j,i].Intensity);
            }

            using (Image<Gray, Byte> img2 = img1.Convert<Byte>(delegate(Byte f) { return (Byte)r.Next(255); }))
            using (Image<Gray, Byte> img3 = img1.Convert<Byte>(delegate(Byte f) { return (Byte)r.Next(255); }))
            using (Image<Gray, Byte> img4 = img2.Max(img3))
            {
                for (int i = 0; i < img2.Width; i++)
                    for (int j = 0; j < img2.Height; j++)
                    {
                        Point2D<int> location = new Point2D<int>(i, j);
                        Assert.GreaterOrEqual(img4[location].Intensity, img2[location].Intensity);
                        Assert.GreaterOrEqual(img4[j,i].Intensity, img3[j,i].Intensity);
                    }
            }

            /*
            using (Image<Gray, Byte> img2 = img1.Convert<Byte>(delegate(Byte f) { return (Byte)r.Next(255); }))
            using (Image<Gray, Byte> img3 = img1.Convert<Byte>(delegate(Byte f) { return (Byte)r.Next(255); }))
            using (Image<Gray, Byte> img4 = img2.Min(img3))
            {
                for (int i = 0; i < img2.Width; i++)
                    for (int j = 0; j < img2.Height; j++)
                    {
                        Assert.GreaterOrEqual(img2.GetPixel(new Point2D<int>(i, j)).Intensity, img4.GetPixel(new Point2D<int>(i, j)).Intensity);
                        Assert.GreaterOrEqual(img3.GetPixel(new Point2D<int>(i, j)).Intensity, img4.GetPixel(new Point2D<int>(i, j)).Intensity);
                    }
            }*/
        }

        [Test]
        public static void testGenericOperation()
        {
            Image<Gray, Single> img1 = new Image<Gray, float>(50, 20);
            img1.ROI = new Rectangle<double>(10, 50, 19, 1);
            img1.SetValue(5.0);

            Image<Gray, Single> img2 = new Image<Gray, float>(50, 20);
            img2.ROI = new Rectangle<double>(0, 40, 20, 2);
            img2.SetValue(new Gray(2.0));

            Assert.AreEqual(img1.Width, img2.Width);
            Assert.AreEqual(img1.Height, img2.Height);

            DateTime t1 = DateTime.Now;
            Image<Gray, Single> img3 = img1.Add(img2);
            DateTime t2 = DateTime.Now;
            Image<Gray, Single> img4 = img1.Convert<Single, Single>(img2, delegate(Single v1, Single v2) { return v1 + v2; });
            DateTime t3 = DateTime.Now;

            Image<Gray, Single> img5 = img3.AbsDiff(img4);
            DateTime t4 = DateTime.Now;
            double sum1 = img5.Sum.Intensity;
            DateTime t5 = DateTime.Now;
            Single sum2 = 0.0f;
            img5.Action(delegate(Single v) { sum2 += v; });
            DateTime t6 = DateTime.Now;

            /*
            TimeSpan ts1 = t2.Subtract(t1);
            TimeSpan ts2 = t3.Subtract(t2);
            TimeSpan ts3 = t5.Subtract(t4);
            TimeSpan ts4 = t6.Subtract(t5);
            Trace.WriteLine(String.Format("CV Add     : {0} milliseconds", ts1.TotalMilliseconds));
            Trace.WriteLine(String.Format("Generic Add: {0} milliseconds", ts2.TotalMilliseconds));
            Trace.WriteLine(String.Format("CV Sum     : {0} milliseconds", ts3.TotalMilliseconds));
            Trace.WriteLine(String.Format("Generic Sum: {0} milliseconds", ts4.TotalMilliseconds));
            Trace.WriteLine(String.Format("Abs Diff = {0}", sum1));
            Trace.WriteLine(String.Format("Abs Diff = {0}", sum2));*/
            Assert.AreEqual(sum1, sum2);

            img3.Dispose();
            img4.Dispose();
            img5.Dispose();

            t1 = DateTime.Now;
            img3 = img1.Mul(2.0);
            t2 = DateTime.Now;
            img4 = img1.Convert<Single>(delegate(Single v1) { return v1 * 2.0f; });
            t3 = DateTime.Now;
           
            /*
            ts1 = t2.Subtract(t1);
            ts2 = t3.Subtract(t2);
            Trace.WriteLine(String.Format("CV Mul     : {0} milliseconds", ts1.TotalMilliseconds));
            Trace.WriteLine(String.Format("Generic Mul: {0} milliseconds", ts2.TotalMilliseconds));
            */
            Assert.IsTrue(img3.Equals(img4));
            img3.Dispose();
            img4.Dispose();

            t1 = DateTime.Now;
            img3 = img1.Add(img1);
            img4 = img3.Add(img1);
            t2 = DateTime.Now;
            img5 = img1.Convert<Single, Single, Single>(img1, img1, delegate(Single v1, Single v2, Single v3) { return v1 + v2 + v3; });
            t3 = DateTime.Now;
            /*
            ts1 = t2.Subtract(t1);
            ts2 = t3.Subtract(t2);
            Trace.WriteLine(String.Format("CV Sum (3 images)     : {0} milliseconds", ts1.TotalMilliseconds));
            Trace.WriteLine(String.Format("Generic Sum (3 images): {0} milliseconds", ts2.TotalMilliseconds));
            */ 
            Assert.IsTrue(img5.Equals(img4) );
            img3.Dispose();
            img4.Dispose();
            img5.Dispose();

            img1.Dispose();
            img2.Dispose();

            Image<Gray, Byte> gimg1 = new Image<Gray, Byte>(400, 300, new Gray(30));
            Image<Gray, Byte> gimg2 = gimg1.Convert<Byte>(delegate(Byte b) { return (Byte) (255-b); });
            gimg1.Dispose();
            gimg2.Dispose();
        }

        [Test]
        public static void TestConvertDepth()
        {
            Image<Gray, Byte> img1 = new Image<Gray, byte>(100, 100, new Gray(10.0));
            img1._RandUniform(new MCvScalar(0, 0, 0), new MCvScalar(255, 255, 255));
            Image<Gray, Single> img2 = img1.ConvertScale<Single>(2.0, 0.0);
            Image<Gray, Byte> img3 = img2.ConvertScale<Byte>(0.5, 0.0);
            Assert.IsTrue(img3.Equals(img1));

            Image<Gray, Double> img4 = img1.Convert<Gray, Double>();
            Image<Gray, Byte> img5 = img4.Convert<Gray, Byte>();
            Assert.IsTrue(img5.Equals(img1));
        }

        [Test]
        public static void TestMemory()
        {
            for (int i = 0; i <= 500; i++)
            {
                Image<Bgr, Single> img = new Image<Bgr, Single>(1000, 1000, new Bgr());
            }
        }

        [Test]
        public static void TestConversion()
        {
            Image<Bgr, Single> img1 = new Image<Bgr, Single>(100, 100);
            Image<Xyz, Single> img2 = img1.Convert<Xyz, Single>();
        }

        [Test]
        public static void TestGenericSetColor()
        {
            Image<Bgr, Byte> img1 = new Image<Bgr, Byte>(20, 40, new Bgr());

            int flag = 0;

            Image<Bgr, Byte> img2 = img1.Convert<Byte>(
                delegate(Byte b)
                {
                    return ((flag++ % 3) == 0) ? (Byte)255 : (Byte)0;
                });

            img1.SetValue(new Bgr(255, 0, 0));

            Assert.IsTrue(img1.Equals(img2));
        }

        [Test]
        public static void TestRuntimeSerialize()
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(100, 80);

            using (MemoryStream ms = new MemoryStream())
            {
                img._RandNormal(new MCvScalar(100, 100, 100), new MCvScalar(50, 50, 50));

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, img);
                Byte[] bytes = ms.GetBuffer();

                using (MemoryStream ms2 = new MemoryStream(bytes))
                {
                    Image<Bgr, Byte> img2 = (Image<Bgr, Byte>)formatter.Deserialize(ms2);
                    Assert.IsTrue(img.Equals(img2));
                }
            }
        }

        [Test]
        public static void TestSampleLine()
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(101, 133);
            img._RandUniform(new MCvScalar(), new MCvScalar(255, 255, 255));
            Byte[,] buffer = img.Sample(new LineSegment2D<int>(new Point2D<int>(0, 0), new Point2D<int>(0, 100)));
            for (int i = 0; i < 100; i++)
                Assert.IsTrue(img[i, 0].Equals(new Bgr( buffer[i,0], buffer[i,1], buffer[i,2])));
        }


        [Test]
        public static void TestGetSize()
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(10, 10, new Bgr(255, 255, 255));

            Point2D<int> size = img.Size;

        }

        [Test]
        public static void TestXmlSerialize()
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(100, 80);

            img._RandNormal(new MCvScalar(100, 100, 100), new MCvScalar(50, 50, 50));

            XmlDocument doc = Emgu.Utils.XmlSerialize<Image<Bgr, Byte>>(img);

            Image<Bgr, Byte> img2 = Emgu.Utils.XmlDeserialize<Image<Bgr, Byte>>(doc);

            Assert.IsTrue(img.Equals(img2));
        }

        [Test]
        public static void TestRotation()
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(100, 80);

            img._RandNormal(new MCvScalar(100, 100, 100), new MCvScalar(50, 50, 50));
            img.Rotate(90, new Bgr());
        }

        [Test]
        public static void TestConstructor()
        {
            for (int i = 0; i < 20; i++)
            {
                Image<Gray, Byte> img = new Image<Gray, Byte>(500, 500, new Gray());
                Assert.AreEqual(0, System.Convert.ToInt32(img.Sum.Intensity));
            }

            for (int i = 0; i < 20; i++)
            {
                Image<Bgr, Single> img = new Image<Bgr, Single>(500, 500);
                Assert.IsTrue(img.Sum.Equals(new Bgr(0.0, 0.0, 0.0)));
            }

            Image<Bgr, Byte> img2 = new Image<Bgr, byte>(1, 2);
            Assert.AreEqual(img2.Data.GetLength(1), 4);

            Byte[, ,] data = new Byte[,,] { { { 255, 0, 0 } }, { { 0, 255, 0 } } };
            Image<Bgr, Byte> img3 = new Image<Bgr, byte>(data);
        }

        [Test]
        public static void TestSubR()
        {
            Image<Bgr, Byte> img = new Image<Bgr, Byte>(101, 133);
            Assert.IsTrue(img.Not().Equals(255 - img));
        }

        [Test]
        public static void TestConvolutionAndLaplace()
        {
            Image<Gray, Byte> image = new Image<Gray, byte>(300, 400);
            image._RandUniform(new MCvScalar(0.0), new MCvScalar(255.0));

            Image<Gray, float> laplace = image.Laplace(1);

            float[,] k = {  {0, 1, 0},
                            {1, -4, 1},
                            {0, 1, 0}};
            ConvolutionKernelF kernel = new ConvolutionKernelF(k);

            Image<Gray, float> convoluted = image * kernel;
            Assert.IsTrue(laplace.Equals(convoluted));
        }

        [Test]
        public static void TestBitmapConstructor()
        {
            Image<Bgr, Byte> image1 = new Image<Bgr, byte>(201, 401);
            image1._RandUniform(new MCvScalar(), new MCvScalar(255.0, 255.0, 255.0));
            Bitmap bmp = image1.ToBitmap();

            Image<Bgr, Byte> image2 = new Image<Bgr, byte>(bmp);
            Assert.IsTrue(image1.Equals(image2));

            Image<Gray, Byte> image3 = new Image<Gray, byte>(11, 7);
            image3._RandUniform(new MCvScalar(), new MCvScalar(255.0, 255.0, 255.0));
            bmp = image3.ToBitmap();
            Image<Gray, Byte> image4 = new Image<Gray, byte>(bmp);
            Assert.IsTrue(image3.Equals(image4));
        }

        [Test]
        public static void TestSplitMerge()
        {
            Image<Bgr, Byte> img1 = new Image<Bgr, byte>(301, 234);
            img1._RandUniform(new MCvScalar(), new MCvScalar(255, 255, 255));
            Image<Gray, Byte>[] channels = img1.Split();

            Image<Bgr, Byte> img2 = new Image<Bgr, byte>(channels);
            Assert.IsTrue(img1.Equals(img2));
        }

        [Test]
        public static void TestCanny()
        {
            Image<Bgr, Byte> image = new Image<Bgr, byte>("stuff.jpg");
            
            //make sure canny works for multi channel image
            Image<Bgr, Byte> image2 = image.Canny(new Bgr(200, 200, 200), new Bgr(100, 100, 100));
         
        }

        [Test]
        public static void TestVideoWriter()
        {
            VideoWriter writer = new VideoWriter("tmp.avi", 2, 200, 100, true);
            Image<Bgr, Byte> img1 = new Image<Bgr, byte>(200, 100);
            Image<Bgr, Byte> img2 = new Image<Bgr, byte>(200, 100);
            writer.WriteFrame(img1);
            writer.WriteFrame(img2);
            writer.Dispose();
        }

    }
}
