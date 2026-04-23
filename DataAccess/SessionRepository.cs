using System;
using System.Collections.Generic;
using TEMA1_II.DataModels;
using Microsoft.Data.Sqlite;

namespace TEMA1_II.DataAccess
{
    public class SessionRepository
    {
        private readonly DatabaseContext _db;

        public SessionRepository(DatabaseContext db)
        {
            _db = db;
        }

        public List<UserSession> GetAll()
        {
            var list = new List<UserSession>();
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Username, LoginTime, LogoutTime FROM Sessions";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new UserSession
                {
                    Username = reader.GetString(0),
                    LoginTime = reader.GetString(1),
                    LogoutTime = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }
            return list;
        }

        // 🔴 INSERT login session
        public void AddSession(string username)
        {

            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText =
            @"
            INSERT INTO Sessions (Username, LoginTime, LogoutTime)
            VALUES ($username, $loginTime, NULL);
            ";

            cmd.Parameters.AddWithValue("$username", username);
            cmd.Parameters.AddWithValue("$loginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            cmd.ExecuteNonQuery();
        }

        // 🔴 UPDATE logout

        public void UpdateLogout(string username)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = conn.CreateCommand();
            // Updatăm doar ultima sesiune deschisă pentru acest user
            cmd.CommandText = @"
        UPDATE Sessions 
        SET LogoutTime = $logoutTime 
        WHERE rowid = (
            SELECT rowid FROM Sessions 
            WHERE Username = $username AND LogoutTime IS NULL 
            ORDER BY LoginTime DESC 
            LIMIT 1
        );";

            cmd.Parameters.AddWithValue("$logoutTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("$username", username);

            cmd.ExecuteNonQuery();
        }
    }
}