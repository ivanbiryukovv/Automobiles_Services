using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class Automobile
{
    public int IdAutomobile { get; set; }

    public int IdManufacturer { get; set; }

    public string EnginePower { get; set; } = null!;

    public int Weight { get; set; }

    public string Color { get; set; } = null!;

    public string AvailabilityOfAirConditioning { get; set; } = null!;

    public DateOnly YearOfManufacture { get; set; }

    public string BodyType { get; set; } = null!;

    public string Series { get; set; } = null!;

    public virtual Manufacturer IdManufacturerNavigation { get; set; } = null!;

    public virtual ICollection<OwnerAutomobile> OwnerAutomobiles { get; set; } = new List<OwnerAutomobile>();

    public virtual ICollection<PassengerCapacity> PassengerCapacities { get; set; } = new List<PassengerCapacity>();
}
