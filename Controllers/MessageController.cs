using myhw.Models;
using myhw.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

public class MessageController : Controller
{
    private readonly MessageService _messageService;

    public MessageController()
    {
        _messageService = new MessageService();
    }

    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Log", "Account");
    }

    public ActionResult Front(string name, int? page)
    {
        const int pageSize = 5;

        if (Session["Username"] == null)
        {
            return RedirectToAction("Log", "Account");
        }

        // Ensure user information is in the Session
        var username = Session["Username"].ToString();

        List<MessageDataModel> messages;

        if (string.IsNullOrEmpty(name))
        {
            messages = _messageService.GetAllMessages(username, page ?? 1, pageSize);
        }
        else
        {
            messages = _messageService.GetMessagesByName(name);
        }

        var paginatedMessages = messages.ToPagedList(page ?? 1, pageSize);

        // 返回 PagedList.IPagedList<MessageDataModel>，而不是 MessageViewModel
        return View(paginatedMessages);
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
                if (Session["Username"] != null)
                {
                    string logInUsername = Session["Username"].ToString();
                    _messageService.AddMessage(message, logInUsername);

                    return RedirectToAction("Front", "Message");
                }
                else
                {
                    return RedirectToAction("Create", "Message");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create action: {ex.Message}");
                ModelState.AddModelError("", "无法创建留言。");
                return View(message);
            }
        }

        return View(message);
    }

    [HttpPost]
    public ActionResult UpdateMessage(int ContentId, string content)
    {
        try
        {
            var message = _messageService.GetMessageByName(ContentId); // 使用 contentId 获取消息

            if (message == null || Session["Username"] == null || Session["Username"].ToString() != message.Username)
            {
                return Json(new { success = false, message = "没有权限更新留言" });
            }
            else
            {
                message.Content = content;
                _messageService.UpdateMessage(message);
                return Json(new { success = true });
            }
        }
        catch (Exception)
        {
            // 处理异常
            return Json(new { success = false, message = "更新留言失败" });
        }
    }



    [HttpPost]
    public ActionResult DeleteMessage(int userId)
    {
        try
        {
            var message = _messageService.GetMessageByName(userId);
            if (message != null && Session["Username"] != null && Session["Username"].ToString() == message.Username)
            {
                _messageService.DeleteMessage(userId);
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "没有权限删除留言" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteMessage action: {ex.Message}");
            return Json(new { success = false, message = "删除留言失败" });
        }
    }

}
