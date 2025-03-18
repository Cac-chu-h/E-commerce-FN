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
        private List<Account> FilterAccounts()
        {
            // Lọc theo role
            var filtered = accountList.Where(a => a.Role == roleSelected).ToList();

            // Lọc theo state (trạng thái từ comboBox)
            switch (state)
            {
                case 0: // Tất cả
                    return filtered;
                case 1: // Hoạt động
                    return filtered.Where(a => a.Status == "Hoạt động").ToList();
                case 2: // Không hoạt động
                    return filtered.Where(a => a.Status == "Bị chặn").ToList();
                default:
                    return filtered;
            }
        }

        // Phương thức tải và hiển thị dữ liệu tài khoản
        private void LoadAccountData()
        {
            // Lọc tài khoản dựa trên tiêu chí hiện tại
            List<Account> filteredAccounts = FilterAccounts();

            // Hiển thị danh sách đã lọc
            DisplayAccounts(filteredAccounts);
        }

        // Hiển thị danh sách tài khoản
        private void DisplayAccounts(List<Account> accounts)
        {
            panel3.Controls.Clear();
            int panelHeight = 140; // Chiều cao của mỗi panel
            if (this.accountList.Count > 0)
            {
                for (int i = 0; i < accounts.Count; i++)
                {
                    Account account = accounts[i];

                    Panel panelAccount = new Panel
                    {
                        Size = new Size(840, 130),
                        Location = new Point(5, (i * panelHeight)),
                        BorderStyle = BorderStyle.FixedSingle,
                        Margin = new Padding(10),
                        Tag = account // Lưu thông tin tài khoản vào Tag
                    };

                    CheckBox check = new CheckBox
                    {
                        AutoSize = true,
                        Location = new Point(8, 10),
                        Name = "checkBox" + i,
                        Text = ""
                    };

                    // Xử lý đường dẫn hình ảnh an toàn
                    Image logoImage = null;
                    try
                    {
                        string logoPath = Path.Combine(Application.StartupPath, "Resources", "logo.jpg");
                        if (File.Exists(logoPath))
                        {
                            logoImage = Image.FromFile(logoPath);
                        }
                        else
                        {
                            logoImage = Image.FromFile(@"E:\C#\logo.jpg");
                        }
                    }
                    catch
                    {
                        // Xử lý nếu có lỗi khi tải hình ảnh
                    }

                    PictureBox pictureBox = new PictureBox
                    {
                        Location = new Point(30, 35),
                        Size = new Size(68, 68),
                        TabStop = false,
                        Image = logoImage,
                        SizeMode = PictureBoxSizeMode.Zoom
                    };

                    PictureBox pictureBoxRole = new PictureBox
                    {
                        Location = new Point(30, 10),
                        Size = new Size(20, 20),
                        TabStop = false,
                        Image = logoImage,
                        SizeMode = PictureBoxSizeMode.Zoom
                    };

                    Label labelRole = new Label
                    {
                        AutoSize = true,
                        Location = new Point(50, 13),
                        Name = "labelRole" + i,
                        Font = new Font("Arial", 8, FontStyle.Bold),
                        Text = account.Role
                    };

                    Label labelUsername = new Label
                    {
                        AutoSize = true,
                        Location = new Point(110, 35),
                        Name = "labelUsername" + i,
                        Font = new Font("Arial", 12, FontStyle.Bold),
                        Text = account.Username
                    };

                    Label labelEmail = new Label
                    {
                        AutoSize = true,
                        Location = new Point(110, 60),
                        Name = "labelEmail" + i,
                        Font = new Font("Arial", 10),
                        Text = account.Email
                    };

                    Label labelStatus = new Label
                    {
                        AutoSize = true,
                        Location = new Point(110, 85),
                        Name = "labelStatus" + i,
                        Font = new Font("Arial", 10, FontStyle.Bold),
                        Text = "Trạng thái: " + account.Status,
                        ForeColor = account.Status == "Hoạt động" ? Color.Green : Color.Red
                    };

                    Label labelCreatedDate = new Label
                    {
                        AutoSize = true,
                        Location = new Point(110, 110),
                        Name = "labelCreatedDate" + i,
                        Font = new Font("Arial", 8, FontStyle.Italic),
                        Text = "Ngày tạo: " + account.CreatedDate.ToString("dd/MM/yyyy")
                    };

                    string status;
                    string value;
                    if (account.Status == "Hoạt động")
                    {
                        status = "Chặn";
                        value = "Bị chặn";
                    }
                    else
                    {
                        status = "Bỏ chặn";
                        value = "Hoạt động";
                    }

                    // Thêm nút chỉnh sửa
                    Button btnEdit = new Button
                    {
                        Size = new Size(80, 30),
                        Location = new Point(650, 85),
                        Text = status,
                        Tag = account,
                        FlatStyle = FlatStyle.Flat
                    };

                    btnEdit.Click += (sender, e) =>
                    {
                        Button btn = sender as Button;
                        Account acc = btn.Tag as Account;
                        // Mở form chỉnh sửa tài khoản
                        if (MessageBox.Show($"{status} tài khoản: {acc.Username}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            if (logic.BanAccount(acc.Id, value, acc.Role))
                            {
                                MessageBox.Show("Thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadData();
                                LoadAccountData();
                            }
                        }
                    };

                    // Thêm nút xóa
                    Button btnDelete = new Button
                    {
                        Size = new Size(80, 30),
                        Location = new Point(750, 85),
                        Text = "Xóa",
                        Tag = account,
                        FlatStyle = FlatStyle.Flat,
                        ForeColor = Color.Red
                    };

                    btnDelete.Click += (sender, e) =>
                    {
                        Button btn = sender as Button;
                        Account acc = btn.Tag as Account;
                        if (MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản {acc.Username}?",
                                           "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {

                            if (logic.DeleteAccount(acc.Id, acc.Role))
                            {
                                MessageBox.Show("Thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                accountList.Remove(acc);
                            }
                            LoadData();
                            LoadAccountData();
                        }
                    };

                    string role;
                    if (roleSelected == "Quản trị viên")
                    {
                        role = "Người dùng";
                    }
                    else
                    {
                        role = "Quản trị viên";
                    }

                    Button btnAdmin = new Button
                    {
                        Size = new Size(120, 30),
                        Location = new Point(510, 85),
                        Text = $"Chỉnh quyền {role}",
                        Tag = account,
                        FlatStyle = FlatStyle.Flat,
                        ForeColor = Color.Red
                    };

                    btnAdmin.Click += (sender, e) =>
                    {
                        Button btn = sender as Button;
                        Account acc = btn.Tag as Account;
                        if (MessageBox.Show($"Bạn có chắc chắn muốn xóa tài khoản {acc.Username} thành {role}?",
                                           "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {

                            if (logic.SetAdminAccount(acc.Id, role))
                            {
                                MessageBox.Show("Thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                acc.Role = role;
                            }
                            LoadAccountData();
                        }
                    };

                    // Thêm các control vào panel
                    panelAccount.Controls.Add(check);
                    panelAccount.Controls.Add(pictureBox);
                    panelAccount.Controls.Add(pictureBoxRole);
                    panelAccount.Controls.Add(labelRole);
                    panelAccount.Controls.Add(labelUsername);
                    panelAccount.Controls.Add(labelEmail);
                    panelAccount.Controls.Add(labelStatus);
                    panelAccount.Controls.Add(labelCreatedDate);
                    panelAccount.Controls.Add(btnEdit);
                    panelAccount.Controls.Add(btnDelete);
                    panelAccount.Controls.Add(btnAdmin);

                    // Thêm panel vào panel3
                    panel3.Controls.Add(panelAccount);
                }
            }
            else
            {
                Label labelRole = new Label
                {
                    AutoSize = true,
                    Location = new Point(300, 200),
                    Name = "labelRole",
                    Font = new Font("Arial", 16, FontStyle.Bold),
                    Text = "Không tồn tại tài khoản"
                };
                panel3.Controls.Add(labelRole);
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