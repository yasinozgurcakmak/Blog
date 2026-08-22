using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Concrete;
using System.Linq.Expressions;

namespace BusinessLayer.Concrete
{
    public class AppUserManager : IAppUserService
    {
        private readonly IAppUserDal _appUserDal;

        public AppUserManager(IAppUserDal appUserDal)
        {
            _appUserDal = appUserDal;
        }

        public void TInsert(AppUser t) => _appUserDal.Insert(t);

        public void TDelete(AppUser t) => _appUserDal.Delete(t);

        public void TUpdate(AppUser t) => _appUserDal.Update(t);

        public AppUser TGetByID(int id) => _appUserDal.GetByID(id);

        public List<AppUser> TGetList() => _appUserDal.GetList();

        public void TMultiUpdate(List<AppUser> t) => _appUserDal.MultiUpdate(t);

        public List<AppUser> TGetListByFilter(Expression<Func<AppUser, bool>> filter)
            => _appUserDal.GetListByFilter(filter);
    }
}
