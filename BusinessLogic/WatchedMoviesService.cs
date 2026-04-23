using System;
using System.Collections.ObjectModel;
using TEMA1_II.DataModels;

namespace TEMA1_II.BusinessLogic
{
    public class WatchedMovieService
    {
        private ObservableCollection<WatchedMovieItem> _watched;
        private ObservableCollection<MovieItem> _watchlist;

        public WatchedMovieService(
            ObservableCollection<WatchedMovieItem> watched,
            ObservableCollection<MovieItem> watchlist)
        {
            _watched = watched;
            _watchlist = watchlist;
        }

        public bool MoveToWatched(MovieItem movie, int stars, string platform, string dateText)
        {
            if (!ValidationService.IsValidWatched(stars, platform))
                return false;

            // 1. Transformăm string-ul primit din UI în obiect DateTime
            if (!DateTime.TryParse(dateText, out DateTime parsedDate))
            {
                parsedDate = DateTime.Now;
            }

            // 2. Adăugăm în listă
            _watched.Add(new WatchedMovieItem
            {
                Title = movie.Title,
                Genre = movie.Genre,
                Rating = stars,
                Platform = platform,

                // Salvăm textul exact așa cum l-am primit (pentru afișare în UI)
                DateWatched = dateText,

                // Salvăm și obiectul DateTime (pentru sortare în HomeWindow)
                DateWatchedParsed = parsedDate
            });

            _watchlist.Remove(movie);
            return true;
        }

        public void DeleteWatched(WatchedMovieItem movie)
        {
            _watched.Remove(movie);
        }
    }
}