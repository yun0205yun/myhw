using MvcPaging;
using myhw.Models;
using myhw.Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static myhw.Repository.MessageRepository;

namespace myhw.Service
{
    public class MessageService
    {
        private readonly MessageRepository _repository;


        // 依賴注入，注入 MessageRepository  的實例
        public MessageService()
        {
            _repository =  new MessageRepository();
        }

        // 以連接字串為參數的建構函式，用於直接建立 MessageRepository 和 ErrorLogService 的實例
        public PagedMessagesResult GetPagedMessages(int? page, int pageSize)
        {
            try
            {

                return  _repository.GetPagedMessages(page ?? 1, pageSize);

               
                
            }
            catch (Exception ex)
            {
                HandleException(ex, "GetPagedMessages");
                ErrorLog.LogError($"Error in GetPagedMessages: {ex.Message}");
                return null;
            }
        }

 
        // 依照名稱獲取留言
        public List<MessageDataModel> GetMessagesByName(string name)
        {
            try
            {
                return _repository.GetMessagesByName(name);
            }
            catch (Exception ex)
            {
                HandleException(ex, "GetMessagesByName");
                return null;
            }
        }

        // 新增留言
        public void AddMessage(CreateModel message)
        {
            try
            {
                _repository.AddMessage(message);
            }
            catch (Exception ex)
            {
                HandleException(ex, "AddMessage");
            }
        }

        // 依照 ContentId 獲取留言
        public MessageDataModel GetMessageByName(int ContentId)
        {
            try
            {
                return _repository.GetMessageByName(ContentId);
            }
            catch (Exception ex)
            {
                HandleException(ex, "GetMessageByName");
                return null;
            }
        }

        // 依照 ContentId 獲取留言，若留言不存在，記錄到錯誤日誌中
        public MessageDataModel GetMessageByContentId(int ContentId)
        {
            try
            {
                var message = _repository.GetMessageByContentId(ContentId);

                if (message == null)
                {
                    Console.WriteLine($"ContentId 為 {ContentId} 的消息未找到");
                    // 記錄到錯誤日誌
                    ErrorLog.LogError($"ContentId 為 {ContentId} 的消息未找到");
                    message = new MessageDataModel(); // 改這裡
                }

                return message;
            }
            catch (Exception ex)
            {
                HandleException(ex, "GetMessageByContentId");
                return null;
            }
        }

        // 更新留言
        public void UpdateMessage(MessageDataModel message)
        {
            try
            {
                _repository.UpdateMessage(message);
            }
            catch (Exception ex)
            {
                HandleException(ex, "UpdateMessage");
            }
        }

        // 刪除留言
        public void DeleteMessage(int ContentId)
        {
            try
            {
                _repository.DeleteMessage(ContentId);
            }
            catch (Exception ex)
            {
                HandleException(ex, "DeleteMessage");
            }
        }

        // 處理異常的私有方法，將錯誤信息輸出到控制台並記錄到錯誤日誌中
        private void HandleException(Exception ex, string methodName)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}");
            ErrorLog.LogError($"Error in {methodName}: {ex.Message}");
        }
    }
}
