using System;
using System.Data.SqlClient;
using Dapper;

public class RememberRepository
{
    private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";

    public bool RegisterUser(string username, string password)
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
                    // 使用者名稱已存在，不允許註冊
                    return false;
                }

                // 如果使用者名稱不存在，則將使用者資料插入資料庫
                var insertUserSql = "INSERT INTO Users (Username, Password) VALUES (@Username, @Password)";
                connection.Execute(insertUserSql, new { Username = username, Password = password });

                return true;
            }
        }
        catch (Exception ex)
        {
            // 處理例外狀況，例如記錄錯誤訊息或回應使用者註冊失敗
            Console.WriteLine($"註冊失敗: {ex.Message}");
            return false;
        }
    }
}
