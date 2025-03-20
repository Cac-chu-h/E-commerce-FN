using BTL_FN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_User
{
    public partial class thanhToan : Form
    {
        public Product product = new Product();
        public thanhToan()
        {
            InitializeComponent();
        }

        private void thanhToan_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {

        }
    }
}
