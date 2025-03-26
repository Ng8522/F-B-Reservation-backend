using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FnbReservationAPI.src;
using FnbReservationAPI.src.utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        public async Task<List<UserDto>> GetAllUsersByRoleAsync(string role, int pageNumber, int pageSize)
        {
            return await _context.Users.Where(u => u.Role.ToLower() == role.ToLower())
                .OrderBy(u => u.Id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    ContactNumber = u.ContactNumber,
                    Role = u.Role,
                    Status = u.Status,
                    CreateBy = u.CreateBy,
                    CreatedAt = u.CreatedAt,
                    UpdateBy = u.UpdateBy,
                    UpdatedAt = u.UpdatedAt
                })
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

        public async Task<UserDto?> GetUserBySelfAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (handler.ReadToken(token) is not JwtSecurityToken jwtToken)
                return null;

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return null;

            string userId = userIdClaim.Value;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            // If user not found, return null
            if (user == null)
                return null;

            // Query the user from the database
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                ContactNumber = user.ContactNumber,
                Role = user.Role,
                Status = user.Status,
                CreateBy = user.CreateBy,
                CreatedAt = user.CreatedAt,
                UpdateBy = user.UpdateBy,
                UpdatedAt = user.UpdatedAt
            };
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
