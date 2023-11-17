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

public class UserService : GenericService<AppUser>, IAppUserService
{
    public UserService(IRepository<AppUser> repository) : base(repository)
    {
    }

    public int FindByEmail(string email)
    {
        var user = _repository.GetAll().FirstOrDefault(user => user.Email == email);
        if (user != null)
        return 11; // He're need to be UserId
        return 0;

    }

    //public List<Order> GetByPredicate(Expression<Func<AppUser, bool>>? filter = null, Expression<Func<IQueryable<AppUser>, IOrderedQueryable<AppUser>>>? orderBy = null)
    //{
    //    IQueryable<AppUser> query = _repository.ToTable();
    //    if (filter != null)
    //    {
    //        query = query.Where(filter);
    //    }
    //    if (orderBy != null)
    //    {
    //        query = orderBy.Compile()(query);
    //    }
    //    query = query
    //        .Include(order => order.AppUserId)
    //        .AsQueryable();
    //    return query.ToList();
    //}

}
