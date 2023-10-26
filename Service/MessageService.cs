using myhw.Models;
using myhw.Repository;
using System;
using System.Collections.Generic;

namespace myhw.Service
{
    public class MessageService
    {
        private readonly MessageRepository _repository;

        public MessageService(MessageRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        public MessageService(string connectionString)
        {
             _repository = new MessageRepository(connectionString);
        }

        public List<MessageDataModel> GetAllMessages(string username, int? page, int pageSize)
        {
            try
            {
                return _repository.GetAllMessages(username);
            }
            catch (Exception ex)
            {
                //處理或記錄異常
                Console.WriteLine($"Error in GetAllMessages: {ex.Message}");
                return new List<MessageDataModel>();
            }
        }

        public List<MessageDataModel> GetMessagesByName(string name)
        {
            try
            {
                return _repository.GetMessagesByName(name);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in GetMessagesByName: {ex.Message}");
                return null;
            }
        }

        public void AddMessage(CreateModel message)
        {
            try
            {
 
                    // 調用相應的 _repository.AddMessage 方法，將 message 對象添加到數據庫
                    _repository.AddMessage(message );
                
            }
            catch (Exception ex)
            {
                HandleException(ex, "AddMessage");
            }
        }





        private void HandleException(Exception ex, string methodName)
        {
            // 在實際應用中，你可能希望使用日誌庫來記錄異常。
            Console.WriteLine($"Error in {methodName}: {ex.Message}");


        }


        public MessageDataModel GetMessageByName(int ContentId)
        {
            try
            {
                return _repository.GetMessageByName(ContentId);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error in GetMessagesByName: {ex.Message}");
                return null;
            }
        }
        public void UpdateMessage(MessageDataModel message)
        {
            try
            {
                // 更新資料庫中的留言
                _repository.UpdateMessage(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateMessage: {ex.Message}");
            }
        }


        public void DeleteMessage(int ContentId)
        {
            try
            {
                // 删除数据库中的留言
                _repository.DeleteMessage(ContentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteMessage: {ex.Message}");
            }
        }
    }
}