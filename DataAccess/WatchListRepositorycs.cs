using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using TEMA1_II.DataModels;

namespace TEMA1_II.DataAccess
{
    public class WatchListRepository
    {
        private readonly DatabaseContext _db;
        public WatchListRepository(DatabaseContext db) => _db = db;

        public List<MovieItem> GetByUser(int userId)
        {
            var list = new List<MovieItem>();
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();

            // Căutăm după UserId, nu după Email!
            cmd.CommandText = "SELECT Id, Title, Genre FROM Watchlist WHERE UserId = $userId";
            cmd.Parameters.AddWithValue("$userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new MovieItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Genre = reader.GetString(2)
                });
            }
            return list;
        }

        public void Add(MovieItem movie, int userId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO Watchlist (UserId, Title, Genre) VALUES ($userId, $title, $genre)";
            cmd.Parameters.AddWithValue("$userId", userId);
            cmd.Parameters.AddWithValue("$title", movie.Title);
            cmd.Parameters.AddWithValue("$genre", movie.Genre);

            cmd.ExecuteNonQuery();
        }

        public void Update(MovieItem movie)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();

            cmd.CommandText = "UPDATE Watchlist SET Title = $title, Genre = $genre WHERE Id = $id";
            cmd.Parameters.AddWithValue("$title", movie.Title);
            cmd.Parameters.AddWithValue("$genre", movie.Genre);
            cmd.Parameters.AddWithValue("$id", movie.Id);

            cmd.ExecuteNonQuery();
        }

        public void Add(MovieItem item, string email)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Watchlist(Email, Title, Genre) VALUES($email, $title, $genre)";
            cmd.Parameters.AddWithValue("$email", email);
            cmd.Parameters.AddWithValue("$title", item.Title);
            cmd.Parameters.AddWithValue("$genre", item.Genre);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Watchlist WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }
    }
}