using myhw.Service;
using System.Collections.Generic;
using System.Web.Mvc;
using myhw.Models;
using System;
using System.Linq;

public class MessageController : Controller
{
    private readonly MessageService _messageService;

    public MessageController()
    {
        _messageService = new MessageService();
    }

    public ActionResult Front(string name)
    {
        if (Session["Username"] == null)
        {
            return RedirectToAction("Log", "Account");
        }

        // 確保使用者資訊存在於 Session 中
        var model = new MemoryDataModel { Username = Session["Username"].ToString() };

        List<MessageDataModel> messages;

        if (string.IsNullOrEmpty(name))
        {
            messages = _messageService.GetAllMessages(model);
        }
        else
        {
            messages = _messageService.GetMessagesByName(name);
        }

        return View(messages);
    }

    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(MessageDataModel message)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // 確保使用者資訊存在於 Session 中
                if (Session["Username"] != null)
                {
                    message.Username = Session["Username"].ToString();

                    // 使用 AddMessageToDb 將留言新增到資料庫
                    _messageService.AddMessage(message);
                    return RedirectToAction("Front", "Message");
                }
                else
                {
                    return RedirectToAction("Create", "Message");
                }
            }
            catch (Exception ex)
            {
                // 處理或記錄異常
                Console.WriteLine($"Error in Create action: {ex.Message}");
                ModelState.AddModelError("", "發生錯誤，無法創建消息。");
                return View(message);
            }
        }
       

        return View(message);
    }
    [HttpPost]
    public ActionResult UpdateMessage(int userId, string content)
    {
        try
        {
            // 根據 userId 更新資料庫中的留言內容
            var message = _messageService.GetMessageById(userId);
            if (message != null)
            {
                message.Content = content;
                _messageService.UpdateMessage(message);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        catch (Exception)
        {
            // 處理異常
            return Json(new { success = false });
        }
    }

    [HttpPost]
    public ActionResult DeleteMessage(int userId)
    {
        try
        {
            // 根據 userId 從資料庫中刪除留言
            _messageService.DeleteMessage(userId);
            return Json(new { success = true });
        }
        catch (Exception)
        {
            // 處理異常
            return Json(new { success = false });
        }
    }


}
