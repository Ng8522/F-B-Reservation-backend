using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FnbReservationAPI.src;
using FnbReservationAPI.src.utils;

namespace FnbReservationAPI.src.features.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext _context;

        public UserRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserBySearchAsync(string searchQuery, string role)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => 
                    u.Name.Contains(searchQuery) || 
                    u.Email.Contains(searchQuery) ||
                    u.ContactNumber.Contains(searchQuery) &&
                    u.Role == role
                );
        }

        public async Task<List<User>> GetAllUsersByRoleAsync(string role, int pageNumber, int pageSize)
        {
            return await _context.Users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User?> RegisterUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> UpdateUserAsync(Guid id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return null;

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.ContactNumber = user.ContactNumber;
            existingUser.Password = user.Password;
            existingUser.Role = user.Role;
            existingUser.Status = user.Status;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> UserExistsAsync(string email, string contactNumber)
        {
            return await _context.Users.AnyAsync(u => u.Email == email || u.ContactNumber == contactNumber);
        }

        public async Task<User?> GetUserBySelfAsync(string token)
        {
            // return await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            return null;
        }
        
        public async Task<bool> DeactiveUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;
            user.Status = "Inactive";
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
