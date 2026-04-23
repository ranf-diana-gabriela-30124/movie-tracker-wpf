using System;
using System.Collections.Generic;
using TEMA1_II.DataModels;

namespace TEMA1_II.DataAccess
{
    public class WatchedMoviesRepository
    {
        private readonly DatabaseContext _db;
        public WatchedMoviesRepository(DatabaseContext db) => _db = db;

        public List<WatchedMovieItem> GetByUser(int userId) // Folosim int UserId
        {
            var list = new List<WatchedMovieItem>();
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();

            // Selectăm folosind UserId care există în tabel
            cmd.CommandText = "SELECT Id, Title, Genre, Platform, DateWatched, Rating FROM WatchedMovies WHERE UserId = $userId";
            cmd.Parameters.AddWithValue("$userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // SQLite stochează datele ca text sau ticks, Reader.GetDateTime e mai sigur
                DateTime dbDate = reader.GetDateTime(4);
                list.Add(new WatchedMovieItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Genre = reader.GetString(2),
                    Platform = reader.GetString(3),
                    DateWatched = dbDate.ToString("dd MMMM yyyy"),
                    DateWatchedParsed = dbDate,
                    Rating = reader.GetInt32(5)
                });
            }
            return list;
        }

        public void Add(WatchedMovieItem item, int userId) // Folosim int UserId
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();

            // Inserăm UserId în coloana UserId
            cmd.CommandText = @"INSERT INTO WatchedMovies(UserId, Title, Genre, Platform, DateWatched, Rating) 
                        VALUES($userId, $title, $genre, $platform, $date, $rating)";

            cmd.Parameters.AddWithValue("$userId", userId);
            cmd.Parameters.AddWithValue("$title", item.Title);
            cmd.Parameters.AddWithValue("$genre", item.Genre);
            cmd.Parameters.AddWithValue("$platform", item.Platform);
            cmd.Parameters.AddWithValue("$date", item.DateWatchedParsed);
            cmd.Parameters.AddWithValue("$rating", item.Rating);
            cmd.ExecuteNonQuery();
        }

        public void Add(WatchedMovieItem item, string email)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO WatchedMovies(Email, Title, Genre, Platform, DateWatched, Rating) 
                                VALUES($email, $title, $genre, $platform, $date, $rating)";
            cmd.Parameters.AddWithValue("$email", email);
            cmd.Parameters.AddWithValue("$title", item.Title);
            cmd.Parameters.AddWithValue("$genre", item.Genre);
            cmd.Parameters.AddWithValue("$platform", item.Platform);
            cmd.Parameters.AddWithValue("$date", item.DateWatchedParsed);
            cmd.Parameters.AddWithValue("$rating", item.Rating);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM WatchedMovies WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }
    }
}