using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Auth;

namespace CursosOnline.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    }
}
