using Automobiles_Services.Data;
using Automobiles_Services.Models;

namespace Automobiles_Services.Services
{

    internal class AgreementService
    {

        public List<Agreement> GetAll()
        {
            using AppDbContext db = new AppDbContext();
            return db.Agreements.ToList();
        }
    }
}
