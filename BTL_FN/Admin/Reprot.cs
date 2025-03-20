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
    public partial class Reprot : Form
    {
        // lấy ra tất cả thông báo 
        // tổng phản hồi
        int tongPhanHoi = 0;
        // phản hồi mới
        int phanHoiMoi = 0;
        // đang sử lý 
        int phanHoiDangSuLy = 0;
        // đánh giá trung bình 
        float danhGiaTrungBinh = 0;

        List<Reprots> reprots = new List<Reprots>();


        public BLL logic => BLL.Instance;
        public Reprot()
        {
            InitializeComponent();
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void Reprot_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;

            LoadData();
            LoadRepostData();
        }

       private void LoadData()
        {
            logic.getAllReport(ref reprots);
        }

        private void LoadRepostData()
        {
            if(reprots.Count != 0)
            {
                HienThi();
            }
            else
            {
                DisplayNoDataMessage();
            }
        }
        private void HienThi()
        {
            // hiển thị các giá trị của phản hồi 
            tongPhanHoi = reprots.Count - 1;
            int total = 0;
            foreach(Reprots r in reprots)
            {
                if(r.TrangThai == "Mới")
                {
                    phanHoiMoi++;
                }
                if(r.TrangThai == "Đang xử lý")
                {
                    phanHoiDangSuLy++;
                }
                if(r.IdNguoiGiaiQuyet == logic.UserID)
                {
                    total += r.TongPhanHoi;
                }
            }

            danhGiaTrungBinh = (float)Convert.ToDecimal(total * 1.0 / tongPhanHoi);

            label10.Text = tongPhanHoi + "";
            label11.Text = phanHoiMoi + "";
            label12.Text = phanHoiDangSuLy + "";
            label13.Text = danhGiaTrungBinh + "";


            try
            {
                // Tạo DataTable và định nghĩa các cột
                DataTable displayTable = new DataTable();
                displayTable.Columns.Add("ID", typeof(int));
                displayTable.Columns.Add("Người dùng", typeof(int));
                displayTable.Columns.Add("Sản phẩm", typeof(int));
                displayTable.Columns.Add("Chủ đề", typeof(string));
                displayTable.Columns.Add("Nội dung", typeof(string));
                displayTable.Columns.Add("Loại phản hồi", typeof(string));
                displayTable.Columns.Add("Trạng thái", typeof(string));
                displayTable.Columns.Add("Tổng phản hồi", typeof(int));
                displayTable.Columns.Add("Ngày đăng", typeof(DateTime));
                displayTable.Columns.Add("Người giải quyết", typeof(int));

                // Đưa dữ liệu từ danh sách vào DataTable
                foreach (var report in reprots)
                {
                    displayTable.Rows.Add(
                        report.Id,
                        report.IdNguoiDung,
                        report.IdSanPham,
                        report.ChuDe,
                        report.NoiDung,
                        report.LoaiPhanHoi,
                        report.TrangThai,
                        report.TongPhanHoi,
                        report.NgayDang,
                        report.IdNguoiGiaiQuyet
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

                // Thêm cột button "Xem"
                DataGridViewButtonColumn xemButton = new DataGridViewButtonColumn
                {
                    HeaderText = "Xem",
                    Name = "Xem",
                    Text = "Xem",
                    UseColumnTextForButtonValue = true
                };
                dgv.Columns.Add(xemButton);

                // Thêm cột button "Phản hồi"
                DataGridViewButtonColumn phanhoiButton = new DataGridViewButtonColumn
                {
                    HeaderText = "Phản hồi",
                    Name = "PhanHoi",
                    Text = "Phản hồi",
                    UseColumnTextForButtonValue = true
                };
                dgv.Columns.Add(phanhoiButton);

                // Gán vào panel (ví dụ panel6)
                panel6.Controls.Clear();
                panel6.Controls.Add(dgv);

                // (Optional) Thêm sự kiện CellClick để xử lý nút
                dgv.CellClick += (sender, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        if (e.ColumnIndex == dgv.Columns["Xem"].Index)
                        {
                            int idReport = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["ID"].Value);
                            MessageBox.Show($"Xem chi tiết phản hồi ID: {idReport}");
                            // Hoặc mở form xem chi tiết
                        }
                        else if (e.ColumnIndex == dgv.Columns["PhanHoi"].Index)
                        {
                            int idReport = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["ID"].Value);
                            MessageBox.Show($"Thực hiện phản hồi cho ID: {idReport}");
                            // Hoặc mở form phản hồi
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hiển thị: {ex.Message}");
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
    }

}
