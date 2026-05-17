using Automobiles_Services.Data;
using Automobiles_Services.Models;

namespace Automobiles_Services.Services
{

    internal class AutomobileService
    {

        public List<Automobile> GetAll()
        {
            using AppDbContext db = new AppDbContext();
            return db.Automobiles.ToList();
        }
    }
}
