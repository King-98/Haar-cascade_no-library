using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PyramidNetwork
{
    public partial class Form1 : Form
    {
        Image image;
        Bitmap totalpicture;

        public Form1()
        {
            InitializeComponent();
        }

        private void pyramidNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            faceDetection run = new faceDetection();
            imageProcessing ip = new imageProcessing();

            // Detection
            pictureBox2.Image = new Bitmap(run.BB_4(totalpicture), pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Update();

            // Interpolation
            int[,] ROI = ip.neighbor(run.ROI_2(totalpicture));
            pictureBox3.Image = new Bitmap(run.Convert(ROI), pictureBox3.Width, pictureBox3.Height);
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = "All Files(*.*)|*.*|Bitmap File(*.bmp)|*.bmp|";
            name = name + "Gif File(*.gif)|*.gif|jpeg File(*.jpg)|*.jpg";
            openFileDialog1.Title = "타이틀";

            openFileDialog1.Filter = name;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string strName = openFileDialog1.FileName;
                image = Image.FromFile(strName);
                totalpicture = new Bitmap(image);
                // totalpicture = new Bitmap(image, 200, 260);
            }

            pictureBox1.Image = new Bitmap(totalpicture, pictureBox1.Width, pictureBox1.Height);

            this.Invalidate();
        }
    }
}
