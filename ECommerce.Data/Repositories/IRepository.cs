using System;

using System.Linq.Expressions;
namespace ECommerce.Data.Repositories
{
    //Database'e doğrudan erişim olmamasını ve güvenliği sağlar.
    ////Data katmanı ile business katmanını birbirinden ayırır. Business katmanı verilerin nereden nasıl geldiğini bilmeden sadece arayüzü kullanarak işlem yapar.
    public interface IRepository<TEntity> where TEntity : class
    {
        //veri ekleme
        void Add(TEntity entity);

        // veri güncelleme
        void Update(TEntity entity);

        //veri silme
        void Delete(TEntity entity);

        //id ile silme
        void Delete(int id);

        //parametre olarak linq sorgusu alır ve veriyi getirir
        TEntity? Get(Expression<Func<TEntity, bool>> predicate);
        //liste üzerinde sorgular yapabilmek için. Default null dönme sebebi parametresiz de çağırılabilmesini sağlamak

        //nesneyi çekmektense sorguyu çekmek daha performanslı olacağı için sorgu tipinde dönen bir metot da ekliyorum
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null);
    }


}

