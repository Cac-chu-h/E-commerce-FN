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
    public partial class add_v : Form
    {
        public Voucher value = null;
        public List<Category> categories = new List<Category>();
        public int total = 0;
        public add_v(Voucher v, List<Category> c)
        {
            this.value = v;
            this.categories = c;
            InitializeComponent();
        }

        public add_v(int total, List<Category> c)
        {
            this.total = total;
            this.categories = c;
            InitializeComponent();
        }

        private void add_v_Load(object sender, EventArgs e)
        {
            foreach(Category c in categories)
            {
                comboBox1.Items.Add(c.Name);
            }
            comboBox2.Items.Add("Chưa kích hoạt");
            comboBox2.Items.Add("Hết hạn");
            comboBox2.Items.Add("Hoạt động");

            
            if(value != null)
            {
                try
                {
                    Name.Text = value.TypeOf;
                    string categoryName = "";
                    Price.Text = value.ValueOfVoucher + "";
                    foreach (Category c in categories)
                    {
                        if (c.Id == value.categoryId)
                        {
                            categoryName = c.Name;
                            break;
                        }
                    }
                    comboBox1.Text = categoryName;
                    comboBox2.Text = value.Status;

                    dateTimePicker1.Value = Convert.ToDateTime(value.StartDate);
                    dateTimePicker2.Value = Convert.ToDateTime(value.EndDate);
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Lỗi {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                button2.Text = "Sửa";
                
            }
            else
            {
                value = new Voucher()
                {
                    Id = total + 1,
                    ValueOfVoucher = 0,
                    categoryId = 0, 
                    Status = "Chưa kích hoạt"
                };
                comboBox2.SelectedIndex = 1;
                comboBox1.SelectedIndex = 0;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                bool isCheck = false;
                if (string.IsNullOrEmpty(Name.Text))
                {
                    MessageBox.Show("Tên sản phẩm không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Name.Focus();
                    return;
                }

                this.value.TypeOf = Name.Text;
                if (!decimal.TryParse(Price.Text, out decimal price))
                {
                    MessageBox.Show("Giá sản phẩm phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Price.Clear();
                    Price.Focus();
                    return;
                }
                this.value.ValueOfVoucher = price;

                if (dateTimePicker1.Value >= DateTime.Now && dateTimePicker2.Value >= DateTime.Now && dateTimePicker1.Value <= dateTimePicker2.Value)
                {
                    this.value.StartDate = dateTimePicker1.Value;
                    this.value.EndDate = dateTimePicker2.Value;
                }
                else
                {
                    MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc và không được nhỏ hơn ngày hiện tại!");
                    dateTimePicker1.Focus();
                    return;
                }



                if (comboBox2.SelectedItem != null)
                {
                    string status = comboBox2.SelectedItem.ToString();
                    this.value.Status = status;
                }
                else
                {
                    isCheck = true;
                    MessageBox.Show("Vui lòng chọn trạng thái!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox2.Focus();
                    return;
                }

                Category cn = categories.FirstOrDefault(ce => ce.Name == comboBox1.SelectedItem?.ToString());
                this.value.categoryId = cn != null ? cn.Id : 0;

                if (isCheck)
                {
                    value = null;
                }

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
            if(MessageBox.Show("Xác nhận hủy!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
