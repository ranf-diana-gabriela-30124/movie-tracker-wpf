using System.Collections.Generic;
using TEMA1_II.DataModels;

public class UserApp : User
{
    public string Username { get; set; }
    public List<MovieItem> Watchlist { get; set; } = new();
    public List<WatchedMovieItem> WatchedList { get; set; } = new();

    public UserApp() { }

    public UserApp(int id, string username, List<MovieItem> watchlist, List<WatchedMovieItem> watchedList)
    {
        this.Id = id;
        this.Username = username;
        this.Watchlist = watchlist;
        this.WatchedList = watchedList;
    }
}