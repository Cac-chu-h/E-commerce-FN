using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;


namespace BTL_FN
{

    public partial class category : Form
    {
        BLL logic;
        private List<Category> categoryList;
        
        private List<Category> select = new List<Category>();
        private List<int> id = new List<int>();

        public category()
        {
            InitializeComponent();
            categoryList = new List<Category>();
        }

        private void category_Load(object sender, EventArgs e)
        {
            logic = new BLL();
            logo.Image = Image.FromFile(logic.logo);
            logo.SizeMode = PictureBoxSizeMode.Zoom;

            LoadData();
            LoadAccountData();
        }

        private void LoadData()
        {
            categoryList = logic.ListCategory();
            foreach (Category c in categoryList)
            {
                id.Add(c.Id);
            }
        }

        private void LoadAccountData()
        {
            if (categoryList != null && categoryList.Count > 0)
            {
                DisplayAccounts(categoryList);
            }
            else
            {
                DisplayNoDataMessage();
            }
        }

        private void DisplayAccounts(List<Category> categories)
        {
            panel3.Controls.Clear();

            DataTable dt = new DataTable();
            dt.Columns.Add("Chọn", typeof(bool));
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("Tên danh mục", typeof(string));
            dt.Columns.Add("Mô tả", typeof(string));
            dt.Columns.Add("ParentId", typeof(int));
            dt.Columns.Add("Sửa", typeof(string));
            dt.Columns.Add("Xóa", typeof(string));

            for (int i = 0; i < categories.Count; i++)
            {
                Category category = categories[i];
                dt.Rows.Add(false, i + 1, category.Name, category.Description, category.ParentId, "Chỉnh sửa", "Xóa");
            }

            DataGridView dgv = new DataGridView
            {
                DataSource = dt,
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
            };

            dgv.CellClick += (sender, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    Category category = categories[e.RowIndex];

                    if (e.ColumnIndex == 5) // Chỉnh sửa
                    {
                        add_cs view = new add_cs(category, id);
                        if (view.ShowDialog() == DialogResult.OK)
                        {
                            category = view.category;
                            if (id.Contains(category.ParentId))
                            {
                                if (logic.UpdateCategory(category))
                                {
                                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    LoadData();
                                    LoadAccountData();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Không tồn tại giá trị danh mục cha!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                LoadData();
                                LoadAccountData();
                            }
                        }
                    }
                    else if (e.ColumnIndex == 6) // Xóa
                    {
                        if (MessageBox.Show($"Bạn có chắc chắn muốn xóa danh mục {category.Name}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            if (logic.DeleteCategory(category.Id))
                            {
                                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadData();
                                LoadAccountData();
                            }
                        }
                    }
                    else if (e.ColumnIndex == 0) // chọn
                    {
                        select.Add(category);
                    }
                }
            };

            panel3.Controls.Add(dgv);
        }




        private void DisplayNoDataMessage()
        {
            panel3.Controls.Clear();
            Label labelNoData = new Label
            {
                AutoSize = true,
                Location = new Point(300, 200),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Text = "Không tồn tại danh mục"
            };
            panel3.Controls.Add(labelNoData);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(select != null)
            {
                foreach (Category c in select)
                {
                    logic.DeleteCategory(c.Id);
                }
                MessageBox.Show("Xóa các danh mục được chọn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                LoadAccountData();
                select = null;
                select = new List<Category>();
            }
            else
            {
                MessageBox.Show("Không có mục nào được chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            add_cs c = new add_cs(id);
            if (c.ShowDialog() == DialogResult.OK)
            {
                Category newCategory = c.category;
                if (logic.AddCategory(newCategory))
                {
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    LoadAccountData();
                }
                else
                {
                    MessageBox.Show("Thêm thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
            LoadAccountData();
        }
    }
}