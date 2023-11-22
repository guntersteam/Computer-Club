using BLL.Interfaces;
using DAL.Interfaces;
using DL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services;

public class PaymentService: GenericService<Payment>,IPaymentService
{
    public PaymentService(IRepository<Payment> repository) : base(repository)
    {
    }

    public List<Payment>? GetByPredicate(Expression<Func<Payment, bool>>? filter = null, Expression<Func<IQueryable<Payment>, IOrderedQueryable<Payment>>>? orderBy = null)
    {
        IQueryable<Payment> query = _repository.ToTable();
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderBy != null)
        {
            query = orderBy.Compile()(query);
        }
        query = query
            .Include(payment => payment.Order)
            .Include(payment => payment.Order.User)
            .Include(payment => payment.Order.Computer)
            .AsQueryable();
        return query.ToList();
    }
}
