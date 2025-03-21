using BTL_FN;
using CrystalDecisions.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTL_User
{
    public partial class thanhToan : Form
    {
        public Product product = new Product();
        public List<Voucher> v = new List<Voucher>();
        public List<PaymentMethod> p = new List<PaymentMethod>();
        public List<ThongTinDiaChiNguoiDung> d = new List<ThongTinDiaChiNguoiDung>();
        public BLL logic => BLL.Instance;
        public int state = 0;

        public thanhToan(Product p, int state, Programs pss)
        {
            this.state = state;
            this.product = p;
            InitializeComponent();
            this.MdiParent = pss;
        }

        private void thanhToan_Load(object sender, EventArgs e)
        {
            LoadImages();
            LoadProduct();


            if (state == 0)
            {
                button10.Text = "Thêm";
            }
        }

        private void LoadImages()
        {
            string logoPath = @"E:\Anhwdf\logo2.jpg";
            try
            {
                pictureBox2.Image = Image.FromFile(logoPath);
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = Image.FromFile(logoPath);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể tải hình ảnh: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProduct()
        {
            // Tải hình ảnh sản phẩm
            LoadProductImage();

            // Cập nhật thông tin sản phẩm
            UpdateProductInfo();

            // Tải danh sách voucher và phương thức thanh toán
            LoadVouchersAndPaymentMethods();

        }

        private void LoadProductImage()
        {
            try
            {
                pictureBox1.Image = File.Exists(product.Image)
                    ? Image.FromFile(product.Image)
                    : Image.FromFile(@"E:\Anhwdf\logo2.jpg");
            }
            catch
            {
                pictureBox1.Image = Image.FromFile(@"E:\Anhwdf\logo2.jpg");
            }
        }

        private void UpdateProductInfo()
        {
            label1.Text = product.Name;
            label4.Text = "#" + product.Id;
            label3.Text = $"(Nhỏ hơn: {product.Total}KG)";
            label9.Text = $"Giá bán: {product.Price}đ/kg";
        }

        private void LoadVouchersAndPaymentMethods()
        {
            // Lấy danh sách voucher
            Dictionary<int, Object> value = logic.ListVouchers();
            v = value[1] as List<Voucher>;

            // Lấy danh sách phương thức thanh toán
            logic.getAllPaymentMothod(ref p);

            // Thêm các phương thức thanh toán vào combobox
            if (p.Count > 0)
            {
                comboBox3.Items.AddRange(p.Select(pay => pay.Name).ToArray());
            }

        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex > 0)
            {
                label33.Text = comboBox3.Text;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Xử lý khi textbox trống
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                ResetQuantityDisplay();
                return;
            }

            // Kiểm tra giá trị nhập vào là số
            if (!decimal.TryParse(textBox2.Text, out decimal quantity))
            {
                MessageBox.Show("Nhập giá trị số nguyên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetQuantityDisplay();
                return;
            }

            // Kiểm tra giá trị nhập vào không vượt quá tổng số lượng
            if (quantity >= product.Total)
            {
                textBox2.Text = "";
                textBox2.Focus();
                ResetQuantityDisplay();
                return;
            }

            // Cập nhật hiển thị
            UpdateQuantityDisplay(quantity);
        }

        private void ResetQuantityDisplay()
        {
            label24.Text = "0";
            label23.Text = "0đ";
            label29.Text = "0đ";
        }

        private void UpdateQuantityDisplay(decimal quantity)
        {
            decimal totalPrice = quantity * product.Price;
            label24.Text = quantity.ToString();
            label23.Text = totalPrice.ToString()+"";
            label29.Text = label23.Text;
        }

        // Các phương thức khác giữ nguyên
        private void label12_Click(object sender, EventArgs e) { }
        private void label18_Click(object sender, EventArgs e) { }
        private void button1_Click(object sender, EventArgs e) { }
        private void button2_Click(object sender, EventArgs e) { }
        private void button10_Click(object sender, EventArgs e) 
        {
            bool inHoaDon = (MessageBox.Show("Bạn có muốn in hóa đơn không!", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes);
            if (!decimal.TryParse(textBox2.Text, out decimal quantity))
            {
                MessageBox.Show("Nhập giá trị số nguyên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetQuantityDisplay();
                return;
            }

            if(state != 0)
            {
                Order od = new Order()
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = Convert.ToDecimal(label23.Text),
                    Status = "Chờ xử lý",
                    AccountId = logic.UserID,
                    UserID = 1,
                    PaymentMethodID = 4,
                    LastUpdated = DateTime.Now,
                    AddressId = 46
                };

                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductID = product.Id,
                    Price = product.Price, 
                    Discount = (decimal)0.15, 
                    Quantity = Convert.ToInt32(textBox2.Text),
                    Total = Convert.ToInt32(textBox2.Text) * product.Price,
                };
                int newOrderID = logic.addOrders(od, orderDetail);
                if (newOrderID !=0)
                {
                    MessageBox.Show("Thêm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (inHoaDon)
                {
                    frmHoaDon inhoadon = new frmHoaDon(newOrderID);
                    inhoadon.ShowDialog();
                }
            }
        }

       

        private void button11_Click(object sender, EventArgs e) { }
        private void button2_Click_1(object sender, EventArgs e) { }
    }
}