using Dapper;
using myhw.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.Web;
using System.Diagnostics;
using myhw.Service;
using NPOI.SS.Formula.Functions;

namespace myhw.Repository
{
    public class MessageRepository
    {
        private readonly string _connectionString;

        private readonly ErrorLogService _errorLogService;


        public MessageRepository(string connectionString)
        {
            _connectionString = connectionString;
            _errorLogService = new ErrorLogService(connectionString);
        }


        public List<MessageDataModel> GetAllMessages(string username)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                       SELECT ContentId,Content.UserId, Users.Username, Content.Content, Users.CreatedAt
                        FROM Content
                        JOIN Users ON Content.UserId = Users.UserId";

                    var messages = connection.Query<MessageDataModel>(query, new { Username = username }).ToList();
                    return messages;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllMessages: {ex.Message}");
                return new List<MessageDataModel>();
            }
        }

        //分頁利用offset和fetch子句
        public List<MessageDataModel> GetPagedMessages(int? page, int pageSize)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT ContentId, Username, Email, Content, CreatedAt as Time
                        FROM Content
                        JOIN Users ON Content.UserId = Users.UserId
                        ORDER BY ContentId 
                        OFFSET @Offset ROWS
                        FETCH NEXT @PageSize ROWS ONLY;
                    ";

                    int offset = ((page ?? 1) - 1) * pageSize;

                    var messages = connection.Query<MessageDataModel>(query, new { Offset = offset, PageSize = pageSize }).ToList();

                    // 偵錯信息
                    Console.WriteLine($"Query: {query}");
                    Console.WriteLine($"Offset: {offset}, PageSize: {pageSize}");
                    Console.WriteLine($"Retrieved {messages.Count} messages.");

                    return messages;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPagedMessages: {ex.Message}");
                _errorLogService.LogError($"Error in GetPagedMessages: {ex.Message}");
                return new List<MessageDataModel>();
            }
        }




        public List<MessageDataModel> GetMessagesByName(string name)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // 明確指定列的順序，確保與 MessageDataModel 類型的屬性順序一致
                    string query = "SELECT ContentId, Username, Email, Content, CreatedAt as Time  FROM Content " +
                                   "JOIN Users ON Content.UserId = Users.UserId " +
                                   "WHERE UserName = @Name";//這裡要加ContentId

                    var messages = connection.Query<MessageDataModel>(query, new { Name = name }).ToList();
                    return messages;
                }
            }
            catch (Exception ex)
            {
                // 記錄異常信息
                Console.WriteLine($"Error in GetMessagesByName: {ex.Message}");
                // 可以將異常拋出，讓上層調用者處理，也可以返回空列表，視情況而定
                throw;
            }
        }



        public void AddMessage(CreateModel message)
        {
            try
            {
                // 使用一個複合的 SQL 查詢，直接從 Users 表中獲取 UserId 並插入留言
                string insertQuery = @"
                    INSERT INTO Content(UserId, Content) VALUES (@UserId,@Content)";

                // 執行 SQL 查詢並獲取受影響的行數
                using (var connection = new SqlConnection(_connectionString))
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
                Console.WriteLine($"在 AddMessage 中出錯: {ex.Message}");

                // 記錄錯誤日誌
                _errorLogService.LogError($"AddMessage failed. Exception: {ex.Message}");
            }
        }



        public MessageDataModel GetMessageByName(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Content WHERE UserId = @UserId";
                    var message = connection.QueryFirstOrDefault<MessageDataModel>(query, new { UserId = userId });

                    return message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMessageByName: {ex.Message}");
                return null;
            }
        }
        public MessageDataModel GetMessageByContentId(int ContentId)
        {
            try
            {

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Content WHERE ContentId = @ContentId";
                    var message = connection.QueryFirstOrDefault<MessageDataModel>(query, new { ContentId = ContentId });

                    return message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMessageByContentId: {ex.Message}");
                return null;
            }
        }
         

        public void UpdateMessage(MessageDataModel message)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Content SET Content = @Content WHERE ContentId = @ContentId";

                    connection.Execute(query, new { message.Content, message.ContentId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateMessage: {ex.Message}");
            }
        }

        public void DeleteMessage(int contentId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Content WHERE ContentId = @ContentId";
                    connection.Execute(query, new { ContentId = contentId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteMessage: {ex.Message}");
            }
        }
    }
}