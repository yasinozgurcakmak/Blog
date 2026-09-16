using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete;

public class CategoryManager(ICategoryDal categoryDal) : ICategoryService
{
    public void TDelete(Category t) => categoryDal.Delete(t);
    public Category TGetByID(int id) => categoryDal.GetByID(id);
    public List<Category> TGetList() => categoryDal.GetList();
    public List<Category> TGetListByFilter(Expression<Func<Category, bool>> filter) => categoryDal.GetListByFilter(filter);
    public void TInsert(Category t) => categoryDal.Insert(t);
    public void TMultiUpdate(List<Category> t) => categoryDal.MultiUpdate(t);
    public void TUpdate(Category t) => categoryDal.Update(t);
}
