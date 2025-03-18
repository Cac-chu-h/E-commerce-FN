using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_FN.Admin
{
    public partial class AddPaymentMethods : Form
    {
        public PaymentMethod pay = null;
        public AddPaymentMethods()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }
        public AddPaymentMethods(PaymentMethod pa)
        {
            this.pay = pa;
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            
        }

        private void AddPaymentMethods_Load(object sender, EventArgs e)
        {
            if (pay != null)
            {
                textBox1.Text = pay.Name;
                richTextBox1.Text = pay.Description;
                if(pay.Status != "Hoạt động")
                {
                    comboBox1.SelectedIndex = 1;
                }
                button2.Text = "Sửa";
            }
            else
            {

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // return this pay value if user click here
            if (pay != null)
            {
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    pay.Name = textBox1.Text;
                    pay.Status = comboBox1.Text;
                    pay.Description = richTextBox1.Text;
                    this.DialogResult = DialogResult.Yes;
                }
                else
                {
                    MessageBox.Show("Tên phương thức không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                }
            }
            else
            {
                pay = new PaymentMethod();
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    pay.Name = textBox1.Text;
                    pay.Status = comboBox1.Text;
                    pay.Description = richTextBox1.Text;
                    this.DialogResult = DialogResult.Yes;
                }
                else
                {
                    MessageBox.Show("Tên phương thức không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Focus();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // exit and close form 
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
