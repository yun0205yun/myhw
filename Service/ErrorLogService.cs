using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace myhw.Service
{
    public class ErrorLogService
    {
        private readonly string _connectionString;

        public ErrorLogService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void LogError(string errorMessage)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // 插入錯誤信息到 ErrorLog 表
                    string insertQuery = @"
                    INSERT INTO ErrorLog (ErrorMessage, LogTime)
                    VALUES (@ErrorMessage, @LogTime)";

                    connection.Execute(insertQuery, new
                    {
                        ErrorMessage = errorMessage,
                        LogTime = DateTime.Now,                       
                    });
                }
            }
            catch (Exception ex)
            {
                // 處理寫入錯誤日誌時可能發生的例外
                Console.WriteLine($"Error in LogError: {ex.Message}");
            }
        }
    }
}