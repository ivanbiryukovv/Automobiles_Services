using Automobiles_Services.Data;
using Automobiles_Services.Models;

namespace Automobiles_Services.Services
{

    public class ServiceCenterService
    {

        public List<ServiceCenter> GetAll()
        {
            using AppDbContext db = new AppDbContext();
            return db.ServiceCenters.ToList();
        }
    }
}