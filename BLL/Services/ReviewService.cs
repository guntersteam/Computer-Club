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


public class ReviewService : GenericService<Review>,IReviewService
{
    public ReviewService(IRepository<Review> repository) : base(repository)
    {

    }

    public List<Review>? GetByPredicate(Expression<Func<Review, bool>>? filter = null, Expression<Func<IQueryable<Review>, IOrderedQueryable<Review>>>? orderBy = null)
    {
        IQueryable<Review> query = _repository.ToTable();
        if (filter != null)
        {
            query = query.Where(filter);
        }
        if (orderBy != null)
        {
            query = orderBy.Compile()(query);
        }
        query = query
            .Include(review => review.User)
            .Include(order => order.Computer)
            .AsQueryable();
        return query.ToList();
    }


}
