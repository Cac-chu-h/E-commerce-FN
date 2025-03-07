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
        public bool isLogin = false;
        public string logo;
        public int UserID { get; set; }
        public  string UserActive { get; set; }
        public string UserRole { get; set; }


        public BLL()
        {
            App app = new App();
            logo = app.logo;
            UserActive = "Active";
            UserRole = "Admin";
        }
        // những vấn đề liên quan đến tài khoản 

        public bool Login(string username, string password)
        {
            DataTable userTable = dataAccess.GetDataTable("SELECT * FROM account WHERE s_Username = @Username AND s_Password = @Password", new Dictionary<string, object>
            {
                {"@Username", username},
                {"@Password", password}
            });

            if (userTable.Rows.Count > 0)
            {
                DataRow userRow = userTable.Rows[0];
                UserID = Convert.ToInt32(userRow["i_Id"]);
                UserActive = userRow["s_Active"].ToString();
                UserRole = userRow["s_Role"].ToString();
                return true;
            }
            return false;
        }

        public bool AddAccount(string username, string email, string password)
        {
            
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                {"@Username", username},
                {"@Email", email},
                {"@Password", password},
                {"@Role", "User"},
                {"@Active", "1"},
                {"@Avatar", "default.jpg"},
                {"@DateCreated", DateTime.Now}
            };

            return dataAccess.ExecuteNonQuery("INSERT INTO account (s_Username, s_Email, s_Password, s_Role, s_Active, s_Avatar, d_DateCreated) VALUES (@Username, @Email, @Password, @Role, @Active, @Avatar, @DateCreated)", parameters);
        }

        public bool BanAccount(int id, string status)
        {
            if (UserRole == "Admin" && UserActive == "Active")
            {
                return dataAccess.ExecuteNonQuery($"UPDATE account SET s_Active = '{status}' WHERE i_Id = @Id", new Dictionary<string, object> { { "@Id", id } });
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public bool DeleteAccount(int id)
        {
            if (UserRole == "Admin" && UserActive == "Active")
            {
                return dataAccess.ExecuteNonQuery("DELETE FROM account WHERE i_Id = @Id", new Dictionary<string, object> { { "@Id", id } });
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public bool SetAdminAccount(int id, string role)
        {
            if (UserRole == "Admin" && UserActive == "Active")
            {
                return dataAccess.ExecuteNonQuery($"UPDATE account SET s_Role = '{role}' WHERE i_Id = @Id", new Dictionary<string, object> { { "@Id", id } });
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

        }

        public List<Account> ListAccounts()
        {
            
            if (UserRole == "Admin" && UserActive == "Active")
            {
                return dataAccess.GetAllUsers();
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
        }


        public List<Account> FindUser(string ids, string uname, string uemail, string tableName)
        {
            StringBuilder queryBuilder = new StringBuilder($"SELECT * FROM {tableName} WHERE 1=1");
            if (!string.IsNullOrEmpty(ids))
            {
                queryBuilder.Append(" AND i_Id = @Id");
            }
            if (!string.IsNullOrEmpty(uname))
            {
                queryBuilder.Append(" AND s_UserName LIKE @UserName");
            }
            if (!string.IsNullOrEmpty(uemail))
            {
                queryBuilder.Append(" AND s_Email LIKE @Email");
            }
            return dataAccess.FindUser(queryBuilder.ToString(), ids, uname, uemail);
        }


        // vấn đề liên quan đến danh mục 
        public List<Category> ListCategory()
        {
            
            if (UserRole == "Admin" && UserActive == "Active")
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
            
            if (UserRole == "Admin" && UserActive == "Active")
            {
                return dataAccess.ExecuteNonQuery("DELETE FROM category WHERE i_id = @Id", new Dictionary<string, object> { { "@Id", id } });
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }


        public bool UpdateCategory(Category value)
        {
            
            if (UserRole == "Admin" && UserActive == "Active")
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
           
            if (UserRole == "Admin" && UserActive == "Active")
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
        public Dictionary<int, object> ListProducts()
        {
            List<Product> products = new List<Product>();
            DataTable dt = dataAccess.GetAllProducts();
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    Product product = new Product()
                    {
                        Id = Convert.ToInt32(row["i_Id"]),
                        Name = row["s_ProductName"].ToString(),
                        Description = row["s_Description"]?.ToString() ?? string.Empty,
                        Image = row["s_ProductImage"]?.ToString() ?? string.Empty,
                        Total = Convert.ToInt32(row["i_Totals"] ?? 0),
                        Status = Convert.ToString(row["i_Status"]),
                        Rating = Convert.ToInt32(row["i_Rating"]),
                        dateAdd = Convert.ToDateTime(row["d_CreatedDate"]),
                        Price = Convert.ToDecimal(row["f_Price"]),
                        TotalPay = Convert.ToInt32(row["i_TotalPay"]),
                        CategoryId = Convert.ToInt32(row["i_CategoryID"])
                    };
                    products.Add(product);
                }
                catch (Exception ex)
                {
                    // Ghi log hoặc xử lý lỗi chuyển đổi từng dòng
                    MessageBox.Show($"Lỗi chuyển đổi dòng: {ex.Message}");
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
            
            if (UserRole == "Admin" && UserActive == "Active")
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
            
            if (UserRole == "Admin" && UserActive == "Active")
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
            
            if (UserRole == "Admin" && UserActive == "Active")
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
            StringBuilder queryBuilder = new StringBuilder("SELECT * FROM product WHERE 1=1");

            if (!string.IsNullOrEmpty(id))
            {
                queryBuilder.Append(" AND [i_id] = @Id");
            }
            if (!string.IsNullOrEmpty(name))
            {
                queryBuilder.Append(" AND [s_ProductName] LIKE @Name");
            }
            if (categoryId.HasValue && categoryId != -1)
            {
                queryBuilder.Append(" AND [i_CategoryID] = @CategoryId");
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
                        Id = Convert.ToInt32(row["id"]),
                        StartDate = Convert.ToDateTime(row["startDate"]),
                        EndDate = Convert.ToDateTime(row["endDate"]),
                        TypeOf = Convert.ToString(row["typeOf"]),
                        ValueOfVoucher = Convert.ToDecimal(row["valueOfVoucher"]),
                        categoryId = Convert.ToInt32(row["categoryId"]),
                        Status = Convert.ToString(row["i_Status"])
                    };
                    vouchers.Add(voucher);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi chuyển đổi dòng: {ex.Message}");
                }
            }

            Dictionary<int, object> result = new Dictionary<int, object>();
            result.Add(1, vouchers);
            result.Add(2, dt);
            result.Add(3, dataAccess.GetMaxVoucherId());

            return result;
        }

        public bool AddVoucher(Voucher value)
        {
            if (UserRole == "Admin" && UserActive == "Active")
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
            if (UserRole == "Admin" && UserActive == "Active")
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
            if (UserRole == "Admin" && UserActive == "Active")
            {
                return dataAccess.UpdateVoucher(value);
            }
            else
            {
                MessageBox.Show("Bạn không đủ quyền hạn truy cập mục này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        public List<Voucher> FindVoucher(string id, string name, int? cId)
        {
            StringBuilder queryBuilder = new StringBuilder("SELECT * FROM voucherModule WHERE 1=1");

            if (!string.IsNullOrEmpty(id))
            {
                queryBuilder.Append(" AND [id] = @Id");
            }
            if (!string.IsNullOrEmpty(name))
            {
                queryBuilder.Append(" AND [typeof] LIKE @Name");
            }
            if (cId.HasValue && cId != 0)
            {
                queryBuilder.Append(" AND [categoryId] = @CategoryId");
            }

            return dataAccess.FindVoucher(queryBuilder.ToString(), id.Trim(), name, cId);
        }

        // thêm xóa sửa
        // role user
        // xem 
        // tìm kiếm 


        // những vấn đề liên quan đến mua hàng 

        // những vấn đề liên quan đến 
    }
}
