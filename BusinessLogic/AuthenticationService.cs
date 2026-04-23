using TEMA1_II.DataAccess;
using TEMA1_II.DataModels;
using System;

namespace TEMA1_II.BusinessLogic
{
    public class AuthenticationService
    {
        private readonly DatabaseContext _db = new();
        private readonly SessionRepository _sessionRepo;
        private readonly UserRepository _repo = new();
        public User? CurrentUser { get; private set; }

        public AuthenticationService()
        {
            _sessionRepo = new SessionRepository(_db);
        }

        public User? Login(string email, string password)
        {
            CurrentUser = null;
            string cleanEmail = email?.Trim() ?? "";
            string cleanPass = password?.Trim() ?? "";

            if (cleanEmail == Admin.Instance.Email && cleanPass == Admin.Instance.Password)
            {
                CurrentUser = Admin.Instance;
                _sessionRepo.AddSession("admin");
                return Admin.Instance;
            }
            var user = _repo.GetUserByEmail(cleanEmail, cleanPass);
            if (user != null)
            {
                CurrentUser = user;
                _sessionRepo.AddSession(user.Username);
                return user;
            }
            return null;
        }

        public void Logout()
        {
            if (CurrentUser == null) return;

            // Identificăm username-ul pentru a închide sesiunea în DB
            string username = "unknown";
            if (CurrentUser is Admin)
            {
                username = "admin";
            }
            else if (CurrentUser is UserApp userApp)
            {
                username = userApp.Username;
            }

            _sessionRepo.UpdateLogout(username);
            CurrentUser = null;
        }

        public string Register(string email, string username, string password, string confirmPassword)
        {
            // Apelăm validările de mai sus
            var error = AuthenticationValidation.ValidateRegister(email, username, password, confirmPassword);

            if (error != null)
                return error;

            // Dacă validarea a trecut, creăm obiectul
            var newUser = new UserApp
            {
                Email = email,
                Username = username,
                Password = password
            };

            try
            {
                _repo.AddUser(newUser);
                return null; // Succes (fără eroare)
            }
            catch (Exception ex)
            {
                return "Eroare la salvarea în baza de date: " + ex.Message;
            }
        }
    }
}