using myhw.Helpers;
using myhw.Models;
using myhw.Repository;
using myhw.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI.WebControls;

public class MessageController : Controller
{
    private readonly MessageService _messageService;


    public MessageController(MessageService messageService)
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
    public ActionResult Create(CreateModel createModel)
    {

        if (Session["Username"] == null)
        {
            return RedirectToAction("Log", "Account");
        }
        try
        {
            int UserId =(int) Session["UserId"];

            // 建立一個新的留言物件
            var newMessage = new CreateModel
            {    
                UserId = UserId,
                Content = createModel.Content,
                Timestamp = DateTime.Now
            };

            // 調用 _messageService.AddMessage 方法將留言添加到數據庫
            _messageService.AddMessage(newMessage);

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
            if (Session["UserId"] != null)
            {
                var message = _messageService.GetMessageByName(ContentId);

                if (message == null || Session["UserId"].ToString() != message.UserId.ToString())
                {
                    return Json(new ApiResponse<string> { Success = false, Message = "没有權限更新留言" });
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
                return Json(new ApiResponse<string> { Success = false, Message = "用戶未登入" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in UpdateMessage action: {ex.Message}");
            return Json(new ApiResponse<string> { Success = false, Message = "更新留言失敗" });
        }
    }

    [HttpPost]
    public ActionResult DeleteMessage(int ContentId)
    {
        try
        {
            if (Session["UserId"] != null)
            {
                // 根據contentId得到消息
                var message = _messageService.GetMessageByContentId(ContentId);
                
               
                     
                    // 檢查用戶是否有權刪除消息
                    if (message!=null&&Session["UserId"].ToString() == message.UserId.ToString())
                    {
                        // 刪除消失
                        _messageService.DeleteMessage(ContentId);
                        return Json(new ApiResponse<int> { Success = true, Message = "刪除留言成功", Data = ContentId });

     
                    }
                    else
                    {
                        return Json(new ApiResponse<string> { Success = false, Message = "没有權限刪除留言" });
                    }
                 
            }
            else
            {
                return Json(new ApiResponse<string> { Success = false, Message = "用戶未登入" });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteMessage 操作發生錯誤: {ex.Message}");
            return Json(new ApiResponse<string> { Success = false, Message = "刪除留言失敗" });
        }
    }




}