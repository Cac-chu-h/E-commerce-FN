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
   
    public partial class frmHoaDon: Form
    {
        private readonly BLL logic = BLL.Instance;
        private int orderId;
        public frmHoaDon(int orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
            LoadHoaDon();
        }
        private void LoadHoaDon() {
            try
            {
                MessageBox.Show($"OrderID: {orderId}"); // Kiểm tra giá trị orderId
                // Tạo instance của Crystal Report
                CrystalReport1 invoiceReport = new CrystalReport1(); // Thay bằng tên class report của bạn

                // Lấy dữ liệu từ stored procedure
                
                DataTable orderDetails = logic.GetOrderDetailsForHoaDon(orderId);
               
                if (orderDetails.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy thông tin đơn hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
                string dataPreview = "Dữ liệu trả về:\n";
                foreach (DataRow row in orderDetails.Rows)
                {
                    dataPreview += $"ProductName: {row["ProductName"]}, ProductID: {row["ProductID"]}, Quantity: {row["Quantity"]}, Price: {row["Price"]}, Discount: {row["Discount"]}, Total: {row["Total"]}, CustomerName: {row["CustomerName"]}, CustomerPhone: {row["CustomerPhone"]}\n";
                }
                MessageBox.Show(dataPreview, "Dữ liệu trả về từ stored procedure");
                // Gán dữ liệu cho report
                invoiceReport.SetDataSource(orderDetails);



                // Gán report vào CrystalReportViewer
                crystalReportViewer1.ReportSource = invoiceReport;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
    }
}
