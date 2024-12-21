using GasDec.Models;
using Microsoft.EntityFrameworkCore;

namespace GasDec.Services
{
    public class UserService
    {
        private readonly GasLeakDbContext _context;

        public UserService(GasLeakDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (_context.Users.Any(u => u.email == user.email))
            {
                throw new System.Exception("Користувач з таким email вже існує.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(int userId, User user)
        {
            var existingUser = await _context.Users.FindAsync(userId);
            if (existingUser == null)
            {
                throw new System.Exception("Користувача не знайдено.");
            }

            existingUser.user_name = user.user_name;
            existingUser.role = user.role;
            existingUser.password = user.password;
            existingUser.email = user.email;
            existingUser.phone_number = user.phone_number;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new System.Exception("Користувачf не знайденo.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersByLocationAsync(int locationId)
        {
            return await _context.Users
                                 .Where(u => u.location_id == locationId)
                                 .ToListAsync();
        }
    }
}

