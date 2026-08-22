using System.Linq.Expressions;

namespace BusinessLayer.Abstract
{
    public interface IGenericService<T>
    {
        void TInsert(T t);
        void TDelete(T t);
        void TUpdate(T t);
        List<T> TGetList();
        T TGetByID(int id);
        void TMultiUpdate(List<T> t);
        List<T> TGetListByFilter(Expression<Func<T, bool>> filter);
    }
}
