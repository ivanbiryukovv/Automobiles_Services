using Automobiles_Services.Data;
using Automobiles_Services.Models;

namespace Automobiles_Services.Services
{

    internal class PassengerCapacityService
    {

        public List<PassengerCapacity> GetAll()
        {
            using AppDbContext db = new AppDbContext();
            return db.PassengerCapacities.ToList();
        }
    }
}
