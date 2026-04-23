using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TEMA1_II.DataModels;
using TEMA1_II.ViewModels;
using TEMA1_II.Views;

namespace TEMA1_II
{
    public sealed partial class MainWindow : Window
    {
        private LoginViewModel _vm = new();

        public MainWindow()
        {
            this.InitializeComponent();

            var presenter = this.AppWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            presenter?.Maximize();
        }

        private void ClickOnSignInButton(object sender, RoutedEventArgs e)
        {
            ErrorBar.Visibility = Visibility.Collapsed;

            // Preluăm datele direct din controale
            string emailInput = EmailBox.Text?.Trim() ?? "";
            string passwordInput = PasswordBox.Password?.Trim() ?? "";

            if (string.IsNullOrEmpty(emailInput) || string.IsNullOrEmpty(passwordInput))
            {
                ErrorBar.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                // Pasăm datele către ViewModel
                _vm.Email = emailInput;
                _vm.Password = passwordInput;

                // Încercăm logarea
                var user = _vm.Login();

                if (user != null)
                {
                    if (user.Role == "Admin")
                    {
                        var adminWin = new AdminWindow();
                        adminWin.Activate();
                    }
                    else
                    {
                        // VERIFICARE: Ne asigurăm că obiectul primit este UserApp
                        if (user is UserApp userApp)
                        {
                            // TRIMITEM OBIECTUL COMPLET (conține ID, Email, Username etc.)
                            var homeWin = new HomeWindow(userApp);
                            homeWin.Activate();
                        }
                        else
                        {
                            // Cazul în care user-ul e valid dar nu e tipul așteptat (protecție)
                            System.Diagnostics.Debug.WriteLine("[LOGIN ERROR] User type mismatch.");
                            ErrorBar.Visibility = Visibility.Visible;
                            return;
                        }
                    }

                    this.Close(); // Închidem login-ul doar după ce noua fereastră s-a deschis
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[LOGIN FAILED] Credentials not found in Database.");
                    ErrorBar.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                // Debugging avansat dacă crapă la deschiderea ferestrei noi
                System.Diagnostics.Debug.WriteLine("!!! CRITICAL ERROR DURING LOGIN FLOW !!!");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");

                ErrorBar.Visibility = Visibility.Visible;
            }
        }

        private void CloseErrorBar_Click(object sender, RoutedEventArgs e)
        {
            ErrorBar.Visibility = Visibility.Collapsed;
        }

        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Activate();
            this.Close();
        }
    }
}