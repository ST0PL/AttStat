namespace AttStat.Db.Models;

public partial class Discipline
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Course { get; set; }

    public int? Specialization { get; set; }

    public virtual ICollection<Attestation> Attestations { get; set; } = new List<Attestation>();

    public virtual Specialization? SpecializationNavigation { get; set; }
}
