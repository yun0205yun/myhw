using MvcPaging;
using myhw.Helpers;
using myhw.Models;
using myhw.Service;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static myhw.Repository.MessageRepository;

public class MessageController : Controller
{
    private readonly MessageService _messageService;

    

    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }
    public MessageController()
    {
        _messageService = new MessageService();
    }

    // 登出
    public ActionResult Logout()
    {
        Session.Clear();
        return RedirectToAction("Log", "Account");
    }

    // 前台顯示留言
    public ActionResult Front(string name, int? page)
    { 
        const int PageSize = 5;

        if (Session["Username"] == null)
        {
            return RedirectToAction("Log", "Account");
        }

        var username = Session["Username"].ToString();

        PagedMessagesResult paginatedMessages;
        IPagedList<MessageDataModel> pages;//IPagedList<T>代表一個分頁的集合，它包含了分頁的相關信息

        try
        {
            int correctedPage = page??1;
            correctedPage = Math.Max(1, correctedPage); // 如果是 null 或小於 1，設置為 1
            ViewBag.name = name;//加這個讓front可以呈現name=15
            if (string.IsNullOrEmpty(name))
            {
                paginatedMessages = _messageService.GetPagedMessages(correctedPage, PageSize);
            }
            else
            {
                paginatedMessages = _messageService.GetMessagesByName(name, correctedPage, PageSize);
            }


            if (paginatedMessages.Messages != null)
            {   //分頁
                pages = paginatedMessages.Messages.ToPagedList(correctedPage, PageSize, paginatedMessages.TotalMessages);
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_ProductGrid", pages);
                }
                else
                {
                    return View(pages);
                }
            }
            else
            {
                return PartialView("_ProductGrid", new PagedList<MessageDataModel>(new List<MessageDataModel>(), correctedPage, PageSize));
            }
        }
        catch (Exception ex)
        {
            // 處理異常情況（記錄或顯示錯誤消息）
            Console.WriteLine($"Front 操作出錯: {ex.Message}");
            ModelState.AddModelError("", "無法顯示留言。");
            return View(new PagedList<MessageDataModel>(new List<MessageDataModel>(), 1, PageSize));
        }
    }

    // 創建留言頁面
    public ActionResult Create()
    {
        return View();
    }

    // 創建留言
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
                CreatedAt = DateTime.Now
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

    // 更新留言
    [HttpPost]
    public ActionResult UpdateMessage(int ContentId, string content)
    {
        try
        {
            // 檢查是否有使用者登錄
            if (Session["UserId"] != null)
            {
                var message = _messageService.GetMessageByContent(ContentId);

                if (message == null || Session["UserId"].ToString() != message.UserId.ToString())
                {
                    return Json(new ApiResponse<string> { Success = false, Message = "没有權限更新留言" });
                }
                else
                {
                    message.Content = content;
                    _messageService.UpdateMessage(message);

                    // 返回統一的響應
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

    // 刪除留言
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
                    // 刪除消息
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