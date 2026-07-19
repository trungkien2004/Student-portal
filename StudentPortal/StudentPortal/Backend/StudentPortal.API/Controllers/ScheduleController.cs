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
public class ScheduleController : ControllerBase
{
    private readonly AppDbContext _db;
    private static readonly string[] DayNames =
        { "Chu nhat", "Thu hai", "Thu ba", "Thu tu", "Thu nam", "Thu sau", "Thu bay" };

    public ScheduleController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/schedule -> "Lich hoc"
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduleRowDto>>> GetMySchedule()
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var rows = await _db.Schedules
            .Include(s => s.Subject)
            .Where(s => s.StudentId == studentId)
            .OrderByDescending(s => s.ClassDate)
            .ToListAsync();

        var result = rows.Select(MapToRow);
        return Ok(result);
    }

    // POST api/schedule -> them 1 buoi hoc moi vao lich hoc
    [HttpPost]
    public async Task<ActionResult<ScheduleRowDto>> AddSchedule(SaveScheduleRequest request)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubjectId == request.SubjectId);
        if (subject is null) return BadRequest(new { message = "Khong tim thay mon hoc." });

        if (!DateTime.TryParse(request.ClassDate, out var classDate))
        {
            return BadRequest(new { message = "Ngay hoc khong hop le." });
        }

        var schedule = new Models.Schedule
        {
            StudentId = studentId,
            SubjectId = request.SubjectId,
            ClassDate = classDate,
            WeekNumber = request.WeekNumber,
            Semester = request.Semester,
            AcademicYear = request.AcademicYear,
            StartPeriod = request.StartPeriod,
            EndPeriod = request.EndPeriod,
            Room = request.Room,
            Lecturer = request.Lecturer
        };

        _db.Schedules.Add(schedule);
        await _db.SaveChangesAsync();

        schedule.Subject = subject;
        return Ok(MapToRow(schedule));
    }

    // PUT api/schedule/{id} -> sua 1 buoi hoc da co
    [HttpPut("{id}")]
    public async Task<ActionResult<ScheduleRowDto>> UpdateSchedule(int id, SaveScheduleRequest request)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var schedule = await _db.Schedules
            .Include(s => s.Subject)
            .FirstOrDefaultAsync(s => s.ScheduleId == id && s.StudentId == studentId);
        if (schedule is null) return NotFound(new { message = "Khong tim thay buoi hoc nay." });

        var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubjectId == request.SubjectId);
        if (subject is null) return BadRequest(new { message = "Khong tim thay mon hoc." });

        if (!DateTime.TryParse(request.ClassDate, out var classDate))
        {
            return BadRequest(new { message = "Ngay hoc khong hop le." });
        }

        schedule.SubjectId = request.SubjectId;
        schedule.ClassDate = classDate;
        schedule.WeekNumber = request.WeekNumber;
        schedule.Semester = request.Semester;
        schedule.AcademicYear = request.AcademicYear;
        schedule.StartPeriod = request.StartPeriod;
        schedule.EndPeriod = request.EndPeriod;
        schedule.Room = request.Room;
        schedule.Lecturer = request.Lecturer;
        schedule.Subject = subject;

        await _db.SaveChangesAsync();
        return Ok(MapToRow(schedule));
    }

    // DELETE api/schedule/{id} -> xoa 1 buoi hoc
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var schedule = await _db.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == id && s.StudentId == studentId);
        if (schedule is null) return NotFound(new { message = "Khong tim thay buoi hoc nay." });

        _db.Schedules.Remove(schedule);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static ScheduleRowDto MapToRow(Models.Schedule s) => new()
    {
        ScheduleId = s.ScheduleId,
        DayOfWeek = DayNames[(int)s.ClassDate.DayOfWeek],
        ClassDate = s.ClassDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
        WeekNumber = s.WeekNumber,
        Semester = s.Semester,
        AcademicYear = s.AcademicYear,
        SubjectId = s.SubjectId,
        SubjectName = s.Subject!.SubjectName,
        StartPeriod = s.StartPeriod,
        EndPeriod = s.EndPeriod,
        Room = s.Room,
        Lecturer = s.Lecturer
    };
}
