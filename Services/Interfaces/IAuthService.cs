using LibaryManagement.ResgisterDTO;
using static LibaryManagement.ResgisterDTO.LoginDTO;

namespace LibaryManagement.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto model);

        Task<string> LoginAsync(LoginDto model);

        Task<string> CreateAdminAsync(CreateAdminDto model);
    }
}
