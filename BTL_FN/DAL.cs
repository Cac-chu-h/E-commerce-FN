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
        public string logo = @"E:\C#\logo.jpg";
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
        private static readonly string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=BTL;Integrated Security=True";
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
                    { "@Role", "User" },
                    { "@Status", "Active" },
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
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            ParentId = reader.GetInt32(3)
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


        public DataTable GetAllProducts()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM sp WHERE trangThaiXoa != N'Đã xóa' ORDER BY id DESC";
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
                                orders.Add(new Order
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
                                });
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

        public bool ApproveOrder(int orderId, string status) => ExecuteNonQuery($"UPDATE Orders SET Status = '{status}' WHERE Id = @OrderId", new Dictionary<string, object> { { "@OrderId", orderId } });

        public bool UpdateOrders(Order va) => ExecuteNonQuery($"UPDATE Orders SET Status = '' WHERE Id = @OrderId", new Dictionary<string, object> { { "@OrderId", va.OrderID } });

        public bool DeleteOrder(int orderId) => ExecuteNonQuery("DELETE FROM Orders WHERE Id = @OrderId", new Dictionary<string, object> { { "@OrderId", orderId } });

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
                string query = "SELECT * FROM ttThanhToan";
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

        public bool DeletePaymentMethod(int paymentMethodId) => ExecuteNonQuery("UPDATE SET trangThai = N'Bị khóa' FROM ttThanhToan WHERE id = @Id", new Dictionary<string, object> { { "@Id", paymentMethodId } });
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

                using(SqlCommand cmd = new SqlCommand("SELECT TOP 10 * FROM phanHoi WHERE [trangThaiXoa] != N'Đã xóa' ORDER BY id DESC", connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Reprots product = new Reprots()
                            {
                                Id = Convert.ToInt32(reader["id"]),
                                IdNguoiDung = Convert.ToInt32(reader["idNguoiDung"]),
                                IdSanPham = Convert.ToInt32(reader["idSanPham"]),
                                ChuDe = reader["chuDe"]?.ToString(),
                                NoiDung = reader["noiDung"].ToString(),
                                TrangThai = Convert.ToString(reader["TrangThai"]),
                                TongPhanHoi = Convert.ToInt32(reader["tongPhanHoi"]),
                                NgayDang = Convert.ToDateTime(reader["ngayDang"]),
                                IdNguoiGiaiQuyet = Convert.ToInt32(reader["idNguoiGiaiQuyet"])
                            };
                            listNewReprots.Add(product);
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand("SELECT TOP 10 * FROM sp WHERE [trangThaiXoa] != N'Đã xóa' ORDER BY id DESC", connection))
                {
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
                            listNewProduct.Add(product);
                        }
                    }
                }


                using (SqlCommand cmd = new SqlCommand("SELECT TOP 10 * FROM v_tongQuanDonHang ORDER BY MaDonHang DESC", connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order product = new Order()
                            {
                                OrderID = Convert.ToInt32(reader["MaDonHang"]),
                                tenNguoiDat = Convert.ToString(reader["TenNguoiDat"]),
                                Status = reader["TrangThaiDonHang"].ToString(),
                                TotalAmount = Convert.ToInt32(reader["TongTien"]),
                                OrderDate = Convert.ToDateTime(reader["NgayDatHang"])
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






        // load lại giá trị 

    }
}
