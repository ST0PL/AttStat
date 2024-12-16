namespace AttStat.Db.Models;

public partial class Student
{
    public int Id { get; set; }

    public int CardId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public int Group { get; set; }

    public int EducationForm { get; set; }

    public int EducationCategory { get; set; }

    public int EnrollmentYear { get; set; }

    public virtual ICollection<Attestation> Attestations { get; set; } = new List<Attestation>();

    public virtual EducationalCategory EducationCategoryNavigation { get; set; } = null!;

    public virtual EducationalForm EducationFormNavigation { get; set; } = null!;

    public virtual Group GroupNavigation { get; set; } = null!;
}
