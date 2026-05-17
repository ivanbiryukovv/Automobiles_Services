using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class OwnerAutomobile
{
    public int IdOwnerAutomobile { get; set; }

    public int IdOwner { get; set; }

    public int IdAutomobile { get; set; }

    public virtual Automobile IdAutomobileNavigation { get; set; } = null!;

    public virtual Owner IdOwnerNavigation { get; set; } = null!;
}
