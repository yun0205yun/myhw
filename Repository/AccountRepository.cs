using Dapper;
using myhw.Models;
using System;
using System.Data.SqlClient;

namespace myhw.Repository
{
    public class AccountRepository
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";

        // 檢查登入是否成功
        public bool? IsLoginSuccessful(LogViewModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT Password FROM Users WHERE Username = @Username";
                    var storedPassword = connection.QuerySingleOrDefault<string>(query, new { Username = model.Username });

                    // 驗證輸入的密碼
                    if (storedPassword != null && VerifyPassword(model.Password, storedPassword))
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"登入失敗: {ex.Message}");
                return false;
            }
        }

        // 密碼驗證邏輯
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }

        // 處理使用者註冊
        public string RegisterViewModel(string username, string password, string email)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // 檢查使用者名稱是否已存在
                    var checkIfExistsSql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                    var existingUserCount = connection.ExecuteScalar<int>(checkIfExistsSql, new { Username = username });

                    if (existingUserCount > 0)
                    {
                        return "已登記";
                    }

                    // 將使用者資料插入資料庫
                    var insertUserSql = "INSERT INTO Users (Username, Password, Email) VALUES (@Username, @Password, @Email)";
                    connection.Execute(insertUserSql, new { Username = username, Password = password, Email = email });

                    return "註冊成功";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"註冊失敗: {ex.Message}");
                return "註冊失敗";
            }
        }
    }
}
