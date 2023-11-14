using HandiPapi.Models;

namespace HandiPapi.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginDto userDto);
        Task<string> CreateToken();
    }
}
