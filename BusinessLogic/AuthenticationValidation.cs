using System.Text.RegularExpressions;
using TEMA1_II.DataAccess;

namespace TEMA1_II.BusinessLogic
{
    public static class AuthenticationValidation
    {
        public static string ValidateRegister(string email, string username, string pass, string confirm)
        {
            var repo = new UserRepository();

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(pass))
                return "Te rugăm să completezi toate câmpurile.";

            if (pass != confirm)
                return "Parolele nu coincid.";

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return "Formatul email-ului este invalid.";

            if (pass.Length < 4)
                return "Parola este prea scurtă (minim 4 caractere).";

            if (repo.EmailExists(email))
                return "Acest email este deja înregistrat.";

            if (repo.UsernameExists(username))
                return "Acest nume de utilizator este deja folosit.";

            return null; 
        }
    }
}