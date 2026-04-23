using System.ComponentModel;
using System.Runtime.CompilerServices;
using TEMA1_II.BusinessLogic;
using TEMA1_II.DataModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly AuthenticationService _auth = new();

    private string _email;
    private string _password;

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public User? Login()
    {
        return _auth.Login(Email, Password);
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}