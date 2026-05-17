using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class Manufacturer
{
    public int IdManufacturer { get; set; }

    public string Name { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string House { get; set; } = null!;

    public DateOnly FoundingDate { get; set; }

    public virtual ICollection<Automobile> Automobiles { get; set; } = new List<Automobile>();
}
