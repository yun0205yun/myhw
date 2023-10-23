using myhw.Models;
using myhw.Repository;
using System;
using System.Collections.Generic;

namespace myhw.Service
{
    public class MessageService
    {
        private readonly MessageRepository _repository;

        public MessageService()
        {
            _repository = new MessageRepository();
        }

        public List<MessageDataModel> GetAllMessages(MemoryDataModel model)
        {
           
            try
            {
                return _repository.GetAllMessages(model);
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

        public void AddMessage(MessageDataModel message)
        {
            try
            {
                message.Timestamp = DateTime.Now;
                _repository.AddMessage(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddMessage: {ex.Message}");
                
            }
        }
        public MessageDataModel GetMessageById(int userId) 
        {
            try
            {
                return _repository.GetMessageById(userId);
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

        public void DeleteMessage(int userId)
        {
            try
            {
                // 從資料庫中刪除留言
                _repository.DeleteMessage(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteMessage: {ex.Message}");
            }
        }

    }
}
