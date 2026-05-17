using Automobiles_Services.Data;
using Automobiles_Services.Models;
using System.Windows.Controls;
using System.Windows;

namespace Automobiles_Services.Services
{

    internal class UserService
    {

        public List<User> GetAll()
        {
            using AppDbContext db = new AppDbContext();
            return db.Users.ToList();
        }       
    }
}
