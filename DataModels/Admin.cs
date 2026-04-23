namespace TEMA1_II.DataModels
{
    public class Admin : User
    {
        private static Admin _instance;

        private Admin()
        {
            Email = "admin@gmail.com";
            Password = "admin";
            Role = "Admin"; // 🔥 FIX IMPORTANT
        }

        public static Admin Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Admin();
                return _instance;
            }
        }
    }
}