using TaskManagementApi.DTO;

namespace TaskManagementApi.Interface;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto request);
    Task<AuthResponseDto> LoginAsync(LoginDto request);
    Task<bool> DeleteTaskAsync(int taskId);

}
 