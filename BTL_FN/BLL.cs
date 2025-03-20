using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Text;

namespace BTL_FN
{
 
    
    

    public class BLL
    {
        private DAL dataAccess = new DAL();
        public bool isLogin = true;
        public string logo;
        public int UserID { get; set; }
        public  string UserActive { get; set; }
        public string UserRole { get; set; }


        public BLL()
        {
            App app = new App();
            logo = app.logo;
            UserActive = "Hoạt động";
            UserRole = "Người dùng";
        }
        // những vấn đề liên quan đến tài khoản 



        public bool Login(string username, string password)
        {
            DataTable userTable = dataAccess.GetDataTable("SELECT * FROM tk WHERE ten_dn = @Username AND mk = @Password", new Dictionary<string, object>
            {
                {"@Username", username},
                {"@Password", password}
            });

            if (userTable.Rows.Count > 0)
            {
                DataRow userRow = userTable.Rows[0];
                UserID = Convert.ToInt32(userRow["id"]);
                UserActive = userRow["TrangThai"].ToString();
                UserRole = userRow["vaiTro"].ToString();
                return true;
            }
            return false;
        }

        public bool AddAccount(Account a)
        {
            if (dataAccess.CheckExistence("tk", "email", a.Email) == 0)
            {
                return dataAccess.AddUser(a);
            }
            else
            {
                return false;
            }

        }

