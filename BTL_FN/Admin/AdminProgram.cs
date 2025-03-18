using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_FN.Admin
{
    public partial class AdminProgram : Form
    {
        private DAL data = new DAL();
        // tổng số lượng đơn hàng
        int tongSoDonHang = 0;
        // tổng số doanh thu
        // tổng số đơn hàng * giá 1 sản phẩm 
        float tongDoanhThu = 0;
        // tổng số sản phẩm 
        int tongSoSanPham = 0;
        // tổng số khách hàng 
        int tongSoKhachHang = 0;
        // phản hồi mới

        List<Reprots> reprots = new List<Reprots>();
        // đơn hàng mới 

        List<Order> orders = new List<Order>();

        // sản phẩm mới 
        List<Product> products = new List<Product>();


        BLL logic = new BLL();

        public AdminProgram()
        {
            InitializeComponent();
        }

        private void AdminProgram_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadAdminData();
        }

        private void LoadData()
        {
            try
            {
                logic.loadAdminDashboard(ref tongDoanhThu, ref tongSoDonHang, ref tongSoSanPham, ref tongSoKhachHang, ref orders, ref products, ref reprots);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Lỗi " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void LoadAdminData()
        {
            if (reprots.Count > 0 && orders.Count > 0 && products.Count > 0)
            {

                DisplayOrders();
            }
            else
            {
                if(reprots.Count == 0)
                {
                    DisplayNoDataMessage(panel8);
                }else if(orders.Count == 0)
                {
                    DisplayNoDataMessage(panel3);
                }else if(products.Count == 0)
                {
                    DisplayNoDataMessage(panel9);
                }
                
            }

        }

        private void DisplayNoDataMessage(Panel p)
        {
            p.Controls.Clear();
            Label labelNoData = new Label
            {
                AutoSize = true,
                Location = new Point(300, 200),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Text = "Không tồn tại danh mục"
            };
            p.Controls.Add(labelNoData);
        }

        public void DisplayOrders()
        {
            try
            {
                // hiện thị header

                label5.Text = tongDoanhThu + " VND";
                label9.Text = tongSoDonHang + "";
                label12.Text = tongSoSanPham + "";
                label15.Text = tongSoKhachHang + "";

                // hiện thi sản phẩm 
                DisplayProducts();
                DisplayOrderss();
                DisplayReports();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị: {ex.Message}");
            }
        }

        public void DisplayReports()
        {
            try
            {
                // Tạo DataTable và định nghĩa các cột
                DataTable displayTable = new DataTable();
                displayTable.Columns.Add("Chủ đề", typeof(string));
                displayTable.Columns.Add("Nội dung", typeof(string));
                displayTable.Columns.Add("Trạng thái", typeof(string));


                // Đưa dữ liệu từ danh sách vào DataTable
                foreach (var report in reprots)
                {
                    displayTable.Rows.Add(
                        report.ChuDe,
                        report.NoiDung,
                        report.TrangThai
                    );
                }

                // Tạo và cấu hình DataGridView
                DataGridView dgv = new DataGridView
                {
                    DataSource = displayTable,
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ReadOnly = true
                };

                // Gán vào panel (ví dụ panel4)
                panel8.Controls.Clear();
                panel8.Controls.Add(dgv);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị: {ex.Message}");
            }
        }


        public void DisplayOrderss()
        {
            try
            {
                // Tạo DataTable và các cột
                DataTable displayTable = new DataTable();
                displayTable.Columns.Add("Mã ĐH", typeof(string));
                displayTable.Columns.Add("Khách Hàng", typeof(string));
                displayTable.Columns.Add("Trạng thái", typeof(string));
                displayTable.Columns.Add("Tổng tiền", typeof(decimal));
                // Giả định listOrders đã được load từ CSDL
                List<Order> order = orders;

                for (int i = 0; i < order.Count; i++)
                {
                    Order itemorder = orders[i];
                    displayTable.Rows.Add(
                        "#"+itemorder.OrderID,
                        itemorder.tenNguoiDat,
                        itemorder.Status,
                        itemorder.TotalAmount
                    );
                }

                // Tạo và cấu hình DataGridView
                DataGridView dgv = new DataGridView
                {
                    DataSource = displayTable,
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ReadOnly = true
                };


                
                panel3.Controls.Clear();
                panel3.Controls.Add(dgv);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị: {ex.Message}");
            }
        }


        public void DisplayProducts()
        {
            // Lấy danh sách các danh mục để ánh xạ
            List<Category> ListCategory = new List<Category>();
            ListCategory = logic.ListCategory();
            Dictionary<int, string> categoryNames = ListCategory.ToDictionary(c => c.Id, c => c.Name);

            // Tạo DataTable mới để hiển thị
            DataTable displayTable = new DataTable();
            displayTable.Columns.Add("STT", typeof(int));
            displayTable.Columns.Add("Hình ảnh", typeof(Image));
            displayTable.Columns.Add("Tên sản phẩm", typeof(string));
            displayTable.Columns.Add("Danh mục", typeof(string));
            displayTable.Columns.Add("Trạng thái", typeof(string));

            List<Product> product = products;
            Form preview = null;

            try
            {
                for (int i = 0; i < products.Count; i++)
                {
                    Product productss = products[i];
                    string categoryName = categoryNames.ContainsKey(productss.CategoryId) ? categoryNames[productss.CategoryId] : "Không xác định";
                    string status = productss.Status;
                    Image productImage = null;

                    if (!string.IsNullOrEmpty(productss.Image) && System.IO.File.Exists(productss.Image))
                    {
                        try
                        {
                            using (Image img = Image.FromFile(productss.Image))
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

                    displayTable.Rows.Add(i + 1, productImage, productss.Name, categoryName, status);
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
                        Product productss = products[hitTest.RowIndex];
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

                                Button btnEditImage = new Button
                                {
                                    Text = "Sửa ảnh",
                                    Dock = DockStyle.Bottom,
                                    Height = 30
                                };

                                btnEditImage.Click += (s, args) =>
                                {
                                    OpenFileDialog openFileDialog = new OpenFileDialog();
                                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                                    {
                                        pictureBox.Image = Image.FromFile(openFileDialog.FileName);
                                        string fileName = openFileDialog.FileName;
                                        productss.Image = fileName;
                                        row["Hình ảnh"] = new Bitmap(pictureBox.Image, new Size(100, 100));
                                    }
                                };

                                logic.updateProduct(productss);

                                preview.Controls.Add(pictureBox);
                                preview.Controls.Add(btnEditImage);
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


                panel9.Controls.Clear();
                panel9.Controls.Add(dgv);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


}
