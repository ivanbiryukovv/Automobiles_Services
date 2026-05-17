using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class ViewAutomobileOwnerInfo
{
    public string Color { get; set; } = null!;

    public string EnginePower { get; set; } = null!;

    public DateOnly YearOfManufacture { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? Surname { get; set; }

    public string PhoneNumber { get; set; } = null!;
}
