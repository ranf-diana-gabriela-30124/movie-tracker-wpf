using System;
namespace TEMA1_II.DataModels
{
    public class WatchedMovieItem : MovieItem
    {
        public string Platform { get; set; }
        public string DateWatched { get; set; } // Pentru afișare text
        public DateTime DateWatchedParsed { get; set; } // Pentru sortare logică
        public int Rating { get; set; }
        public string StarsDisplay => new string('⭐', Rating);
    }
}