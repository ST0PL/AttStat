using System;
using System.Collections.Generic;

namespace AttStat.Db.Models;

public partial class Group
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Specialization { get; set; }

    public int Course { get; set; }

    public virtual Specialization SpecializationNavigation { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
