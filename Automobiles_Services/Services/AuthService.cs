using Automobiles_Services.Data;
using Automobiles_Services.Models;

namespace Automobiles_Services.Services
{

    public class AuthService
    {

        public User? Login(string login, string password)
        {
            using AppDbContext db = new AppDbContext();
            return db.Users
                .FirstOrDefault(u => u.Login == login
                               && u.Password == password);
        }

        public RegistrationResult Register(string login, string password)
        {
            if (!IsPasswordValid(password))
                return RegistrationResult.WeakPassword;

            using AppDbContext db = new AppDbContext();

            if (db.Users.Any(u => u.Login == login))
                return RegistrationResult.LoginExists;

            db.Users.Add(new User
            {
                Login = login,
                Password = password,
                Role = "Service_Employee"
            });
            db.SaveChanges();

            return RegistrationResult.Success;
        }

        private bool IsPasswordValid(string password)
        {
            if (password.Length < 6) return false;
            if (!password.Any(char.IsUpper)) return false;
            if (!password.Any(char.IsDigit)) return false;

            string specialChars = "!@#$%^";
            if (!password.Any(c => specialChars.Contains(c))) return false;

            return true;
        }
    }
}