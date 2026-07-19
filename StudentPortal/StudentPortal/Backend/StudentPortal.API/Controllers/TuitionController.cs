using System.Globalization;
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
public class TuitionController : ControllerBase
{
    private readonly AppDbContext _db;

    public TuitionController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/tuition -> danh sach hoc phi theo tung ky cua sinh vien dang dang nhap
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TuitionDto>>> GetMyTuitions()
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var list = await _db.Tuitions
            .Where(t => t.StudentId == studentId)
            .OrderBy(t => t.Semester)
            .ToListAsync();

        return Ok(list.Select(MapToDto));
    }

    // POST api/tuition -> tao 1 ky hoc phi moi (VD: ky hoc moi phat sinh)
    [HttpPost]
    public async Task<ActionResult<TuitionDto>> AddTuition(SaveTuitionRequest request)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (!DateTime.TryParse(request.DueDate, out var dueDate))
        {
            return BadRequest(new { message = "Han nop khong hop le." });
        }

        var alreadyExists = await _db.Tuitions.AnyAsync(t =>
            t.StudentId == studentId && t.Semester == request.Semester);
        if (alreadyExists)
        {
            return Conflict(new { message = "Hoc ky nay da co ban ghi hoc phi." });
        }

        var tuition = new Models.Tuition
        {
            StudentId = studentId,
            Semester = request.Semester,
            AcademicYear = request.AcademicYear,
            AmountDue = request.AmountDue,
            AmountPaid = request.AmountPaid,
            DueDate = dueDate,
            PaidDate = request.AmountPaid >= request.AmountDue ? DateTime.UtcNow.Date : null
        };

        _db.Tuitions.Add(tuition);
        await _db.SaveChangesAsync();

        return Ok(MapToDto(tuition));
    }

    // PUT api/tuition/{id} -> cap nhat so tien da nop (VD: sinh vien vua dong hoc phi)
    [HttpPut("{id}")]
    public async Task<ActionResult<TuitionDto>> UpdateTuition(int id, SaveTuitionRequest request)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var tuition = await _db.Tuitions.FirstOrDefaultAsync(t => t.TuitionId == id && t.StudentId == studentId);
        if (tuition is null) return NotFound(new { message = "Khong tim thay ky hoc phi nay." });

        if (!DateTime.TryParse(request.DueDate, out var dueDate))
        {
            return BadRequest(new { message = "Han nop khong hop le." });
        }

        tuition.Semester = request.Semester;
        tuition.AcademicYear = request.AcademicYear;
        tuition.AmountDue = request.AmountDue;
        tuition.AmountPaid = request.AmountPaid;
        tuition.DueDate = dueDate;
        tuition.PaidDate = request.AmountPaid >= request.AmountDue
            ? (tuition.PaidDate ?? DateTime.UtcNow.Date)
            : null;

        await _db.SaveChangesAsync();
        return Ok(MapToDto(tuition));
    }

    // DELETE api/tuition/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTuition(int id)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var tuition = await _db.Tuitions.FirstOrDefaultAsync(t => t.TuitionId == id && t.StudentId == studentId);
        if (tuition is null) return NotFound(new { message = "Khong tim thay ky hoc phi nay." });

        _db.Tuitions.Remove(tuition);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static TuitionDto MapToDto(Models.Tuition t)
    {
        var remaining = t.AmountDue - t.AmountPaid;
        string status;

        if (remaining <= 0)
        {
            status = "Đã nộp đủ";
        }
        else if (t.AmountPaid > 0)
        {
            status = "Nộp thiếu";
        }
        else if (t.DueDate.Date < DateTime.UtcNow.Date)
        {
            status = "Quá hạn - Chưa nộp";
        }
        else
        {
            status = "Chưa nộp";
        }

        return new TuitionDto
        {
            TuitionId = t.TuitionId,
            Semester = t.Semester,
            AcademicYear = t.AcademicYear,
            AmountDue = t.AmountDue,
            AmountPaid = t.AmountPaid,
            AmountRemaining = remaining < 0 ? 0 : remaining,
            DueDate = t.DueDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
            PaidDate = t.PaidDate?.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
            Status = status
        };
    }
}
