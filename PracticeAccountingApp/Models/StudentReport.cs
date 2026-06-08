using System;
using System.Collections.Generic;

namespace PracticeAccountingApp.Models;

public partial class StudentReport
{
    public int ReportId { get; set; }

    public int StudentId { get; set; }

    public int PracticeSheetId { get; set; }

    public string FilePath { get; set; } = null!;

    public byte? Grade { get; set; }

    public DateOnly? SubmissionDate { get; set; }

    public virtual PracticeSheet PracticeSheet { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
