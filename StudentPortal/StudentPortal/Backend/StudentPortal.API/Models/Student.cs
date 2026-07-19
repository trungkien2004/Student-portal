namespace StudentPortal.API.Models;

public class Student
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? PlaceOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? IdentityCard { get; set; }
    public string? InsuranceCode { get; set; }
    public string? Ethnicity { get; set; }

    public int MajorId { get; set; }
    public Major? Major { get; set; }

    public string? PersonalEmail { get; set; }
    public string? SchoolEmail { get; set; }
    public string? PermanentAddress { get; set; }
    public string? ParentAddress { get; set; }
    public string Status { get; set; } = "Dang hoc";

    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankAccountHolder { get; set; }

    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<FamilyContact> FamilyContacts { get; set; } = new List<FamilyContact>();
    public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    public ICollection<ConductScore> ConductScores { get; set; } = new List<ConductScore>();
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
}
