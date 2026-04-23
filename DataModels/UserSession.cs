using System;

namespace TEMA1_II.DataModels
{
    public class UserSession
    {
        public string Username { get; set; }

        // se salvează ca string în DB
        public string LoginTime { get; set; }

        public string LogoutTime { get; set; }

        public TimeSpan Duration
        {
            get
            {
                if (DateTime.TryParse(LoginTime, out var login) &&
                    DateTime.TryParse(LogoutTime, out var logout))
                    return logout - login;

                return TimeSpan.Zero;
            }
        }
    }
}