﻿using Dapper;
using myhw.Models;
using myhw.Service;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace myhw.Repository
{
    public class AccountRepository
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        public AccountRepository()
        {
             
        }
        // 檢查登入是否成功
        public MemoryDataModel IsLoginSuccessful(LogViewModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT * FROM Users WHERE Username = @Username";
                    var dbData = connection.QuerySingleOrDefault<UserDataModel>(query, new { Username = model.Username });

                    // 驗證輸入的密碼
                    if (dbData?.Password != null && VerifyPassword(model.Password, dbData.Password))
                    {
                        return new MemoryDataModel
                        {
                            Password = dbData.Password,
                            IsLoginSuccessful = true,
                            UserId=dbData.UserId,
                            Username = model.Username,

                        };
                    }

                    return new MemoryDataModel
                    {
                        IsLoginSuccessful = false,
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"登入失敗: {ex.Message}");

                // 記錄錯誤日誌
                ErrorLog.LogError($"Login failed. Exception: {ex.Message}");

                return new MemoryDataModel
                {
                    IsLoginSuccessful = false,
                };
            }
        }

        // 密碼驗證邏輯
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            return enteredPassword == storedPassword;
        }

        // 處理使用者註冊
        public string RegisterViewModel(string username, string password)
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
                    connection.Execute(insertUserSql, new { Username = username, Password = password, Email = "沒改資料庫(db is not null)" });



                    return "註冊成功";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"註冊失敗: {ex.Message}");

                // 記錄錯誤日誌
                ErrorLog.LogError($"Registration failed. Exception: {ex.Message}");

                return "註冊失敗";
            }
        }
    }
}
