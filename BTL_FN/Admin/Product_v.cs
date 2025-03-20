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

    public partial class Product_v : Form
    {
        public BLL logic => BLL.Instance;
        List<Product> listProduct = new List<Product>();
        DataTable dt = new DataTable();
        List<int> selected = new List<int>();
        List<Category> ListCategory = new List<Category>();
        int total = 0;
        public Product_v()
        {
            InitializeComponent();
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Product_v_Load(object sender, EventArgs e)
        {
            logo.Image = Image.FromFile(logic.logo);
            logo.SizeMode = PictureBoxSizeMode.Zoom;

            categoryName.SelectedIndex = 0;

            ListCategory = logic.ListCategory();
            foreach (Category c in ListCategory)
            {
                categoryName.Items.Add(c.Name);
            }

            LoadData();
            LoadProdcutData();
        }

        private void LoadData()
        {
            Dictionary<int, object> result = logic.ListProducts();
            listProduct = result[1] as List<Product>; // Ép kiểu về List<Product>
            dt = result[2] as DataTable;              // Ép kiểu về DataTable
            int total = Convert.ToInt32(result[3]);

            if (listProduct == null || dt == null)
            {
                MessageBox.Show("Lỗi khi chuyển đổi dữ liệu!");
            }

        }

        public void DisplayProducts()
        {
            // Lấy danh sách các danh mục để ánh xạ
            Dictionary<int, string> categoryNames = ListCategory.ToDictionary(c => c.Id, c => c.Name);

            // Tạo DataTable mới để hiển thị
            DataTable displayTable = new DataTable();
            displayTable.Columns.Add("Chọn", typeof(bool));
            displayTable.Columns.Add("STT", typeof(int));
            displayTable.Columns.Add("Hình ảnh", typeof(Image));
            displayTable.Columns.Add("Tên sản phẩm", typeof(string));
            displayTable.Columns.Add("Danh mục", typeof(string));
            displayTable.Columns.Add("Khối lượng", typeof(string));
            displayTable.Columns.Add("Sửa", typeof(string));
            displayTable.Columns.Add("Xóa", typeof(string));

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

                    displayTable.Rows.Add(false, i + 1, productImage, product.Name, categoryName, product.Total, "Chỉnh sửa", "Xóa");
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
                                        product.Image = fileName;
                                        row["Hình ảnh"] = new Bitmap(pictureBox.Image, new Size(100, 100));
                                    }
                                };

                                logic.updateProduct(product);

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


                dgv.CellClick += (sender, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        Product product = products[e.RowIndex];
                        if (e.ColumnIndex == 6)
                        {

                            add_p p = new add_p(product, ListCategory);
                            if (p.ShowDialog() == DialogResult.OK)
                            {
                                Product ps = p.value;
                                if (logic.updateProduct(ps))
                                {
                                    MessageBox.Show("Sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadData();
                                    LoadProdcutData();
                                }
                                else
                                {
                                    MessageBox.Show("Sửa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else if (e.ColumnIndex == 7)
                        {
                            if (MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm {product.Name}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                if (logic.deleteProduct(product.Id))
                                {
                                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadData();
                                    LoadProdcutData();
                                }
                            }
                        }
                        else if (e.ColumnIndex == 0)
                        {
                            selected.Add(product.Id);
                            DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

                            // Đảo ngược giá trị checkbox
                            if (chk.Value == null || (bool)chk.Value == false)
                            {
                                chk.Value = true;
                            }
                            else
                            {
                                chk.Value = false;
                            }
                        }
                    }
                };

                panel3.Controls.Clear();
                panel3.Controls.Add(dgv);
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
            panel3.Controls.Clear();
            Label labelNoData = new Label
            {
                AutoSize = true,
                Location = new Point(300, 200),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Text = "Không tồn tại danh mục"
            };
            panel3.Controls.Add(labelNoData);
        }



        // thêm mới
        private void button4_Click(object sender, EventArgs e)
        {
            add_p p = new add_p(total, ListCategory);
            if (p.ShowDialog() == DialogResult.OK)
            {
                Product ps = p.value;
                if (logic.addProduct(ps))
                {
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    LoadProdcutData();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        // xóa
        private void button3_Click(object sender, EventArgs e)
        {
            bool isSucess = false;

            if (selected != null)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa sản phẩm các sản phẩm được chọn không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (int i in selected)
                    {
                        if (logic.deleteProduct(i))
                        {
                            isSucess = true;
                        }
                        else
                        {
                            isSucess = false;
                            break;
                        }
                    }
                    selected = null;
                    selected = new List<int>();
                }

            }
            else
            {
                MessageBox.Show("Không có mục nào được chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (isSucess)
            {
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                DisplayProducts();
            }
            else
            {
                MessageBox.Show("Không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                DisplayProducts();
            }
        }
        // tìm kiếm
        // dữ liệu cần 
        // danh mục 
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string ids = string.IsNullOrEmpty(id.Text) ? null : id.Text;
                string names = string.IsNullOrEmpty(name.Text) ? null : name.Text;

                Category c = null;
                foreach (Category ce in ListCategory)
                {
                    if (ce.Name == categoryName.Text)
                    {
                        c = ce;
                        break;
                    }

                }
                int idc = -1;
                if (c != null)
                {
                    idc = c.Id;
                }
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


        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
            LoadProdcutData();
            button1.Visible = false;
        }

        
    }
}
