using System;
using System.Collections.Generic;

namespace BookStore.DAL.Entities;

public partial class User
{
    public int MemberId { get; set; }

    public string FullName { get; set; } = null!;

    public string? EmailAddress { get; set; }

    public string Password { get; set; } = null!;

    public int Role { get; set; }
}
