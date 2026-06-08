using System;
using System.Collections.Generic;

namespace PracticeAccountingApp.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime RegistrationDate { get; set; }

    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
}
