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
    public partial class add_cs : Form
    {
        public Category category = null;
        public List<int> id = new List<int>();
        public add_cs(List<int> id)
        {
            this.id = id;
            InitializeComponent();
        }

        public add_cs(Category c, List<int> id)
        {
            this.id = id;
            this.category = c;
            InitializeComponent();
        }

        private void add_cs_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            foreach (int i in id)
            {
                comboBox1.Items.Add(i);
            }

            if(category != null)
            {
                textBox1.Text = category.Name;
                textBox2.Text = category.Description;
                comboBox1.SelectedIndex = category.ParentId != 0 ? category.ParentId : 1;
            }
            else
            {
                category = new Category();
                this.category.Id = id.Count > 0 ? id.Max() + 1 : 1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Tên danh mục không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox1.Focus();
                return; // Dừng lại nếu dữ liệu không hợp lệ
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Mô tả danh mục không được để trống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }

            
            this.category.Name = textBox1.Text;
            this.category.Description = textBox2.Text;

            if (comboBox1.SelectedIndex >= 0)
            {
                this.category.ParentId = Convert.ToInt32(comboBox1.SelectedItem);
            }
            else
            {
                this.category.ParentId = 0; // Gán giá trị mặc định nếu không chọn
            }

            this.DialogResult = DialogResult.OK;
            this.Close();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
