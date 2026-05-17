using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class PassengerCapacity
{
    public int IdPassengerCapacity { get; set; }

    public int IdAutomobile { get; set; }

    public int NumberOfPassengers { get; set; }

    public virtual Automobile IdAutomobileNavigation { get; set; } = null!;
}
