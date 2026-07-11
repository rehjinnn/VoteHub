using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VoteHub.Data;
using VoteHub.Models;

namespace VoteHub.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly VoteHubContext _context;

        public AuthenticationService(VoteHubContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(string fullName, string email, string password)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
                throw new Exception("Email already registered");

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = "User",
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
            if (user == null)
                throw new Exception("Invalid email or password");

            if (!VerifyPassword(password, user.PasswordHash))
                throw new Exception("Invalid email or password");

            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public string HashPassword(string password)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);

                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(20);
                    byte[] hashWithSalt = new byte[36];
                    Array.Copy(salt, 0, hashWithSalt, 0, 16);
                    Array.Copy(hash, 0, hashWithSalt, 16, 20);

                    return Convert.ToBase64String(hashWithSalt);
                }
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            byte[] hashWithSalt = Convert.FromBase64String(hash);
            byte[] salt = new byte[16];
            Array.Copy(hashWithSalt, 0, salt, 0, 16);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] computedHash = pbkdf2.GetBytes(20);
                for (int i = 0; i < 20; i++)
                {
                    if (hashWithSalt[i + 16] != computedHash[i])
                        return false;
                }
                return true;
            }
        }
    }
}