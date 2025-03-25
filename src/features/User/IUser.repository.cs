namespace FnbReservationAPI.src.features.User
{
    public interface IUserRepository
    {
        Task<User?> GetUserBySearchAsync(string searchQuery, string role);
        Task<List<UserDto>> GetAllUsersByRoleAsync(string role, int pageNumber, int pageSize);
        Task<User?> RegisterUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> UpdateUserAsync(Guid id, User user);
        Task<bool> UserExistsAsync(string email, string contactNumber);
        Task<User?> GetUserBySelfAsync(string token);
        Task<bool> DeactiveUserAsync(Guid id);
    }
}
