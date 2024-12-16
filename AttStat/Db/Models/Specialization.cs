namespace AttStat.Db.Models;

public partial class Specialization
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Faculty { get; set; }

    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();

    public virtual Faculty? FacultyNavigation { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
