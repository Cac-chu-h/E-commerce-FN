using BTL_FN;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public BLL logic = new BLL();
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
        }

        private void LoadImages()
        {
            string logoPath = @"E:\C#\logo.jpg";
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

            // Tải thông tin địa chỉ người dùng
            LoadUserAddresses();

            // Cài đặt giá trị mặc định
            SetDefaultValues();

            // Cập nhật giao diện dựa trên trạng thái
            UpdateUIBasedOnState();
        }

        private void LoadProductImage()
        {
            try
            {
                pictureBox1.Image = File.Exists(product.Image)
                    ? Image.FromFile(product.Image)
                    : Image.FromFile(@"E:\C#\logo.jpg");
            }
            catch
            {
                pictureBox1.Image = Image.FromFile(@"E:\C#\logo.jpg");
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

            // Thêm các voucher phù hợp với categoryId vào combobox
            if (v.Count > 0)
            {
                var matchingVouchers = v.Where(vou => product.CategoryId == vou.categoryId).ToList();
                if (matchingVouchers.Any())
                {
                    comboBox2.Items.AddRange(matchingVouchers.Select(vou => vou.voucherCode).ToArray());
                }
            }
        }

        private void LoadUserAddresses()
        {
            d = logic.getTTND();

            if (d.Count > 0)
            {
                // Thêm địa chỉ người dùng vào combobox
                comboBox1.Items.AddRange(d.Select(ds => ds.FullAddress).ToArray());

                // Hiển thị thông tin địa chỉ mặc định
                label14.Text = d[0].FullName;
                label15.Text = d[0].Contact;
                label16.Text = d[0].FullAddress;
            }

            // Hiển thị thông tin voucher mặc định nếu có
            if (v.Count > 0)
            {
                label17.Text = "#" + v[0].voucherCode;
                label20.Text = (v[0].ValueOfVoucher * product.Price).ToString();
            }
        }

        private void SetDefaultValues()
        {
            // Đặt index mặc định cho các combobox
            comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;
            comboBox2.SelectedIndex = comboBox2.Items.Count > 0 ? 0 : -1;
            comboBox3.SelectedIndex = comboBox3.Items.Count > 0 ? 0 : -1;
        }

        private void UpdateUIBasedOnState()
        {
            // Hiển thị/ẩn button dựa trên trạng thái
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
            // Chỉ xử lý khi có dữ liệu và combobox đã chọn một giá trị khác mặc định
            if (p.Count <= 0 || v.Count <= 0 || comboBox2.SelectedIndex <= 0)
                return;

            string selectedVoucherCode = comboBox2.Text;

            // Tìm voucher phù hợp với categoryId và voucherCode
            Voucher matchingVoucher = v.FirstOrDefault(vou =>
                product.CategoryId == vou.categoryId &&
                selectedVoucherCode == vou.voucherCode);

            // Cập nhật UI
            label17.Text = "#" + selectedVoucherCode;

            if (matchingVoucher != null)
            {
                label20.Text = (matchingVoucher.ValueOfVoucher * product.Price).ToString();
            }
            else
            {
                label20.Text = "0";
            }
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
            label23.Text = totalPrice.ToString() + "đ";
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
            if(state == 0)
            {

            }
        }
        private void button11_Click(object sender, EventArgs e) { }
        private void button2_Click_1(object sender, EventArgs e) { }
    }
}