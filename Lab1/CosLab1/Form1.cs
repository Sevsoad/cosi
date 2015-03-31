using System;
using System.Drawing;
using System.Windows.Forms;

namespace CosLab1
{
    public partial class Form1 : Form
    {
        private const string ImageLocation = "D:\\1.jpg";        
        private ImageFiltrator filtrator;
        private ImageClastirization imageClastirizator;
        private Bitmap processedImage;

        public Form1()
        {
            InitializeComponent();
            try
            {                
                processedImage = new Bitmap(ImageLocation);
                filtrator = new ImageFiltrator(processedImage);
                pictureBox1.Image = processedImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Open error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            filtrator.ToGrayTone();
            pictureBox1.Image = filtrator.ProcessedImage;
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            filtrator.Binarize();
            pictureBox1.Image = filtrator.ProcessedImage;
            pictureBox1.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                processedImage = new Bitmap(ImageLocation);
                pictureBox1.Image = processedImage;
                pictureBox1.Refresh();
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
            imageClastirizator = new ImageClastirization(filtrator.ProcessedImage);
            imageClastirizator.Clastiraize();
            pictureBox1.Image = imageClastirizator.ProcessedImage;
            pictureBox1.Refresh();
        }
    }
}
