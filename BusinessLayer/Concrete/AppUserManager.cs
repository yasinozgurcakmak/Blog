using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete;

public class AppUserManager(IAppUserDal appUserDal) : IAppUserService
{
    public void TInsert(AppUser t) => appUserDal.Insert(t);
    public void TDelete(AppUser t) => appUserDal.Delete(t);
    public void TUpdate(AppUser t) => appUserDal.Update(t);
    public AppUser TGetByID(int id) => appUserDal.GetByID(id);
    public List<AppUser> TGetList() => appUserDal.GetList();
    public void TMultiUpdate(List<AppUser> t) => appUserDal.MultiUpdate(t);
    public List<AppUser> TGetListByFilter(Expression<Func<AppUser, bool>> filter) => appUserDal.GetListByFilter(filter);
}
