using System;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace TEMA1_II.DataAccess
{
    public class DatabaseContext
    {
        private readonly string _dbPath;

        public DatabaseContext()
        {
            // 1. Găsim rădăcina proiectului căutând fișierul .csproj
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string projectRoot = baseDir;

            // Urcăm în ierarhie până dăm de fișierul de proiect
            while (projectRoot != null && !Directory.GetFiles(projectRoot, "*.csproj").Any())
            {
                var parent = Directory.GetParent(projectRoot);
                projectRoot = parent?.FullName;
            }

            // Fallback în caz că nu găsește (pentru siguranță)
            if (projectRoot == null) projectRoot = baseDir;

            // 2. Construim calea către folderul DataAccess/Database din proiect
            // Atenție: Verifică dacă folderul se numește "DataAccess" sau "DataAcces" (fără un s)
            string dbFolder = Path.Combine(projectRoot, "DataAccess", "Database");

            if (!Directory.Exists(dbFolder))
            {
                Directory.CreateDirectory(dbFolder);
            }

            _dbPath = Path.Combine(dbFolder, "app.db");

            // 3. Inițializăm tabelele la crearea contextului
            Initialize();
        }

        public SqliteConnection GetConnection()
        {
            // Folosim _dbPath setat în constructor
            return new SqliteConnection($"Data Source={_dbPath}");
        }

        public void Initialize()
        {
            using var conn = GetConnection();
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Users(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Email TEXT UNIQUE,
            Username TEXT,
            Password TEXT,
            Role TEXT
        );

        CREATE TABLE IF NOT EXISTS Watchlist(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER, 
            Title TEXT,
            Genre TEXT,
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        );

        CREATE TABLE IF NOT EXISTS WatchedMovies(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER,
            Title TEXT,
            Genre TEXT,
            Platform TEXT,
            DateWatched TEXT,
            Rating INTEGER,
            FOREIGN KEY(UserId) REFERENCES Users(Id)
        );

        CREATE TABLE IF NOT EXISTS Sessions(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT,
            LoginTime TEXT,
            LogoutTime TEXT
        );
    ";
            cmd.ExecuteNonQuery();
        }
    }
}