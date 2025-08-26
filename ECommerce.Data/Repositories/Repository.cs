using System;

using ECommerce.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ECommerce.Data.Entities;
using System.Net.WebSockets;

namespace ECommerce.Data.Repositories
{
    //doğrudan database ile çalışarak veri güvenliğini, verinin kaybolmasını riske atmak yerine ara katman oluşturuyoruz
    //bu sayede istekler hiç bi zaman veritabanına atılamayacağı için hard delete gibi riskler ortadan kalkar

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        //veritabanınımı Dependency injection ile enjekte ediyorum 
        private readonly ECommerceDbContext _context;
        private readonly DbSet<TEntity> _dbSet;


        //_dbsete hangi değişken gönderildiyse o tipteki tabloyu ata
        public Repository(ECommerceDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            
        }

        //public void Add(TEntity entity) => _dbSet.Add(entity);

        //database'e kaydetme işlemlerini SaveChanges güvenlik nedenleriyle burada yapmıyoruz
        //unitofwork kısmında yapıyoruz ve kaydın belirli işlemler tamamlandığında olmasını sağlıyoruz 
        public void Add(TEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            _dbSet.Add(entity);
            //_context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }


        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            Delete(entity);
        }

        public TEntity? Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>>? predicate = null) =>
            predicate == null ? _dbSet : _dbSet.Where(predicate);

        public TEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }
    }
}

