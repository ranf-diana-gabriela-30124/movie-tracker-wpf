using TEMA1_II.BusinessLogic;

namespace TEMA1_II.ViewModels
{
    public class RegisterViewModel
    {
        private readonly AuthenticationService _auth = new();

        public string Error { get; private set; }

        public string Register(string email, string username, string password, string confirmPassword)
        {
            return _auth.Register(email, username, password, confirmPassword);
        }
    }
}