namespace PracticeAccountingApp.Models;

public partial class Group
{
    public string GroupNumber { get; set; } = null!;

    public string Specialization { get; set; } = null!;

    public byte Course { get; set; }

    public virtual ICollection<PracticeSheet> PracticeSheets { get; set; } = new List<PracticeSheet>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
