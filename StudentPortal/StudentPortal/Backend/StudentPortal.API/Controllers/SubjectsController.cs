using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.API.Data;
using StudentPortal.API.DTOs;
using StudentPortal.API.Models;

namespace StudentPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SubjectsController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/subjects -> toan bo danh sach mon hoc (de chon vao dropdown khi them)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll()
    {
        var subjects = await _db.Subjects
            .OrderBy(s => s.SubjectCode)
            .Select(s => new SubjectDto
            {
                SubjectId = s.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                CreditsLT = s.CreditsLT,
                CreditsTH = s.CreditsTH
            })
            .ToListAsync();

        return Ok(subjects);
    }

    // POST api/subjects -> tao mon hoc moi (neu chua co san trong danh sach)
    [HttpPost]
    public async Task<ActionResult<SubjectDto>> Create(CreateSubjectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SubjectCode) || string.IsNullOrWhiteSpace(request.SubjectName))
        {
            return BadRequest(new { message = "Ma mon va Ten mon khong duoc de trong." });
        }

        var exists = await _db.Subjects.AnyAsync(s => s.SubjectCode == request.SubjectCode);
        if (exists)
        {
            return Conflict(new { message = "Ma mon nay da ton tai." });
        }

        var subject = new Subject
        {
            SubjectCode = request.SubjectCode,
            SubjectName = request.SubjectName,
            ShortName = request.ShortName,
            SubjectType = request.SubjectType,
            CreditsLT = request.CreditsLT,
            CreditsTH = request.CreditsTH,
            PeriodsLT = request.PeriodsLT,
            PeriodsTH = request.PeriodsTH
        };

        _db.Subjects.Add(subject);
        await _db.SaveChangesAsync();

        return Ok(new SubjectDto
        {
            SubjectId = subject.SubjectId,
            SubjectCode = subject.SubjectCode,
            SubjectName = subject.SubjectName,
            CreditsLT = subject.CreditsLT,
            CreditsTH = subject.CreditsTH
        });
    }
}
