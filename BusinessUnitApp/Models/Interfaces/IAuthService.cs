using BusinessUnitApp.Models.Dtos;

namespace BusinessUnitApp.Models.Interfaces
{
    public interface IAuthService
    {
        Task<AuthServiceResponseDto> RegisterAsync(RegisterDto registerDto);

        Task<AuthServiceResponseDto> LoginAsync(LoginDto loginDto);

        Task<AuthServiceResponseDto> SeedRolesAndUserAdminAsync();

        Task<ResponseAPIDto> GetUser(HttpContext httpContext);
    }
}