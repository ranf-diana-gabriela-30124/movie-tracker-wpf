using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TEMA1_II.ViewModels;

namespace TEMA1_II.Views
{
    public sealed partial class RegisterWindow : Window
    {
        private RegisterViewModel _vm = new();

        public RegisterWindow()
        {
            this.InitializeComponent();

            var presenter = this.AppWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            presenter?.Maximize();
        }

        private void OnRegisterClick(object sender, RoutedEventArgs e)
        {
            string result = _vm.Register(
                EmailBox.Text,
                UsernameBox.Text,
                PasswordBox.Password,
                ConfirmPasswordBox.Password
            );

            if (result == null)
            {
                var login = new MainWindow();
                login.Activate();
                this.Close();
            }
            else
            {
                ShowError(result);
            }
        }

        private void ShowError(string message)
        {
            var dialog = new ContentDialog
            {
                Title = "Register error",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            _ = dialog.ShowAsync();
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Activate();
            this.Close();
        }
    }
}