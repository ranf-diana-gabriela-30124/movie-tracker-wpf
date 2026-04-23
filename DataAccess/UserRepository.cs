using System;
using Microsoft.Data.Sqlite;
using TEMA1_II.DataModels;

namespace TEMA1_II.DataAccess
{
    public class UserRepository
    {
        private readonly DatabaseContext _db = new();

        public UserApp? GetUserByEmail(string email, string password)
        {
            try
            {
                using var conn = _db.GetConnection();
                conn.Open();
                var cmd = conn.CreateCommand();

                // Căutăm direct după email și parolă, curățate de spații
                cmd.CommandText = @"
            SELECT Id, Email, Username, Password, Role 
            FROM Users 
            WHERE LOWER(TRIM(Email)) = LOWER(TRIM($email)) 
            AND TRIM(Password) = TRIM($password) 
            LIMIT 1";

                cmd.Parameters.AddWithValue("$email", email ?? "");
                cmd.Parameters.AddWithValue("$password", password ?? "");

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new UserApp
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        Username = reader.GetString(2),
                        Password = reader.GetString(3),
                        Role = reader.IsDBNull(4) ? "User" : reader.GetString(4)
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DB ERROR] GetUserByEmail: {ex.Message}");
            }
            return null;
        }

        public void AddUser(UserApp user)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText =
            @"
    INSERT INTO Users (Email, Username, Password)
    VALUES ($email, $username, $password);
    ";

            cmd.Parameters.AddWithValue("$email", user.Email);
            cmd.Parameters.AddWithValue("$username", user.Username);
            cmd.Parameters.AddWithValue("$password", user.Password);

            int rows = cmd.ExecuteNonQuery();

            System.Diagnostics.Debug.WriteLine($"[DB] Insert user rows affected: {rows}");
        }

        public bool EmailExists(string email)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Email = $email";
            cmd.Parameters.AddWithValue("$email", email);

            // SQLite returnează long pentru COUNT
            var count = Convert.ToInt64(cmd.ExecuteScalar());
            return count > 0;
        }

        public bool UsernameExists(string username)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username=$username";
            cmd.Parameters.AddWithValue("$username", username);

            return (long)cmd.ExecuteScalar() > 0;
        }
    }
}