using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using myhw.Models;
using myhw.Repository;

namespace myhw.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountRepository _repository = new AccountRepository();

        public ActionResult Log()
        {
            var model = new LogViewModel();

            // 檢查是否存在 "RememberMe" Cookie 並相應地設置模型
            var rememberMeCookie = Request.Cookies["RememberMe"];
            if (rememberMeCookie != null)
            {
                model.RememberMe = true;
            }

            // 檢查是否在 Session 中存在成功登入的標誌
            var loginIsSuccessful = Session["LoginIsSuccessful"] as bool?;
            if (loginIsSuccessful != null && loginIsSuccessful.Value)
            {
                // 清除 Session 中的標誌
                Session.Remove("LoginIsSuccessful");
                return RedirectToAction("Front", "Message");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Log(LogViewModel model)
        {

            // 檢查登入是否成功
            var user = _repository.IsLoginSuccessful(model);

            if (user.IsLoginSuccessful)
            {
                // 在 Session 中設置使用者資訊
                Session["UserId"] = user.UserId; // 假設您的 User 對象中有一個 UserId 屬性
                // 在 Session 中設置使用者資訊
                Session["Username"] = model.Username; // 請根據 User 物件中的實際屬性進行替換

                // 如果勾選了 "RememberMe"，則設置 Cookie
                if (model.RememberMe)
                {
                    SetRememberMeCookie(model.Username);
                }

                // 重定向到 "Front"
                return RedirectToAction("Front", "Message");
            }

            // 如果登入失敗，則添加模型錯誤
            ModelState.AddModelError("", "登入失敗，請檢查帳號密碼");
            return View(model);
        }



        public ActionResult Register()
        {
            // 初始化視圖模型
            var viewModel = new RegisterViewModel();

            // 你的動作方法邏輯

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 嘗試註冊使用者
                string registrationStatus = _repository.RegisterViewModel(model.Username, model.Password, model.Email);

                if (registrationStatus == "註冊成功")
                {
                    // 註冊成功，重定向到登入頁面
                    return RedirectToAction("Log");
                }
                else
                {
                    // 如果有註冊錯誤，顯示錯誤
                    ModelState.AddModelError("", registrationStatus);
                }
            }

            // 如果 ModelState 無效或註冊失敗，返回帶有錯誤的視圖
            return View(model);
        }
        private void SetRememberMeCookie(string username)
        {
            // 使用 Cookie 存儲使用者名稱
            var cookie = new HttpCookie("RememberMe")
            {
                Value = username,
                Expires = DateTime.Now.AddMonths(1)
            };

            Response.Cookies.Add(cookie);
        }

        public ActionResult Front()
        {
            // 檢查使用者是否已經登入
            if (Session["Username"] != null)
            {
                // 使用者已經登入，顯示 "Front" 視圖
                return RedirectToAction("Front", "Message");
            }
            else
            {
                // 使用者尚未登入，重定向到登入頁面
                return RedirectToAction("Log", "Account");
            }
        }






    }
}