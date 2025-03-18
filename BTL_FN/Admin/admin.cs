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



        private void admin_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Danh mục quản lý");
            comboBox1.SelectedIndex = 0;
            pictureBox2.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;


            LoadData();
            LoadAdminData();
        }

    // Khai báo biến trạng thái (nếu cần)
    private bool comboBox1State = false;

    // Xử lý sự kiện click button1
    private void button1_Click(object sender, EventArgs e)
    {
        // Làm mới ComboBox
        comboBox1.Items.Clear();
        comboBox1.Items.Add("Chọn chức năng");
        comboBox1.Items.Add("Quản lý sản phẩm");
        comboBox1.Items.Add("Quản lý danh mục");
        comboBox1.Items.Add("Quản lý voucher");
        comboBox1.SelectedIndex = 1; // Mặc định chọn "Quản lý sản phẩm"

        // Load form mặc định
        LoadFormToPanel(new Product_v());
    }

    // Xử lý sự kiện click button2
    private void button2_Click(object sender, EventArgs e)
    {
        // Làm mới ComboBox
        comboBox1.Items.Clear();
        comboBox1.Items.Add("Chọn chức năng");
        comboBox1.Items.Add("Quản lý tài khoản");
        comboBox1.SelectedIndex = 1;

        // Load form mặc định
        LoadFormToPanel(new account());
    }

    // Xử lý sự kiện click button3
    private void button3_Click(object sender, EventArgs e)
    {
        // Làm mới ComboBox
        comboBox1.Items.Clear();
        comboBox1.Items.Add("Chọn chức năng");
        comboBox1.Items.Add("Quản lý đơn hàng");
        comboBox1.Items.Add("Quản lý thanh toán");
        comboBox1.SelectedIndex = 1;

        // Load form mặc định
        LoadFormToPanel(new Admin.Orders());
    }

    // Xử lý sự kiện thay đổi lựa chọn trong ComboBox
    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (comboBox1.SelectedIndex > 0) // Bỏ qua mục "Chọn chức năng"
        {
            switch (comboBox1.Text)
            {
                case "Quản lý sản phẩm":
                    LoadFormToPanel(new Product_v());
                    break;
                case "Quản lý danh mục":
                    LoadFormToPanel(new category());
                    break;
                case "Quản lý voucher":
                    LoadFormToPanel(new voucher());
                    break;
                case "Quản lý tài khoản":
                    LoadFormToPanel(new account());
                    break;
                case "Quản lý đơn hàng":
                    LoadFormToPanel(new Admin.Orders());
                    break;
                case "Quản lý thanh toán":
                    LoadFormToPanel(new Admin.PaymentMethods());
                    break;
            }
        }
    }

    // Phương thức helper để load form vào panel
    private void LoadFormToPanel(Form form)
    {
        form.TopLevel = false;
        form.FormBorderStyle = FormBorderStyle.None;
        form.Dock = DockStyle.Fill;
        panel2.Controls.Clear();
        panel2.Controls.Add(form);
        form.Show();
    }
        // quản lý phản hồi
        private void button4_Click(object sender, EventArgs e)
        {
          
            // Làm mới ComboBox
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Chọn chức năng");
            comboBox1.Items.Add("Quản lý tài khoản");
            comboBox1.SelectedIndex = 1;

            // Load form mặc định
            LoadFormToPanel(new Admin.Reprot());
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
            LoadFormToPanel(new Admin.setting());
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
