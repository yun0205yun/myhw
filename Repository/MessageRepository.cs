// MessageRepository.cs
using myhw.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace myhw.Repository
{
    public class MessageRepository
    {
        private readonly string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LOG;Integrated Security=True;";

        public MessageRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<MessageDataModel> GetAllMessages()
        {
            List<MessageDataModel> messages = new List<MessageDataModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT Content.UserId as 編號, Content.Username as 姓名, Users.Email as Email, Content.Content as 留言,Users.CreatedAt as 時間\r\nFROM Content\r\nJOIN Users ON Content.UserId = Users.UserId\r\nWHERE Users.Username = Content.Username;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MessageDataModel message = new MessageDataModel
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Username = Convert.ToString(reader["UserName"]),
                                Email = Convert.ToString(reader["Email"]),
                                Content = Convert.ToString(reader["Content"]),
                                Timestamp = Convert.ToDateTime(reader["Timestamp"])
                            };

                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }

        public List<MessageDataModel> GetMessagesByName(string name)
        {
            List<MessageDataModel> messages = new List<MessageDataModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM Content WHERE UserName LIKE @Name";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", "%" + name + "%");

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MessageDataModel message = new MessageDataModel
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Username = Convert.ToString(reader["UserName"]),
                                Email = Convert.ToString(reader["Email"]),
                                Content = Convert.ToString(reader["Content"]),
                                Timestamp = Convert.ToDateTime(reader["Timestamp"])
                            };

                            messages.Add(message);
                        }
                    }
                }
            }

            return messages;
        }

        public void AddMessage(MessageDataModel message)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "INSERT INTO Content(Username, Email, Content, Timestamp) " +
                               "VALUES (@Username, @Email, @Content, @Timestamp)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", message.Username);
                    command.Parameters.AddWithValue("@Email", message.Email);
                    command.Parameters.AddWithValue("@Content", message.Content);
                    command.Parameters.AddWithValue("@Timestamp", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
