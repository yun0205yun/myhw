using myhw.Models;
using PagedList;
using System.Web.Mvc;

internal class MessageViewModel  
{
    public string Username { get; set; }
    public IPagedList<MessageDataModel> Messages { get; set; }
}