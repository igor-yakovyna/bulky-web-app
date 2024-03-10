using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Repository.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll(string? includeProperties = null);

    Task<T?> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);

    Task SaveAsync();
}
