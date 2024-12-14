using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using Urban_Tide_Bus_Booking_Service_Application.Models;

namespace Urban_Tide_Bus_Booking_Service_Application.Repositories
{
    public class AccessUserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public AccessUserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<User> GetUsers()
        {
            var users = new List<User>();

            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT Username, Password, Roles FROM Users";  // Add Roles here

                using (var command = new OleDbCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new User
                        {
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString(),
                            Roles = reader["Roles"].ToString().Split(',').ToList()  // Split roles by comma
                        };
                        users.Add(user);
                    }
                }
            }

            return users;
        }


        public void AddUser(User user)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Users (Username, Password, Roles, [Contact no]) VALUES (@Username, @Password, @Roles, @ContactNo)";

                using (var command = new OleDbCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Roles", string.Join(",", user.Roles));
                    command.Parameters.AddWithValue("@ContactNo", user.ContactNumber);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (var connection = new OleDbConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = ? AND [Password] = ?";
                using (var command = new OleDbCommand(query, connection))
                {
                    command.Parameters.Add("@Username", OleDbType.VarChar).Value = username;
                    command.Parameters.Add("@Password", OleDbType.VarChar).Value = password;

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return true; // User exists
                        }
                    }
                }
            }
            return false; // User not found
        }
    }
}