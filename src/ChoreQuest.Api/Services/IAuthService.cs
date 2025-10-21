using ChoreQuest.Api.DTOs;

namespace ChoreQuest.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<UserProfileResponse?> GetUserProfileAsync(int userId);
    Task<UserProfileResponse?> UpdateUserProfileAsync(int userId, UpdateProfileRequest request);
}
