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

    

    public partial class admin : Form
    {
        

        public BLL logic = new BLL();
        public bool comboBox1Statte = false;



        public List<Order> Orders = new List<Order>();
        public List<Order> selected = new List<Order>();

        public admin()
        {
            InitializeComponent();
        }

        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }


        private void admin_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Danh mục quản lý");
            comboBox1.SelectedIndex = 0;
            pictureBox2.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;


            LoadData();
            LoadAdminData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!comboBox1Statte)
            {
                comboBox1Statte = true;
                comboBox1.Items.Add("Quản lý sản phẩm");
                comboBox1.Items.Add("Quản lý danh mục");
                comboBox1.Items.Add("Quản lý voucher");
                comboBox1.SelectedIndex = 1;
            }

            Product_v p = new Product_v(); // Khởi tạo form
            p.TopLevel = false;           // Không cho phép form là cửa sổ chính
            p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
            p.Dock = DockStyle.Fill;      // Giãn đầy panel
            panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
            panel2.Controls.Add(p);       // Thêm form con vào panel
            p.Show();                    // Hiển thị form
        }

        // quản lý tài khoản
        private void button2_Click(object sender, EventArgs e)
        {
            account a = new account();
            a.TopLevel = false;
            a.FormBorderStyle = FormBorderStyle.None;
            a.Dock = DockStyle.Fill;
            panel2.Controls.Clear();
            panel2.Controls.Add(a);
            a.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex != 0)
            {
                if(comboBox1.SelectedIndex == 1)
                {
                    Product_v p = new Product_v(); // Khởi tạo form
                    p.TopLevel = false;           // Không cho phép form là cửa sổ chính
                    p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                    p.Dock = DockStyle.Fill;      // Giãn đầy panel
                    panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                    panel2.Controls.Add(p);       // Thêm form con vào panel
                    p.Show();
                }
                else if(comboBox1.SelectedIndex == 2)
                {
                    category c = new category();
                    c.TopLevel = false;           // Không cho phép form là cửa sổ chính
                    c.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                    c.Dock = DockStyle.Fill;      // Giãn đầy panel
                    panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                    panel2.Controls.Add(c);       // Thêm form con vào panel
                    c.Show();
                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    voucher v = new voucher();
                    v.TopLevel = false;           // Không cho phép form là cửa sổ chính
                    v.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                    v.Dock = DockStyle.Fill;      // Giãn đầy panel
                    panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                    panel2.Controls.Add(v);       // Thêm form con vào panel
                    v.Show();
                }
            }
        }
        // danh mục đơn hàng đang chờ
        private void button6_Click(object sender, EventArgs e)
        {

            comboBox1.Controls.Clear();

            // thêm các giá trị vào combox 
            comboBox1.Items.Add("Quản lý đơn hàng");
            comboBox1.Items.Add("Quản lý thanh toán");
            comboBox1.SelectedIndex = 1;

            // hiện thị danh mục đơn hàng
            if (comboBox1.SelectedIndex == 1)
            {
                Admin.Orders p = new Admin.Orders("Đang chờ"); // Khởi tạo form
                p.TopLevel = false;           // Không cho phép form là cửa sổ chính
                p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                p.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(p);       // Thêm form con vào panel
                p.Show();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                Admin.PaymentMethods c = new Admin.PaymentMethods();
                c.TopLevel = false;           // Không cho phép form là cửa sổ chính
                c.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                c.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(c);       // Thêm form con vào panel
                c.Show();
            }

        }
        // đơn hàng đang chuẩn bị hàng và chờ được giao
        private void button7_Click(object sender, EventArgs e)
        {
            comboBox1.Controls.Clear();

            // thêm các giá trị vào combox 
            comboBox1.Items.Add("Quản lý đơn hàng");
            comboBox1.Items.Add("Quản lý thanh toán");
            comboBox1.SelectedIndex = 1;

            // hiện thị danh mục đơn hàng
            if (comboBox1.SelectedIndex == 1)
            {
                Admin.Orders p = new Admin.Orders("Đang chuẩn bị"); // Khởi tạo form
                p.TopLevel = false;           // Không cho phép form là cửa sổ chính
                p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                p.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(p);       // Thêm form con vào panel
                p.Show();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                Admin.PaymentMethods c = new Admin.PaymentMethods();
                c.TopLevel = false;           // Không cho phép form là cửa sổ chính
                c.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                c.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(c);       // Thêm form con vào panel
                c.Show();
            }
        }
        // đơn hàng đang được giao
        private void button8_Click(object sender, EventArgs e)
        {
            comboBox1.Controls.Clear();

            // thêm các giá trị vào combox 
            comboBox1.Items.Add("Quản lý đơn hàng");
            comboBox1.Items.Add("Quản lý thanh toán");
            comboBox1.SelectedIndex = 1;

            // hiện thị danh mục đơn hàng
            if (comboBox1.SelectedIndex == 1)
            {
                Admin.Orders p = new Admin.Orders("Đã giao"); // Khởi tạo form
                p.TopLevel = false;           // Không cho phép form là cửa sổ chính
                p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                p.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(p);       // Thêm form con vào panel
                p.Show();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                Admin.PaymentMethods c = new Admin.PaymentMethods();
                c.TopLevel = false;           // Không cho phép form là cửa sổ chính
                c.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                c.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(c);       // Thêm form con vào panel
                c.Show();
            }
        }
        // đơn hàng đã hoàn thành

        private void button9_Click(object sender, EventArgs e)
        {
            comboBox1.Controls.Clear();

            // thêm các giá trị vào combox 
            comboBox1.Items.Add("Quản lý đơn hàng");
            comboBox1.Items.Add("Quản lý thanh toán");
            comboBox1.SelectedIndex = 1;

            // hiện thị danh mục đơn hàng
            if (comboBox1.SelectedIndex == 1)
            {
                Admin.Orders p = new Admin.Orders("Hoàn thành"); // Khởi tạo form
                p.TopLevel = false;           // Không cho phép form là cửa sổ chính
                p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                p.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(p);       // Thêm form con vào panel
                p.Show();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                Admin.PaymentMethods c = new Admin.PaymentMethods();
                c.TopLevel = false;           // Không cho phép form là cửa sổ chính
                c.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                c.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(c);       // Thêm form con vào panel
                c.Show();
            }
        }
        // đơn hàng bị hủy bởi khách hàng
        private void button10_Click(object sender, EventArgs e)
        {
            comboBox1.Controls.Clear();

            // thêm các giá trị vào combox 
            comboBox1.Items.Add("Quản lý đơn hàng");
            comboBox1.Items.Add("Quản lý thanh toán");
            comboBox1.SelectedIndex = 1;

            // hiện thị danh mục đơn hàng
            if (comboBox1.SelectedIndex == 1)
            {
                Admin.Orders p = new Admin.Orders("Đã hủy"); // Khởi tạo form
                p.TopLevel = false;           // Không cho phép form là cửa sổ chính
                p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                p.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(p);       // Thêm form con vào panel
                p.Show();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                Admin.PaymentMethods c = new Admin.PaymentMethods();
                c.TopLevel = false;           // Không cho phép form là cửa sổ chính
                c.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                c.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(c);       // Thêm form con vào panel
                c.Show();
            }
        }
        // các mặc hàng sắp hết hàng 
        private void button11_Click(object sender, EventArgs e)
        {
            

        }
        // quản lý đơn hàng
        private void button3_Click(object sender, EventArgs e)
        {
            
            comboBox1.Items.Clear();

            // thêm các giá trị vào combox 
            comboBox1.Items.Add("Quản lý đơn hàng");
            comboBox1.Items.Add("Quản lý thanh toán");
            comboBox1.SelectedIndex = 0;

            // hiện thị danh mục đơn hàng
            if (comboBox1.SelectedIndex == 1)
            {
                Admin.Orders p = new Admin.Orders(); // Khởi tạo form
                p.TopLevel = false;           // Không cho phép form là cửa sổ chính
                p.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                p.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(p);       // Thêm form con vào panel
                p.Show();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                Admin.PaymentMethods c = new Admin.PaymentMethods();
                c.TopLevel = false;           // Không cho phép form là cửa sổ chính
                c.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                c.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(c);       // Thêm form con vào panel
                c.Show();
            }
        }
        // quản lý phản hồi
        private void button4_Click(object sender, EventArgs e)
        {
            Admin.Reprot reprot = new Admin.Reprot();
            reprot.TopLevel = false;           // Không cho phép form là cửa sổ chính
            reprot.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
            reprot.Dock = DockStyle.Fill;      // Giãn đầy panel
            panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
            panel2.Controls.Add(reprot);       // Thêm form con vào panel
            reprot.Show();
        }
        // quản lý doanh thu - yêu cầu phân tích dữ liệu
        private void button5_Click(object sender, EventArgs e)
        {
            
        }
        // trờ lại sau khi thao tác với dữ liệu
        private void button12_Click(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            LoadData();
            LoadAdminData();
        }

        public void LoadData()
        {
            try
            {
                
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void LoadAdminData()
        {
            if(true)
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
            panel2.Controls.Clear();
            Label labelNoData = new Label
            {
                AutoSize = true,
                Location = new Point(300, 200),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Text = "Không tồn tại danh mục"
            };
            panel2.Controls.Add(labelNoData);
        }

        public void DisplayOrders()
        {
            try
            {
                Admin.AdminProgram program = new Admin.AdminProgram();
                program.TopLevel = false;   // Không cho phép form là cửa sổ chính
                program.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
                program.Dock = DockStyle.Fill;      // Giãn đầy panel
                panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
                panel2.Controls.Add(program);       // Thêm form con vào panel
                program.Show();
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

        private void label7_Click(object sender, EventArgs e)
        {
            LoadData();
            LoadAdminData();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            logic.isLogin = false;
            logic.UserID = -1;
            logic.UserRole = null;
            logic.UserActive = null;
        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Admin.AdminProgram program = new Admin.AdminProgram();
            program.TopLevel = false;   // Không cho phép form là cửa sổ chính
            program.FormBorderStyle = FormBorderStyle.None; // Loại bỏ viền form
            program.Dock = DockStyle.Fill;      // Giãn đầy panel
            panel2.Controls.Clear();      // Xóa các controls cũ trong panel2 (nếu có)
            panel2.Controls.Add(program);       // Thêm form con vào panel
            program.Show();
        }
    }
}
