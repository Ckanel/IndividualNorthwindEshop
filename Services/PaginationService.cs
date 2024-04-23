using IndividualNorthwindEshop.Helpers;
using Microsoft.EntityFrameworkCore;

namespace IndividualNorthwindEshop.Services
{
    public class PaginationService : IPaginationService
    {
        public async Task<PaginatedList<T>> CreatePaginatedListAsync<T>(IQueryable<T> source, int pageIndex, int pageSize) where T : class
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "PageSize must be positive.");
            }
            var count = await source.CountAsync();
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);
            pageIndex = Math.Clamp(pageIndex, 1, totalPages);
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

    }
}
