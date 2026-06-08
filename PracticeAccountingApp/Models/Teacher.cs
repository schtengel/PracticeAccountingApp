namespace PracticeAccountingApp.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public string FullName { get; set; } = null!;

    public virtual ICollection<PracticeSheet> PracticeSheets { get; set; } = new List<PracticeSheet>();
}
