using MvcPaging;
using myhw.Helpers;
using myhw.Models;
using myhw.Service;
using System;
using System.Web.Mvc;
using static myhw.Repository.MessageRepository;

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
        const int PageSize = 5;

        if (Session["Username"] == null)
        {
            return RedirectToAction("Log", "Account");
        }

        var username = Session["Username"].ToString();

        PagedMessagesResult paginatedMessages;
        IPagedList<MessageDataModel> pages;
        if (string.IsNullOrEmpty(name))
        {
            // 調用 GetPagedMessages 方法
            paginatedMessages = _messageService.GetPagedMessages(page,PageSize);
            pages = paginatedMessages.Messages.ToPagedList(page.GetValueOrDefault(), PageSize, paginatedMessages.TotalMessages);
             
        }
        else
        {
            // 調用 GetMessagesByName 方法
            var messages = _messageService.GetMessagesByName(name);
            pages =messages.ToPagedList(page.GetValueOrDefault(), PageSize, messages.Count);
             
        }

        return View(pages);
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
            int UserId = (int)Session["UserId"];

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
            ErrorLog.LogError($"Error in MessageController: {ex.Message}");
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
            ErrorLog.LogError($"Error in UpdateMessage action: {ex.Message}");
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
                if (message != null && Session["UserId"].ToString() == message.UserId.ToString())
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
            ErrorLog.LogError($"Error in UpdateMessage action: {ex.Message}");
            Console.WriteLine($"DeleteMessage 操作發生錯誤: {ex.Message}");
            return Json(new ApiResponse<string> { Success = false, Message = "刪除留言失敗" });
        }
    }




}
