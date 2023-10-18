/*using System;
using System.Data.SqlClient;
using Dapper;

public class RememberRepository
{
    private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";

    public string RegisterUser(string username, string password)
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
                    // 使用者已存在
                    return "已經有登記了";
                }

                // 將使用者插入資料庫
                var insertUserSql = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                connection.Execute(insertUserSql, new { Username = username, Password = password });

                // 註冊成功
                return "註冊成功";
            }
        }
        catch (Exception ex)
        {
            // 記錄例外狀況詳細資訊
            Console.WriteLine($"註冊失敗: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            Console.WriteLine($"InnerException: {ex.InnerException?.ToString()}");

            // 註冊失敗
            return "註冊失敗";
        }
    }
}*/

