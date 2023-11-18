using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;

namespace DAL.Repositories;

public class Repository<T>:IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _table;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _table = _context.Set<T>();
    }

    public void Insert(T item)
    {
        _table.Add(item);
    }

    public void Update(T item)
    {
        _table.Update(item);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var item = _table.Find(id);

        if (item != null)
        {
            _table.Remove(item);
            _context.SaveChanges();
        }
    }

    public T? FindById(int id)
    {
        var item = _table.Find(id);

        return item;
    }

    public List<T> GetAll()
    {
        var resultList = _table.ToList();

        return resultList;
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public DbSet<T> ToTable()
    {
        return _table;
    }
    public List<T> GetByPredicate(Expression<Func<T, bool>>? filter = null, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null)
    {
        IQueryable<T> query = _table.AsQueryable();
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

    public T? FindById(string Id)
    {
        throw new NotImplementedException();
    }
}
