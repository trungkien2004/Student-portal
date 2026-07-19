namespace StudentPortal.API.DTOs;

public class LoginRequest
{
    public string StudentCode { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
}

public class StudentProfileDto
{
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string? PlaceOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? IdentityCard { get; set; }
    public string? InsuranceCode { get; set; }
    public string? Ethnicity { get; set; }
    public string ClassCode { get; set; } = string.Empty;
    public string MajorName { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string? PersonalEmail { get; set; }
    public string? SchoolEmail { get; set; }
    public string? PermanentAddress { get; set; }
    public string? ParentAddress { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankAccountHolder { get; set; }
    public List<FamilyContactDto> FamilyContacts { get; set; } = new();
}

public class FamilyContactDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Relationship { get; set; }
    public string? Phone { get; set; }
}

// ---- Cap nhat thong tin ca nhan (SDT, email, dia chi, ngan hang, lien he gia dinh, ho so co ban) ----
public class UpdateProfileRequest
{
    public string? DateOfBirth { get; set; }  // "yyyy-MM-dd"
    public string? PlaceOfBirth { get; set; }
    public string? IdentityCard { get; set; }
    public string? InsuranceCode { get; set; }
    public string? Ethnicity { get; set; }
    public string? Status { get; set; }

    public string? Phone { get; set; }
    public string? PersonalEmail { get; set; }
    public string? SchoolEmail { get; set; }
    public string? PermanentAddress { get; set; }
    public string? ParentAddress { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankAccountHolder { get; set; }
    public List<FamilyContactDto> FamilyContacts { get; set; } = new();
}

public class CurriculumSubjectDto
{
    public int CurriculumItemId { get; set; }
    public int SubjectId { get; set; }
    public int Stt { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string SubjectType { get; set; } = string.Empty;
    public int CreditsLT { get; set; }
    public int CreditsTH { get; set; }
    public int PeriodsLT { get; set; }
    public int PeriodsTH { get; set; }
    public int Semester { get; set; }
}

// ---- Cap nhat 1 muc trong chuong trinh dao tao (sua Ky hoc) ----
public class UpdateCurriculumItemRequest
{
    public int Semester { get; set; }
}

public class GradeRowDto
{
    public int GradeId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int CreditsLT { get; set; }
    public int CreditsTH { get; set; }
    public decimal? MidtermLT { get; set; }
    public decimal? MidtermTH { get; set; }
    public decimal? FinalLT { get; set; }
    public decimal? FinalTH { get; set; }
    public decimal? AverageScore { get; set; }
}

// ---- Cap nhat diem cho 1 ban ghi Grade da co san ----
public class UpdateGradeRequest
{
    public decimal? MidtermLT { get; set; }
    public decimal? MidtermTH { get; set; }
    public decimal? FinalLT { get; set; }
    public decimal? FinalTH { get; set; }
}

public class SemesterGradesDto
{
    public int Semester { get; set; }
    public List<GradeRowDto> Subjects { get; set; } = new();
    public decimal SemesterAverage { get; set; }
}

public class TranscriptDto
{
    public List<ConductScoreDto> ConductScores { get; set; } = new();
    public decimal ConductAverage { get; set; }
    public List<GradeRowDto> FailedSubjects { get; set; } = new();
    public List<SemesterGradesDto> Semesters { get; set; } = new();

    // ---- Canh bao hoc vu, tinh tu so mon khong qua ----
    public string WarningLevel { get; set; } = "none"; // none | mild | severe
    public string? WarningMessage { get; set; }
}

public class ConductScoreDto
{
    public int AcademicYear { get; set; }
    public int Score { get; set; }
    public string? Rating { get; set; }
}

// ---- Danh sach mon hoc dung chung (de chon khi them vao chuong trinh dao tao) ----
public class SubjectDto
{
    public int SubjectId { get; set; }
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int CreditsLT { get; set; }
    public int CreditsTH { get; set; }
}

// ---- Tao mon hoc moi (neu chua co trong danh sach chung) ----
public class CreateSubjectRequest
{
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public string SubjectType { get; set; } = "Mon tinh diem";
    public int CreditsLT { get; set; }
    public int CreditsTH { get; set; }
    public int PeriodsLT { get; set; }
    public int PeriodsTH { get; set; }
}

// ---- Them mon hoc vao chuong trinh dao tao cua nganh (sinh vien dang hoc) ----
public class AddCurriculumItemRequest
{
    public int SubjectId { get; set; }
    public int Semester { get; set; }
}

// ---- Them / cap nhat diem cho 1 mon trong 1 hoc ky ----
public class AddGradeRequest
{
    public int SubjectId { get; set; }
    public int Semester { get; set; }
    public decimal? MidtermLT { get; set; }
    public decimal? MidtermTH { get; set; }
    public decimal? FinalLT { get; set; }
    public decimal? FinalTH { get; set; }
}

// ---- Lich hoc: DTO tra ve co kem Id de sua/xoa ----
public class ScheduleRowDto
{
    public int ScheduleId { get; set; }
    public string DayOfWeek { get; set; } = string.Empty;
    public string ClassDate { get; set; } = string.Empty;
    public int WeekNumber { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string StartPeriod { get; set; } = string.Empty;
    public string EndPeriod { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Lecturer { get; set; } = string.Empty;
}

// ---- Tao / cap nhat 1 buoi hoc trong Lich hoc ----
public class SaveScheduleRequest
{
    public int SubjectId { get; set; }
    public string ClassDate { get; set; } = string.Empty; // "yyyy-MM-dd"
    public int WeekNumber { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public string StartPeriod { get; set; } = string.Empty;
    public string EndPeriod { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Lecturer { get; set; } = string.Empty;
}

// ---- Hoc phi ----
public class TuitionDto
{
    public int TuitionId { get; set; }
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountRemaining { get; set; }
    public string DueDate { get; set; } = string.Empty;
    public string? PaidDate { get; set; }
    public string Status { get; set; } = string.Empty; // "Da nop du" | "Chua nop" | "Nop thieu" | "Qua han"
}

public class SaveTuitionRequest
{
    public int Semester { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public string DueDate { get; set; } = string.Empty; // "yyyy-MM-dd"
}
