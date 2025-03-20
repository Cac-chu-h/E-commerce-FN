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

    public partial class voucher : Form
    {

        public BLL logic => BLL.Instance;
        List<Voucher> listVoucher = new List<Voucher>();
        DataTable dt = new DataTable();
        List<int> selected = new List<int>();
        List<Category> ListCategory = new List<Category>();
        int total = 0;
        

        public voucher()
        {
            InitializeComponent();
        }
       
        

        private void voucher_Load(object sender, EventArgs e)
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
            LoadVoucherData();
        }

        public void LoadData()
        {
            Dictionary<int, object> result = logic.ListVouchers();
            listVoucher = result[1] as List<Voucher>; // Ép kiểu về List<Product>
            dt = result[2] as DataTable;              // Ép kiểu về DataTable

            if (listVoucher == null || dt == null)
            {
                MessageBox.Show("Lỗi khi chuyển đổi dữ liệu!");
            }
        }
        
        public void LoadVoucherData()
        {
            if (listVoucher != null && listVoucher.Count > 0)
            {
                DisplayVoucher();
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

        public void DisplayVoucher()
        {
            Dictionary<int, string> categoryNames = ListCategory.ToDictionary(c => c.Id, c => c.Name);
            DataTable displayTable = new DataTable();
            displayTable.Columns.Add("Chọn", typeof(bool));
            displayTable.Columns.Add("Mã Voucher", typeof(string)); // Thêm cột mã
            displayTable.Columns.Add("Loại voucher", typeof(string));
            displayTable.Columns.Add("Giá trị", typeof(decimal));
            displayTable.Columns.Add("Ngày bắt đầu", typeof(DateTime)); // Thêm cột ngày
            displayTable.Columns.Add("Ngày kết thúc", typeof(DateTime)); // Thêm cột ngày
            displayTable.Columns.Add("Danh mục áp dụng", typeof(string)); // Lấy từ view
            displayTable.Columns.Add("Mô tả", typeof(string)); // Thêm cột mô tả
            displayTable.Columns.Add("Trạng thái", typeof(string));
            displayTable.Columns.Add("Sửa", typeof(string));
            displayTable.Columns.Add("Xóa", typeof(string));

            List<Voucher> vouchers = listVoucher;

            try
            {
                for (int i = 0; i < vouchers.Count; i++)
                {
                    try
                    {
                        Voucher voucher = vouchers[i];

                        displayTable.Rows.Add(
                            false,
                            "#" + voucher.voucherCode, // Lấy mã voucher
                            voucher.TypeOf,
                            voucher.ValueOfVoucher,
                            voucher.StartDate, // Ngày bắt đầu
                            voucher.EndDate, // Ngày kết thúc
                            voucher.categoryName, // Tên danh mục từ view
                            voucher.Description, // Mô tả
                            voucher.Status,
                            "Chỉnh sửa",
                            "Xóa"
                        );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"[Error] Lỗi khi thêm dòng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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


                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                // Chỉ tạo ToolTip một lần bên ngoài event (để tránh tạo mới liên tục)
                ToolTip toolTip1 = new ToolTip();
                dgv.CellMouseEnter += (sender, e) =>
                {


                    if (e.RowIndex >= 0 && e.RowIndex < dgv.Rows.Count && e.ColumnIndex == 8) // Kiểm tra chỉ số hàng & cột hợp lệ
                    {
                        var cellValue = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        string tooltipText = cellValue?.ToString() ?? "";
                        toolTip1.SetToolTip(dgv, tooltipText);
                    }
                };



                dgv.CellClick += (sender, e) =>
                {
                    try
                    {
                        if (e.RowIndex >= 0)
                        {
                            Voucher voucher = vouchers[e.RowIndex];
                            if (e.ColumnIndex == 9)
                            {
                                add_v p = new add_v(voucher, ListCategory);
                                if (p.ShowDialog() == DialogResult.OK)
                                {
                                    if(p.value != null)
                                    {
                                        Voucher v = p.value;
                                        if (logic.UpdateVoucher(v))
                                        {
                                            MessageBox.Show("Sửa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            LoadData();
                                            DisplayVoucher();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Sửa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Sửa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    
                                }
                            }
                            else if (e.ColumnIndex == 10)
                            {
                                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa voucher {voucher.TypeOf}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    if (logic.DeleteVoucher(voucher.Id))
                                    {
                                        MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LoadData();
                                        DisplayVoucher();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Xóa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                            }
                            else if (e.ColumnIndex == 0)
                            {
                                try
                                {
                                    
                                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

                                    if (chk.Value == null || (bool)chk.Value == false)
                                    {
                                        chk.Value = true;
                                        selected.Add(voucher.Id);
                                    }
                                    else
                                    {
                                        chk.Value = false;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show($"[Error] Lỗi khi xử lý checkbox: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"[Error] Lỗi trong sự kiện CellClick: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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



        // tìm kiếm thất bại/ thành công quay lại trang đâu 
        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
            LoadVoucherData();
            button1.Visible = false;
        }
        // thêm mới
        // thêm mới
        private void button4_Click(object sender, EventArgs e)
        {
            add_v p = new add_v(total, ListCategory);
            if (p.ShowDialog() == DialogResult.OK)
            {
                Voucher v = p.value;
                if (logic.AddVoucher(v))
                {
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    LoadVoucherData();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadData();
                    LoadVoucherData();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool isSuccess = true;

            if (selected != null && selected.Count > 0)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa các voucher được chọn không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (int i in selected)
                    {
                        if (!logic.DeleteVoucher(i))
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                    selected.Clear();
                }
                if (isSuccess)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LoadData();
                LoadVoucherData();
            }
            else
            {
                MessageBox.Show("Không có mục nào được chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        // tìm kiếm
        // dữ liệu cần 
        // danh mục 
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                string voucherId = id.Text; // mã 
                string nameVoucher = name.Text; // loại
                string nameCategory = categoryName.Text; // danh mục 

                int categoryId = 0;
                foreach(Category ca in ListCategory)
                {
                    if(ca.Name == nameCategory)
                    {
                        categoryId = ca.Id;
                        break;
                    }
                }

                if (logic.FindVoucher(voucherId, nameVoucher, categoryId) != null)
                {
                    listVoucher = logic.FindVoucher(voucherId, nameVoucher, categoryId);
                }
                else
                {
                    MessageBox.Show("Không tìm thấy voucher!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                button1.Visible = true;
                LoadVoucherData();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi {ex.Message}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DisplayNoDataMessage();
                button1.Visible = true;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void id_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
