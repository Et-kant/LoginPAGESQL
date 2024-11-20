using Microsoft.AspNetCore.WebUtilities;
using MySqlConnector;
using System.IO.Pipelines;
using System.Security.Cryptography;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Utils;

namespace WebApplication1.Services
{
    public class UserServices
    {
        private readonly DbConnection _db;

        public UserServices(DbConnection db)
        {
            _db = db;
        }

        public bool Register(string username, string email, string password)
        {
            var HashedPassword = PasswordHasher.HashPassword(password);

            try
            {
                using (var conn = _db.GetConnection())
                {

                    conn.Open();
                    string query = "INSERT INTO Users (Username, Email, PasswordHash, CreatedAt) VALUES (@username, @email, @passwordHash, @createdAt)";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@passwordHash", HashedPassword);
                        cmd.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                        return cmd.ExecuteNonQuery() > 0;
                    }


                }
            }
            catch (Exception)
            {
                return false;
            }
        }


        public User Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            try
            {
                using (var conn = _db.GetConnection())
                {

                    conn.Open();
                    string query = "SELECT Id, Username, Email, PasswordHash, CreatedAt FROM Users WHERE Email = @email";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read()) return null;

                            var storedHash = reader.GetString("PasswordHash");

                            if (!PasswordHasher.VerifyPassword(storedHash, password))
                            {
                                return null;
                            }

                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                Username = reader.GetString("Username"),
                                Email = reader.GetString("Email"),
                                CreatedAT = reader.GetDateTime("CreatedAT")
                            };
                        }
                    }

                }
            }
            catch (Exception)
            {
                return null;
            }
        }

    }

}