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
public class GradesController : ControllerBase
{
    private readonly AppDbContext _db;

    public GradesController(AppDbContext db)
    {
        _db = db;
    }

    // GET api/grades -> "Bang diem"
    [HttpGet]
    public async Task<ActionResult<TranscriptDto>> GetMyTranscript()
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var conductScores = await _db.ConductScores
            .Where(c => c.StudentId == studentId)
            .OrderBy(c => c.AcademicYear)
            .ToListAsync();

        var grades = await _db.Grades
            .Include(g => g.Subject)
            .Where(g => g.StudentId == studentId)
            .ToListAsync();

        var failed = grades.Where(g => !g.IsPassed)
            .Select(MapToGradeRow)
            .ToList();

        var semesters = grades
            .GroupBy(g => g.Semester)
            .OrderBy(g => g.Key)
            .Select(g => new SemesterGradesDto
            {
                Semester = g.Key,
                Subjects = g.Select(MapToGradeRow).ToList(),
                SemesterAverage = g.Any(x => x.AverageScore.HasValue)
                    ? Math.Round(g.Average(x => x.AverageScore ?? 0), 1)
                    : 0
            })
            .ToList();

        var dto = new TranscriptDto
        {
            ConductScores = conductScores.Select(c => new ConductScoreDto
            {
                AcademicYear = c.AcademicYear,
                Score = c.Score,
                Rating = c.Rating
            }).ToList(),
            ConductAverage = conductScores.Any()
                ? Math.Round((decimal)conductScores.Average(c => c.Score), 0)
                : 0,
            FailedSubjects = failed,
            Semesters = semesters
        };

        // ---- Tinh canh bao hoc vu dua tren so mon khong qua ----
        // Quy uoc don gian: 1-2 mon = canh bao nhe, tu 3 mon tro len = canh bao nghiem trong
        if (failed.Count >= 3)
        {
            dto.WarningLevel = "severe";
            dto.WarningMessage = $"Cảnh báo học vụ: bạn đang có {failed.Count} môn học chưa qua. " +
                "Theo quy chế, sinh viên có từ 3 môn nợ trở lên có nguy cơ bị cảnh báo học vụ / buộc thôi học. " +
                "Vui lòng liên hệ phòng đào tạo để được tư vấn học lại sớm.";
        }
        else if (failed.Count >= 1)
        {
            dto.WarningLevel = "mild";
            dto.WarningMessage = $"Bạn đang có {failed.Count} môn học chưa qua. Hãy đăng ký học lại trong học kỳ gần nhất để tránh dồn nợ môn.";
        }

        return Ok(dto);
    }

    // POST api/grades -> them diem cho 1 mon hoc trong 1 hoc ky
    [HttpPost]
    public async Task<ActionResult<GradeRowDto>> AddGrade(AddGradeRequest request)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var subject = await _db.Subjects.FirstOrDefaultAsync(s => s.SubjectId == request.SubjectId);
        if (subject is null) return BadRequest(new { message = "Khong tim thay mon hoc." });

        var alreadyExists = await _db.Grades.AnyAsync(g =>
            g.StudentId == studentId && g.SubjectId == request.SubjectId && g.Semester == request.Semester);
        if (alreadyExists)
        {
            return Conflict(new { message = "Mon hoc nay da co diem trong hoc ky nay roi." });
        }

        var grade = new Models.Grade
        {
            StudentId = studentId,
            SubjectId = request.SubjectId,
            Semester = request.Semester,
            MidtermLT = request.MidtermLT,
            MidtermTH = request.MidtermTH,
            FinalLT = request.FinalLT,
            FinalTH = request.FinalTH
        };
        RecalculateAverage(grade);

        _db.Grades.Add(grade);
        await _db.SaveChangesAsync();

        grade.Subject = subject; // gan tam de dung chung ham MapToGradeRow
        return Ok(MapToGradeRow(grade));
    }

    // PUT api/grades/{id} -> sua diem cho 1 ban ghi da co
    [HttpPut("{id}")]
    public async Task<ActionResult<GradeRowDto>> UpdateGrade(int id, UpdateGradeRequest request)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var grade = await _db.Grades
            .Include(g => g.Subject)
            .FirstOrDefaultAsync(g => g.GradeId == id && g.StudentId == studentId);
        if (grade is null) return NotFound(new { message = "Khong tim thay ban ghi diem nay." });

        grade.MidtermLT = request.MidtermLT;
        grade.MidtermTH = request.MidtermTH;
        grade.FinalLT = request.FinalLT;
        grade.FinalTH = request.FinalTH;
        RecalculateAverage(grade);

        await _db.SaveChangesAsync();
        return Ok(MapToGradeRow(grade));
    }

    // DELETE api/grades/{id} -> xoa 1 ban ghi diem
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGrade(int id)
    {
        var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var grade = await _db.Grades.FirstOrDefaultAsync(g => g.GradeId == id && g.StudentId == studentId);
        if (grade is null) return NotFound(new { message = "Khong tim thay ban ghi diem nay." });

        _db.Grades.Remove(grade);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private static void RecalculateAverage(Models.Grade grade)
    {
        decimal? average = null;
        if (grade.FinalLT.HasValue || grade.FinalTH.HasValue)
        {
            var values = new List<decimal>();
            if (grade.FinalLT.HasValue) values.Add(grade.FinalLT.Value);
            if (grade.FinalTH.HasValue) values.Add(grade.FinalTH.Value);
            average = Math.Round(values.Average(), 1);
        }
        grade.AverageScore = average;
        grade.IsPassed = !average.HasValue || average.Value >= 5;
    }

    private static GradeRowDto MapToGradeRow(Models.Grade g) => new()
    {
        GradeId = g.GradeId,
        SubjectCode = g.Subject!.SubjectCode,
        SubjectName = g.Subject.SubjectName,
        CreditsLT = g.Subject.CreditsLT,
        CreditsTH = g.Subject.CreditsTH,
        MidtermLT = g.MidtermLT,
        MidtermTH = g.MidtermTH,
        FinalLT = g.FinalLT,
        FinalTH = g.FinalTH,
        AverageScore = g.AverageScore
    };
}
