using myhw.Helpers;
using myhw.Models;
using myhw.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;

public class MessageController : Controller
{
    private readonly MessageService _messageService;
    

    public MessageController(MessageService messageService  )
    {
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
    }
    public MessageController()
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";
        _messageService = new MessageService(connectionString);
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
    public ActionResult Create(CreateModel createModel, HttpSessionState session)
    {
        if (Session["UserId"] == null)
        {
            return RedirectToAction("Log", "Account");
        } 
        try
        {
             
                // 獲取已登錄使用者的名稱
                string username = session["Username"].ToString();
 

                // 建立一個新的留言物件
                var newMessage = new CreateModel
                {
                    // 不需要再指定 UserId，因為您已經從已登錄的使用者中取得
                    Content = createModel.Content,
                    Timestamp = DateTime.Now
                };

                // 調用 _messageService.AddMessage 方法將留言添加到數據庫
                _messageService.AddMessage(newMessage, session);

                return RedirectToAction("Front", "Message");
            
        }
        catch (Exception ex)
        {
            // 在實際應用中，你可能還需要進一步的錯誤處理，例如記錄到日誌或發送通知
            Console.WriteLine($"Create 操作出錯: {ex.Message}");
            ModelState.AddModelError("", "無法創建留言。");
            // 如果有錯誤，可能需要重新渲染表單，帶有用戶輸入的值
            return View(createModel);
        }
    }


    [HttpPost]
    public ActionResult UpdateMessage(int ContentId, string content)
    {
        try
        {
            // 檢查是否有使用者登錄
            if (Session["Username"] != null)
            {
                var message = _messageService.GetMessageByName(ContentId);

                if (message == null || Session["Username"].ToString() != message.Username)
                {
                    return Json(new ApiResponse<string> { Success = false, Message = "没有权限更新留言" });
                }
                else
                {
                    message.Content = content;
                    _messageService.UpdateMessage(message);

                    // 返回统一的响应
                    return Json(new ApiResponse<string> { Success = true, Message = "更新成功", Data = content });
                }
            }
            else
            {
                return Json(new ApiResponse<string> { Success = false, Message = "用户未登录" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateMessage action: {ex.Message}");
            return Json(new ApiResponse<string> { Success = false, Message = "更新留言失败" });
        }
    }

    [HttpPost]
    public ActionResult DeleteMessage(int userId)
    {
        try
        {
            // 檢查是否有使用者登錄
            if (Session["Username"] != null)
            {
                var message = _messageService.GetMessageByName(userId);
                if (message != null && Session["Username"].ToString() == message.Username)
                {
                    _messageService.DeleteMessage(userId);
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "没有权限删除留言" });
                }
            }
            else
            {
                return Json(new ApiResponse<string> { Success = false, Message = "用户未登录" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteMessage action: {ex.Message}");
            return Json(new { success = false, message = "删除留言失败" });
        }
    }


}