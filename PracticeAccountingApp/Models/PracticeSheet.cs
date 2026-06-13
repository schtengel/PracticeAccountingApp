namespace PracticeAccountingApp.Models;

public partial class PracticeSheet
{
    public int PracticeSheetId { get; set; }

    public int PracticeTypeId { get; set; }

    public string ModuleId { get; set; } = null!;

    public int TeacherId { get; set; }

    public string GroupNumber { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public virtual Group GroupNumberNavigation { get; set; } = null!;

    public virtual Module Module { get; set; } = null!;

    public virtual PracticeType PracticeType { get; set; } = null!;

    public virtual ICollection<StudentReport> StudentReports { get; set; } = new List<StudentReport>();

    public virtual Teacher Teacher { get; set; } = null!;
}
