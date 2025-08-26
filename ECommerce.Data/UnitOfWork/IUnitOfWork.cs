using System;
namespace ECommerce.Data.UnitOfWork
{

    /// <summary>
    /// gerektiğinde garbage collector'ı çalıştırabililmek için IDısposable'daan miras alıyorum
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();

        //kaç kayda savechanges işleminin etki ettiğini geri döneceği için int kuklanıyoruz

        Task BeginTranssaction();
        //Task->> asenkron metotların void hali gibi geri bir şey dönmediğimde kullanıyorum

        Task CommitTransaction();

        Task RollbackAsync();
    }

}

