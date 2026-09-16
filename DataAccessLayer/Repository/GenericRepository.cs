using DataAccessLayer.Abstract;
using DataAccessLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataAccessLayer.Repository;

public class GenericRepository<T>(Context context) : IGenericDal<T> where T : class
{
    public void Delete(T t)
    {
        context.Remove(t);
        context.SaveChanges();
    }

    public T? GetByID(int id) => context.Set<T>().Find(id);

    public List<T> GetList() => context.Set<T>().ToList();

    public List<T> GetListByFilter(Expression<Func<T, bool>> filter) => context.Set<T>().Where(filter).ToList();

    public void Insert(T t)
    {
        context.Add(t);
        context.SaveChanges();
    }

    public void Update(T t)
    {
        context.Update(t);
        context.SaveChanges();
    }

    public void MultiUpdate(List<T> t)
    {
        context.UpdateRange(t);
        context.SaveChanges();
    }
}
