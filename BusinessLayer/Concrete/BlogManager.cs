using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete;

public class BlogManager(IBlogDal blogDal) : IBlogService
{
    public void TDelete(Blog t) => blogDal.Delete(t);
    public Blog TGetByID(int id) => blogDal.GetByID(id);
    public List<Blog> TGetList() => blogDal.GetList();
    public List<Blog> TGetListByFilter(Expression<Func<Blog, bool>> filter) => blogDal.GetListByFilter(filter);
    public void TInsert(Blog t) => blogDal.Insert(t);
    public void TMultiUpdate(List<Blog> t) => blogDal.MultiUpdate(t);
    public void TUpdate(Blog t) => blogDal.Update(t);
}
