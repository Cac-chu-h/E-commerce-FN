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
        List<Product> listProduct = new List<Product>();

        DataTable dt = new DataTable();
        Programs pss = null;

        public TrangChu(Programs pss)
        {
            this.pss = pss;
            InitializeComponent();
        }


        private void TrangChu_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadProdcutData();
            this.MaximizeBox = false;
            this.MinimizeBox = false; // Hoặc false nếu muốn tắt luôn nút thu nhỏ
            this.ControlBox = false;

            pss.Width = this.Width + 30;
            pss.Height = this.Height + 20;
        }

        private void Product_v_Load(object sender, EventArgs e)
        {
            logic = new BLL();
            pictureBox2.Image = Image.FromFile(logic.logo);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            categories = logic.ListCategory();
        }

        private void LoadData()
        {
            Dictionary<int, object> result = logic.ListProducts();
            listProduct = result[1] as List<Product>; // Ép kiểu về List<Product>
            dt = result[2] as DataTable;              // Ép kiểu về DataTable
            int total = Convert.ToInt32(result[3]);

            categories = logic.ListCategory();
            if (listProduct == null || dt == null)
            {
                MessageBox.Show("Lỗi khi chuyển đổi dữ liệu!");
            }

        }

        public void DisplayProducts()
        {
            // Lấy danh sách các danh mục để ánh xạ
            Dictionary<int, string> categoryNames = categories.ToDictionary(c => c.Id, c => c.Name);

            // Tạo DataTable mới để hiển thị
            DataTable displayTable = new DataTable();
            displayTable.Columns.Add("STT", typeof(int));
            displayTable.Columns.Add("Hình ảnh", typeof(Image));
            displayTable.Columns.Add("Tên sản phẩm", typeof(string));
            displayTable.Columns.Add("Danh mục", typeof(string));
            displayTable.Columns.Add("Khối lượng tồn", typeof(string));
            displayTable.Columns.Add("Mua ngay", typeof(string));

            List<Product> products = listProduct;
            Form preview = null;

            try
            {
                for (int i = 0; i < listProduct.Count; i++)
                {
                    Product product = products[i];
                    string categoryName = categoryNames.ContainsKey(product.CategoryId) ? categoryNames[product.CategoryId] : "Không xác định";
                    string status = product.Status;
                    Image productImage = null;

                    if (!string.IsNullOrEmpty(product.Image) && System.IO.File.Exists(product.Image))
                    {
                        try
                        {
                            using (Image img = Image.FromFile(product.Image))
                            {
                                productImage = new Bitmap(img, new Size(100, 100));
                            }
                        }
                        catch
                        {
                            productImage = new Bitmap(Image.FromFile(@"E:\C#\logo.jpg"), new Size(100, 100));
                        }
                    }
                    else
                    {
                        productImage = new Bitmap(Image.FromFile(@"E:\C#\logo.jpg"), new Size(100, 100));
                    }

                    displayTable.Rows.Add(i + 1, productImage, product.Name, categoryName, product.Total, "Mua");
                }

                DataGridView dgv = new DataGridView
                {
                    DataSource = displayTable,
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ReadOnly = true
                };

                dgv.CellFormatting += (sender, e) =>
                {
                    if (dgv.Columns[e.ColumnIndex].Name == "Hình ảnh" && e.Value is Image)
                    {
                        e.FormattingApplied = true;
                    }
                };


                dgv.MouseMove += (sender, e) =>
                {
                    DataGridView.HitTestInfo hitTest = dgv.HitTest(e.X, e.Y);
                    if (hitTest.ColumnIndex == 2 && hitTest.RowIndex >= 0)
                    {
                        Product product = products[hitTest.RowIndex];
                        DataRow row = displayTable.Rows[hitTest.RowIndex];
                        if (row["Hình ảnh"] is Image image)
                        {
                            if (preview == null)
                            {
                                preview = new Form
                                {
                                    Size = new Size(220, 220),
                                    StartPosition = FormStartPosition.Manual,
                                    Location = dgv.PointToScreen(e.Location),
                                    FormBorderStyle = FormBorderStyle.None
                                };

                                PictureBox pictureBox = new PictureBox
                                {
                                    Image = image,
                                    Dock = DockStyle.Fill,
                                    SizeMode = PictureBoxSizeMode.Zoom
                                };

                                preview.Controls.Add(pictureBox);
                                preview.Show();
                                LoadData();
                            }
                        }
                    }
                    else
                    {
                        preview?.Close();
                        preview = null;
                    }
                };


                dgv.CellClick += (sender, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        Product product = products[e.RowIndex];
                        if (e.ColumnIndex == 5)
                        {
                            AddToCart(product);
                        }
                        else if (e.ColumnIndex == 6)
                        {
                            buyNow(product);
                        }
                    }
                };

                panel1.Controls.Clear();
                panel1.Controls.Add(dgv);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        public void LoadProdcutData()
        {
            if (listProduct != null && listProduct.Count > 0)
            {
                DisplayProducts();
            }
            else
            {
                DisplayNoDataMessage();
            }
        }
        private void DisplayNoDataMessage()
        {
            panel1.Controls.Clear();
            Label labelNoData = new Label
            {
                AutoSize = true,
                Location = new Point(300, 200),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Text = "Không tồn tại danh mục"
            };
            panel1.Controls.Add(labelNoData);
        }


        // Method to handle adding product to cart
        private void AddToCart(Product productId)
        {
            thanhToan dh = new thanhToan(productId, 0, pss);
            dh.Show();
        }

        // Method to handle adding product to cart
        private void buyNow(Product productId)
        {
            thanhToan dh = new thanhToan(productId, 1, pss);
            dh.Show();
        }

        

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string ids = "";
                string names = string.IsNullOrEmpty(textBox1.Text) ? null : textBox1.Text;

                int idc = -1;
                listProduct = logic.findProduct(ids, names, idc);
                LoadProdcutData();

                if (listProduct == null || listProduct.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DisplayNoDataMessage();

                }
                button1.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi {ex.Message}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayNoDataMessage();
                button1.Visible = true;
            }
        }
    }
}
