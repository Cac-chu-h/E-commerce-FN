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


    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ParentId { get; set; } // Thiết lập quan hệ
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

    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class PlatformConfig
    {
        public string Logo { get; set; }
        public string Banner { get; set; }
        public string SystemConfig { get; set; }
    }


    public class Voucher
    {
        public int Id { get; set; }             // Mã voucher
        public DateTime StartDate { get; set; } // Ngày bắt đầu
        public DateTime EndDate { get; set; }   // Ngày kết thúc
        public string TypeOf { get; set; }         // Mã danh mục (tham chiếu category)
        public decimal ValueOfVoucher { get; set; } // Giá trị voucher
        public int categoryId { get; set; }
        public string Status { get; set; }
    }

    class DAL
    {
        private static readonly string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=APPDB;Integrated Security=True";
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
                string query = "SELECT * FROM account";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
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
                                Status = reader.GetInt32(reader.GetOrdinal("i_isActive")) == 1 ? "Active" : "Inactive",
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
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
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
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            finally
            {
                CloseConnection();
            }
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
                                Status = reader.GetInt32(reader.GetOrdinal("i_isActive")) == 1 ? "Active" : "Inactive",
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
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                string query = "SELECT i_id, categoryName, mota, danhMucCha FROM category";
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
            "UPDATE product SET s_ProductName = @Name, s_Description = @Description, s_ProductImage = @ProductImage, i_Totals = @Stock, i_Status = @Status, i_Rating = @Rating, d_UpdatedDate = @UpdatedDate, f_Price = @Price, i_TotalPay = @TotalPay, i_CategoryID = @CategoryID WHERE i_Id = @Id",
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
            "INSERT INTO voucherModule (id, startDate, endDate, typeOf, valueOfVoucher, categoryId, i_Status) " +
            "VALUES (@Id, @StartDate, @EndDate, @TypeOf, @ValueOfVoucher, @CategoryId, @Status)",
            new Dictionary<string, object> {
                { "@Id", voucher.Id },
                { "@StartDate", voucher.StartDate },
                { "@EndDate", voucher.EndDate },
                { "@TypeOf", voucher.TypeOf },
                { "@ValueOfVoucher", voucher.ValueOfVoucher },
                { "@CategoryId", voucher.categoryId },
                { "@Status", voucher.Status }
            });

        public bool UpdateVoucher(Voucher voucher) => ExecuteNonQuery(
            "UPDATE voucherModule SET startDate = @StartDate, endDate = @EndDate, typeOf = @TypeOf, valueOfVoucher = @ValueOfVoucher, categoryId = @CategoryId, i_Status = @Status WHERE id = @Id",
            new Dictionary<string, object> {
                { "@Id", voucher.Id },
                { "@StartDate", voucher.StartDate },
                { "@EndDate", voucher.EndDate },
                { "@TypeOf", voucher.TypeOf },
                { "@ValueOfVoucher", voucher.ValueOfVoucher },
                { "@CategoryId", voucher.categoryId },
                { "@Status", voucher.Status }
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
                                Id = Convert.ToInt32(reader["id"]),
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
        public DataTable GetAllOrders()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM Orders";
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

        public bool ApproveOrder(int orderId) => ExecuteNonQuery("UPDATE Orders SET Status = 'Approved' WHERE Id = @OrderId", new Dictionary<string, object> { { "@OrderId", orderId } });

        public bool DeleteOrder(int orderId) => ExecuteNonQuery("DELETE FROM Orders WHERE Id = @OrderId", new Dictionary<string, object> { { "@OrderId", orderId } });

        public DataTable SearchOrders(string keyword)
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM Orders WHERE Status LIKE @Keyword";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Keyword", $"%{keyword}%");
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
                MessageBox.Show($"[Error] {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                CloseConnection();
            }
        }

        // quản lý vấn đề thanh toán 
        // thêm mới phương thức thanh toán vào nền tảng 
        // lấy ra các phương thức thanh toàn hiện có 
        // chỉnh sửa 
        // xóa 
        public bool AddPaymentMethod(PaymentMethod paymentMethod) => ExecuteNonQuery("INSERT INTO PaymentMethods (Name, Description) VALUES (@Name, @Description)", new Dictionary<string, object> { { "@Name", paymentMethod.Name }, { "@Description", paymentMethod.Description } });

        public DataTable GetAllPaymentMethods()
        {
            try
            {
                OpenConnection();
                string query = "SELECT * FROM PaymentMethods";
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

        public bool UpdatePaymentMethod(PaymentMethod paymentMethod) => ExecuteNonQuery("UPDATE PaymentMethods SET Name = @Name, Description = @Description WHERE Id = @Id", new Dictionary<string, object> { { "@Id", paymentMethod.Id }, { "@Name", paymentMethod.Name }, { "@Description", paymentMethod.Description } });

        public bool DeletePaymentMethod(int paymentMethodId) => ExecuteNonQuery("DELETE FROM PaymentMethods WHERE Id = @Id", new Dictionary<string, object> { { "@Id", paymentMethodId } });
        // quản lý vấn đề liên quan đến nền tảng 
        // thay thế logo
        // thay thế banner 
        // cấu hình hệ thống
        public bool UpdatePlatformConfig(PlatformConfig config) => ExecuteNonQuery("UPDATE PlatformConfig SET Logo = @Logo, Banner = @Banner, SystemConfig = @SystemConfig", new Dictionary<string, object> { { "@Logo", config.Logo }, { "@Banner", config.Banner }, { "@SystemConfig", config.SystemConfig } });

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

    }
}
