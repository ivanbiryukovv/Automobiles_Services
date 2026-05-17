using Automobiles_Services.Data;
using Automobiles_Services.Models;  

namespace Automobiles_Services.Services
{

    internal class ManufacturerService
    {

        public List<Manufacturer> GetAll()
        {
            using AppDbContext db = new AppDbContext();
            return db.Manufacturers.ToList();
        }
    }
}
