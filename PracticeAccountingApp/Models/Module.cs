namespace PracticeAccountingApp.Models;

public partial class Module
{
    public string ModuleId { get; set; } = null!;

    public string ModuleName { get; set; } = null!;

    public virtual ICollection<PracticeSheet> PracticeSheets { get; set; } = new List<PracticeSheet>();
}
