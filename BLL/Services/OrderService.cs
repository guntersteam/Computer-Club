using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Repositories;
using DL.Entities;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BLL.Services;

public class OrderService : GenericService<Order>, IOrderService
{
    public OrderService(IRepository<Order> repository) : base(repository)
    {
    }

    public new Order? GetById(int id)
    {
        var item = _repository.ToTable()
            .Include(order => order.User)
            .Include(order => order.Computer)
            .FirstOrDefault(order => order.OrderId == id);

        return item;
    }

    public List<Order> GetByPredicate(Expression<Func<Order, bool>>? filter = null, Expression<Func<IQueryable<Order>, IOrderedQueryable<Order>>>? orderBy = null)
    {
        IQueryable<Order> query = _repository.ToTable();
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderBy != null)
        {
            query = orderBy.Compile()(query);
        }
        query = query
            .Include(order => order.User)
            .Include(order => order.Computer)
            .AsQueryable();
        return query.ToList();
    }

}
