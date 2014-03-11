using System;
using System.Windows.Forms;

namespace CosLab1
{
    public partial class Form1 : Form
    {
        private const string ImageLocation = "D:\\1.jpg";
        private ImageClastirization imageClastirization = new ImageClastirization(ImageLocation);

        public Form1()
        {
            InitializeComponent();
            try
            {
                pictureBox1.Image = imageClastirization.Image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Open error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            imageClastirization.ToGrayTone();
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            imageClastirization.Binarize();
            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                imageClastirization = new ImageClastirization(ImageLocation);
                pictureBox1.Image = imageClastirization.Image;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("D:\\Saved image.jpg");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            imageClastirization.Clastiraize();
            pictureBox1.Refresh();
        }
    }
}
