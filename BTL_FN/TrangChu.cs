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
                if (!string.IsNullOrEmpty(product.Image) && System.IO.File.Exists(product.Image))
                {
                    productImage.Image = Image.FromFile(product.Image);
                }
                else
                {
                    // Set placeholder image
                    productImage.Image = Image.FromFile(logic.logo);
                }
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
                TextAlign = ContentAlignment.MiddleCenter, // Căn giữa cả ngang và dọc
            };
            // Product price label
            Label priceLabel = new Label
            {
                Text = "Giá bán: " + string.Format("{0:N0} đ", product.Price),
                Font = new Font("Arial", 9, FontStyle.Regular),
                ForeColor = Color.Red,
                Location = new Point(20, 230),
                AutoSize = false,
                Width = 100,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Label oldpriceLabel = new Label
            {
                Text = string.Format("{0:N0} đ", product.Price),
                Font = new Font("Arial", 7, FontStyle.Regular | FontStyle.Strikeout),
                ForeColor = Color.Black,
                Location = new Point(120, 230),
                AutoSize = false,
                Width = 80,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label ratting = new Label
            {
                Text = $"Đánh giá {product.Rating}/5",
                Font = new Font("Arial", 9, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(20, 210),
                AutoSize = false,
                Width = 80,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Label total = new Label
            {
                Text = $"Đã bán {product.Total}",
                Font = new Font("Arial", 9, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(100, 210),
                AutoSize = false,
                Width = 80,
                Height = 20,
                TextAlign = ContentAlignment.MiddleLeft
            };
            // Product rating display


            // Add to cart button
            Button addToCartButton = new Button
            {
                Text = "Thêm vào giỏ",
                Location = new Point(20, 270),
                Width = 170,
                Height = 25,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            Button Buy = new Button
            {
                Text = "Mua ngay",
                Location = new Point(20, 300),
                Width = 170,
                Height = 25,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            // Set button event
            addToCartButton.Tag = product.Id;
            addToCartButton.Click += (sender, e) =>
            {
                Button btn = sender as Button;
                int productId = (int)btn.Tag;
                AddToCart(productId);
            };

            // Add all controls to product card
            productCard.Controls.Add(productImage);
            productCard.Controls.Add(nameLabel);
            productCard.Controls.Add(priceLabel);
            productCard.Controls.Add(oldpriceLabel);
            productCard.Controls.Add(addToCartButton);
            productCard.Controls.Add(total);
            productCard.Controls.Add(ratting);
            productCard.Controls.Add(Buy);

            // Add click event to the card for product details
            productCard.Tag = product.Id;
            productCard.Click += (sender, e) =>
            {
                if (e.GetType() != typeof(MouseEventArgs))
                    return;

                Panel panel = sender as Panel;
                int productId = (int)panel.Tag;
                ShowProductDetails(productId);
            };

            return productCard;
        }

        // Method to handle adding product to cart
        private void AddToCart(int productId)
        {
            // Find product by id
            Product productToAdd = null;
            foreach (Product p in listProduct)
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

        // Method to show product details
        private void ShowProductDetails(int productId)
        {
            
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
