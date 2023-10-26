using Dapper;
using myhw.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.SessionState;
using System.Web;

namespace myhw.Repository
{
    public class MessageRepository
    {
        private readonly string _connectionString;

        public MessageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
 

        public List<MessageDataModel> GetAllMessages(string username)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                       SELECT Content.UserId, Users.Username, Content.Content, Users.CreatedAt
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
                                   "WHERE UserName = @Name";

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