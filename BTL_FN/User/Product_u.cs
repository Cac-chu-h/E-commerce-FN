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
    public partial class Product_u : Form
    {
        public BLL logic = new BLL();
        public Product p = new Product();
        List<Product> products = new List<Product>();


        public Product_u(Product p, List<Product> products) 
        {
            this.p = p;
            this.products = products;
            InitializeComponent();
        }

        private void Product_u_Load(object sender, EventArgs e)
        {
            pictureBox2.Image = Image.FromFile(@"E:\C#\logo.jpg");
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }
    }
}
