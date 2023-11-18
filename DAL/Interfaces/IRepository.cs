using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces;

public interface IRepository<T> where T : class 
{
    T? FindById(int Id);

    T? FindById(string Id);

    void Delete(int Id);

    void Update(T item);

    List<T> GetAll();

    void Insert(T item);

    void Commit();

    DbSet<T> ToTable();

    List<T> GetByPredicate(Expression<Func<T, bool>>? filter = null, Expression<Func<IQueryable<T>, IOrderedQueryable<T>>>? orderBy = null);
}