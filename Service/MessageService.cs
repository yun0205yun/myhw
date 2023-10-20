using myhw.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using myhw.Models;

namespace myhw.Service
{
    public class MessageService
    {
        private readonly MessageRepository _repository;

        public MessageService(MessageRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<MessageDataModel> GetAllMessages()
        {
            return _repository.GetAllMessages();
        }

        public IEnumerable<MessageDataModel> GetMessagesByName(string name)
        {
            return _repository.GetMessagesByName(name);
        }

        public void AddMessage(MessageDataModel message)
        {
            message.Timestamp = DateTime.Now;
            _repository.AddMessage(message);
        }
    }
}