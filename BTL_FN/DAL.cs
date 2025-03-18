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
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string password { get; set; }
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

        // Cập nhật phương thức ExecuteScalar
        public void AmdinDefault(ref int soLuongDonHangDangCho, ref int soLuongDonHangDangChuanBi, ref int soLuongDonHangDangGiao, ref int soLuongDonHangHoanThanh, ref int soLuongDonHangBiHuy, ref int soLuongMatHangSapHetHang)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command1 = new SqlCommand("SELECT COUNT(Status) FROM Orders WHERE Status = 'Đang chờ'", connection);
                 soLuongDonHangDangCho = (int)command1.ExecuteScalar();

                SqlCommand command2 = new SqlCommand("SELECT COUNT(Status) FROM Orders WHERE Status = 'Đang chuẩn bị'", connection);
                 soLuongDonHangDangChuanBi = (int)command2.ExecuteScalar();

                SqlCommand command3 = new SqlCommand("SELECT COUNT(Status) FROM Orders WHERE Status = 'Đang giao'", connection);
                 soLuongDonHangDangGiao = (int)command3.ExecuteScalar();

                SqlCommand command4 = new SqlCommand("SELECT COUNT(Status) FROM Orders WHERE Status = 'Hoàn thành'", connection);
                 soLuongDonHangHoanThanh = (int)command4.ExecuteScalar();

                SqlCommand command5 = new SqlCommand("SELECT COUNT(Status) FROM Orders WHERE Status = 'Đơn hủy'", connection);
                 soLuongDonHangBiHuy = (int)command5.ExecuteScalar();

                SqlCommand command6 = new SqlCommand("SELECT COUNT(*) FROM product WHERE i_Totals < 10", connection);
                 soLuongMatHangSapHetHang = (int)command6.ExecuteScalar();
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
                                Username = reader.GetString(reader.GetOrdinal("s_UserName")),
                                Email = reader.GetString(reader.GetOrdinal("s_Email")),
                                Role = reader.GetString(reader.GetOrdinal("s_Role")),
                                Status = reader.GetString(reader.GetOrdinal("s_Active")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("d_CreateDate"))
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
            int newID = getMaxID() + 1; // Lấy ID mới
            string query = @"INSERT INTO [account]
            (i_Id, s_UserName, s_PassWord, s_Email, s_Image, s_Role, s_Active, d_CreateDate)
            VALUES (@id, @Username, @Password, @Email, @image, @Role, @Status, @CreatedDate)";

            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", newID }, // Gán ID mới
                    { "@Username", a.Username },
                    { "@Password", a.password },
                    { "@Email", a.Email },
                    { "@image", new BLL().logo },
                    { "@Role", "User" },
                    { "@Status", "Active" },
                    { "@CreatedDate", DateTime.Now }
                };

            return ExecuteNonQuery(query, parameters);
        }


        public int getMaxID()
        {
            string query = "SELECT MAX(i_Id) AS MaxID FROM [account]";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                connection.Open();
                object result = cmd.ExecuteScalar();
                connection.Close();

                if (result != DBNull.Value && result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    return 0; // Trả về 0 nếu không có bản ghi nào
                }
            }
        }


        public bool BanUser(int userId) => ExecuteNonQuery("UPDATE account SET i_isActive = 0 WHERE i_Id = @UserId", new Dictionary<string, object> { { "@UserId", userId } });

        public bool DeleteUser(int userId) => ExecuteNonQuery("DELETE FROM account WHERE i_Id = @UserId", new Dictionary<string, object> { { "@UserId", userId } });

        public bool PromoteToAdmin(int userId) => ExecuteNonQuery("UPDATE account SET s_Role = 'Admin' WHERE i_Id = @UserId", new Dictionary<string, object> { { "@UserId", userId } });

        


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
                                Id = reader.GetInt32(reader.GetOrdinal("i_Id")),
                                Username = reader.GetString(reader.GetOrdinal("s_UserName")),
                                Email = reader.GetString(reader.GetOrdinal("s_Email")),
                                Role = reader.GetString(reader.GetOrdinal("s_Role")),
                                Status = reader.GetString(reader.GetOrdinal("s_Active")),
                                CreatedDate = reader.GetDateTime(reader.GetOrdinal("d_CreateDate"))
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
           ExecuteNonQuery("INSERT INTO category (i_id, categoryName, mota, danhMucCha) VALUES (@id, @categoryName, @mota, @ParentId)",
           new Dictionary<string, object>
           {
                {"@id", category.Id },
                { "@categoryName", category.Name },
                { "@mota", category.Description },
                { "@ParentId", category.ParentId }
           });



        public bool DeleteCategory(int id) => ExecuteNonQuery("DELETE FROM category WHERE Id = @Id", new Dictionary<string, object> { { "@Id", id } });

        public bool UpdateCategory(Category category) => ExecuteNonQuery(
            "UPDATE category SET categoryName = @valueA, mota = @valueB, danhMucCha = @valueC WHERE i_id = @Id",
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
                string query = "SELECT id, tenDanhMuc, moTa, danhMucCha FROM dm";
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


        public int GetMaxCategoryId()
        {
            int maxId = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(i_id), 0) FROM category";
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

        // quản lý sản phẩm 
        // thêm sản phẩm 
        // xửa 
        // lấy ra danh sách sản phẩm 
        // xóa 
        // tìm kiếm 
        public bool AddProduct(Product product) => ExecuteNonQuery(
            "INSERT INTO product (i_Id, s_ProductName, s_Description, s_ProductImage, i_Totals, i_Status, i_Rating, d_CreatedDate, d_UpdatedDate, f_Price, i_TotalPay, i_CategoryID) " +
            "VALUES (@id, @Name, @Description, @ProductImage, @Totals, @Status, @Rating, @CreatedDate, @UpdatedDate, @Price, @TotalPay, @CategoryID)",
            new Dictionary<string, object> {
                { "@id",  product.Id},
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
                { "@UpdatedDate", product.dateAdd },
                { "@Price", product.Price },
                { "@TotalPay", product.TotalPay },
                { "@CategoryID", product.CategoryId }
            });

        public bool DeleteProduct(int id) => ExecuteNonQuery("DELETE FROM product WHERE i_Id = @Id", new Dictionary<string, object> { { "@Id", id } });

        public DataTable GetAllProducts()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM product";
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
                                Id = Convert.ToInt32(reader["i_Id"]),
                                Name = reader["s_ProductName"].ToString(),
                                Description = reader["s_Description"]?.ToString() ?? string.Empty,
                                Image = reader["s_ProductImage"]?.ToString() ?? string.Empty,
                                Total = Convert.ToInt32(reader["i_Totals"] ?? 0),
                                Status = Convert.ToString(reader["i_Status"]),
                                Rating = Convert.ToInt32(reader["i_Rating"]),
                                dateAdd = Convert.ToDateTime(reader["d_CreatedDate"]),
                                Price = Convert.ToDecimal(reader["f_Price"]),
                                TotalPay = Convert.ToInt32(reader["i_TotalPay"]),
                                CategoryId = Convert.ToInt32(reader["i_CategoryID"])
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
                    string query = "SELECT ISNULL(MAX(i_id), 0) FROM product";
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
            "INSERT INTO voucherModule (id, startDate, endDate, typeOf, valueOfVoucher, categoryId, i_Status, VoucherCode, Description) " +
            "VALUES (@Id, @StartDate, @EndDate, @TypeOf, @ValueOfVoucher, @CategoryId, @Status, @VoucherCode, @Description)",
            new Dictionary<string, object> {
                { "@Id", voucher.Id },
                { "@StartDate", voucher.StartDate },
                { "@EndDate", voucher.EndDate },
                { "@TypeOf", voucher.TypeOf },
                { "@ValueOfVoucher", voucher.ValueOfVoucher },
                { "@VoucherCode" , voucher.voucherCode},
                { "@CategoryId", voucher.categoryId },
                { "@Status", voucher.Status },
                { "@Description", voucher.Description }
            });

        public bool UpdateVoucher(Voucher voucher) => ExecuteNonQuery(
            "UPDATE voucherModule SET startDate = @StartDate, endDate = @EndDate, typeOf = @TypeOf, valueOfVoucher = @ValueOfVoucher, categoryId = @CategoryId, i_Status = @Status, VoucherCode = @VoucherCode, Description =  @Description WHERE id = @Id",
            new Dictionary<string, object> {
                { "@Id", voucher.Id },
                { "@StartDate", voucher.StartDate },
                { "@EndDate", voucher.EndDate },
                { "@TypeOf", voucher.TypeOf },
                { "@ValueOfVoucher", voucher.ValueOfVoucher },
                { "@CategoryId", voucher.categoryId },
                { "@Status", voucher.Status },
                { "@VoucherCode" , voucher.voucherCode},
                { "@Description", voucher.Description }
            });

        public bool DeleteVoucher(int id) => ExecuteNonQuery("DELETE FROM voucherModule WHERE id = @Id", new Dictionary<string, object> { { "@Id", id } });

        public DataTable GetAllVouchers()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM voucherModule";
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

        public List<Voucher> FindVoucher(string query, string id, string name, int? categoryId)
        {
            List<Voucher> voucherList = new List<Voucher>();
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
                    if (categoryId.HasValue && categoryId != 0)
                    {
                        cmd.Parameters.AddWithValue("@CategoryId", categoryId);
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Voucher voucher = new Voucher()
                            {
                                Id = Convert.ToInt32(reader["[VoucherCode]"]),
                                StartDate = Convert.ToDateTime(reader["startDate"]),
                                EndDate = Convert.ToDateTime(reader["endDate"]),
                                TypeOf = reader["typeOf"].ToString(),
                                ValueOfVoucher = Convert.ToDecimal(reader["valueOfVoucher"]),
                                categoryId = Convert.ToInt32(reader["categoryId"]),
                                Status = reader["i_Status"].ToString()
                            };
                            voucherList.Add(voucher);
                        }
                    }
                }
                return voucherList;
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

        public int GetMaxVoucherId()
        {
            int maxId = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ISNULL(MAX(id), 0) FROM voucherModule";
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
                MessageBox.Show($"Lỗi 1: {ex.Message}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return maxId;
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
        public bool AddPaymentMethod(PaymentMethod paymentMethod) => ExecuteNonQuery("INSERT INTO PaymentMethods (MethodName, Status, Description) VALUES (@Name, @Status, @Description)", new Dictionary<string, object> { { "@Name", paymentMethod.Name }, { "@Status", paymentMethod.Status}, { "@Description", paymentMethod.Description } });

        public List<PaymentMethod> GetAllPaymentMethods()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM PaymentMethods";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    List<PaymentMethod> paymentMethods = new List<PaymentMethod>();
                    while (reader.Read())
                    {
                        PaymentMethod method = new PaymentMethod
                        {
                            Name = reader["MethodName"].ToString(),
                            Status = Convert.ToString(reader["Status"]),
                            Description = reader["Description"].ToString()
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


        public bool UpdatePaymentMethod(PaymentMethod paymentMethod) => ExecuteNonQuery("UPDATE PaymentMethods SET MethodName = @Name, Status = @Status, Description = @Description WHERE PaymentMethodID = @Id", new Dictionary<string, object> { { "@Id", paymentMethod.Id }, { "@Name", paymentMethod.Name }, { "@Status", paymentMethod.Status }, { "@Description", paymentMethod.Description } });

        public bool DeletePaymentMethod(int paymentMethodId) => ExecuteNonQuery("DELETE FROM PaymentMethods WHERE PaymentMethodID = @Id", new Dictionary<string, object> { { "@Id", paymentMethodId } });
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
