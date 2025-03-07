using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_FN
{
    public partial class admin : Form
    {
        public admin()
        {
            InitializeComponent();
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }


        private void admin_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Danh mục quản lý");
            comboBox1.SelectedIndex = 0;
            pictureBox2.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Quản lý sản phẩm");
            comboBox1.Items.Add("Quản lý danh mục");
            comboBox1.Items.Add("Quản lý voucher");
            comboBox1.SelectedIndex = 1;
            Product_v p = new Product_v(); // Khởi tạo form
            p.TopLevel = false;           // Không cho phép form là cửa sổ chính
            p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
            p.Dock = DockStyle.Fill;      // Giãn đầy panel
            panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
            panel2.Controls.Add(p);       // Thêm form con vào panel
            p.Show();                    // Hiển thị form
        }

        private void button2_Click(object sender, EventArgs e)
        {
            account a = new account();
            a.TopLevel = false;
            a.FormBorderStyle = FormBorderStyle.None;
            a.Dock = DockStyle.Fill;
            panel2.Controls.Clear();
            panel2.Controls.Add(a);
            a.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex != 0)
            {
                if(comboBox1.SelectedIndex == 1)
                {
                    Product_v p = new Product_v(); // Khởi tạo form
                    p.TopLevel = false;           // Không cho phép form là cửa sổ chính
                    p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                    p.Dock = DockStyle.Fill;      // Giãn đầy panel
                    panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                    panel2.Controls.Add(p);       // Thêm form con vào panel
                    p.Show();
                }
                else if(comboBox1.SelectedIndex == 2)
                {
                    category c = new category();
                    c.TopLevel = false;           // Không cho phép form là cửa sổ chính
                    c.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                    c.Dock = DockStyle.Fill;      // Giãn đầy panel
                    panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                    panel2.Controls.Add(c);       // Thêm form con vào panel
                    c.Show();
                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    voucher v = new voucher();
                    v.TopLevel = false;           // Không cho phép form là cửa sổ chính
                    v.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                    v.Dock = DockStyle.Fill;      // Giãn đầy panel
                    panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                    panel2.Controls.Add(v);       // Thêm form con vào panel
                    v.Show();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}
