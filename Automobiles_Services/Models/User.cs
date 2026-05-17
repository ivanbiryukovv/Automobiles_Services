using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;
    public string Role { get; set; } = "Service_Employee";
}
