using myhw.Service;
using System.Collections.Generic;
using System.Web.Mvc;
using myhw.Models;
using System;

public class MessageController : Controller
{
    private readonly MessageService _messageService;

    public MessageController(MessageService messageService)
    {
        _messageService = messageService;
    }

    public ActionResult Front(string name)
    {
        IEnumerable<MessageDataModel> messages;

        if (string.IsNullOrEmpty(name))
        {
            messages = _messageService.GetAllMessages();
        }
        else
        {
            messages = _messageService.GetMessagesByName(name);
        }

        return View(messages);
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
            message.Timestamp = DateTime.Now;
            _messageService.AddMessage(message);
            return RedirectToAction("Front");
        }

        return View(message);
    }

}
