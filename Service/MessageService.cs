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

        public void AddMessage(MessageDataModel message,string logInUsername)
        {
            try
            { 
                //設置留言的用戶名
               // message.Username  = logInUsername;
                message.Timestamp = DateTime.Now;
                _repository.AddMessage(message,logInUsername);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddMessage: {ex.Message}");
                
            }
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
