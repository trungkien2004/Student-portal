namespace StudentPortal.API.Models;

public class FamilyContact
{
    public int ContactId { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Relationship { get; set; }
    public string? Phone { get; set; }
}

public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string SubjectType { get; set; } = string.Empty;
    public int CreditsLT { get; set; }
    public int CreditsTH { get; set; }
    public int PeriodsLT { get; set; }
    public int PeriodsTH { get; set; }
}

public class CurriculumItem
{
    public int CurriculumItemId { get; set; }
    public int MajorId { get; set; }
    public Major? Major { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public int Semester { get; set; }
}

public class Grade
{
    public int GradeId { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public int Semester { get; set; }
    public decimal? MidtermLT { get; set; }
    public decimal? MidtermTH { get; set; }
    public decimal? FinalLT { get; set; }
    public decimal? FinalTH { get; set; }
    public decimal? AverageScore { get; set; }
    public bool IsPassed { get; set; } = true;
}

public class ConductScore
{
    public int ConductScoreId { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int AcademicYear { get; set; }
    public int Score { get; set; }
    public string? Rating { get; set; }
}

public class Tuition
{
    public int TuitionId { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
}

public class Schedule
{
    public int ScheduleId { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public DateTime ClassDate { get; set; }
    public int WeekNumber { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string StartPeriod { get; set; } = string.Empty;
    public string EndPeriod { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Lecturer { get; set; } = string.Empty;
}
