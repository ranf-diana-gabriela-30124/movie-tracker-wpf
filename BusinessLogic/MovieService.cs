using System;
using System.Collections.Generic;
using System.Linq;
using TEMA1_II.DataAccess;
using TEMA1_II.DataModels;

namespace TEMA1_II.BusinessLogic
{
    public class MovieService
    {
        private readonly WatchListRepository _watchRepo;
        private readonly WatchedMoviesRepository _watchedRepo;

        public MovieService(DatabaseContext db)
        {
            _watchRepo = new WatchListRepository(db);
            _watchedRepo = new WatchedMoviesRepository(db);
        }

        // --- OPERAȚII WATCHLIST ---

        // Schimbat din string email -> int userId
        public List<MovieItem> GetWatchlist(int userId)
        {
            return _watchRepo.GetByUser(userId);
        }

        public string AddToWatchlist(MovieItem movie, int userId)
        {
            if (!ValidationService.IsValidMovie(movie.Title, movie.Genre))
            {
                return "Titlul și genul sunt obligatorii.";
            }

            // Verificăm duplicat folosind ID-ul
            var existing = _watchRepo.GetByUser(userId);
            if (existing.Any(m => m.Title.Equals(movie.Title, StringComparison.OrdinalIgnoreCase)))
            {
                return "Acest film este deja în lista ta de așteptare.";
            }

            _watchRepo.Add(movie, userId);
            return null;
        }

        public void UpdateWatchlistMovie(MovieItem movie)
        {
            if (ValidationService.IsValidMovie(movie.Title, movie.Genre))
            {
                _watchRepo.Update(movie);
            }
        }

        public void RemoveFromWatchlist(int movieId)
        {
            _watchRepo.Delete(movieId);
        }

        // --- OPERAȚII WATCHED ---

        public List<WatchedMovieItem> GetWatched(int userId)
        {
            return _watchedRepo.GetByUser(userId);
        }

        public string AddWatched(WatchedMovieItem movie, int userId)
        {
            if (!ValidationService.IsValidWatched(movie.Rating, movie.Platform))
            {
                return "Te rugăm să introduci un rating valid și platforma.";
            }

            _watchedRepo.Add(movie, userId);

            // LOGICA DE MUTARE folosind UserId
            var watchlist = _watchRepo.GetByUser(userId);
            var itemInWatchlist = watchlist.FirstOrDefault(m =>
                m.Title.Equals(movie.Title, StringComparison.OrdinalIgnoreCase));

            if (itemInWatchlist != null)
            {
                _watchRepo.Delete(itemInWatchlist.Id);
            }

            return null;
        }

        public void RemoveWatched(int watchedMovieId)
        {
            _watchedRepo.Delete(watchedMovieId);
        }

        // --- LOGICĂ RANDOM PICK ---

        public MovieItem GetRandomMovie(int userId, string genre = "Any genre")
        {
            var list = _watchRepo.GetByUser(userId);

            if (genre != "Any genre" && !string.IsNullOrEmpty(genre))
            {
                list = list.Where(m => m.Genre == genre).ToList();
            }

            if (!list.Any()) return null;

            var rand = new Random();
            return list[rand.Next(list.Count)];
        }
    }
}