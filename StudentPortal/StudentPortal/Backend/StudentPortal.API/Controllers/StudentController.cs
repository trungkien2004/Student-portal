using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.API.Data;
using StudentPortal.API.DTOs;

namespace StudentPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _db;

    public StudentController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/student/me  -> "Thong tin ca nhan"
    [HttpGet("me")]
    public async Task<ActionResult<StudentProfileDto>> GetMyProfile()
    {
        var studentId = GetCurrentStudentId();

        var student = await _db.Students
            .Include(s => s.Major)
            .Include(s => s.FamilyContacts)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (student is null) return NotFound();

        var dto = new StudentProfileDto
        {
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            DateOfBirth = student.DateOfBirth.ToString("dd/MM/yyyy"),
            Gender = student.Gender,
            PlaceOfBirth = student.PlaceOfBirth,
            Phone = student.Phone,
            IdentityCard = student.IdentityCard,
            InsuranceCode = student.InsuranceCode,
            Ethnicity = student.Ethnicity,
            ClassCode = student.Major?.ClassCode ?? "",
            MajorName = student.Major?.MajorName ?? "",
            Course = student.Major?.Course ?? "",
            PersonalEmail = student.PersonalEmail,
            SchoolEmail = student.SchoolEmail,
            PermanentAddress = student.PermanentAddress,
            ParentAddress = student.ParentAddress,
            Status = student.Status,
            BankName = student.BankName,
            BankAccountNumber = student.BankAccountNumber,
            BankAccountHolder = student.BankAccountHolder,
            FamilyContacts = student.FamilyContacts.Select(f => new FamilyContactDto
            {
                FullName = f.FullName,
                Relationship = f.Relationship,
                Phone = f.Phone
            }).ToList()
        };

        return Ok(dto);
    }

    // PUT api/student/me -> cap nhat SDT, email, dia chi, ngan hang, lien he gia dinh
    [HttpPut("me")]
    public async Task<ActionResult<StudentProfileDto>> UpdateMyProfile(UpdateProfileRequest request)
    {
        var studentId = GetCurrentStudentId();

        var student = await _db.Students
            .Include(s => s.Major)
            .Include(s => s.FamilyContacts)
            .FirstOrDefaultAsync(s => s.StudentId == studentId);

        if (student is null) return NotFound();

        if (!string.IsNullOrWhiteSpace(request.DateOfBirth) && DateTime.TryParse(request.DateOfBirth, out var dob))
        {
            student.DateOfBirth = dob;
        }
        student.PlaceOfBirth = request.PlaceOfBirth;
        student.IdentityCard = request.IdentityCard;
        student.InsuranceCode = request.InsuranceCode;
        student.Ethnicity = request.Ethnicity;
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            student.Status = request.Status;
        }

        student.Phone = request.Phone;
        student.PersonalEmail = request.PersonalEmail;
        student.SchoolEmail = request.SchoolEmail;
        student.PermanentAddress = request.PermanentAddress;
        student.ParentAddress = request.ParentAddress;
        student.BankName = request.BankName;
        student.BankAccountNumber = request.BankAccountNumber;
        student.BankAccountHolder = request.BankAccountHolder;

        // Thay toan bo danh sach lien he gia dinh bang danh sach moi gui len (don gian, de quan ly)
        _db.FamilyContacts.RemoveRange(student.FamilyContacts);
        var newContacts = request.FamilyContacts
            .Where(f => !string.IsNullOrWhiteSpace(f.FullName))
            .Select(f => new Models.FamilyContact
            {
                StudentId = studentId,
                FullName = f.FullName,
                Relationship = f.Relationship,
                Phone = f.Phone
            })
            .ToList();
        _db.FamilyContacts.AddRange(newContacts);

        await _db.SaveChangesAsync();

        var dto = new StudentProfileDto
        {
            StudentCode = student.StudentCode,
            FullName = student.FullName,
            DateOfBirth = student.DateOfBirth.ToString("dd/MM/yyyy"),
            Gender = student.Gender,
            PlaceOfBirth = student.PlaceOfBirth,
            Phone = student.Phone,
            IdentityCard = student.IdentityCard,
            InsuranceCode = student.InsuranceCode,
            Ethnicity = student.Ethnicity,
            ClassCode = student.Major?.ClassCode ?? "",
            MajorName = student.Major?.MajorName ?? "",
            Course = student.Major?.Course ?? "",
            PersonalEmail = student.PersonalEmail,
            SchoolEmail = student.SchoolEmail,
            PermanentAddress = student.PermanentAddress,
            ParentAddress = student.ParentAddress,
            Status = student.Status,
            BankName = student.BankName,
            BankAccountNumber = student.BankAccountNumber,
            BankAccountHolder = student.BankAccountHolder,
            FamilyContacts = newContacts.Select(f => new FamilyContactDto
            {
                FullName = f.FullName,
                Relationship = f.Relationship,
                Phone = f.Phone
            }).ToList()
        };

        return Ok(dto);
    }

    private int GetCurrentStudentId()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.Parse(idClaim!);
    }
}
