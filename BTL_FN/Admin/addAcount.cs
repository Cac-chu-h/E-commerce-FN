﻿using System;
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
    public partial class addAcount : Form
    {
        public BLL logic => BLL.Instance;
        public Account value = new Account();
        public addAcount()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void addAcount_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(logic.logo);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }



        private void button1_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && !string.IsNullOrEmpty(textBox3.Text))
            {
                Account a = new Account()
                {
                    Username = textBox1.Text,
                    Password = textBox2.Text,
                    Email = textBox3.Text
                };
                if (logic.AddAccount(a))
                {
                    logic.isLogin = true;
                }
                else
                {
                    MessageBox.Show("Email đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox3.Focus();
                    return;
                }

                }
                else
            {
                MessageBox.Show("Kiểm tra lại thông tin!", "Sai thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
