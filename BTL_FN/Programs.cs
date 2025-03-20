using BTL_User;
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
    public partial class Programs : Form
    {
        public BLL logic = new BLL();

        private Dictionary<Type, Form> _openForms = new Dictionary<Type, Form>();
        public Programs()
        {
            InitializeComponent();
            IsMdiContainer = true;
        }

        private void Programs_Load(object sender, EventArgs e)
        {
                
        }

        private void ShowForm<T>(Func<T> createForm, bool requireLogin = false) where T : Form
        {
            if (requireLogin && !logic.isLogin)
            {
                MessageBox.Show("Bạn cần đăng nhập để xem mục này!", "Thông báo",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Type formType = typeof(T);
            if (_openForms.TryGetValue(formType, out Form existingForm))
            {
                if (existingForm.IsDisposed)
                {
                    _openForms.Remove(formType);
                }
                else
                {
                    existingForm.BringToFront();
                    return;
                }
            }

            T newForm = createForm();
            newForm.MdiParent = this;
            newForm.FormClosed += (s, e) => _openForms.Remove(formType);
            _openForms.Add(formType, newForm);
            newForm.Show();
        }
        private void trangChủToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logic.isLogin)
            {
                if ( logic.UserRole == "Người dùng")
                {
                    ShowForm<TrangChu>(() => new TrangChu(this));
                }
                else
                {
                    ShowForm<admin>(() => new admin(this));
                }
            }else
            {
                TrangChu tc = new TrangChu(this)
                {
                    MdiParent = this
                };
                tc.Show();
            }

        }

        private void giỏHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm<gioHang>(() => new gioHang(), true);
        }

        private void đăngNhậpĐăngKíToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // ShowForm<DangNhapDangKy>(() => new DangNhapDangKy());
        }
    }
}
