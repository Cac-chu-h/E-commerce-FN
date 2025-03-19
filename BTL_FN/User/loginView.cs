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
    public partial class loginView : Form
    {
        public string userName;
        public string password;
        public string state;
        BLL logic = new BLL();
        public loginView()
        {
            state = "login";
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (state == "login")
            {
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
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
                            ad.ShowDialog();
                            this.Hide();
                            if(ad.DialogResult == DialogResult.OK)
                            {
                                this.Close();
                            }
                        }
                        else
                        {
                            TrangChu t = new TrangChu();
                            t.ShowDialog();
                            this.Hide();
                            if(t.DialogResult == DialogResult.OK)
                            {
                                this.Close();
                            }

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
                    MessageBox.Show("Kiểm tra lại thông tin!", "Sai thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            addAcount add = new addAcount();
            add.ShowDialog();
            if (add.DialogResult == DialogResult.Cancel)
            {
                this.Close();
            }
        }

        private void loginView_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(logic.logo);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
