using System;
using System.Web;
using System.Web.Mvc;
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

            // 如果登入成功，則重定向到 "Front"
            if (TempData.Peek("LoginIsSuccessful") != null && (bool)TempData["LoginIsSuccessful"])
            {
                return RedirectToAction("Front");
            }

            return View(model);
        }

        public ActionResult Register()
        {
            // 初始化視圖模型
            var viewModel = new RegisterViewModel();

            // 你的動作方法邏輯

            return View(viewModel);
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
            // 顯示 "Front" 視圖
            return View("Front");
        }

        [HttpPost]
        public ActionResult Log(LogViewModel model)
        {
            // 檢查登入是否成功
            var user = _repository.IsLoginSuccessful(model);

            if (user != null)
            {
                // 設置 TempData 表示成功登入
                TempData["LoginIsSuccessful"] = true;

                // 如果勾選了 "RememberMe"，則設置 Cookie
                if (model.RememberMe)
                {
                    SetRememberMeCookie(model.Username);
                }

                // 將使用者資訊存放在 Session 中
                Session["UserId"] = user.UserId;
                Session["Username"] = user.Username;
                Session["Email"] = user.Email;

                // 保留 TempData 並重定向到 "Front"
                TempData.Keep("LoginIsSuccessful");
                return RedirectToAction("Front");
            }

            // 如果登入失敗，則添加模型錯誤
            ModelState.AddModelError("", "登入失敗，請檢查帳號密碼");
            return View(model);
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
    }
}
