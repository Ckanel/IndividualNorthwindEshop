using IndividualNorthwindEshop.Helpers;
namespace IndividualNorthwindEshop.Services
{ 
    public interface IPaginationService
    {
        Task<PaginatedList<T>> CreatePaginatedListAsync<T>(IQueryable<T> source, int pageIndex, int pageSize) where T : class;
    }
}
