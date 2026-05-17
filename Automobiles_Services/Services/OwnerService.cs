using Automobiles_Services.Data;
using Automobiles_Services.Models;

namespace Automobiles_Services.Services
{

    internal class OwnerService
    {

        public List<Owner> GetAll() 
        {
            using AppDbContext db = new AppDbContext();
            return db.Owners.ToList();
        }
}
}
