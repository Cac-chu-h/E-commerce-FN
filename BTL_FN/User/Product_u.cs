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

namespace BTL_FN
{
    public partial class Product_u : Form
    {
        public BLL logic = new BLL();
        public Product p = new Product();

        public List<Product> sameProduct = new List<Product>();
        public List<Reprots> reprots = new List<Reprots>();

        Programs pss = null;
        public Product_u(Product p, Programs ss)
        {
            this.pss = ss;
            this.p = p;
            InitializeComponent();
            this.MdiParent = pss;
        }

        private void Product_u_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;


            pictureBox4.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
            LoadProduct();
            LoadPoductData();
        }

        private void LoadProduct()
        {
            Dictionary<int, object> d =  logic.ListProducts($"SELECT * FROM sp WHERE dm = {p.CategoryId} AND trangThaiXoa!= N'Đã xóa'");
            List<Product> ls = d[1] as List<Product>;

            sameProduct = ls;

            logic.getAllReport(ref reprots, $"SELECT * FROM phanHoi WHERE idSanPham = {p.Id} AND trangThaiXoa!= N'Đã xóa'");

        }


        private void LoadPoductData()
        {
            try
            {
                pictureBox1.Image = !string.IsNullOrEmpty(p.Image) && File.Exists(p.Image)
                    ? Image.FromFile(p.Image)
                    : Image.FromFile(logic.logo);
            }
            catch
            {
                pictureBox1.Image = Image.FromFile(logic.logo);
            }
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;


            label1.Text = p.Name;
            label4.Text = "#" + p.Id;

            label5.Text = p.Rating + "/5 Đánh giá";

            label6.Text = reprots.Count + " Phản hổi";

            label3.Text = p.Total + " KG";

            label9.Text = $"Giá bán: {p.Price}Đ";

            label11.Text = p.Description;

            label17.Text = $"Sản phẩm gợi ý ({sameProduct.Count})";
            if (reprots.Count == 0)
            {
                panel2.Controls.Clear();
                Label labelNoData = new Label
                {
                    AutoSize = true,
                    Location = new Point(300, 200),
                    Font = new Font("Arial", 16, FontStyle.Bold),
                    Text = "Không tồn tại danh mục"
                };
                panel2.Controls.Add(labelNoData);
            }
            else
            {
                panel2.Controls.Clear();
                panel2.AutoScroll = true;

                FlowLayoutPanel reportFlow = new FlowLayoutPanel
                {
                    Width = panel2.Width - 20,
                    AutoSize = true,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false
                };

                foreach (var report in reprots)
                {
                    reportFlow.Controls.Add(CreateReportPanel(report));
                }

                panel2.Controls.Add(reportFlow);
            }


            DisplayData();
        }

        public Panel CreateReportPanel(Reprots report)
        {
            Panel panelReport = new Panel
            {
                Size = new Size(530, 80),
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle,
                Tag = report.Id
            };

            // Thông tin chính
            Label lblTitle = new Label
            {
                Text = $"[{report.ChuDe}] {report.NoiDung}",
                Location = new Point(30, 5),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Size = new Size(400, 25)
            };

            // Chi tiết
            Label lblDetails = new Label
            {
                Text = $"User: {report.IdNguoiDung} | SP: {report.IdSanPham} | Loại: {report.LoaiPhanHoi}",
                Location = new Point(30, 39),
                Font = new Font("Arial", 8),
                ForeColor = Color.Gray,
                AutoSize = true
            };


            // Thời gian
            Label lblTime = new Label
            {
                Text = report.NgayDang.ToString("HH:mm dd/MM"),
                Location = new Point(400, 5),
                Font = new Font("Arial", 8),
                ForeColor = Color.DimGray
            };


            panelReport.Controls.AddRange(new Control[] {
            lblTitle, lblDetails, lblTime
            });

            return panelReport;
        }

        public void DisplayData()
        {
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.Size = new Size(563, 299);
            flowLayoutPanel1.AutoScroll = true;

            // Hiển thị sản phẩm gợi ý
            if (sameProduct?.Count > 0)
            {
                var recommendationPanel = CreateSectionPanel("Sản Phẩm Gợi Ý");
                var recommendedFlow = new FlowLayoutPanel
                {
                    Size = new Size(540, 130),
                    AutoScroll = true,
                    WrapContents = true
                };

                foreach (var product in sameProduct.Take(4))
                {
                    recommendedFlow.Controls.Add(CreateProductCard(product));
                }

                recommendationPanel.Controls.Add(recommendedFlow);
                flowLayoutPanel1.Controls.Add(recommendationPanel);
            }

            // Hiển thị danh mục (nếu có)

        }

