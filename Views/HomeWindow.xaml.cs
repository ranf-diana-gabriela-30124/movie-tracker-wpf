using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TEMA1_II.BusinessLogic;
using TEMA1_II.DataModels;
using TEMA1_II.DataAccess;

namespace TEMA1_II
{
    public sealed partial class HomeWindow : Window
    {
        private readonly MovieService _movieService;
        private readonly UserApp _currentUser;

        private ObservableCollection<MovieItem> _watchlist = new();
        private ObservableCollection<WatchedMovieItem> _watched = new();

        private MovieItem? _pendingWatchedMovie = null;
        private MovieItem? _editingMovie = null;
        private int _selectedStars = 0;

        private int _sortGenreDir = 0;
        private int _sortDateDir = 0;
        private int _sortRatingDir = 0;

        public HomeWindow(UserApp user)
        {
            this.InitializeComponent();

            _currentUser = user;
                var db = new DatabaseContext();
                _movieService = new MovieService(db);

                WatchlistItems.ItemsSource = _watchlist;
                WatchedItems.ItemsSource = _watched;

                LoadDataFromDatabase();

            // Maximizare fereastră
            var presenter = this.AppWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            if (presenter != null) presenter.Maximize();
        }

        private void LoadDataFromDatabase()
        {
            // Ne asigurăm că rulăm pe thread-ul de UI pentru a evita crash-ul la ObservableCollection
            this.DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    if (_movieService == null || _currentUser == null) return;

                    var watchlistFromDb = _movieService.GetWatchlist(_currentUser.Id) ?? new List<MovieItem>();
                    var watchedFromDb = _movieService.GetWatched(_currentUser.Id) ?? new List<WatchedMovieItem>();

                    _watchlist.Clear();
                    foreach (var item in watchlistFromDb) _watchlist.Add(item);

                    _watched.Clear();
                    foreach (var item in watchedFromDb) _watched.Add(item);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Eroare LoadData: {ex.Message}");
                }
            });
        }

        private async void ShowError(string message)
        {
            // Verificare critică pentru a preveni crash-ul ContentDialog în WinUI 3
            if (this.Content == null || this.Content.XamlRoot == null) return;

            try
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Atenție",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
            catch { }
        }

        // --- GESTIONARE WATCHLIST ---

        private void OnAddMovieClick(object sender, RoutedEventArgs e)
        {
            AddTitleBox.Text = "";
            AddGenreBox.SelectedItem = null;
            AddMovieOverlay.Visibility = Visibility.Visible;
        }

        private void OnAddMovieCancel(object sender, RoutedEventArgs e) => AddMovieOverlay.Visibility = Visibility.Collapsed;

        private void OnAddMovieConfirm(object sender, RoutedEventArgs e)
        {
            if (AddTitleBox == null || AddGenreBox == null) return;

            string title = AddTitleBox.Text.Trim();
            string genre = (AddGenreBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(genre))
            {
                ShowError("Te rugăm să completezi toate câmpurile.");
                return;
            }

            try
            {
                var newMovie = new MovieItem { Title = title, Genre = genre };
                string error = _movieService.AddToWatchlist(newMovie, _currentUser.Id);

                if (error != null)
                {
                    ShowError(error);
                    return;
                }

                AddMovieOverlay.Visibility = Visibility.Collapsed;
                LoadDataFromDatabase();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Crash prevenit la Add: {ex.Message}");
            }
        }

        private void OnDeleteWatchlistMovie(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is MovieItem movie)
            {
                _movieService.RemoveFromWatchlist(movie.Id);
                _watchlist.Remove(movie);
            }
        }

        private void OnEditWatchlistMovie(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is MovieItem movie)
            {
                _editingMovie = movie;
                EditTitleBox.Text = movie.Title;
                EditGenreBox.SelectedItem = EditGenreBox.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(i => i.Content?.ToString() == movie.Genre);

                EditMovieOverlay.Visibility = Visibility.Visible;
            }
        }

        private void OnEditMovieCancel(object sender, RoutedEventArgs e)
        {
            EditMovieOverlay.Visibility = Visibility.Collapsed;
            _editingMovie = null;
        }

        private void OnEditMovieConfirm(object sender, RoutedEventArgs e)
        {
            if (_editingMovie == null) return;

            string newTitle = EditTitleBox.Text.Trim();
            string newGenre = (EditGenreBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            if (string.IsNullOrEmpty(newTitle))
            {
                ShowError("Titlul nu poate fi gol.");
                return;
            }

            _editingMovie.Title = newTitle;
            _editingMovie.Genre = newGenre;

            _movieService.UpdateWatchlistMovie(_editingMovie);
            EditMovieOverlay.Visibility = Visibility.Collapsed;
            LoadDataFromDatabase();
            _editingMovie = null;
        }

        // --- GESTIONARE WATCHED ---

        private void OnMovieWatched(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox)?.Tag is MovieItem movie)
            {
                _pendingWatchedMovie = movie;
                WatchedMovieName.Text = movie.Title;
                WatchedDateBox.Text = DateTime.Now.ToString("dd MMMM yyyy");
                WatchedPlatformBox.Text = "";
                _selectedStars = 0;
                UpdateStarDisplay();
                WatchedOverlay.Visibility = Visibility.Visible;
                (sender as CheckBox).IsChecked = false;
            }
        }

        private void OnWatchedConfirm(object sender, RoutedEventArgs e)
        {
            if (_pendingWatchedMovie == null) return;

            if (_selectedStars == 0 || string.IsNullOrWhiteSpace(WatchedPlatformBox.Text))
            {
                ShowError("Te rugăm să alegi un rating și o platformă.");
                return;
            }

            var watchedMovie = new WatchedMovieItem
            {
                Title = _pendingWatchedMovie.Title,
                Genre = _pendingWatchedMovie.Genre,
                Rating = _selectedStars,
                Platform = WatchedPlatformBox.Text.Trim(),
                DateWatched = WatchedDateBox.Text,
                DateWatchedParsed = DateTime.Now
            };

            string error = _movieService.AddWatched(watchedMovie, _currentUser.Id);
            if (error != null)
            {
                ShowError(error);
                return;
            }

            _movieService.RemoveFromWatchlist(_pendingWatchedMovie.Id);
            WatchedOverlay.Visibility = Visibility.Collapsed;
            LoadDataFromDatabase();
            _pendingWatchedMovie = null;
        }

        private void OnDeleteWatchedMovie(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is WatchedMovieItem movie)
            {
                _movieService.RemoveWatched(movie.Id);
                _watched.Remove(movie);
            }
        }

        // --- HELPERS ---

        private void UpdateStarDisplay()
        {
            var buttons = new[] { StarBtn1, StarBtn2, StarBtn3, StarBtn4, StarBtn5 };
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null)
                    buttons[i].Content = i < _selectedStars ? "★" : "☆";
            }
        }

        private void OnStarClick(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is string tagStr && int.TryParse(tagStr, out int stars))
            {
                _selectedStars = stars;
                UpdateStarDisplay();
            }
        }

        private void RefreshWatchedList(IEnumerable<WatchedMovieItem> sorted)
        {
            var list = sorted.ToList();
            _watched.Clear();
            foreach (var item in list) _watched.Add(item);
        }

        private void OnSortByGenre(object sender, RoutedEventArgs e)
        {
            _sortGenreDir = _sortGenreDir == 1 ? -1 : 1;
            var sorted = _sortGenreDir == 1 ? _watched.OrderBy(m => m.Genre) : _watched.OrderByDescending(m => m.Genre);
            RefreshWatchedList(sorted);
        }

        private void OnSortByDate(object sender, RoutedEventArgs e)
        {
            _sortDateDir = _sortDateDir == 1 ? -1 : 1;
            var sorted = _sortDateDir == 1 ? _watched.OrderBy(m => m.DateWatchedParsed) : _watched.OrderByDescending(m => m.DateWatchedParsed);
            RefreshWatchedList(sorted);
        }

        private void OnSortByRating(object sender, RoutedEventArgs e)
        {
            _sortRatingDir = _sortRatingDir == 1 ? -1 : 1;
            var sorted = _sortRatingDir == 1 ? _watched.OrderBy(m => m.Rating) : _watched.OrderByDescending(m => m.Rating);
            RefreshWatchedList(sorted);
        }

        private void OnRandomPickClick(object sender, RoutedEventArgs e)
        {
            var selectedGenre = (RandomGenreBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var filteredList = _watchlist.Where(f =>
                string.IsNullOrEmpty(selectedGenre) ||
                selectedGenre == "Any genre" ||
                f.Genre == selectedGenre).ToList();

            RandomResultPanel.Visibility = Visibility.Visible;
            if (!filteredList.Any())
            {
                RandomResultTitle.Text = "Nu am găsit filme.";
                RandomResultGenre.Text = "";
                return;
            }

            var picked = filteredList[new Random().Next(filteredList.Count)];
            RandomResultTitle.Text = picked.Title;
            RandomResultGenre.Text = picked.Genre;
        }

        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            // 1. Înregistrăm logout-ul în baza de date
            // Presupunem că ai acces la repository prin intermediul App-ului sau injectat
            var sessionRepo = new SessionRepository(new DatabaseContext());
            sessionRepo.UpdateLogout(_currentUser.Username); // Folosește variabila ta de username

            // 2. Navigăm înapoi la Login
            var loginWindow = new MainWindow(); // Sau cum se numește fereastra ta de Login
            loginWindow.Activate();

            // 3. Închidem fereastra actuală
            this.Close();
        }

        private void OnWatchedCancel(object sender, RoutedEventArgs e)
        {
            WatchedOverlay.Visibility = Visibility.Collapsed;
            _pendingWatchedMovie = null;
        }


    }
}