        public bool BanAccount(int id, string status, string role)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                if (role != "Quản trị viên" && id != UserID)
                {
                    return dataAccess.ExecuteNonQuery($"UPDATE tk SET trangThai = N'Bị chặn' WHERE id = @Id", new Dictionary<string, object> { { "@Id", id } });
                }
                else
                {
                    MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public bool DeleteAccount(int id, string role)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                if (role != "Quản trị viên" && id != UserID)
                {
                    return dataAccess.ExecuteNonQuery("DELETE FROM tk WHERE id = @Id", new Dictionary<string, object> { { "@Id", id } });
                }
                else
                {
                    MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public bool SetAdminAccount(int id, string role)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                if (role != "Quản trị viên" && id != UserID)
                {
                    return dataAccess.ExecuteNonQuery($"UPDATE account SET vaiTro = N'Quản trị viên' WHERE id = @Id", new Dictionary<string, object> { { "@Id", id } });
                }
                else
                {
                    MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

        }

        public List<Account> ListAccounts()
        {

            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.GetAllUsers();
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }


        public List<Account> FindUser(string ids, string uname, string uemail)
        {
            StringBuilder queryBuilder = new StringBuilder($"SELECT * FROM tk WHERE 1=1");
            if (!string.IsNullOrEmpty(ids))
            {
                queryBuilder.Append(" AND id = @Id");
            }
            if (!string.IsNullOrEmpty(uname))
            {
                queryBuilder.Append(" AND ten_dn LIKE @UserName");
            }
            if (!string.IsNullOrEmpty(uemail))
            {
                queryBuilder.Append(" AND email LIKE @Email");
            }
            return dataAccess.FindUser(queryBuilder.ToString(), ids, uname, uemail);
        }




        // vấn đề liên quan đến danh mục 
        public List<Category> ListCategory()
        {
            
            if (true)
            {
                return dataAccess.GetAllCategories();
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }
        public bool DeleteCategory(int id)
        {
            
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.DeleteCategory(id);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }


        public bool UpdateCategory(Category value)
        {
            
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.UpdateCategory(value);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public bool AddCategory(Category value)
        {
           
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.AddCategory(value);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        // những vấn đề liên quan đến hàng hóa 
        public Dictionary<int, object> ListProducts(string query = null)
        {

            List<Product> products = new List<Product>();
            DataTable dt = new DataTable(); 
            if (query == null)
            {
                dt = dataAccess.GetAllProducts();
                foreach (DataRow row in dt.Rows)
                {

                    try
                    {
                        Product product = new Product()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Name = row["tenSP"].ToString(),
                            Description = row["moTa"]?.ToString() ?? string.Empty,
                            Image = row["hinh"]?.ToString() ?? string.Empty,
                            Total = Convert.ToInt32(row["khoiLuong"] ?? 0),
                            Status = Convert.ToString(row["TrangThai"]),
                            Rating = Convert.ToInt32(row["danh_gia"]),
                            dateAdd = Convert.ToDateTime(row["ngay_tao"]),
                            Price = Convert.ToDecimal(row["gia"]),
                            TotalPay = Convert.ToInt32(row["da_ban"]),
                            CategoryId = Convert.ToInt32(row["dm"])
                        };
                        products.Add(product);
                    }
                    catch (Exception ex)
                    {
                        // Ghi log hoặc xử lý lỗi chuyển đổi từng dòng
                        MessageBox.Show($"Lỗi chuyển đổi dòng: {ex.Message}");
                    }
                }
            }
            else
            {
                dt = dataAccess.GetAllProducts(query);
                foreach (DataRow row in dt.Rows)
                {

                    try
                    {
                        Product product = new Product()
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Name = row["tenSP"].ToString(),
                            Description = row["moTa"]?.ToString() ?? string.Empty,
                            Image = row["hinh"]?.ToString() ?? string.Empty,
                            Total = Convert.ToInt32(row["khoiLuong"] ?? 0),
                            Status = Convert.ToString(row["TrangThai"]),
                            Rating = Convert.ToInt32(row["danh_gia"]),
                            dateAdd = Convert.ToDateTime(row["ngay_tao"]),
                            Price = Convert.ToDecimal(row["gia"]),
                            TotalPay = Convert.ToInt32(row["da_ban"]),
                            CategoryId = Convert.ToInt32(row["dm"])
                        };
                        products.Add(product);
                    }
                    catch (Exception ex)
                    {
                        // Ghi log hoặc xử lý lỗi chuyển đổi từng dòng
                        MessageBox.Show($"Lỗi chuyển đổi dòng: {ex.Message}");
                    }
                }
            }
            

            Dictionary<int, object> result = new Dictionary<int, object>();

            result.Add(1, products);
            result.Add(2, dt);
            result.Add(3, dataAccess.GetMaxProductId());

            return result;
        }
        // role admin
            // add
        public bool addProduct(Product value)
        {
            
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.AddProduct(value);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        // delete
        public bool deleteProduct(int id)
        {
            
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.DeleteProduct(id);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        // update 
        public bool updateProduct(Product value)
        {
            
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.UpdateProduct(value);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public List<Product> findProduct(string id, string name, int? categoryId)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT * FROM sp WHERE 1=1");

            if (!string.IsNullOrEmpty(id))
            {
                queryBuilder.Append(" AND [id] = @Id");
            }
            if (!string.IsNullOrEmpty(name))
            {
                queryBuilder.Append(" AND [tenSP] LIKE @Name");
            }
            if (categoryId.HasValue && categoryId != -1)
            {
                queryBuilder.Append(" AND [dm] = @CategoryId");
            }

            return dataAccess.FindProduct(queryBuilder.ToString(), id, name, categoryId);
        }


        // những vấn dề liên quan đến voucher 
        public Dictionary<int, object> ListVouchers()
        {
            List<Voucher> vouchers = new List<Voucher>();
            DataTable dt = dataAccess.GetAllVouchers();
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    Voucher voucher = new Voucher()
                    {
                        Id = row["VoucherID"] != DBNull.Value ? Convert.ToInt32(row["VoucherID"]) : 0,
                        voucherCode = row["maVoucher"]?.ToString(),
                        StartDate = row["NgayBatDau"] != DBNull.Value ? Convert.ToDateTime(row["NgayBatDau"]) : DateTime.MinValue,
                        EndDate = row["NgayKetThuc"] != DBNull.Value ? Convert.ToDateTime(row["NgayKetThuc"]) : DateTime.MinValue,
                        TypeOf = row["Loai"]?.ToString(),
                        ValueOfVoucher = row["GiaTri"] != DBNull.Value ? Convert.ToDecimal(row["GiaTri"]) : 0,
                        categoryId = row["DanhMucApDung"] != DBNull.Value ? Convert.ToInt32(row["DanhMucApDung"]) : 0,
                        categoryName = row["TenDanhMucApDung"]?.ToString(),
                        Status = row["TrangThai"]?.ToString(),
                        Description = row["MoTa"]?.ToString()
                    };

                    vouchers.Add(voucher);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi chuyển đổi dòng 1: {ex.Message}");
                }
            }

            Dictionary<int, object> result = new Dictionary<int, object>();
            result.Add(1, vouchers);
            result.Add(2, dt);

            return result;
        }


        public bool AddVoucher(Voucher value)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.AddVoucher(value);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public bool DeleteVoucher(int id)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.DeleteVoucher(id);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public bool UpdateVoucher(Voucher value)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.UpdateVoucher(value);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public List<Voucher> FindVoucher(string maVoucher, string loai, int? danhMucId)
        {
            StringBuilder queryBuilder = new StringBuilder(@"
                SELECT * 
                FROM vw_VoucherWithCategory 
                WHERE trangThaiXoa != N'Đã xóa' 
            ");

            // Thêm điều kiện tìm kiếm
            if (!string.IsNullOrEmpty(maVoucher))
            {
                queryBuilder.Append(" AND maVoucher LIKE @maVoucher");
            }
            if (!string.IsNullOrEmpty(loai))
            {
                queryBuilder.Append(" AND Loai LIKE @loai");
            }
            if (danhMucId.HasValue && danhMucId != 0)
            {
                queryBuilder.Append(" AND DanhMucApDung = @DanhMucApDung");
            }

            return dataAccess.FindVoucher(
                queryBuilder.ToString(),
                maVoucher?.Trim(),
                loai?.Trim(),
                danhMucId
            );
        }

        // thêm xóa sửa
        // role user
        // xem 
        // tìm kiếm 


        // những vấn đề liên quan đến mua hàng 
        public void GetAllOrders(ref List<Order> orders, string query = null)
        {

            orders = dataAccess.GetAllOrders("SELECT * FROM Orders");
        }

        

        public bool UpdateOrders(int OrdersId, string status)
            {
                return dataAccess.ApproveOrder(OrdersId, status);
           }

            public bool UpdateOrders(Order value)
            {
                if(value.Status == "Đang chờ" || value.Status == "Đã duyệt")
                {
                    return dataAccess.UpdateOrders(value);
                }

                return false;
            }
        public bool DeleteOrders(int OrdersId)
            {
                return dataAccess.DeleteOrder(OrdersId);
            }

        // những vấn đề liên quan đến thanh toán 
        public bool AddPayMethod(PaymentMethod p)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.AddPaymentMethod(p);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public void getAllPaymentMothod(ref List<PaymentMethod> pay)
        {
            pay = dataAccess.GetAllPaymentMethods();
        }

        public bool DeletePaymentMethod(int pay)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.DeletePaymentMethod(pay);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        
        public bool UpdatePayMethod(PaymentMethod pay)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                return dataAccess.UpdatePaymentMethod(pay);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        // trangg chủ 

        public void loadAdminDashboard(
            ref float tongDoanhThu, ref int tongDonHang, ref int tongSanPham, ref int tongKhachHang,
            ref List<Order> listNewOrder,
            ref List<Product> listNewProduct,
            ref List<Reprots> listNewReprots)
        {
            if (UserRole == "Quản trị viên" && UserActive == "Hoạt động")
            {
                dataAccess.loadAdminDashboard(ref tongDoanhThu, ref tongDonHang, ref tongSanPham, ref tongKhachHang, ref listNewOrder,ref listNewProduct, ref listNewReprots);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }


        // phản hồi 
        public void getAllReport(ref List<Reprots> reprots, string query = null)
        {
            if(query != null)
            {
                reprots = dataAccess.getAllReport(query);
            }
            else
            {
                reprots = dataAccess.getAllReport();
            }

        }



        public List<ThongTinDiaChiNguoiDung> getTTND()
        {
            List<ThongTinDiaChiNguoiDung> l = dataAccess.GetThongTinDiaChiNguoiDung(UserID);

            return l;
        }
    }
}
