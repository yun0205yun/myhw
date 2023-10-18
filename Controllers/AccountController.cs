using Dapper;
using myhw.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myhw.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";

        public bool LoginIsSuccessful { get; private set; }

        public ActionResult Log()
        {
            var model = new LogViewModel();

            var rememberMeCookie = Request.Cookies["RememberMe"];
            if (rememberMeCookie != null)
            {
                model.Username = Decrypt(rememberMeCookie.Value);
                model.RememberMe = true;
            }

            if (TempData.Peek("LoginIsSuccessful") != null && (bool)TempData["LoginIsSuccessful"])
            {
                return RedirectToAction("Front");
            }

            return View(model);
        }

        private string Decrypt(string encryptedText)
        {
            // 在這裡實現解密邏輯，與加密邏輯相對應
            // 注意：實際上，應該使用安全的解密算法，這裡僅作為示例
            return encryptedText; // 這裡示範一個簡單的假解密，實際應用中應使用更安全的方法
        }

        public ActionResult Register()
        {
            // 你的動作方法邏輯
            return View();
        }




        private bool IsLoginSuccessful(LogViewModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = "SELECT Password FROM Users WHERE Username = @Username";
                    var storedPassword = connection.QuerySingleOrDefault<string>(query, new { Username = model.Username });

                    if (storedPassword != null)
                    {
                        if (VerifyPassword(model.Password, storedPassword))
                        {
                            return true;
                        }
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
        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // 在這裡實現密碼比對邏輯，使用適當的加密或雜湊算法
            // 以下僅作為示例，不建議在實際應用中使用此方式
            return enteredPassword == storedPassword;
        }


        [HttpPost]
        public ActionResult Log(LogViewModel model)
        {
            if (IsLoginSuccessful(model))
            {
                TempData["LoginIsSuccessful"] = true;

                if (model.RememberMe)
                {
                    SetRememberMeCookie(model.Username);
                }

                TempData.Keep("LoginIsSuccessful");

                return RedirectToAction("Front");
            }

            ModelState.AddModelError("", "登入失敗，請檢查帳號密碼");
            return View(model);
        }


        // 登入成功的頁面
        public ActionResult Front()
        {
            return View("Front");

        }

        private void SetRememberMeCookie(string username)
        {
            // 使用 cookie 來存儲帳號
            HttpCookie cookie = new HttpCookie("RememberMe");
            cookie.Value = username;
            cookie.Expires = DateTime.Now.AddMonths(1); // 設定 cookie 的過期時間
            Response.Cookies.Add(cookie);
        }

        private string Encrypt(string text)
        {
            // 在這裡實現加密邏輯，例如使用 ASP.NET 的 MachineKey 加密
            // 注意：實際上，應該使用安全的加密算法，這裡僅作為示例
            return text; // 這裡示範一個簡單的假加密，實際應用中應使用更安全的方法
        }

        // 註冊
        private bool RegisterViewModel(string username, string password)
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
                Console.WriteLine($"註冊失敗: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine($"InnerException: {ex.InnerException?.ToString()}");
                return false;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 在這裡實現註冊邏輯
                bool registrationSuccessful = RegisterViewModel(model.Username, model.Password);

                if (registrationSuccessful)
                {
                    // 註冊成功，可以導向到其他頁面或執行其他操作
                    return RedirectToAction("RegisterSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "註冊失敗，可能是因為使用者名稱已存在。");

                }
            }

            // 如果 ModelState 無效，或註冊失敗，返回註冊頁面並顯示錯誤
            return View(model);
        }
    }
}
