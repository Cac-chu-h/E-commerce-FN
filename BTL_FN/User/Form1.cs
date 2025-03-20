using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_FN.User
{
    public partial class Form1 : Form
    {
        public BLL logic => BLL.Instance;

        Programs pss = null;
        public Form1(Programs pss)
        {
            this.pss = pss;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label4.Text = "Đăng ký";
            label3.Visible = true;
            textBox3.Visible = true;

            // Kiểm tra các trường bắt buộc cho đăng ký
            if (!ValidateFields(
                new[] { textBox1, textBox2, textBox3 },
                new[] { "tên đăng nhập", "mật khẩu", "email" }))
            {
                return;
            }

            string tk = textBox1.Text;
            string mk = textBox2.Text;
            string email = textBox3.Text;

            if(logic.dki(tk, mk, email))
            {
                MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pss.load();
                this.Close();
            }
            else
            {
                MessageBox.Show("Đăng ký không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label4.Text = "Đăng Nhập";
            label3.Visible = false;
            textBox3.Visible = false;

            // Kiểm tra các trường bắt buộc cho đăng nhập
            if (!ValidateFields(
                new[] { textBox1, textBox2 },
                new[] { "tên đăng nhập", "mật khẩu" }))
            {
                return;
            }

            string tk = textBox1.Text;
            string mk = textBox2.Text;

            if(logic.Login(tk, mk))
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                pss.load();
                this.Close();
            }
            else
            {
                MessageBox.Show("Đăng nhập không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Hàm helper kiểm tra các TextBox và focus vào ô trống đầu tiên
        private bool ValidateFields(TextBox[] textBoxes, string[] fieldNames)
        {
            for (int i = 0; i < textBoxes.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(textBoxes[i].Text))
                {
                    textBoxes[i].Focus(); // Focus vào ô trống
                    MessageBox.Show($"Vui lòng nhập {fieldNames[i]}!",
                                  "Lỗi",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(logic.logo);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            logo.Image = Image.FromFile(logic.logo);
            logo.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
