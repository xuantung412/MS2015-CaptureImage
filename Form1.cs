using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing.Drawing2D;

namespace testabc
{
    public partial class Form1 : Form
    {

        VideoCapture capture;

        public Form1()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += pictureBox1_MouseWhell;
        }

        private void pictureBox1_MouseWhell(object sender, MouseEventArgs e)
        {
            if( e.Delta > 0)
            {
                this.Width -= 50;
                this.Height -= 50;
                pictureBox1.Width -= 50;
                pictureBox1.Height -= 50;
            }
            else
            {
                this.Width += 50;
                this.Height += 50;
                pictureBox1.Width += 50;
                pictureBox1.Height += 50;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                capture = new VideoCapture(ofd.FileName);
                Mat m = new Mat();
                capture.Read(m);
                pictureBox1.Image = m.Bitmap;
            }
            //this.pauseToolStripMenuItem.Click
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            if (capture == null)
            {
                return;
            }

            try
            {
                Mat m = new Mat();
                capture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, 501);
                capture.Read(m);
                //pictureBox1.Image = m.Bitmap;

                //Clone PictureBox
                System.Windows.Forms.PictureBox a = new System.Windows.Forms.PictureBox();
                a.Image = m.Bitmap;
                a.Width = 500;
                a.Height = 500;
                a.SizeMode = PictureBoxSizeMode.AutoSize;

                //Crop image
                int rectW = 150;
                int rectH = 150;

                Bitmap bmp2 = new Bitmap(a.Width, a.Height);
                a.DrawToBitmap(bmp2, a.ClientRectangle);

                Bitmap crpImg = new Bitmap(rectW, rectH);

                for (int i = 0; i < rectW; i++)
                {
                    for (int y = 0; y < rectH; y++)
                    {
                        Color pxlclr = bmp2.GetPixel(1300 + i, 750 + y);
                        crpImg.SetPixel(i, y, pxlclr);
                    }
                }

                a.SizeMode = PictureBoxSizeMode.Zoom;
                a.Image = (Image)crpImg;

                this.pictureBox1.Image = a.Image;
                //pictureBox2.Image = m.Bitmap;

                //pictureBox1.Image = m.Bitmap;
                /*
                while (true)
                {
                    Mat m = new Mat();
                    capture.Read(m);

                    if (!m.IsEmpty)
                    {
                        pictureBox1.Image = m.Bitmap;
                        DetectText(m.ToImage<Bgr, byte>());
                        double fps = capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                        await Task.Delay(1000 / Convert.ToInt32(fps));
                    }
                    else
                    {
                        break;
                    }
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DetectText(Image<Bgr, byte> img)
        {
            /*
             1. Edge detection (sobel)
             2. Dilation (10,1)
             3. FindContours
             4. Geometrical Constrints
             */
            //sobel
            Image<Gray, byte> sobel = img.Convert<Gray, byte>().Sobel(1, 0, 3).AbsDiff(new Gray(0.0)).Convert<Gray, byte>().ThresholdBinary(new Gray(50), new Gray(255));
            Mat SE = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(10, 2), new Point(-1, -1));
            sobel = sobel.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Dilate, SE, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(255));
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat m = new Mat();

            CvInvoke.FindContours(sobel, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

            List<Rectangle> list = new List<Rectangle>();

            for (int i = 0; i < contours.Size; i++)
            {
                Rectangle brect = CvInvoke.BoundingRectangle(contours[i]);

                double ar = brect.Width / brect.Height;
                if (ar > 2 && brect.Width > 25 && brect.Height > 8 && brect.Height < 100)
                {
                    list.Add(brect);
                }
            }


            Image<Bgr, byte> imgout = img.CopyBlank();
            foreach (var r in list)
            {
                CvInvoke.Rectangle(img, r, new MCvScalar(0, 0, 255), 2);
                CvInvoke.Rectangle(imgout, r, new MCvScalar(0, 255, 255), -1);
            }
            imgout._And(img);

            //pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            Bitmap a = img.Bitmap;
            pictureBox1.Image = img.Bitmap;
            //pictureBox1.Image = imgout.Bitmap;
            
           

        }

        private void pauseToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
    }
