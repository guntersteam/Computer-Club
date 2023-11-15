using System.Linq.Expressions;
using DL.Entities;

namespace BLL.Interfaces;

public interface IGenericService<T>
{
    void Insert(T item);
    T? GetById(int id);
    List<T> GetByPredicate(Expression<Func<T, bool>> filter = null, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderBy = null);
}