using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BTL_FN.Admin;

namespace BTL_FN
{
    public partial class frmDangNhap : Form
    {
        public frmDangNhap()
        {
            InitializeComponent();
        }

        // Tích hợp vào hàm đăng nhập
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập.", "Lỗi đăng nhập",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenDangNhap.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu.", "Lỗi đăng nhập",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMatKhau.Focus();
                return;
            }

            // Sử dụng lớp BLL để đăng nhập (BLL Login gọi bên trong DAL)
            BLL bll = new BLL();
            bool isLoggedIn = bll.Login(txtTenDangNhap.Text, txtMatKhau.Text);

            if (isLoggedIn)
            {
                Form frmTrangChu;

                // Dựa vào vai trò của người dùng đã được lấy trong BLL.Login
                if (bll.UserRole == "Quản trị viên")
                    frmTrangChu = new AdminProgram();
                else
                    frmTrangChu = new TrangChu();

                frmTrangChu.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi đăng nhập",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void linkDangKy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmDangKy frm = new frmDangKy();
            frm.Show();
            this.Hide();
        }
    }
}

