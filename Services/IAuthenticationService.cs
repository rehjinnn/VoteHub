using VoteHub.Models;

namespace VoteHub.Services
{
    public interface IAuthenticationService
    {
        Task<User> RegisterAsync(string fullName, string email, string password);
        Task<User> LoginAsync(string email, string password);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserByEmailAsync(string email);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}