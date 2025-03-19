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
    public partial class taiKhoan : Form
    {
        public taiKhoan()
        {
            InitializeComponent();
        }

        private void taiKhoan_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;


            for (int i = 0; i < 10; i++)
            {
                // Tạo panel chứa từng sản phẩm
                Panel panelSanPham = new Panel
                {
                    Size = new Size(740, 130), // Kích thước cho từng panel sản phẩm
                    BorderStyle = BorderStyle.FixedSingle // Viền cho panel
                };

                // Tính toán vị trí để các panel cách nhau 10px
                int marginTop = 10;
                int panelHeight = 130;
                panelSanPham.Location = new Point(5, marginTop + i * (panelHeight + marginTop));

                CheckBox check = new CheckBox
                {
                    AutoSize = true,
                    Location = new Point(8, 10),
                    Name = "checkBox" + i,
                    Text = ""
                };

                // Tạo hình ảnh sản phẩm
                PictureBox pictureBox = new PictureBox
                {
                    Location = new Point(30, 35),
                    Size = new Size(68, 68),
                    TabStop = false,
                    Image = Image.FromFile(@"E:\C#\logo.jpg"),
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                PictureBox pictureBoxStore = new PictureBox
                {
                    Location = new Point(30, 10),
                    Size = new Size(20, 20),
                    TabStop = false,
                    Image = Image.FromFile(@"E:\C#\logo.jpg"),
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                Label label14 = new Label
                {
                    AutoSize = true,
                    Location = new Point(50, 13),
                    Name = "storeName" + i,
                    Font = new Font("Arial", 8, FontStyle.Bold),
                    Text = "Cửa hàng A"
                };

                Label label13 = new Label
                {
                    AutoSize = true,
                    Location = new Point(110, 35),
                    Name = "label13_" + i,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    Text = "Honor 200 5G 12GB 256GB"
                };

                // Panel nhỏ chứa bộ điều chỉnh số lượng
                Panel panelQuantity = new Panel
                {
                    Location = new Point(500, 90), // Điều chỉnh vị trí phù hợp
                    Size = new Size(240, 25), // Kích thước phù hợp
                    BorderStyle = BorderStyle.None // Không viền
                };

                // Button hủy đơn hàng
                Button bt1 = new Button
                {
                    Size = new Size(100, 25),
                    ForeColor = Color.White,
                    BackColor = Color.OrangeRed,
                    Font = new Font("Arial", 8, FontStyle.Regular),
                    Text = "Hủy đơn hàng",
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat
                };

                // Button liên hệ shop
                Button bt2 = new Button
                {
                    Size = new Size(100, 25),
                    Font = new Font("Arial", 8, FontStyle.Regular),
                    Text = "Liên hệ shop",
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat
                };

                // Thêm các thành phần vào panel nhỏ
                panelQuantity.Controls.Add(bt1);
                panelQuantity.Controls.Add(bt2);

                // Căn chỉnh vị trí bên trong panelQuantity
                bt1.Location = new Point(0, 0);
                bt2.Location = new Point(110, 0);

                // Thêm panel nhỏ vào panel chính
                panelSanPham.Controls.Add(panelQuantity);

                // Tạo ComboBox
                ComboBox comboBox1 = new ComboBox
                {
                    Location = new Point(115, 59),
                    Size = new Size(80, 20),
                };

                comboBox1.Items.Add("Sản phẩm 1");
                comboBox1.Items.Add("Sản phẩm 2");
                comboBox1.SelectedIndex = 0;

                Label label15 = new Label
                {
                    AutoSize = true,
                    Location = new Point(110, 103),
                    Name = "label15_" + i,
                    Font = new Font("Arial", 8, FontStyle.Italic),
                    ForeColor = Color.OrangeRed,
                    Text = "Tiết kiệm: 500.000đ"
                };

                Label label17 = new Label
                {
                    AutoSize = true,
                    Location = new Point(110, 86),
                    Name = "label17_" + i,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Text = "Giá Bán: 11.490.000đ"
                };

                // Thêm các thành phần vào panel sản phẩm
                panelSanPham.Controls.Add(pictureBox);
                panelSanPham.Controls.Add(pictureBoxStore);
                panelSanPham.Controls.Add(check);
                panelSanPham.Controls.Add(label13);
                panelSanPham.Controls.Add(label14);
                panelSanPham.Controls.Add(comboBox1);
                panelSanPham.Controls.Add(label15);
                panelSanPham.Controls.Add(label17);

                // Thêm panel sản phẩm vào panel cha (panel2)
                this.panel2.Controls.Add(panelSanPham);
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

        }
    }
}
