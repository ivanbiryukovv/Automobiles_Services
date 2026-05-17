using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class ServiceCenter
{
    public int IdServiceCenter { get; set; }

    public string CompanyName { get; set; } = null!;

    public string NameOfTheCenter { get; set; } = null!;

    public int NumberOfSeats { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string House { get; set; } = null!;

    public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();
}
