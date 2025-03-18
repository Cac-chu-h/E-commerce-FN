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


    public partial class Orders : Form
    {
        public BLL logic = new BLL();

        public List<Order> ListOrders = new List<Order>();
        public List<Order> selected = new List<Order>();

        string state = "";
        public Orders()
        {
            InitializeComponent();
        }

        public Orders(string state)
        {
            this.state = state;
            InitializeComponent();
        }

        private void Orders_Load(object sender, EventArgs e)
        {
            logo.Image = Image.FromFile(logic.logo);
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            Status.SelectedIndex = 0;
            Status.Items.Add("Đang chờ"); // duyệt 
            Status.Items.Add("Đang chuẩn bị"); // hủy 
            Status.Items.Add("Đang giao"); // thất bại
            Status.Items.Add("Đã giao"); // xóa
            Status.Items.Add("Đơn hủy"); // xóa 
            Status.Items.Add("Giao Thất bại"); // xóa 

            LoadData();
            LoadAdminData();
        }

        public void LoadData(string query = null)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                {
                    logic.GetAllOrders(ref ListOrders);
                }
                else
                {
                    logic.GetAllOrders(ref ListOrders, query);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void LoadAdminData()
        {
            if (ListOrders != null && ListOrders.Count > 0)
            {
                DisplayOrders();
            }
            else
            {
                DisplayNoDataMessage();
            }

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

        public void DisplayOrders()
        {
            try
            {
                // Tạo DataTable và các cột
                DataTable displayTable = new DataTable();
                displayTable.Columns.Add("Chọn", typeof(bool));
                displayTable.Columns.Add("STT", typeof(int));
                displayTable.Columns.Add("Mã ĐH", typeof(int));
                displayTable.Columns.Add("Ngày đặt", typeof(DateTime));
                displayTable.Columns.Add("Tổng tiền", typeof(decimal));
                displayTable.Columns.Add("Trạng thái", typeof(string));
                displayTable.Columns.Add("Mã vận đơn", typeof(string));
                displayTable.Columns.Add("Duyệt", typeof(string));
                displayTable.Columns.Add("Chi tiết", typeof(string));

                // Giả định listOrders đã được load từ CSDL
                List<Order> orders = ListOrders;

                for (int i = 0; i < orders.Count; i++)
                {
                    Order order = orders[i];

                   


                    displayTable.Rows.Add(
                        false,
                        i + 1,
                        order.OrderID,
                        order.OrderDate,
                        order.TotalAmount,
                        order.Status,
                        order.TrackingNumber,
                        "Duyệt",    // Nút duyệt
                        "Xem chi tiết" // Nút xem chi tiết
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

                        int OrderId = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["Mã ĐH"].Value);
                        var order = orders.FirstOrDefault(o => o.OrderID == OrderId);

                        if (order == null) return;

                        // Xử lý nút Duyệt
                        if (e.ColumnIndex == 7)
                        {
                            if (e.RowIndex < 0) return;

                            int OrderIds = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["Mã ĐH"].Value);
                            var orderss = orders.FirstOrDefault(o => o.OrderID == OrderIds);
                            if (orderss == null) return;

                            switch (orderss.Status)
                            {
                                case "Đang chờ":
                                    HandlePendingOrder(OrderIds);
                                    break;
                                case "Đã duyệt":
                                    HandleApprovedOrder(OrderIds);
                                    break;
                                case "Đang chuẩn bị":
                                    HandlePreparingOrder(OrderIds);
                                    break;
                                case "Đang giao":
                                    HandleDeliveringOrder(OrderIds);
                                    break;
                                case "Giao thất bại":
                                    HandleFailedOrder(OrderIds);
                                    break;
                                default:
                                    MessageBox.Show("Trạng thái không hợp lệ!");
                                    break;
                            }
                            LoadData();
                            DisplayOrders();
                        }
                        // Xử lý nút Xem chi tiết
                        else if (e.ColumnIndex == 8)
                        {
                            renderOrderDetail(OrderId);
                        }
                        // Xử lý checkbox
                        else if (e.ColumnIndex == 0)
                        {
                            DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dgv.Rows[e.RowIndex].Cells[0];
                            cell.Value = cell.Value == null ? true : !(bool)cell.Value;
                            if (cell.Value == null || (bool)cell.Value == false)
                            {
                                cell.Value = true;
                                selected.Add(order);
                            }
                            else
                            {
                                cell.Value = false;
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

        private void HandlePendingOrder(int orderId)
        {
            if (MessageBox.Show($"Duyệt đơn hàng #{orderId}?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (logic.UpdateOrders(orderId, "Đã duyệt"))
                {
                    MessageBox.Show("Đã duyệt đơn!");
                }
            }
        }

        private void HandleApprovedOrder(int orderId)
        {
            logic.UpdateOrders(orderId, "Đang chuẩn bị");
        }

        private void HandlePreparingOrder(int orderId)
        {
            if (MessageBox.Show($"Xác nhận đơn hàng #{orderId} đã sẵn sàng giao?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (logic.UpdateOrders(orderId, "Đang giao"))
                {
                    MessageBox.Show("Đơn hàng đã chuyển sang trạng thái giao!");
                }
            }
        }

        private void HandleDeliveringOrder(int orderId)
        {
            // Hỏi người dùng chọn thành công hay thất bại
            var result = MessageBox.Show("Đơn hàng đã giao thành công?", "Xác nhận",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (logic.UpdateOrders(orderId, "Đã giao"))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    GenerateInvoice(orderId); // Xuất hóa đơn
                }
            }
            else if (result == DialogResult.No)
            {
                if (logic.UpdateOrders(orderId, "Giao thất bại"))
                {
                    MessageBox.Show("Đã cập nhật trạng thái giao thất bại!");
                }
            }
        }

        private void GenerateInvoice(int orderId)
        {
            try
            {
                // Code xuất hóa đơn PDF/Excel tại đây
                MessageBox.Show("Xuất hóa đơn thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất hóa đơn: {ex.Message}");
            }
        }

        private void HandleFailedOrder(int orderId)
        {
            var result = MessageBox.Show($"Xử lý đơn hàng #{orderId}:\n1. Giao lại\n2. Hủy", "Lựa chọn",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                logic.UpdateOrders(orderId, "Đang giao"); // Giao lại
            }
            else
            {
                logic.UpdateOrders(orderId, "Đã hủy"); // Hủy đơn
            }
        }


        // trở lại sau khi tìm kiếm và chỉnh sửa
        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
            LoadAdminData();
            button1.Visible = false;
        }
        // xóa các giá trị được chọn 
        private void button3_Click(object sender, EventArgs e)
        {
            bool isSuccess = true;

            if (selected != null && selected.Count > 0)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa các đơn hàng được chọn không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    foreach (Order i in selected)
                    {
                        if(i.Status == "Đã hủy" || i.Status == "Giao thất bại")
                        {
                            if (!logic.DeleteVoucher(i.OrderID))
                            {
                                isSuccess = false;
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Đơn hàng chưa được giao!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                LoadAdminData();
            }
            else
            {
                MessageBox.Show("Không có mục nào được chọn!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        // lấy giá trị được chọn
        private void categoryName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Status.SelectedIndex != 0)
            {
                string query = Status.Text;
                LoadData(query);
                LoadAdminData();
            }
            else
            {
                LoadData();
                LoadAdminData();
            }
            button1.Visible = true;
        }
        // tìm kiếm -- chưa làm 
        private void button6_Click(object sender, EventArgs e)
        {
            string id = name.Text;
            string userName = customerName.Text;
            if (string.IsNullOrEmpty(id))
            {
                
            }

            if (string.IsNullOrEmpty(userName))
            {

            }

            LoadData();
        }

        private void renderOrderDetail(int OrderId)
        {

        }
    }
}
