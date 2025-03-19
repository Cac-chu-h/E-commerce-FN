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
    public partial class danhMuc : Form
    {
        public danhMuc()
        {
            InitializeComponent();
        }

        private void danhMuc_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                // Tạo panel chứa từng sản phẩm
                Panel panelSanPham = new Panel
                {
                    Size = new Size(560, 130), // Kích thước cho từng panel sản phẩm
                    Location = new Point(5, (i * 140)), // Khoảng cách giữa các panel
                    BorderStyle = BorderStyle.FixedSingle // Viền cho panel
                };
                panelSanPham.Margin = new Padding(10, 10, 10, 10);

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

                // Tạo các nhãn
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
                    Location = new Point(450, 90), // Điều chỉnh vị trí phù hợp
                    Size = new Size(90, 25), // Kích thước phù hợp
                    BorderStyle = BorderStyle.None // Không viền (hoặc bạn có thể để FixedSingle)
                };


                // Button giảm số lượng
                Button bt1 = new Button
                {
                    Size = new Size(25, 25), // Kích thước đủ để hiển thị rõ dấu "-"
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Text = "-",
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat // Làm cho nút gọn gàng hơn
                };

                // Label hiển thị số lượng
                Label label16 = new Label
                {
                    AutoSize = false,
                    Size = new Size(30, 25), // Kích thước vừa vặn giữa 2 nút
                    TextAlign = ContentAlignment.MiddleCenter, // Căn giữa
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    Text = "1"
                };

                // Button tăng số lượng
                Button bt2 = new Button
                {
                    Size = new Size(25, 25),
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Text = "+",
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat
                };

                // Thêm các thành phần vào panel nhỏ
                panelQuantity.Controls.Add(bt1);
                panelQuantity.Controls.Add(label16);
                panelQuantity.Controls.Add(bt2);

                // Căn chỉnh vị trí bên trong panelQuantity
                bt1.Location = new Point(0, 0);
                label16.Location = new Point(29, 0);
                bt2.Location = new Point(60, 0);

                // Thêm panel nhỏ vào panel chính
                panelSanPham.Controls.Add(panelQuantity);


                // Tạo ComboBox
                ComboBox comboBox1 = new ComboBox
                {
                    Location = new Point(115, 59), // Vị trí trên form
                    Size = new Size(80, 20), // Kích thước
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


                // Thêm panel sản phẩm vào panel3
                this.panel2.Controls.Add(panelSanPham);
            }


            for (int i = 0; i < 10; i++)
            {
                // Tạo panel chứa từng sản phẩm
                Panel panelSanPham = new Panel
                {
                    Size = new Size(560, 130), // Kích thước cho từng panel sản phẩm
                    BorderStyle = BorderStyle.FixedSingle, // Viền cho panel
                    Margin = new Padding(10, 10, 10, 10) // Khoảng cách giữa các panel
                };

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
                    Location = new Point(450, 90),
                    Size = new Size(90, 25),
                    BorderStyle = BorderStyle.None
                };

                Button bt1 = new Button
                {
                    Size = new Size(25, 25),
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Text = "-",
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat
                };

                Label label16 = new Label
                {
                    AutoSize = false,
                    Size = new Size(30, 25),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Arial", 10, FontStyle.Regular),
                    Text = "1"
                };

                Button bt2 = new Button
                {
                    Size = new Size(25, 25),
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Text = "+",
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat
                };

                panelQuantity.Controls.Add(bt1);
                panelQuantity.Controls.Add(label16);
                panelQuantity.Controls.Add(bt2);

                bt1.Location = new Point(0, 0);
                label16.Location = new Point(29, 0);
                bt2.Location = new Point(60, 0);

                panelSanPham.Controls.Add(panelQuantity);

                ComboBox comboBox1 = new ComboBox
                {
                    Location = new Point(115, 59),
                    Size = new Size(80, 20)
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

                panelSanPham.Controls.Add(pictureBox);
                panelSanPham.Controls.Add(pictureBoxStore);
                panelSanPham.Controls.Add(check);
                panelSanPham.Controls.Add(label13);
                panelSanPham.Controls.Add(label14);
                panelSanPham.Controls.Add(comboBox1);
                panelSanPham.Controls.Add(label15);
                panelSanPham.Controls.Add(label17);

                // Thêm panel sản phẩm vào FlowLayoutPanel
                this.flowLayoutPanel1.Controls.Add(panelSanPham);
            }
        }
    }
}
