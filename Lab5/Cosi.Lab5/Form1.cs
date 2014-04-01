namespace Cosi.Lab5
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Cosi.Lab5.Core;
    using Cosi.Lab5.Properties;
    using ImageProcessingHelper;

    public partial class Form1 : Form
    {
        private CompetitiveNeuronNetwork neuronNetwork = new CompetitiveNeuronNetwork(100, 1);
        private PictureContainer noisedPicture;
        private int noiseLevel = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            neuronNetwork.Teach(
                new PictureContainer(Resources.A1, 10).ConvertToVector(),
                new PictureContainer(Resources.A2, 10).ConvertToVector(),
                new PictureContainer(Resources.A3, 10).ConvertToVector(),
                new PictureContainer(Resources.B1, 10).ConvertToVector(),
                new PictureContainer(Resources.B2, 10).ConvertToVector(),
                new PictureContainer(Resources.B3, 10).ConvertToVector(),
                new PictureContainer(Resources.C1, 10).ConvertToVector(),
                new PictureContainer(Resources.C2, 10).ConvertToVector(),
                new PictureContainer(Resources.C3, 10).ConvertToVector()
                );

            textBox1.Text += @"Teaching ready";
            textBox1.Text += Environment.NewLine;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += @"Answer is:";

            textBox1.Text +=
            neuronNetwork.DetermineClaster(noisedPicture.ConvertToVector());
            textBox1.Text += Environment.NewLine;
            ScrollTextBox();

            textBox1.Refresh();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap picture;

            if (radioButtonA.Checked)
            {
                picture = new Bitmap(Resources.A1);
            }
            else
            {
                picture = radioButtonB.Checked ? new Bitmap(Resources.B1) : new Bitmap(Resources.C1);
            }

            noisedPicture = NoiseGenerator.Generate(new PictureContainer(
                picture, 10),
                trackBar1.Value * 10);
            pictureBox4.Image = noisedPicture.Picture;
            pictureBox4.Refresh();
        }

        private void ScrollTextBox()
        {
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
            textBox1.Refresh();
        }
    }
}
