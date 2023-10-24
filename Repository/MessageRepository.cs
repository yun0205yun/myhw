using Dapper;
using myhw.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace myhw.Repository
{
    public class MessageRepository
    {
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";

        public List<MessageDataModel> GetAllMessages(string username)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Content.ContentId, Content.Username, Users.Email, Content.Content, Users.CreatedAt as Time FROM Content JOIN Users ON Content.Username = Users.Username";
                    var storedPName = connection.Query<MessageDataModel>(query, new { Username = username }).ToList();
                    return storedPName;
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Content WHERE UserName LIKE @Name";
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

        public void AddMessage(MessageDataModel message, string logInUsername)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = "INSERT INTO Content(Username, Content) " +
                                       "VALUES ((SELECT UserId FROM Users WHERE Username=@Username), @Username, @Content)";

                        connection.Execute(query, new
                        {
                            Username = logInUsername,
                            message.Content,
                        }, transaction);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in AddMessageWithTransaction: {ex.Message}");
                        transaction.Rollback();
                    }
                }
            }
        }

        public MessageDataModel GetMessageByName(int userId)
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
                Console.WriteLine($"Error in GetMessageByName: {ex.Message}");
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

                    string query = "UPDATE Content SET Content = @Content WHERE ContentId = @ContentId";

                    connection.Execute(query, new { message.Content, message.ContentId });
                }
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
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE FROM Content WHERE ContentId = @ContentId";
                    connection.Execute(query, new { ContentId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteMessage: {ex.Message}");
            }
        }

    }
}
