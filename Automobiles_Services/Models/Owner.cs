using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class Owner
{
    public int IdOwner { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Surname { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string DriversLicenseSeries { get; set; } = null!;

    public string DriversLicenseNumber { get; set; } = null!;

    public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();

    public virtual ICollection<OwnerAutomobile> OwnerAutomobiles { get; set; } = new List<OwnerAutomobile>();
}
