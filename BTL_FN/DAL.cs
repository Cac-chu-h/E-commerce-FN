using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;

namespace BTL_FN
{
    class App
    {
        public string logo = @"E:\Anhwdf\logo2.jpg";
    }

    public class ThongTinDiaChiNguoiDung
    {
        public string FullName { get; set; }
        public string Contact { get; set; }
        public string FullAddress { get; set; }
    }

    public class OrderDetail
    {
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int VoucherID { get; set; } // Nếu không có voucher, có thể đặt giá trị 0
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Total { get; set; }
        public int TotalPay { get; set; }
        public string Status { get; set; }
        public int Rating { get; set; }
        public DateTime dateAdd { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
    }
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; } // Thiết lập quan hệ
    }
    public class Account
    {
        public int Id { get; set; }                // [id]
        public string Username { get; set; }       // [ten_dn]
        public string Password { get; set; }       // [mk]
        public string Email { get; set; }          // [email]
        public string Avatar { get; set; }         // [hinh]
        public string Role { get; set; }           // [vaiTro]
        public DateTime CreatedDate { get; set; }  // [ngayTao]
        public DateTime LastUpdated { get; set; }  // [ngayCapNhatCuoi]
        public string Status { get; set; }         // [trangThai]
    }

    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
        public int AccountId { get; set; }
        public int UserID { get; set; }
        public int PaymentMethodID { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public string ShippingProvider { get; set; }
        public string TrackingNumber { get; set; }
        public int? VoucherId { get; set; }
        public DateTime LastUpdated { get; set; }
        public int AddressId { get; set; }

        public string tenNguoiDat { get; set; }
    }
    public class Voucher
    {
        public int Id { get; set; }             // Mã voucher
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; }   // Ngày kết thúc
        public string TypeOf { get; set; }         // Mã danh mục (tham chiếu category)
        public decimal ValueOfVoucher { get; set; } // Giá trị voucher
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string voucherCode { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }

    public class Reprots
    {
        public int Id { get; set; }
        public int IdNguoiDung { get; set; }
        public int IdSanPham { get; set; }
        public string NoiDung { get; set; }
        public int TongPhanHoi { get; set; }
        public string LoaiPhanHoi { get; set; }   // VD: "Đánh giá", "Phản hồi", "Khiếu nại"
        public string TrangThai { get; set; }     // VD: "Đã giải quyết", "Đang xử lý"
        public string ChuDe { get; set; }         // Chủ đề của phản hồi
        public DateTime NgayDang { get; set; }
        public int IdNguoiGiaiQuyet { get; set; }
    }

    // Cập nhật class Order để khớp với cấu trúc bảng Orders











    class DAL
    {
        private static readonly string connectionString = @"Data Source=DESKTOP-LN6KH37;Initial Catalog=btltest;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        private SqlConnection connection;

        public DAL()
        {
            connection = new SqlConnection(connectionString);
        }

        private void OpenConnection()
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void CloseConnection()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        public bool ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    foreach (var param in parameters)
                        cmd.Parameters.AddWithValue(param.Key, param.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                CloseConnection();
            }
        }

        // quản lý vấn đề tài khoản

        public bool DangKyTaiKhoan(string tenDN, string matKhau, string email)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra tên đăng nhập tồn tại
                    string checkQuery = "SELECT COUNT(*) FROM [BTL].[dbo].[tk] WHERE [ten_dn] = @ten_dn";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@ten_dn", tenDN);
                        int existingUser = (int)checkCmd.ExecuteScalar();
                        if (existingUser > 0)
                        {
                            MessageBox.Show("Tên đăng nhập đã tồn tại!");
                            return false;
                        }
                    }


                    // Câu truy vấn INSERT
                    string insertQuery = @"
                    INSERT INTO [BTL].[dbo].[tk] 
                    (
                        [ten_dn], 
                        [mk], 
                        [email], 
                        [hinh], 
                        [vaiTro], 
                        [ngayTao], 
                        [ngayCapNhatCuoi], 
                        [trangThai]
                    )
                    VALUES 
                    (
                        @ten_dn, 
                        @mk, 
                        @email, 
                        N'E:\Anhwdf\logo2.jpg', 
                        N'Người dùng', 
                        GETDATE(), 
                        GETDATE(), 
                        N'Hoạt động'
                    )";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ten_dn", tenDN);
                        cmd.Parameters.AddWithValue("@mk", matKhau); // Lưu password đã hash
                        cmd.Parameters.AddWithValue("@email", email);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi đăng ký: {ex.Message}");
                return false;
            }
        }

         public List<Account> GetAllUsers()
        {
            List<Account> userList = new List<Account>();
            try
            {
                OpenConnection();
                string query = "SELECT * FROM tk";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Account user = new Account()
                            {
                                Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                                Username = reader.IsDBNull(reader.GetOrdinal("ten_dn")) ? string.Empty : reader.GetString(reader.GetOrdinal("ten_dn")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? string.Empty : reader.GetString(reader.GetOrdinal("email")),
                                Role = reader.IsDBNull(reader.GetOrdinal("vaiTro")) ? string.Empty : reader.GetString(reader.GetOrdinal("vaiTro")),
                                Status = reader.IsDBNull(reader.GetOrdinal("trangThai")) ? string.Empty : reader.GetString(reader.GetOrdinal("trangThai")),
                                CreatedDate = reader.IsDBNull(reader.GetOrdinal("ngayTao")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("ngayTao"))
                            };
                            userList.Add(user);
                        }
                    }
                }
                return userList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] 4 {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public DataTable GetDataTable(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] 3 {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public bool AddUser(Account a)
        {
            string query = @"INSERT INTO [tk]
            (ten_dn, mk, email, hinh, vaiTro, trangThai, ngayTao)
            VALUES (@Username, @Password, @Email, @image, @Role, @Status, @CreatedDate)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Username", a.Username },
                    { "@Password", a.Password },
                    { "@Email", a.Email },
                    { "@image", new BLL().logo },
                    { "@Role", "Người dùng" },
                    { "@Status", "Hoạt động" },
                    { "@CreatedDate", DateTime.Now }
                };

            return ExecuteNonQuery(query, parameters);
        }




        public bool BanUser(int userId) => ExecuteNonQuery("UPDATE tk SET trangThai = 0 WHERE i_Id = @UserId", new Dictionary<string, object> { { "@UserId", userId } });

        public bool DeleteUser(int userId) => ExecuteNonQuery("DELETE FROM tk WHERE id = @UserId", new Dictionary<string, object> { { "@UserId", userId } });

        public bool PromoteToAdmin(int userId) => ExecuteNonQuery("UPDATE tk SET s_Role = 'Admin' WHERE i_Id = @UserId", new Dictionary<string, object> { { "@UserId", userId } });




        public int CheckExistence(string table, string column, string value)
        {
            try
            {
                OpenConnection();
                string query = $"SELECT COUNT(*) FROM {table} WHERE {column} = @Value";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Value", value);
                    return (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] 2 {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                CloseConnection();
            }
            return 0;
        }

        public List<Account> FindUser(string query, string ids, string uname, string uemail)
        {
            List<Account> userList = new List<Account>();
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(ids))
                    {
                        cmd.Parameters.AddWithValue("@Id", ids);
                    }
                    if (!string.IsNullOrEmpty(uname))
                    {
                        cmd.Parameters.AddWithValue("@UserName", $"%{uname}%");
                    }
                    if (!string.IsNullOrEmpty(uemail))
                    {
                        cmd.Parameters.AddWithValue("@Email", $"%{uemail}%");
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Account user = new Account()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                Username = reader.GetString(reader.GetOrdinal("ten_dn")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                Role = reader.GetString(reader.GetOrdinal("vaiTro")),
                                Status = reader.GetString(reader.GetOrdinal("trangThai")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("ngayTao"))
                            };
                            userList.Add(user);
                        }
                    }
                }
                return userList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] 1 {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }



        // quản lý vấn đề hàng hóa 
        // quản lý danh mục hàng 
        // thêm một danh mục mới 
        // thiết lập quan hệ giữa các danh mục 
        // xóa 
        // chỉnh sửa 
        // lấy ra tất cả danh mục 
        // tìm kiếm 
        public bool AddCategory(Category category) =>
           ExecuteNonQuery("INSERT INTO dm (tenDanhMuc, moTa, danhMucCha) VALUES (@categoryName, @mota, @ParentId)",
           new Dictionary<string, object>
           {
                { "@categoryName", category.Name },
                { "@mota", category.Description },
                { "@ParentId", category.ParentId }
           });



        public bool DeleteCategory(int id) => ExecuteNonQuery(
                "UPDATE dm SET trangThai = N'Ẩn' WHERE id = @Id",
                new Dictionary<string, object> { { "@Id", id } }
            );

        public bool UpdateCategory(Category category) => ExecuteNonQuery(
            "UPDATE dm SET tenDanhMuc = @valueA, moTa = @valueB, danhMucCha = @valueC WHERE id = @Id",
            new Dictionary<string, object>
            {
                { "@Id", category.Id },
                { "@valueA", category.Name },
                { "@valueB", category.Description },
                { "@valueC", category.ParentId }
            });

        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            try
            {
                OpenConnection();
                string query = "SELECT id, tenDanhMuc, moTa, danhMucCha FROM dm WHERE trangThai != N'Ẩn'";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                            Name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            ParentId = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }
            return categories;
        }



        // quản lý sản phẩm 
        // thêm sản phẩm 
        // xửa 
        // lấy ra danh sách sản phẩm 
        // xóa 
        // tìm kiếm 
        public bool AddProduct(Product product) => ExecuteNonQuery(
            "INSERT INTO sp (tenSP, moTa, hinh, khoiLuong, TrangThai, danh_gia, ngay_tao, ngay_cap_nhat, gia, da_ban, dm) " +
            "VALUES (@Name, @Description, @ProductImage, @Totals, @Status, @Rating, @CreatedDate, @UpdatedDate, @Price, @TotalPay, @CategoryID)",
            new Dictionary<string, object> {
                { "@Name", product.Name },
                { "@Description", product.Description ?? string.Empty}, // Xử lý null
                { "@ProductImage", product.Image ?? string.Empty}, // Xử lý null
                { "@Totals", product.Total},
                { "@Status", product.Status},
                { "@Rating", product.Rating},
                { "@CreatedDate", product.dateAdd},
                { "@UpdatedDate", product.dateAdd}, // Sửa tên tham số
                { "@Price", product.Price },
                { "@TotalPay", product.TotalPay},
                { "@CategoryId", product.CategoryId }
            });

        public bool UpdateProduct(Product product) => ExecuteNonQuery(
            "UPDATE sp SET tenSP = @Name, moTa = @Description, hinh = @ProductImage, khoiLuong = @Stock, TrangThai = @Status, danh_gia = @Rating, ngay_cap_nhat = @UpdatedDate, gia = @Price, da_ban = @TotalPay, dm = @CategoryID WHERE id = @Id",
            new Dictionary<string, object> {
                { "@Id", product.Id },
                { "@Name", product.Name },
                { "@Description", product.Description },
                { "@ProductImage", product.Image },
                { "@Stock", product.Total },
                { "@Status", product.Status },
                { "@Rating", product.Rating },
                { "@UpdatedDate", DateTime.Now },
                { "@Price", product.Price },
                { "@TotalPay", product.TotalPay },
                { "@CategoryID", product.CategoryId }
            });

        public bool DeleteProduct(int id) =>
            ExecuteNonQuery(
                "UPDATE sp SET trangThaiXoa = N'Đã xóa' WHERE id = @Id",
                new Dictionary<string, object> { { "@Id", id } }
            );


        public DataTable GetAllProducts(string db = null)
        {
            try
            {
                OpenConnection();
                string query = "";
                if (db == null)
                {
                    query = "SELECT * FROM sp WHERE trangThaiXoa != N'Đã xóa' ORDER BY id DESC";
                }
                else
                {
                    query = db;
                }
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        public List<Product> FindProduct(string query, string id, string name, int? categoryId)
        {
            List<Product> productList = new List<Product>();
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                    }
                    if (!string.IsNullOrEmpty(name))
                    {
                        cmd.Parameters.AddWithValue("@Name", $"%{name}%");
                    }
                    if (categoryId.HasValue && categoryId != -1)
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                Name = reader["tenSP"].ToString(),
                                Description = reader["moTa"]?.ToString() ?? string.Empty,
                                Image = reader["hinh"]?.ToString() ?? string.Empty,
                                Total = Convert.ToInt32(reader["khoiLuong"] ?? 0),
                                Status = Convert.ToString(reader["TrangThai"]),
                                Rating = Convert.ToInt32(reader["danh_gia"]),
                                dateAdd = Convert.ToDateTime(reader["ngay_tao"]),
                                Price = Convert.ToDecimal(reader["gia"]),
                                TotalPay = Convert.ToInt32(reader["da_ban"]),
                                CategoryId = Convert.ToInt32(reader["dm"])
                            };
                            productList.Add(product);
                        }
                    }
                }
                return productList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }


        public int GetMaxProductId()
        {
            int maxId = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(id), 0) FROM sp";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            maxId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return maxId;
        }


        // quản lý vấn đề voucher 
        public bool AddVoucher(Voucher voucher) => ExecuteNonQuery(
            "INSERT INTO voucher (NgayBatDau, NgayKetThuc, Loai, GiaTri, DanhMucApDung, TrangThai, maVoucher, MoTa) " +
            "VALUES (@NgayBatDau, @NgayKetThuc, @Loai, @GiaTri, @DanhMucApDung, @TrangThai, @maVoucher, @MoTa)",
            new Dictionary<string, object> {
                { "@NgayBatDau", voucher.StartDate },
                { "@NgayKetThuc", voucher.EndDate },
                { "@Loai", voucher.TypeOf },
                { "@GiaTri", voucher.ValueOfVoucher },
                { "@DanhMucApDung", voucher.categoryId },
                { "@TrangThai", voucher.Status },
                { "@maVoucher", voucher.voucherCode },
                { "@MoTa", voucher.Description }
            });

        public bool UpdateVoucher(Voucher voucher) => ExecuteNonQuery(
            "UPDATE voucher SET NgayBatDau = @NgayBatDau, NgayKetThuc = @NgayKetThuc, Loai = @Loai, GiaTri = @GiaTri, DanhMucApDung = @DanhMucApDung, TrangThai = @TrangThai, maVoucher = @maVoucher, MoTa = @MoTa WHERE id = @Id",
            new Dictionary<string, object> {
                { "@Id", voucher.Id },
                { "@NgayBatDau", voucher.StartDate },
                { "@NgayKetThuc", voucher.EndDate },
                { "@Loai", voucher.TypeOf },
                { "@GiaTri", voucher.ValueOfVoucher },
                { "@DanhMucApDung", voucher.categoryId },
                { "@TrangThai", voucher.Status },
                { "@maVoucher", voucher.voucherCode },
                { "@MoTa", voucher.Description }
            });

        public bool DeleteVoucher(int id) => ExecuteNonQuery("UPDATE voucher SET trangThaiXoa = N'Đã xóa' WHERE id = @Id", new Dictionary<string, object> { { "@Id", id } });

        public DataTable GetAllVouchers()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM vw_VoucherWithCategory WHERE trangThaiXoa != N'Đã xóa'";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        // Trong lớp DAL
        public List<Voucher> FindVoucher(string query, string maVoucher, string loai, int? danhMucId)
        {
            List<Voucher> voucherList = new List<Voucher>();
            try
            {
                OpenConnection();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    // Thêm các tham số tìm kiếm
                    if (!string.IsNullOrEmpty(maVoucher))
                    {
                        cmd.Parameters.AddWithValue("@maVoucher", $"%{maVoucher}%");
                    }
                    if (!string.IsNullOrEmpty(loai))
                    {
                        cmd.Parameters.AddWithValue("@loai", $"%{loai}%");
                    }
                    if (danhMucId.HasValue && danhMucId != 0)
                    {
                        cmd.Parameters.AddWithValue("@DanhMucApDung", danhMucId);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            voucherList.Add(new Voucher()
                            {
                                Id = reader["VoucherID"] != DBNull.Value ? Convert.ToInt32(reader["VoucherID"]) : 0,
                                voucherCode = reader["maVoucher"]?.ToString(),
                                StartDate = reader["NgayBatDau"] != DBNull.Value ? Convert.ToDateTime(reader["NgayBatDau"]) : DateTime.MinValue,
                                EndDate = reader["NgayKetThuc"] != DBNull.Value ? Convert.ToDateTime(reader["NgayKetThuc"]) : DateTime.MinValue,
                                TypeOf = reader["Loai"]?.ToString(),
                                ValueOfVoucher = reader["GiaTri"] != DBNull.Value ? Convert.ToDecimal(reader["GiaTri"]) : 0,
                                categoryId = reader["DanhMucApDung"] != DBNull.Value ? Convert.ToInt32(reader["DanhMucApDung"]) : 0,
                                categoryName = reader["TenDanhMucApDung"]?.ToString(),
                                Status = reader["TrangThai"]?.ToString(),
                                Description = reader["MoTa"]?.ToString()
                            });
                        }
                    }
                }
                return voucherList;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[DAL Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }




        // quản lý vấn đề mua hàng 
        // danh sách đơn đặt hàng 
        // phê duyệt 
        // xóa
        // tìm kiểm 
        // Hàm trả về List<Order>
        public List<Order> GetAllOrders(string query)
        {
            List<Order> orders = new List<Order>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Order order = new Order
                                {
                                    OrderID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                                    OrderDate = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                                    TotalAmount = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                                    Status = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                                    Notes = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    AccountId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                                    UserID = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                                    PaymentMethodID = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                    ExpectedDeliveryDate = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                                    ShippingProvider = reader.IsDBNull(9) ? null : reader.GetString(9),
                                    TrackingNumber = reader.IsDBNull(10) ? null : reader.GetString(10),
                                    VoucherId = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                                    LastUpdated = reader.IsDBNull(12) ? DateTime.MinValue : reader.GetDateTime(12),
                                    AddressId = reader.IsDBNull(13) ? 0 : reader.GetInt32(13)
                                };

                                orders.Add(order);
                            }
                        }
                    }
                }
                return orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }


        public bool ApproveOrder(int orderId, string status) => ExecuteNonQuery($"UPDATE Orders SET trangThai = N'{status}' WHERE OrderID = @OrderId", new Dictionary<string, object> { { "@OrderId", orderId } });



        public bool DeleteOrder(int orderId)
        {
            bool result = false;
            result =  ExecuteNonQuery("DELETE FROM OrderDetails WHERE OrderID = @OrderId", new Dictionary<string, object> { { "@OrderId", orderId } });
            result =  ExecuteNonQuery("DELETE FROM Orders WHERE OrderID = @OrderId", new Dictionary<string, object> { { "@OrderId", orderId } });
            return result;
        }

        public List<Order> SearchOrders(string customerName = null, string voucherId = null)
        {
            List<Order> orders = new List<Order>();
            try
            {
                OpenConnection();

                using (SqlCommand cmd = new SqlCommand("sp_SearchOrders", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters - passing NULL if empty
                    cmd.Parameters.AddWithValue("@CustomerName", string.IsNullOrWhiteSpace(customerName) ? DBNull.Value : (object)customerName);
                    cmd.Parameters.AddWithValue("@VoucherID", string.IsNullOrWhiteSpace(voucherId) ? DBNull.Value : (object)voucherId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order order = new Order
                            {
                                OrderID = reader.GetInt32(0),
                                OrderDate = reader.GetDateTime(1),
                                TotalAmount = reader.GetDecimal(2),
                                Status = reader.GetString(3),
                                Notes = reader.IsDBNull(4) ? null : reader.GetString(4),
                                AccountId = reader.GetInt32(5),
                                UserID = reader.GetInt32(6),
                                PaymentMethodID = reader.GetInt32(7),
                                ExpectedDeliveryDate = reader.GetDateTime(8),
                                ShippingProvider = reader.GetString(9),
                                TrackingNumber = reader.GetString(10),
                                VoucherId = reader.IsDBNull(11) ? (int?)null : reader.GetInt32(11),
                                LastUpdated = reader.GetDateTime(12),
                                AddressId = reader.GetInt32(13)
                            };

                            orders.Add(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }

            return orders;
        }

        // quản lý vấn đề thanh toán 
        // thêm mới phương thức thanh toán vào nền tảng 
        // lấy ra các phương thức thanh toàn hiện có 
        // chỉnh sửa 
        // xóa 
        public bool AddPaymentMethod(PaymentMethod paymentMethod) => ExecuteNonQuery("INSERT INTO ttThanhToan (tenPhuongThuc, trangThai, moTa) VALUES (@Name, @Status, @Description)", new Dictionary<string, object> { { "@Name", paymentMethod.Name }, { "@Status", paymentMethod.Status}, { "@Description", paymentMethod.Description } });

        public List<PaymentMethod> GetAllPaymentMethods()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM ttThanhToan WHERE trangThai != N'Bị khóa'";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<PaymentMethod> paymentMethods = new List<PaymentMethod>();
                    while (reader.Read())
                    {
                        PaymentMethod method = new PaymentMethod
                        {
                            Name = reader["tenPhuongThuc"].ToString(),
                            Status = Convert.ToString(reader["trangThai"]),
                            Description = reader["moTa"].ToString()
                        };
                        paymentMethods.Add(method);
                    }
                    return paymentMethods;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<PaymentMethod>();
            }
            finally
            {
                CloseConnection();
            }
        }


        public bool UpdatePaymentMethod(PaymentMethod paymentMethod) => ExecuteNonQuery("UPDATE ttThanhToan SET tenPhuongThuc = @Name, trangThai = @Status, moTa = @Description WHERE id = @Id", new Dictionary<string, object> { { "@Id", paymentMethod.Id }, { "@Name", paymentMethod.Name }, { "@Status", paymentMethod.Status }, { "@Description", paymentMethod.Description } });

        public bool DeletePaymentMethod(int paymentMethodId) =>
            ExecuteNonQuery(
                "UPDATE ttThanhToan SET trangThai = N'Bị khóa' WHERE id = @Id AND trangThai != N'Bị khóa'",
                new Dictionary<string, object> { { "@Id", paymentMethodId } }
            );

        // quản lý vấn đề liên quan đến nền tảng 
        // thay thế logo
        // thay thế banner 
        // cấu hình hệ thống
        //public bool UpdatePlatformConfig(Admin.PlatformConfig config) => ExecuteNonQuery("UPDATE PlatformConfig SET Logo = @Logo, Banner = @Banner, SystemConfig = @SystemConfig", new Dictionary<string, object> { { "@Logo", config.Logo }, { "@Banner", config.Banner }, { "@SystemConfig", config.SystemConfig } });

        public DataTable GetPlatformConfig()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM PlatformConfig";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }


        public void loadAdminDashboard(
            ref float tongDoanhThu, ref int tongDonHang, ref int tongSanPham, ref int tongKhachHang,
            ref List<Order> listNewOrder,
            ref List<Product> listNewProduct,
            ref List<Reprots> listNewReprots)
        {
            try
            {
                OpenConnection();

                using (SqlCommand cmd = new SqlCommand("SELECT SUM(Total) FROM OrderDetails", connection))
                {
                    object result = cmd.ExecuteScalar();
                    tongDoanhThu = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Orders", connection))
                {
                    object result = cmd.ExecuteScalar();
                    tongDonHang = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM sp", connection))
                {
                    object result = cmd.ExecuteScalar();
                    tongSanPham = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM tk", connection))
                {
                    object result = cmd.ExecuteScalar();
                    tongKhachHang = result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }

                // Xử lý cho bảng phanHoi
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 10 * FROM phanHoi WHERE [trangThaiXoa] != N'Đã xóa' AND TrangThai = N'Mới'  ORDER BY id DESC", connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reprots product = new Reprots()
                            {
                                Id = reader["id"] as int? ?? 0,
                                IdNguoiDung = reader["idNguoiDung"] as int? ?? 0,
                                IdSanPham = reader["idSanPham"] as int? ?? 0,
                                ChuDe = reader["chuDe"] as string ?? string.Empty,
                                NoiDung = reader["noiDung"] as string ?? string.Empty,
                                TrangThai = reader["TrangThai"] as string ?? "Chưa xác định",
                                TongPhanHoi = reader["tongPhanHoi"] as int? ?? 0,
                                NgayDang = reader["ngayDang"] as DateTime? ?? DateTime.MinValue,
                                IdNguoiGiaiQuyet = reader["idNguoiGiaiQuyet"] as int? ?? 0
                            };
                            listNewReprots.Add(product);
                        }
                    }
                }

                // Xử lý cho bảng sp
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 10 * FROM sp WHERE [trangThaiXoa] != N'Đã xóa' ORDER BY id DESC", connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product()
                            {
                                Id = reader["id"] as int? ?? 0,
                                Name = reader["tenSP"] as string ?? "Tên mặc định",
                                Description = reader["moTa"] as string ?? string.Empty,
                                Image = reader["hinh"] as string ?? "default.jpg",
                                Total = reader["khoiLuong"] as int? ?? 0,
                                Status = reader["TrangThai"] as string ?? "Chưa xác định",
                                Rating = reader["danh_gia"] as int? ?? 0,
                                dateAdd = reader["ngay_tao"] as DateTime? ?? DateTime.Now,
                                Price = reader["gia"] as decimal? ?? 0m,
                                TotalPay = reader["da_ban"] as int? ?? 0,
                                CategoryId = reader["dm"] as int? ?? 0
                            };
                            listNewProduct.Add(product);
                        }
                    }
                }

                // Xử lý cho view v_tongQuanDonHang
                using (SqlCommand cmd = new SqlCommand("SELECT TOP 10 * FROM v_tongQuanDonHang ORDER BY MaDonHang DESC", connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order product = new Order()
                            {
                                OrderID = reader["MaDonHang"] as int? ?? 0,
                                tenNguoiDat = reader["TenNguoiDat"] as string ?? "Khách vãng lai",
                                Status = reader["TrangThaiDonHang"] as string ?? "Chờ xử lý",
                                TotalAmount = reader["TongTien"] as int? ?? 0,
                                OrderDate = reader["NgayDatHang"] as DateTime? ?? DateTime.Now
                            };
                            listNewOrder.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }
        }



        public List<Reprots> getAllReport(string query = null)
        {
            List<Reprots> reprots = new List<Reprots>();
            try
            {
                OpenConnection();
                if (query == null)
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM phanHoi WHERE [trangThaiXoa] != N'Đã xóa'  ORDER BY id DESC", connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Reprots product = new Reprots()
                                {
                                    Id = reader["id"] as int? ?? 0,
                                    IdNguoiDung = reader["idNguoiDung"] as int? ?? 0,
                                    IdSanPham = reader["idSanPham"] as int? ?? 0,
                                    ChuDe = reader["chuDe"] as string ?? string.Empty,
                                    NoiDung = reader["noiDung"] as string ?? string.Empty,
                                    TrangThai = reader["TrangThai"] as string ?? "Chưa xác định",
                                    TongPhanHoi = reader["tongPhanHoi"] as int? ?? 0,
                                    NgayDang = reader["ngayDang"] as DateTime? ?? DateTime.MinValue,
                                    IdNguoiGiaiQuyet = reader["idNguoiGiaiQuyet"] as int? ?? 0
                                };
                                reprots.Add(product);
                            }

                        }
                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Reprots product = new Reprots()
                                {
                                    Id = reader["id"] as int? ?? 0,
                                    IdNguoiDung = reader["idNguoiDung"] as int? ?? 0,
                                    IdSanPham = reader["idSanPham"] as int? ?? 0,
                                    ChuDe = reader["chuDe"] as string ?? string.Empty,
                                    NoiDung = reader["noiDung"] as string ?? string.Empty,
                                    TrangThai = reader["TrangThai"] as string ?? "Chưa xác định",
                                    TongPhanHoi = reader["tongPhanHoi"] as int? ?? 0,
                                    NgayDang = reader["ngayDang"] as DateTime? ?? DateTime.MinValue,
                                    IdNguoiGiaiQuyet = reader["idNguoiGiaiQuyet"] as int? ?? 0
                                };
                                reprots.Add(product);
                            }

                        }
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show($"Lỗi {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CloseConnection();
            }
            return reprots;
        }

        // vấn đề liên quan đến đơn đặt hàng 
        // thông tin vận chuyền 
        // tình trạng đơn 


        // những vấn đề liên quan đến voucher 
        // áp cho những danh mục nào 
        // áp cho những tài khoản có điều kiện nào 


        // người dùng 
        // mỗi danh mục 10 sản phẩm 
        // xem được thông tin của các sản phẩm 
        // tìm kiếm sản phẩm 
        // các vấn đê liên quan đến việc thanh toán 
        //  trao đổi giữa ngươi dùng và shop 
        // việc mua hàng 
        // xuất hóa đơn (creater report)
        // thanh toán 
        // vấn đề về thanh chat 
        // vần đề liên quan đến bình luận về sản phẩm 


        public List<ThongTinDiaChiNguoiDung> GetThongTinDiaChiNguoiDung(int accountId)
        {
            List<ThongTinDiaChiNguoiDung> list = new List<ThongTinDiaChiNguoiDung>();

            string query = @"
        SELECT 
            COALESCE(ttnd.hoVaTen, tk.ten_dn) AS FullName,
            CASE 
                WHEN ttnd.soDienThoai IS NULL THEN ttnd.Email
                WHEN ttnd.Email IS NULL THEN ttnd.soDienThoai
                ELSE ttnd.soDienThoai
            END AS Contact,
            RTRIM(
                COALESCE(d.soNha + ', ', '') +
                COALESCE(d.pho + ', ', '') +
                COALESCE(d.quan + ', ', '') +
                COALESCE(d.huyen + ', ', '') +
                COALESCE(d.thanhPho, '')
            ) AS FullAddress
        FROM [BTL].[dbo].[tk] tk
        INNER JOIN [BTL].[dbo].[ttnd] ttnd ON tk.id = ttnd.idTaiKhoan
        LEFT JOIN [BTL].[dbo].[diaChi] d ON d.idNguoiDung = tk.id
        WHERE tk.id = @AccountId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AccountId", accountId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var info = new ThongTinDiaChiNguoiDung
                        {
                            FullName = reader["FullName"].ToString(),
                            Contact = reader["Contact"].ToString(),
                            FullAddress = reader["FullAddress"].ToString()
                        };
                        list.Add(info);
                    }
                }
            }

            return list;
        }




        // load lại giá trị 
        public int CreateOrder(Account account, Product product, int quantity, int addressId,
                              int paymentMethodId, int? voucherId = null, string notes = null)
        {
            int orderId = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Sử dụng transaction để đảm bảo tính toàn vẹn dữ liệu
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Thêm thông tin đơn hàng
                        decimal totalAmount = product.Price * quantity;

                        // Áp dụng giảm giá từ voucher nếu có
                        decimal discount = 0;
                        if (voucherId.HasValue)
                        {
                            // Logic tính giảm giá từ voucher có thể được thêm ở đây
                            // Giả sử lấy được thông tin discount từ voucher
                            discount = GetVoucherDiscountAmount(voucherId.Value, totalAmount, connection, transaction);
                            totalAmount -= discount;
                        }

                        string insertOrderQuery = @"
                        INSERT INTO [BTL].[dbo].[Orders] 
                        ([ngayDatHang], [tongTien], [trangThai], [Ghichu], 
                         [AccountId], [UserID], [PaymentMethodID], [NgayGiaoHangDuKien], 
                         [donViVanChuyen], [maDonVanChuyen], [VoucherId], [NgayCapNhatCuoi], [AddressId])
                        VALUES 
                        (@OrderDate, @TotalAmount, @Status, @Notes, 
                         @AccountId, @UserID, @PaymentMethodID, @ExpectedDeliveryDate, 
                         @ShippingProvider, @TrackingNumber, @VoucherId, @LastUpdated, @AddressId);
                        
                        SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd = new SqlCommand(insertOrderQuery, connection, transaction))
                        {
                            // Ngày đặt hàng là hiện tại
                            DateTime orderDate = DateTime.Now;

                            // Mặc định dự kiến giao hàng sau 3 ngày
                            DateTime expectedDeliveryDate = orderDate.AddDays(3);

                            cmd.Parameters.AddWithValue("@OrderDate", orderDate);
                            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            cmd.Parameters.AddWithValue("@Status", "Pending"); // Trạng thái mặc định khi tạo đơn hàng
                            cmd.Parameters.AddWithValue("@Notes", notes ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@AccountId", account.Id);
                            cmd.Parameters.AddWithValue("@UserID", account.Id); // Giả định UserID bằng AccountId, có thể điều chỉnh
                            cmd.Parameters.AddWithValue("@PaymentMethodID", paymentMethodId);
                            cmd.Parameters.AddWithValue("@ExpectedDeliveryDate", expectedDeliveryDate);
                            cmd.Parameters.AddWithValue("@ShippingProvider", "Default"); // Có thể điều chỉnh theo yêu cầu thực tế
                            cmd.Parameters.AddWithValue("@TrackingNumber", DBNull.Value); // Chưa có mã vận đơn khi tạo
                            cmd.Parameters.AddWithValue("@VoucherId", voucherId.HasValue ? (object)voucherId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                            cmd.Parameters.AddWithValue("@AddressId", addressId);

                            // Lấy ID của đơn hàng vừa thêm
                            orderId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        // 2. Thêm chi tiết đơn hàng
                        string insertOrderDetailQuery = @"
                        INSERT INTO [BTL].[dbo].[OrderDetails]
                        ([OrderID], [ProductID], [Quantity], [Price], [Discount], [Total], [VoucherID], [isDeleted])
                        VALUES
                        (@OrderID, @ProductID, @Quantity, @Price, @Discount, @Total, @VoucherID, @isDeleted);";

                        using (SqlCommand cmd = new SqlCommand(insertOrderDetailQuery, connection, transaction))
                        {
                            decimal itemTotal = product.Price * quantity - discount;

                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.Parameters.AddWithValue("@ProductID", product.Id);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@Price", product.Price);
                            cmd.Parameters.AddWithValue("@Discount", discount);
                            cmd.Parameters.AddWithValue("@Total", itemTotal);
                            cmd.Parameters.AddWithValue("@VoucherID", voucherId.HasValue ? (object)voucherId.Value : DBNull.Value);
                            cmd.Parameters.AddWithValue("@isDeleted", false);

                            cmd.ExecuteNonQuery();
                        }

                        // Cập nhật số lượng sản phẩm trong kho
                        UpdateProductInventory(product.Id, quantity, connection, transaction);

                        // Commit transaction khi mọi thứ thành công
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback nếu có lỗi
                        transaction.Rollback();
                        throw new Exception("Lỗi khi tạo đơn hàng: " + ex.Message);
                    }
                }
            }

            return orderId;
        }

        private void UpdateProductInventory(int productId, int quantity, SqlConnection connection, SqlTransaction transaction)
        {
            string updateQuery = @"
            UPDATE [BTL].[dbo].[Products]
            SET [Stock] = [Stock] - @Quantity
            WHERE [ProductID] = @ProductID AND [Stock] >= @Quantity;";

            using (SqlCommand cmd = new SqlCommand(updateQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@ProductID", productId);
                cmd.Parameters.AddWithValue("@Quantity", quantity);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    throw new Exception("Sản phẩm không đủ số lượng trong kho.");
                }
            }
        }

        // Phương thức lấy số tiền giảm giá từ voucher
        private decimal GetVoucherDiscountAmount(int voucherId, decimal totalAmount, SqlConnection connection, SqlTransaction transaction)
        {
            decimal discount = 0;

            string query = @"
            SELECT [DiscountAmount], [DiscountPercent], [MinOrderValue], [MaxDiscountAmount]
            FROM [BTL].[dbo].[Vouchers]
            WHERE [VoucherID] = @VoucherID AND [isValid] = 1 AND [ExpiryDate] > GETDATE();";

            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@VoucherID", voucherId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal discountAmount = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0);
                        decimal discountPercent = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                        decimal minOrderValue = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                        decimal maxDiscountAmount = reader.IsDBNull(3) ? decimal.MaxValue : reader.GetDecimal(3);

                        // Kiểm tra điều kiện áp dụng voucher
                        if (totalAmount >= minOrderValue)
                        {
                            if (discountAmount > 0)
                            {
                                // Giảm giá cố định
                                discount = discountAmount;
                            }
                            else if (discountPercent > 0)
                            {
                                // Giảm giá theo phần trăm
                                discount = totalAmount * (discountPercent / 100);

                                // Giới hạn số tiền giảm giá tối đa
                                if (discount > maxDiscountAmount)
                                {
                                    discount = maxDiscountAmount;
                                }
                            }
                        }
                    }
                }
            }

            return discount;
        }



        public int InsertOrderAndDetail(Order order, OrderDetail orderDetail)
        {
            int newOrderId = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // 1. Chèn Order và lấy OrderID mới tạo
                        string insertOrderQuery = @"
                    INSERT INTO [dbo].[Orders]
                    ([ngayDatHang], [tongTien], [trangThai], [AccountId], [UserID], [PaymentMethodID], [NgayCapNhatCuoi], [AddressId])
                    VALUES
                    (@OrderDate, @TotalAmount, @Status, @AccountId, @UserID, @PaymentMethodID, @LastUpdated, @AddressId);
                    
                    SELECT SCOPE_IDENTITY();";

                        using (SqlCommand cmd = new SqlCommand(insertOrderQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                            cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
                            cmd.Parameters.AddWithValue("@Status", order.Status);
                            cmd.Parameters.AddWithValue("@AccountId", order.AccountId);
                            cmd.Parameters.AddWithValue("@UserID", order.UserID);
                            cmd.Parameters.AddWithValue("@PaymentMethodID", order.PaymentMethodID);
                            cmd.Parameters.AddWithValue("@LastUpdated", order.LastUpdated);
                            cmd.Parameters.AddWithValue("@AddressId", order.AddressId);

                            object result = cmd.ExecuteScalar();
                            if (result != null && int.TryParse(result.ToString(), out newOrderId) && newOrderId > 0)
                            {
                                // Order đã được chèn thành công, newOrderId nhận giá trị mới
                            }
                            else
                            {
                                throw new Exception("Chèn Order thất bại.");
                            }
                        }

                        // 2. Chèn OrderDetail sử dụng OrderID vừa chèn
                        string insertOrderDetailQuery = @"
                        INSERT INTO [dbo].[OrderDetails]
                        ([OrderID], [ProductID], [Quantity], [Price], [Discount])
                        VALUES
                        (@OrderID, @ProductID, @Quantity, @Price, @Discount);";

                        using (SqlCommand cmd = new SqlCommand(insertOrderDetailQuery, connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", newOrderId);
                            cmd.Parameters.AddWithValue("@ProductID", orderDetail.ProductID);
                            cmd.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
                            cmd.Parameters.AddWithValue("@Price", orderDetail.Price);
                            cmd.Parameters.AddWithValue("@Discount", orderDetail.Discount);
                            cmd.Parameters.AddWithValue("@Total", orderDetail.Total);

                            cmd.ExecuteNonQuery();
                        }

                        // Commit nếu mọi thứ thành công
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Lỗi khi thêm Order và OrderDetail: " + ex.Message, ex);
                    }
                }
            }
            return newOrderId;
        }

    }
}
