using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using TEMA1_II.DataAccess;
using TEMA1_II.DataModels;

namespace TEMA1_II.Views
{
    public sealed partial class AdminWindow : Window
    {
        private ObservableCollection<UserSession> _sessions = new();

        public AdminWindow()
        {
            this.InitializeComponent();

            var presenter = this.AppWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            presenter?.Maximize();
            LoadSessions();
        }

        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            var sessionRepo = new SessionRepository(new DatabaseContext());
            sessionRepo.UpdateLogout("admin");

            var loginWindow = new MainWindow();
            loginWindow.Activate();
            this.Close();
        }

        private void LoadSessions()
        {
            var sessionRepo = new SessionRepository(new DatabaseContext());
            var sessions = sessionRepo.GetAll();

            // Nu mai facem buclă/calcul manual aici, modelul tău are deja logica în Duration
            SessionsGrid.ItemsSource = sessions;
        }
    }
}