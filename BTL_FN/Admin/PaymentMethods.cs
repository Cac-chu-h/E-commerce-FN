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

    public partial class PaymentMethods : Form
    {
        public BLL logic = new BLL();

        public List<PaymentMethod> pay = new List<PaymentMethod>();
        public List<PaymentMethod> selected = new List<PaymentMethod>();

        public PaymentMethods()
        {
            InitializeComponent();
        }

        private void PaymentMethods_Load(object sender, EventArgs e)
        {
            logo.Image = Image.FromFile(logic.logo);
            logo.SizeMode = PictureBoxSizeMode.Zoom;

            LoadData();
            LoadPaymentData();
        }

        private void LoadData()
        {
            logic.getAllPaymentMothod(ref pay);
        }

        private void LoadPaymentData()
        {
            if(pay != null && pay.Count() > 0)
            {
                DisplayData();
            }
            else
            {
                DisplayNoDataMesage();
            }
        }

        public void DisplayData()
        {
            try
            {
                // Tạo DataTable và các cột
                DataTable displayTable = new DataTable();
                displayTable.Columns.Add("Chọn", typeof(bool));
                displayTable.Columns.Add("STT", typeof(int));
                displayTable.Columns.Add("Phương thức", typeof(string));
                displayTable.Columns.Add("Trạng thái", typeof(string));
                displayTable.Columns.Add("Mô tả", typeof(string));
                displayTable.Columns.Add("Chỉnh sửa", typeof(string));
                displayTable.Columns.Add("Xóa", typeof(string));

                // Giả định listPaymentMethods đã được load từ CSDL
                List<PaymentMethod> methods = pay;

                for (int i = 0; i < methods.Count; i++)
                {
                    PaymentMethod method = methods[i];

                    displayTable.Rows.Add(
                        false,
                        i + 1,
                        method.Name,
                        method.Status,
                        method.Description,
                        "Chỉnh sửa",    // Nút chỉnh sửa
                        "Xóa"          // Nút xóa
                    );
                }

                // Tạo và cấu hình DataGridView
                DataGridView dgv = new DataGridView
                {
                    DataSource = displayTable,
                    Dock = DockStyle.Fill,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ReadOnly = true
                };

                // Xử lý sự kiện click
                dgv.CellClick += (sender, e) =>
                {
                    try
                    {
                        if (e.RowIndex < 0) return;

                        string methodName = dgv.Rows[e.RowIndex].Cells["Phương thức"].Value.ToString();
                        var method = methods.FirstOrDefault(m => m.Name == methodName);

                        if (method == null) return;

                        // Xử lý nút Chỉnh sửa
                        if (e.ColumnIndex == 5)
                        {
                            PaymentMethod p = method;
                            Admin.AddPaymentMethods add = new AddPaymentMethods(p);
                            if (add.ShowDialog() == DialogResult.Yes)
                            {
                                p = add.pay;
                                if (logic.UpdatePayMethod(p))
                                {
                                    MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Cập nhật thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }
                        // Xử lý nút Xóa
                        else if (e.ColumnIndex == 6)
                        {
                            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa phương thức thanh toán này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                if (logic.DeletePaymentMethod(method.Id))
                                {
                                    MessageBox.Show("Thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show("Thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                LoadData();
                                DisplayData();
                            }
                        }
                        // Xử lý checkbox
                        else if (e.ColumnIndex == 0)
                        {
                            DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dgv.Rows[e.RowIndex].Cells[0];
                            if (cell.Value == null || (bool)cell.Value == false)
                            {
                                cell.Value = true;
                                selected.Add(method);
                            }
                            else
                            {
                                cell.Value = false;
                                selected.Remove(method);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi xử lý: {ex.Message}");
                    }
                };

                panel3.Controls.Clear();
                panel3.Controls.Add(dgv);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị: {ex.Message}");
            }
        }


        private void DisplayNoDataMesage()
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

        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
            LoadPaymentData();
            button1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // ad new 
            PaymentMethod p = new PaymentMethod();
            Admin.AddPaymentMethods add = new AddPaymentMethods();
            if(add.ShowDialog() == DialogResult.Yes)
            {
                p = add.pay;
                if (logic.AddPayMethod(p)){
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Thêm thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool isSuccess = true;
            if (selected != null && selected.Count > 0)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa các đơn hàng được chọn không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (PaymentMethod i in selected)
                    {
                        if (!logic.DeletePaymentMethod(i.Id))
                        {
                            isSuccess = false;
                            break;
                        }
                    }
                    selected.Clear();
                }
                if (isSuccess)
                {
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Xóa thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                LoadData();
                LoadPaymentData();
            }
            else
            {
                MessageBox.Show("Không có mục nào được chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
