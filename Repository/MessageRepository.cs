using Dapper;
using myhw.Models;
using myhw.Service;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;

namespace myhw.Repository
{
    public class MessageRepository
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";
        //分頁利用offset和fetch子句
        public PagedMessagesResult GetPagedMessages(int? page, int pageSize)
        {
            try
            {
                 

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                          SELECT ContentId, Content.UserId, Username, Email, Content, CreatedAt as Time,
                                 COUNT(*) OVER () as TotalMessages
                          FROM Content
                          JOIN Users ON Content.UserId = Users.UserId
                          ORDER BY ContentId 
                          OFFSET @Offset ROWS
                          FETCH NEXT @PageSize ROWS ONLY;";


                    int offset = ((page ?? 1) <= 0 ? 1 : (page ?? 1) - 1) * pageSize;//確保計算出的 offset 值不會小於 0

                    var messages = connection.Query<MessageDataModel>(query, new { Offset = offset, PageSize = pageSize }).ToList();

                    // 計算總留言
                    int totalMessages = messages?.FirstOrDefault()?.TotalMessages ?? 0;

                        return new PagedMessagesResult
                        {
                            CurrentPage = page ?? 1,
                            PageSize = pageSize,
                            TotalMessages = totalMessages,
                            Messages = messages
                        };

                     

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPagedMessages: {ex.Message}");
                ErrorLog.LogError($"Error in GetPagedMessages: {ex.Message}");
                return new PagedMessagesResult
                {
                    ErrorMessage = ex.Message,
                    ExceptionStackTrace = ex.StackTrace
                };
            }
        }

        public class PagedMessagesResult
        {
            public List<MessageDataModel> Messages { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
            public int TotalMessages { get; set; }
            public string ErrorMessage { get; set; }
            public string ExceptionStackTrace { get; set; }

        }

        // 根據用戶名稱獲取留言
        public PagedMessagesResult GetMessagesByName(string name, int? page, int pageSize)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // 明確指定列的順序，確保與 MessageDataModel 類型的屬性順序一致
                    string query = @"
                        SELECT ContentId, Content.UserId, Username, Email, Content, CreatedAt as Time,
                                COUNT(*) OVER () as TotalMessages
                        FROM Content
                        JOIN Users ON Content.UserId = Users.UserId
                        WHERE UserName = @Name
                        ORDER BY ContentId 
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY;";
                    int offset = ((page ?? 1) <= 0 ? 1 : (page ?? 1) - 1) * pageSize;
                    var messages = connection.Query<MessageDataModel>(query, new { Name = name, Offset = offset, PageSize = pageSize }).ToList();
                     
                    int totalMessages = messages?.FirstOrDefault()?.TotalMessages ?? 0;
                    
                    return new PagedMessagesResult
                    {
                        CurrentPage = page ?? 1,
                        PageSize = pageSize,
                        TotalMessages = totalMessages,
                        Messages = messages,
                        
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMessagesByName: {ex.Message}");
                ErrorLog.LogError($"Error in GetMessagesByName: {ex.Message}");
                return new PagedMessagesResult
                {
                    ErrorMessage = ex.Message,
                    ExceptionStackTrace = ex.StackTrace
                };
            }
        }


        // 新增留言
        public void AddMessage(CreateModel message)
        {
            try
            {
                string insertQuery = @"
                    INSERT INTO Content(UserId, Content) VALUES (@UserId,@Content)";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    int rowsAffected = connection.Execute(insertQuery, new
                    {
                        message.UserId,
                        message.Content
                    });

                    // 檢查是否成功插入留言
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Message added successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"未找到用戶名對應的用戶。");
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "AddMessage");
            }
        }
        
        // 根據用戶ID獲取留言
        public MessageDataModel GetMessageByContent(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Content WHERE UserId = @UserId";
                    var message = connection.QueryFirstOrDefault<MessageDataModel>(query, new { UserId = userId });

                    return message;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "GetMessageByName");
                return null;
            }
        }
       
        // 根據留言ID獲取留言
        public MessageDataModel GetMessageByContentId(int ContentId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Content WHERE ContentId = @ContentId";
                    var message = connection.QueryFirstOrDefault<MessageDataModel>(query, new { ContentId = ContentId });

                    if (message == null)
                    {
                        Console.WriteLine($"Message with ContentId {ContentId} not found.");
                        ErrorLog.LogError($"Message with ContentId {ContentId} not found.");
                        message = new MessageDataModel(); // 改這裡
                    }

                    return message;
                }
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
                using (var connection = new SqlConnection())
                {
                    connection.Open();

                    string query = "UPDATE Content SET Content = @Content WHERE ContentId = @ContentId";
                    connection.Execute(query, new { message.Content, message.ContentId });
                }
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
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Content WHERE ContentId = @ContentId";
                    connection.Execute(query, new { ContentId = ContentId });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "DeleteMessage");
            }
        }

        // 處理異常
        private void HandleException(Exception ex, string methodName)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}");
            ErrorLog.LogError($"Error in {methodName}: {ex.Message}");
        }
    }
}
