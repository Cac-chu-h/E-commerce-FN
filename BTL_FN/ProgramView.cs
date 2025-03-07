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
    public partial class ProgramView : Form
    {
        public BLL logic = new BLL();
        public string state = "";
        public ProgramView()
        {
            this.state = "login";
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(state == "login")
            {
                if(!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
                {
                    if (logic.Login(textBox1.Text.Trim(), textBox2.Text.Trim()))
                    {
                        logic.isLogin = true;
                        if (logic.UserRole == "Admin" && logic.UserActive != "Inacctive")
                        {
                            admin ad = new admin();
                            ad.TopLevel = false;
                            ad.FormBorderStyle = FormBorderStyle.None;
                            ad.Dock = DockStyle.Fill;
                            panel1.Controls.Clear();
                            panel1.Controls.Add(ad);
                            ad.Show();
                        }else
                        {
                            TrangChu t = new TrangChu();
                            this.Hide();
                            t.ShowDialog();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Kiểm tra lại thông tin!", "Sai thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        textBox1.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Kiểm tra lại thông tin!", "Sai thông tin",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            state = "resgister";
            addAcount a = new addAcount();
            a.TopLevel = false;
            a.FormBorderStyle = FormBorderStyle.None;
            a.Dock = DockStyle.Fill;
            panel2.Controls.Clear();
            panel2.Controls.Add(a);
            a.Show();
            if (a.value != null)
            {
                if (logic.AddAccount(a.value.Username, a.value.Email, a.value.password))
                {
                    MessageBox.Show("Thất bại! Hãy kiểm tra lại thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TrangChu t = new TrangChu();
                    t.TopLevel = false;
                    t.FormBorderStyle = FormBorderStyle.None;
                    t.Dock = DockStyle.Fill;
                    panel1.Controls.Clear();
                    panel1.Controls.Add(t);
                    t.Show();
                }
                else
                {
                    MessageBox.Show("Thất bại! Hãy kiểm tra lại thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ProgramView_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(logic.logo);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
