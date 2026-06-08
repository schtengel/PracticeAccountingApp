using System;
using System.Collections.Generic;

namespace PracticeAccountingApp.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string GroupNumber { get; set; } = null!;

    public virtual Group GroupNumberNavigation { get; set; } = null!;

    public virtual ICollection<StudentReport> StudentReports { get; set; } = new List<StudentReport>();
}
