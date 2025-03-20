using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace BTL_FN
{
    public partial class frmDangKy : Form
    {
        public frmDangKy()
        {
            InitializeComponent();
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTenDangNhap.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Vui lòng nhập email.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Email không đúng định dạng.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }
            if (txtMatKhau.Text.Length < 6)
            {
                MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtXacNhanMK.Text))
            {
                MessageBox.Show("Vui lòng xác nhận mật khẩu.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtXacNhanMK.Focus();
                return;
            }
            if (!chkDongYDieuKhoan.Checked)
            {
                MessageBox.Show("Bạn phải đồng ý với điều khoản!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                chkDongYDieuKhoan.Focus();
                return;
            }
            if (txtMatKhau.Text != txtXacNhanMK.Text)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtXacNhanMK.Focus();
                return;
            }

            // Tạo đối tượng Account dựa trên dữ liệu nhập từ Form
            Account newAccount = new Account()
            {
                Username = txtTenDangNhap.Text,
                Password = txtMatKhau.Text,
                Email = txtEmail.Text
                // Nếu cần, bạn có thể thêm thông tin khác như hình ảnh hay vai trò.
            };

            // Sử dụng lớp BLL để thêm tài khoản. 
            // Lớp BLL sẽ gọi đến các hàm của DAL để thực hiện thao tác với CSDL.
            BLL bll = new BLL();

            try
            {
                bool isAdded = bll.AddAccount(newAccount);
                if (isAdded)
                {
                    MessageBox.Show("Đăng ký thành công! Bạn có thể đăng nhập ngay.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    frmDangNhap frm = new frmDangNhap();
                    frm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Đăng ký thất bại! Tên đăng nhập hoặc email đã tồn tại.", "Lỗi đăng ký",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi đăng ký tài khoản:\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkDangNhap_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmDangNhap frm = new frmDangNhap();
            frm.Show();
            this.Hide();
        }

        // Hàm kiểm tra định dạng email
        private bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
    }
}

