using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using Automobiles_Services.Data;

namespace Automobiles_Services
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Task.Run(() => WarmUpDatabase());
        }

        private void WarmUpDatabase()
        {
            try
            {
                using (AppDbContext db = new AppDbContext())
                {
                    db.Database.EnsureCreated();

                    var _ = db.Users.FirstOrDefault();
                }
            }
            catch
            {

            }
        }
    }
}