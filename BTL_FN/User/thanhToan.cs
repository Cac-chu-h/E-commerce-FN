using BTL_FN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_User
{
    public partial class thanhToan : Form
    {
        public Product product = new Product();


        public List<>

        public int state = 0;
        public thanhToan(Product p, int state)
        {
            this.state = state;
            this.product = p;
            InitializeComponent();
        }

        private void thanhToan_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;


            LoadProduct();
        }


        private void LoadProduct()
        {
            try
            {
                pictureBox1.Image = File.Exists(product.Image) ? Image.FromFile(product.Image) : Image.FromFile(@"E:\C#\logo.jpg");
            }
            catch
            {
                pictureBox1.Image = Image.FromFile(@"E:\C#\logo.jpg");
            }



            label1.Text = product.Name;
            label4.Text = "#" + product.Id;
            label3.Text = $"(Nhỏ hơn: {product.Total}KG)";

            label9.Text = $"Giá bán: {product.Price}đ/kg";


            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            if(state == 0)
            {
                button2.Visible = false;
            }
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }
        // thêm đia chỉ mới
        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        // đặt hàng
        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {

        }
        // xuất crytal report

        private void button2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
