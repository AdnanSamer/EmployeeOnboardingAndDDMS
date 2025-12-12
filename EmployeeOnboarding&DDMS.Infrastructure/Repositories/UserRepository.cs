using EmployeeOnboarding_DDMS.Domain.Entities;
using EmployeeOnboarding_DDMS.Domain.Interfaces;
using EmployeeOnboarding_DDMS.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace EmployeeOnboarding_DDMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // CRUD Operations
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dbContext.Users.FindAsync(id);
        }

        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<User> AddAsync(User entity)
        {
            await _dbContext.Users.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(User entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(User entity)
        {
            _dbContext.Users.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        // Custom Operations
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbContext.Users
                .AnyAsync(u => u.Email == email);
        }

        public async Task<(IReadOnlyList<User> Users, int TotalRecords)> GetPagedAsync(int pageNumber, int pageSize, string? search)
        {
            var query = _dbContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(lowered) ||
                    u.FirstName.ToLower().Contains(lowered) ||
                    u.LastName.ToLower().Contains(lowered));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(u => u.Created)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }
}

