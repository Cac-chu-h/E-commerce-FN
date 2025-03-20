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


    public partial class account : Form
    {
        // Giá trị mặc định 
        public BLL logic;
        public string roleSelected = "Quản trị viên";
        public int state = 0;
        private List<Account> accountList; // Danh sách tài khoản


        public account()
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false; // Hoặc false nếu muốn tắt luôn nút thu nhỏ
            this.ControlBox = true;
            InitializeComponent();
            accountList = new List<Account>(); // Khởi tạo danh sách
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Thực hiện chức năng khi nhấn nút (ví dụ: xóa tài khoản đã chọn)
            List<Account> selectedAccounts = GetSelectedAccounts();
            if (selectedAccounts.Count > 0)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa {selectedAccounts.Count} tài khoản đã chọn?",
                                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Xóa tài khoản đã chọn
                    foreach (var acc in selectedAccounts)
                    {
                        if (logic.DeleteAccount(acc.Id, acc.Role))
                        {
                            MessageBox.Show("Thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            accountList.Remove(acc);
                        }

                    }
                    // Cập nhật hiển thị
                    LoadAccountData();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tài khoản để xóa", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Thực hiện chức năng khi nhấn nút (ví dụ: xóa tài khoản đã chọn)
            List<Account> selectedAccounts = GetSelectedAccounts();
            if (selectedAccounts.Count > 0)
            {
                if (MessageBox.Show($"Bạn có chắc chắn muốn thao tác {selectedAccounts.Count} tài khoản đã chọn?",
                                    "Xác nhận thao tác", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Xóa tài khoản đã chọn
                    foreach (var acc in selectedAccounts)
                    {
                        string status;
                        if (acc.Status == "Hoạt động")
                        {
                            status = "Bị chặn";
                        }
                        else
                        {
                            status = "Hoạt động";
                        }
                        if (logic.BanAccount(acc.Id, status, acc.Role))
                        {
                            MessageBox.Show("Thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            acc.Status = status;
                        }

                    }
                    // Cập nhật hiển thị
                    LoadAccountData();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn tài khoản để thực hiện thao tác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void account_Load(object sender, EventArgs e)
        {
            logic = new BLL();
            logo.Image = Image.FromFile(logic.logo);
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            comboBox1.SelectedIndex = 0;

            // Nạp dữ liệu mẫu (trong thực tế sẽ lấy từ cơ sở dữ liệu)
            LoadData();

            // Hiển thị dữ liệu
            LoadAccountData();
        }

        // Phương thức nạp dữ liệu mẫu
        private void LoadData()
        {

            // Tạo dữ liệu mẫu để hiển thị
            if (logic.ListAccounts() != null)
            {
                this.accountList = logic.ListAccounts();
            }
        }

        // Phương thức lấy danh sách tài khoản đã chọn
        private List<Account> GetSelectedAccounts()
        {
            List<Account> selected = new List<Account>();

            foreach (Control control in panel3.Controls)
            {
                if (control is Panel panelItem)
                {
                    CheckBox checkBox = panelItem.Controls.OfType<CheckBox>().FirstOrDefault();
                    if (checkBox != null && checkBox.Checked && panelItem.Tag is Account account)
                    {
                        selected.Add(account);
                    }
                }
            }

            return selected;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            // Không cần code trong sự kiện Paint
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            this.roleSelected = "Quản trị viên";
            LoadAccountData();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            // Không cần code trong sự kiện Paint này
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            this.roleSelected = "Người dùng";
            LoadAccountData();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            state = comboBox1.SelectedIndex;
            LoadAccountData();
        }

        // Phương thức lọc tài khoản dựa trên vai trò và trạng thái hiện tại

        // Phương thức tải và hiển thị dữ liệu tài khoản
        private void LoadAccountData()
        {
            // Lọc tài khoản dựa trên tiêu chí hiện tại

            // Hiển thị danh sách đã lọc
            DisplayAccounts();
        }

        // Hiển thị danh sách tài khoản
        private void DisplayAccounts()
        {
            try
            {
                // Tạo DataTable để binding dữ liệu
                DataTable displayTable = new DataTable();
                displayTable.Columns.Add("Chọn", typeof(bool));
                displayTable.Columns.Add("STT", typeof(int));
                displayTable.Columns.Add("Avatar", typeof(Image));
                displayTable.Columns.Add("Tên đăng nhập", typeof(string));
                displayTable.Columns.Add("Vai trò", typeof(string));
                displayTable.Columns.Add("Trạng thái", typeof(string));
                displayTable.Columns.Add("Ngày tạo", typeof(string));
                displayTable.Columns.Add("Thao tác", typeof(string));

                // Load danh sách tài khoản
                List<Account> accounts = accountList;
                Form preview = null;

                for (int i = 0; i < accounts.Count; i++)
                {
                    Account acc = accounts[i];

                    // Xử lý avatar
                    Image productImage = null;

                    if (!string.IsNullOrEmpty(acc.Avatar) && System.IO.File.Exists(acc.Avatar))
                    {
                        try
                        {
                            using (Image img = Image.FromFile(acc.Avatar))
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


                    // Thêm dòng dữ liệu
                    displayTable.Rows.Add(
                        false,                              // Checkbox
                        i + 1,                              // STT
                        productImage,                        // Avatar
                        acc.Username,                       // Tên đăng nhập
                        acc.Role,                           // Vai trò
                        acc.Status,                         // Trạng thái
                        acc.CreatedDate.ToString("dd/MM/yyyy"), // Ngày tạo
                        acc.Status == "Hoạt động" ? "Chặn" : "Bỏ chặn"
                    );
                }

                // Tạo DataGridView
                DataGridView dgv = new DataGridView
                {
                    DataSource = displayTable,
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ReadOnly = true,
                    RowTemplate = { Height = 85 } // Chiều cao hàng lớn hơn để hiển thị ảnh
                };

                // Xử lý hiển thị ảnh
                dgv.CellFormatting += (sender, e) =>
                {
                    if (dgv.Columns[e.ColumnIndex].Name == "Avatar" && e.Value is Image)
                    {
                        e.FormattingApplied = true;
                    }
                };

                // Xử lý hover preview ảnh
                dgv.MouseMove += (sender, e) =>
                {
                    var hitTest = dgv.HitTest(e.X, e.Y);
                    if (hitTest.ColumnIndex == 2 && hitTest.RowIndex >= 0)
                    {
                        if (preview == null || preview.IsDisposed)
                        {
                            preview = new Form
                            {
                                Size = new Size(200, 200),
                                FormBorderStyle = FormBorderStyle.None,
                                StartPosition = FormStartPosition.Manual,
                                Location = dgv.PointToScreen(new Point(e.X + 20, e.Y + 20))
                            };

                            PictureBox pb = new PictureBox
                            {
                                Image = (Image)dgv.Rows[hitTest.RowIndex].Cells["Avatar"].Value,
                                SizeMode = PictureBoxSizeMode.Zoom,
                                Dock = DockStyle.Fill
                            };

                            preview.Controls.Add(pb);
                            preview.Show();
                        }
                    }
                    else
                    {
                        preview?.Close();
                    }
                };

                // Xử lý click các nút
                dgv.CellClick += (sender, e) =>
                {
                    if (e.RowIndex < 0) return;

                    Account acc = accounts[e.RowIndex];

                    // Xử lý checkbox chọn
                    if (e.ColumnIndex == 0)
                    {
                        DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dgv.Rows[e.RowIndex].Cells[0];
                        cell.Value = !(cell.Value as bool? ?? false);
                        return;
                    }

                    // Xử lý các nút chức năng
                    switch (dgv.Columns[e.ColumnIndex].Name)
                    {
                        case "Thao tác":
                            string newStatus = acc.Status == "Hoạt động" ? "Bị chặn" : "Hoạt động";
                            if (logic.BanAccount(acc.Id, newStatus, acc.Role))
                            {
                                acc.Status = newStatus;
                                dgv.InvalidateRow(e.RowIndex);
                            }
                            break;

                        case "Xóa":
                            if (MessageBox.Show($"Xóa tài khoản {acc.Username}?", "Xác nhận",
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                if (logic.DeleteAccount(acc.Id, acc.Role))
                                {
                                    accountList.Remove(acc);
                                    dgv.Rows.RemoveAt(e.RowIndex);
                                }
                            }
                            break;

                        case "Quyền":
                            string newRole = acc.Role == "Admin" ? "User" : "Admin";
                            if (logic.SetAdminAccount(acc.Id, newRole))
                            {
                                acc.Role = newRole;
                                dgv.InvalidateRow(e.RowIndex);
                            }
                            break;
                    }
                };

                // Thêm vào panel
                panel3.Controls.Clear();
                panel3.Controls.Add(dgv);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị: {ex.Message}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            // Không thực hiện getAccount() trong sự kiện Paint
            // Paint sẽ được gọi nhiều lần và gây tình trạng tạo lại các control liên tục
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.roleSelected = "Quản trị viên";
            LoadAccountData();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.roleSelected = "Người dùng";
            LoadAccountData();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string ids = string.IsNullOrEmpty(id.Text) ? null : id.Text;
            string uname = string.IsNullOrEmpty(name.Text) ? null : name.Text;
            string uemail = string.IsNullOrEmpty(email.Text) ? null : email.Text;
            string tableName = "account"; // Tên bảng có thể thay đổi

            accountList = logic.FindUser(ids, uname, uemail);
            LoadAccountData();
            if (accountList == null || accountList.Count == 0)
            {
                MessageBox.Show("Không tìm thấy tài khoản!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            button4.Visible = true;
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            LoadData();
            LoadAccountData();
            button4.Visible = false;
        }
    }


}