using Riwi.CoursesAssessment.Application.DTOs;

namespace Riwi.CoursesAssessment.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserDto?> GetUserByIdAsync(string userId);
    Task<bool> IsUserInRoleAsync(string userId, string role);
}

