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
public class CurriculumController : ControllerBase
{
    private readonly AppDbContext _db;

    public CurriculumController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/curriculum -> "Chuong trinh dao tao" cua sinh vien dang dang nhap
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CurriculumSubjectDto>>> GetMyCurriculum()
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var student = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student is null) return NotFound();

        var items = await _db.CurriculumItems
            .Include(c => c.Subject)
            .Where(c => c.MajorId == student.MajorId)
            .OrderBy(c => c.Semester)
            .ThenBy(c => c.CurriculumItemId)
            .ToListAsync();

        var result = items.Select((c, index) => new CurriculumSubjectDto
        {
            CurriculumItemId = c.CurriculumItemId,
            SubjectId = c.SubjectId,
            Stt = index + 1,
            SubjectCode = c.Subject!.SubjectCode,
            SubjectName = c.Subject.SubjectName,
            ShortName = c.Subject.ShortName,
            SubjectType = c.Subject.SubjectType,
            CreditsLT = c.Subject.CreditsLT,
            CreditsTH = c.Subject.CreditsTH,
            PeriodsLT = c.Subject.PeriodsLT,
            PeriodsTH = c.Subject.PeriodsTH,
            Semester = c.Semester
        }).ToList();

        return Ok(result);
    }

    // POST api/curriculum -> them 1 mon hoc vao chuong trinh dao tao (nganh cua sinh vien dang dang nhap)
    [HttpPost]
    public async Task<ActionResult<CurriculumSubjectDto>> AddSubject(AddCurriculumItemRequest request)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var student = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student is null) return NotFound();

        var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubjectId == request.SubjectId);
        if (subject is null) return BadRequest(new { message = "Khong tim thay mon hoc." });

        var alreadyExists = await _db.CurriculumItems.AnyAsync(c =>
            c.MajorId == student.MajorId && c.SubjectId == request.SubjectId);
        if (alreadyExists)
        {
            return Conflict(new { message = "Mon hoc nay da co trong chuong trinh dao tao." });
        }

        var item = new Models.CurriculumItem
        {
            MajorId = student.MajorId,
            SubjectId = request.SubjectId,
            Semester = request.Semester
        };
        _db.CurriculumItems.Add(item);
        await _db.SaveChangesAsync();

        return Ok(new CurriculumSubjectDto
        {
            CurriculumItemId = item.CurriculumItemId,
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            ShortName = subject.ShortName,
            SubjectType = subject.SubjectType,
            CreditsLT = subject.CreditsLT,
            CreditsTH = subject.CreditsTH,
            PeriodsLT = subject.PeriodsLT,
            PeriodsTH = subject.PeriodsTH,
            Semester = item.Semester
        });
    }

    // PUT api/curriculum/{id} -> sua ky hoc cua 1 muc trong chuong trinh dao tao
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubject(int id, UpdateCurriculumItemRequest request)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var student = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student is null) return NotFound();

        var item = await _db.CurriculumItems.FirstOrDefaultAsync(c =>
            c.CurriculumItemId == id && c.MajorId == student.MajorId);
        if (item is null) return NotFound(new { message = "Khong tim thay muc nay trong chuong trinh dao tao." });

        item.Semester = request.Semester;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE api/curriculum/{id} -> xoa 1 mon khoi chuong trinh dao tao
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var student = await _db.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student is null) return NotFound();

        var item = await _db.CurriculumItems.FirstOrDefaultAsync(c =>
            c.CurriculumItemId == id && c.MajorId == student.MajorId);
        if (item is null) return NotFound(new { message = "Khong tim thay muc nay trong chuong trinh dao tao." });

        _db.CurriculumItems.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
