// MessageRepository.cs
using Dapper;
using myhw.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace myhw.Repository
{
    public class MessageRepository
    {
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";

        public MessageRepository( )
        {
        }

        public List<MessageDataModel> GetAllMessages(MemoryDataModel model)
        {
          
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Content.UserId , Content.Username , Users.Email , Content.Content , Users.CreatedAt as time FROM Content JOIN Users ON Content.UserId = Users.UserId WHERE Users.Username = @Username;";
                    var storedPName = connection.Query<MessageDataModel>(query, new { Username = model.Username }).ToList();
                    return storedPName;
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error in GetAllMessages: {ex.Message}");
                return new List<MessageDataModel>();
            }
        }


        public List<MessageDataModel> GetMessagesByName(string name)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Content WHERE UserName LIKE @Name";

                    // 使用 Dapper 的 Query 方法，直接映射到 MessageDataModel
                    var messages = connection.Query<MessageDataModel>(query, new { Name = "%" + name + "%" }).ToList();

                    return messages;
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error in GetMessagesByName: {ex.Message}");
                return new List<MessageDataModel>();
            }
        }


        public void AddMessage(MessageDataModel message)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Content(Username, Email, Content, Timestamp) " +
                                   "VALUES ((SELECT UserId FROM Users WHERE Username = @Username),@Username, @Email, @Content, @Timestamp)";

                    // 使用 Dapper 的 Execute 方法
                    connection.Execute(query, new
                    {
                        message.Username,
                        message.Email,
                        message.Content,
                        message.Timestamp
                    });
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                Console.WriteLine($"Error in AddMessage: {ex.Message}");
            
            }
        }
        public MessageDataModel GetMessageById(int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Content WHERE UserId = @UserId";
                    var message = connection.QueryFirstOrDefault<MessageDataModel>(query, new { UserId = userId });

                    return message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMessageById: {ex.Message}");
                return null;
            }
        }

        public void UpdateMessage(MessageDataModel message)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Content SET Content = @Content WHERE UserId = @UserId";
                    connection.Execute(query, new { message.Content, message.UserId });
                }
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Content WHERE UserId = @UserId";
                    connection.Execute(query, new { UserId = userId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteMessage: {ex.Message}");
            }
        }

    }
}
