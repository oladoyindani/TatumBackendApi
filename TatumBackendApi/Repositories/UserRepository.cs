using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TatumBackendApi.Common.Models;
using TatumBackendApi.Data;
using TatumBackendApi.Entities;

namespace TatumBackendApi.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) {}
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserwithTokensAsync(Guid userId)
        {
            return await _dbSet.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithTokensAsync(Guid userId)
        {
            var now = DateTime.UtcNow;

            return await _dbSet.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => 
                u.Id == userId);
        }

        public async Task<User?> GetByPasswordSetupTokenAsync(string token)
        {
            var now = DateTime.UtcNow;

            return await _dbSet.FirstOrDefaultAsync(u => 
                u.PasswordSetupToken == token && 
                u.PasswordSetupTokenExpiresAt > now);
        }

        public async Task<bool> ExistsByStaffIdAsync(string staffId)
        {
            return await _dbSet.AnyAsync(u => u.StaffId == staffId);
        }

        public async Task<PagedResult<User>> GetPagedAsync(
            Guid? userId,
            PaginationParameters pagination,
            CancellationToken ct = default
        )
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            // Filter by userId 

            if (userId.HasValue)
            {
                query = query.Where(x => x.Id == userId.Value);
            }

            // Total records
            var totalCount = await query.CountAsync(ct);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);

            // Get Current Page
            var items = await query
                .OrderByDescending(x => x.CreatedAt).Skip(
                    (pagination.PageNumber - 1) * pagination.PageSize
                ).Take(pagination.PageSize).ToListAsync(ct);

            return new PagedResult<User>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}