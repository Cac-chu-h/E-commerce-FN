using BTL_FN;
using BTL_User;
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
    public partial class TrangChu : Form
    {

        BLL logic = new BLL();
        List<Category> categories = new List<Category>();
        List<List<Product>> products = new List<List<Product>>();
        List<Product> listProduct = new List<Product>();
        bool isCategory = true;
        public TrangChu()
        {
            InitializeComponent();
        }
        // tài khoản 
        private void button8_Click(object sender, EventArgs e)
        {

        }
        // thông báo
        private void button7_Click(object sender, EventArgs e)
        {

        }
        // giỏ hàng
        private void button6_Click(object sender, EventArgs e)
        {

        }
        // danh mục sản phẩm
        private void button5_Click(object sender, EventArgs e)
        {

        }
        // quay về trang chủ
        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void TrangChu_Load(object sender, EventArgs e)
        {
            loadData();
            LoadHomeData();
        }
        // tìm kiểm
        private void button3_Click(object sender, EventArgs e)
        {

        }

        // load data 
        public void loadData()
        {
            //data nền tảng 
            pictureBox2.Image = Image.FromFile(logic.logo);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            // dữ liệu hiện thị 
            categories = logic.ListCategory();

            Dictionary<int, object> p = logic.ListProducts();

            List<Product> ps = p[1] as List<Product>;
            listProduct = ps;
            if (categories != null && categories.Count > 0)
            {
                foreach (Category c in categories)
                {
                    List<Product> pfc = new List<Product>();
                    foreach(Product value in ps)
                    {
                        if(c.Id == value.CategoryId)
                        {
                            pfc.Add(value);
                        }
                    }
                    products.Add(pfc);
                }
            }
            
            

        }

        public void LoadHomeData()
        {
            if (products != null && products.Count > 0)
            {
                DisplayData();
            }
            else
            {
                if(listProduct != null && listProduct.Count > 0)
                {
                    isCategory = false;
                }
                else
                {
                    DisplayNoDataMessage();
                }
            }
        }
        // dislaydata

        public void DisplayData()
        {
            // Clear existing panels
            flowLayoutPanel4.Controls.Clear();

            // === RECOMMENDED PRODUCTS SECTION ===
            // Create header panel for recommended products
            Panel recommendedHeaderPanel = new Panel
            {
                Width = flowLayoutPanel4.Width,
                Height = 30,
                Margin = new Padding(2, 2, 2, 2)
            };

            Label recommendedLabel = new Label
            {
                Text = "Sản Phẩm Gợi Ý",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            recommendedHeaderPanel.Controls.Add(recommendedLabel);
            flowLayoutPanel4.Controls.Add(recommendedHeaderPanel);

            // Create FlowLayoutPanel for recommended products
            FlowLayoutPanel recommendedProductsPanel = new FlowLayoutPanel
            {
                Width = flowLayoutPanel4.Width,
                Height = 480,
                Margin = new Padding(10, 0, 10, 20),
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            flowLayoutPanel4.Controls.Add(recommendedProductsPanel);

            // Generate recommended products list with unique random items
            List<Product> rcm = new List<Product>();
            Random random = new Random();

            for (int i = 0; i < 10 && listProduct.Count > 0; i++)
            {
                int attempts = 0;
                bool added = false;

                while (!added && attempts < 20) // Limit attempts to prevent infinite loop
                {
                    int index = random.Next(0, listProduct.Count);
                    if (!rcm.Contains(listProduct[index]))
                    {
                        rcm.Add(listProduct[index]);
                        added = true;
                    }
                    attempts++;
                }

                if (!added) break; // Break if we couldn't add a unique product after multiple attempts
            }

            // Display recommended products
            foreach (Product product in rcm)
            {
                recommendedProductsPanel.Controls.Add(CreateProductCard(product));
            }

            // === CATEGORY PRODUCTS SECTION ===
            if (isCategory && products != null && products.Count > 0)
            {
                for (int i = 0; i < products.Count; i++)
                {
                    List<Product> categoryProducts = products[i];
                    if (categoryProducts.Count == 0)
                        continue;

                    // Find category name
                    string categoryName = "Danh Mục";
                    foreach (Category c in categories)
                    {
                        if (c.Id == categoryProducts[0].CategoryId)
                        {
                            categoryName = c.Name;
                            break;
                        }
                    }

                    // Create header panel for category
                    Panel categoryHeaderPanel = new Panel
                    {
                        Width = flowLayoutPanel4.Width - 20,
                        Height = 40,
                        Margin = new Padding(10, 10, 10, 5)
                    };

                    Label categoryLabel = new Label
                    {
                        Text = categoryName,
                        Font = new Font("Arial", 14, FontStyle.Bold),
                        Location = new Point(10, 10),
                        AutoSize = true
                    };

                    categoryHeaderPanel.Controls.Add(categoryLabel);
                    flowLayoutPanel4.Controls.Add(categoryHeaderPanel);

                    // Create FlowLayoutPanel for category products
                    FlowLayoutPanel categoryProductsPanel = new FlowLayoutPanel
                    {
                        Width = flowLayoutPanel4.Width - 20,
                        Height = 280,
                        Margin = new Padding(10, 0, 10, 20),
                        AutoScroll = true,
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    flowLayoutPanel4.Controls.Add(categoryProductsPanel);

                    // Display category products
                    foreach (Product product in categoryProducts)
                    {
                        categoryProductsPanel.Controls.Add(CreateProductCard(product));
                    }
                }
            }
            else if (!isCategory && listProduct != null && listProduct.Count > 0)
            {
                // Create header panel for all products
                Panel allProductsHeaderPanel = new Panel
                {
                    Width = flowLayoutPanel4.Width - 20,
                    Height = 40,
                    Margin = new Padding(10, 10, 10, 5)
                };

                Label allProductsLabel = new Label
                {
                    Text = "Tất Cả Sản Phẩm",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    Location = new Point(10, 10),
                    AutoSize = true
                };

                allProductsHeaderPanel.Controls.Add(allProductsLabel);
                flowLayoutPanel4.Controls.Add(allProductsHeaderPanel);

                // Create FlowLayoutPanel for all products
                FlowLayoutPanel allProductsPanel = new FlowLayoutPanel
                {
                    Width = flowLayoutPanel4.Width - 20,
                    Height = 500,
                    Margin = new Padding(10, 0, 10, 20),
                    AutoScroll = true,
                    BorderStyle = BorderStyle.FixedSingle
                };
                flowLayoutPanel4.Controls.Add(allProductsPanel);

                // Display all products
                foreach (Product product in listProduct)
                {
                    allProductsPanel.Controls.Add(CreateProductCard(product));
                }
            }
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
                buyNow(product);
            };
            // Event handlers
            addToCartButton.Click += (sender, e) =>
            {
                Button btn = sender as Button;
                int productId = (int)btn.Tag;
                AddToCart(product);
            };
            productCard.Click += (sender, e) =>
            {
                ShowProductDetails(product);
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

        // Method to handle adding product to cart
        private void AddToCart(Product productId)
        {
            thanhToan dh = new thanhToan(productId, 0);
            dh.ShowDialog();
        }

        // Method to handle adding product to cart
        private void buyNow(Product productId)
        {
            thanhToan dh = new thanhToan(productId, 1);
            dh.ShowDialog();
        }
        // Method to show product details
        private void ShowProductDetails(Product pa)
        {
            Product_u p = new Product_u(pa);
            p.ShowDialog();
        }

        private void DisplayNoDataMessage()
        {
            flowLayoutPanel4.Controls.Clear();
            Label labelNoData = new Label
            {
                AutoSize = true,
                Location = new Point(300, 200),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Text = "Không tồn tại danh mục"
            };
            flowLayoutPanel4.Controls.Add(labelNoData);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
