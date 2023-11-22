using BLL.Interfaces;
using DAL.Interfaces;
using System.Linq.Expressions;

namespace BLL.Services;

public class GenericService<T> : IGenericService<T> where T : class
{
    public readonly IRepository<T> _repository;

    public GenericService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public void Insert(T item)
    {
        _repository.Insert(item);
        Commit();
    }

    public T? GetById(int id)
    {
        return _repository.FindById(id);
    }
    public T? GetById(string id)
    {
        return _repository.FindById(id);
    }

    public void Commit()
    {
        _repository.Commit();
    }
    public List<T> GetByPredicate(Expression<Func<T, bool>>? filter = null, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null)
    {
        IQueryable<T> query = _repository.GetAll().AsQueryable();
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderBy != null)
        {
            query = orderBy.Compile()(query);
        }
        return query.ToList();
    }

    public void Delete(int id)
    {
        _repository.Delete(id);
        Commit();
    }

    public void Delete(string id)
    {
        _repository.Delete(id);
        Commit();
    }

    public void Update(T item)
    {
        _repository.Update(item);
        Commit();
    }
    public List<T> GetAll()
    {
        return _repository.GetAll();
    }

}
