namespace AttStat.Db.Models;

public partial class Attestation
{
    public int Id { get; set; }

    public int Student { get; set; }

    public int Discipline { get; set; }

    public int? Score { get; set; }

    public virtual Discipline DisciplineNavigation { get; set; } = null!;

    public virtual Student StudentNavigation { get; set; } = null!;
}
