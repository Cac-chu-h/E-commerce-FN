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
    public partial class setting : Form
    {
        public setting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            LoadFormToPanel(new Form1());
        }

        private void LoadFormToPanel(Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panel2.Controls.Clear();
            panel2.Controls.Add(form);
            form.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            LoadFormToPanel(new Form2());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            LoadFormToPanel(new Form3());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            LoadFormToPanel(new Form4());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel2.Controls.Clear();
            LoadFormToPanel(new Form5());
        }
    }
}
