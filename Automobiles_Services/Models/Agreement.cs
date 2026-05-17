using System;
using System.Collections.Generic;

namespace Automobiles_Services.Models;

public partial class Agreement
{
    public int IdAgreement { get; set; }

    public int IdServiceCenter { get; set; }

    public int IdOwner { get; set; }

    public DateOnly DateOfService { get; set; }

    public DateOnly GuaranteeUpTo { get; set; }

    public string TypeOfService { get; set; } = null!;

    public virtual Owner IdOwnerNavigation { get; set; } = null!;

    public virtual ServiceCenter IdServiceCenterNavigation { get; set; } = null!;
}