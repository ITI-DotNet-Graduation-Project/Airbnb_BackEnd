using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Airbnb.Infrastructure.Abstract
{
    public interface IGenericRepo<T> where T : class
    {
        Task DeleteRangeAsync(ICollection<T> entities);
        Task<T> GetByIdAsync(int id);
        Task SaveChangesAsync();
        IDbContextTransaction BeginTransaction();
        void Commit();
        void RollBack();
        IQueryable<T> GetTableNoTracking();
        IQueryable<T> GetTableAsTracking();
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(ICollection<T> entities);
        Task DeleteAsync(T entity);

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollBackAsync();
        Task<IQueryable<T>> FindAllInclude(string[] Include = null);
        Task<IQueryable<T>> FindAllByInclude(Expression<Func<T, bool>> match, string[] Include = null);
        Task<T> FindByInclude(Expression<Func<T, bool>> match, string[] Include = null);
    }
}
