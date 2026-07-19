using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.API.Data;
using StudentPortal.API.DTOs;

namespace StudentPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.StudentCode == request.StudentCode);

        if (student is null || !BCrypt.Net.BCrypt.Verify(request.Password, student.PasswordHash))
        {
            return Unauthorized(new { message = "Ma sinh vien hoac mat khau khong dung." });
        }

        var token = _tokenService.CreateToken(student);

        return Ok(new LoginResponse
        {
            Token = token,
            FullName = student.FullName,
            StudentCode = student.StudentCode
        });
    }
}