        private Panel CreateProductCard(Product product)
        {
            // Main product card panel
            Panel productCard = new Panel
            {
                Width = 210,
                Height = 350,
                Margin = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Product image
            PictureBox productImage = new PictureBox
            {
                Width = 200,
                Height = 140,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(5, 5)
            };

            // Load image from file or set placeholder
            try
            {
                productImage.Image = !string.IsNullOrEmpty(product.Image) && File.Exists(product.Image)
                    ? Image.FromFile(product.Image)
                    : Image.FromFile(logic.logo);
            }
            catch
            {
                productImage.Image = Image.FromFile(logic.logo);
            }

            // Product name label
            Label nameLabel = new Label
            {
                Text = product.Name,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(5, 170),
                Width = 200,
                Height = 35,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Weight label
            Label weightLabel = new Label
            {
                Text = $"Khối lượng: {product.Total}kg",
                Font = new Font("Arial", 9),
                ForeColor = Color.Black,
                Location = new Point(5, 210),
                Width = 200,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Price per kg label
            Label pricePerKgLabel = new Label
            {
                Text = $"Giá 1kg: {product.Price:N0} đ",
                Font = new Font("Arial", 9),
                ForeColor = Color.Red,
                Location = new Point(5, 230),
                Width = 200,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Rating label
            Label ratingLabel = new Label
            {
                Text = $"Đánh giá {product.Rating}/5",
                Font = new Font("Arial", 9),
                Location = new Point(20, 250),
                Width = 80,
                Height = 20
            };

            // Total sold label
            Label totalSoldLabel = new Label
            {
                Text = $"Đã bán {product.TotalPay}",
                Font = new Font("Arial", 9),
                Location = new Point(100, 250),
                Width = 80,
                Height = 20
            };

            // Add to cart button
            Button addToCartButton = new Button
            {
                Text = "Thêm vào giỏ",
                Location = new Point(20, 275),
                Width = 170,
                Height = 25,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Tag = product.Id
            };

            // Buy button
            Button buyButton = new Button
            {
                Text = "Mua ngay",
                Location = new Point(20, 305),
                Width = 170,
                Height = 25,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            buyButton.Click += (sender, e) =>
            {
                Button btn = sender as Button;
                int productId = (int)btn.Tag;
                buyNow(productId);
            };
            // Event handlers
            addToCartButton.Click += (sender, e) =>
            {
                Button btn = sender as Button;
                int productId = (int)btn.Tag;
                AddToCart(productId);
            };
            productCard.Click += (sender, e) =>
            {
                this.p = product;
                panel1.Refresh();
            };

            // Add controls to panel
            productCard.Controls.AddRange(new Control[] {
            productImage,
            nameLabel,
            weightLabel,
            pricePerKgLabel,
            ratingLabel,
            totalSoldLabel,
            addToCartButton,
            buyButton
        });

            return productCard;
        }


        private void AddToCart(int productId)
        {
            // Find product by id
            Product productToAdd = null;
            foreach (Product p in sameProduct)
            {
                if (p.Id == productId)
                {
                    productToAdd = p;
                    break;
                }
            }

            if (productToAdd != null)
            {
                // TODO: Implement your cart logic here
                MessageBox.Show($"Đã thêm sản phẩm {productToAdd.Name} vào giỏ hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void buyNow(int productId)
        {
            // Find product by id
            
        }

        private Image LoadProductImage(string path)
        {
            try
            {
                return File.Exists(path) ? Image.FromFile(path) : Image.FromFile(@"E:\C#\logo.jpg");
            }
            catch
            {
                return Image.FromFile(@"E:\C#\logo.jpg");
            }
        }

        private Panel CreateSectionPanel(string title)
        {
            Panel panel = new Panel
            {
                Size = new Size(560, 160),
                Margin = new Padding(0, 0, 0, 10)
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            panel.Controls.Add(titleLabel);
            return panel;
        }
    

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
