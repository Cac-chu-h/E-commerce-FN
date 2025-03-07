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
    public partial class add_p : Form
    {
        public Product value = null;
        public int total = 0;
        public List<Category> c = new List<Category>();
        public add_p(int total, List<Category> categories) 
        {
            this.c = categories;
            this.total = total;
            InitializeComponent();
        }
        public add_p(Product value, List<Category> categories)
        {
            this.c = categories;
            this.value = value;
            InitializeComponent();
        }

        private void add_p_Load(object sender, EventArgs e)
        {
            comboBox2.Items.Add("Còn Hàng");
            comboBox2.Items.Add("Hết Hàng");

            foreach (Category cs in c)
            {
                comboBox1.Items.Add(cs.Name);
            }

            if (value != null)
            {
                pictureBox1.Image = Image.FromFile(value.Image);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                Name.Text = value.Name;
                Description.Text = value.Description;
                Price.Text = value.Price.ToString();
                textBox3.Text = value.Total.ToString();
                comboBox2.Text = value.Status;

                Category selectedCategory = c.FirstOrDefault(cs => cs.Id == value.CategoryId);
                if (selectedCategory != null)
                {
                    comboBox1.Text = selectedCategory.Name;
                }
                button2.Text = "Sửa";
            }
            else
            {
                value = new Product()
                {
                    Id = this.total + 1,
                    Image = @"E:\C#\logo.jpg",
                    Status = "Còn Hàng",
                    Rating = 5,
                    dateAdd = DateTime.Now,
                    Total = 0,
                    TotalPay = 0,
                    CategoryId = 0
                };

                pictureBox1.Image = Image.FromFile(value.Image);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                comboBox2.SelectedIndex = 0;
                comboBox1.SelectedIndex = 0;
            }
            
        }




        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho phép nhập số và phím điều khiển (Backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Chặn ký tự không hợp lệ
                MessageBox.Show("Chỉ được phép nhập số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Name.Text))
                {
                    MessageBox.Show("Tên sản phẩm không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Name.Focus();
                    return;
                }

                this.value.Name = Name.Text;
                this.value.Description = Description.Text;

                if (!decimal.TryParse(Price.Text, out decimal price))
                {
                    MessageBox.Show("Giá sản phẩm phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Price.Clear();
                    Price.Focus();
                    return;
                }
                this.value.Price = price;

                if (!int.TryParse(textBox3.Text, out int total))
                {
                    MessageBox.Show("Số lượng phải là số nguyên hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox3.Clear();
                    textBox3.Focus();
                    return;
                }
                this.value.Total = total;

                if (comboBox2.SelectedItem != null)
                {
                    string status = comboBox2.SelectedItem.ToString();
                    this.value.Status = status;
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn trạng thái!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox2.Focus();
                    return;
                }

                Category cn = c.FirstOrDefault(ce => ce.Name == comboBox1.SelectedItem?.ToString());
                this.value.CategoryId = cn != null ? cn.Id : 0;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Có chắc chắn muốn đóng lại!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog();
                if (file.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.Image = Image.FromFile(file.FileName);
                    value.Image = file.FileName;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Lỗi {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
