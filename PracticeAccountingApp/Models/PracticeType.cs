using System;
using System.Collections.Generic;

namespace PracticeAccountingApp.Models;

public partial class PracticeType
{
    public int PracticeTypeId { get; set; }

    public string PracticeTypeName { get; set; } = null!;

    public virtual ICollection<PracticeSheet> PracticeSheets { get; set; } = new List<PracticeSheet>();
}
