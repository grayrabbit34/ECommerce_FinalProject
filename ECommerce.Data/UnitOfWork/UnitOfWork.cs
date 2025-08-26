using System;

using ECommerce.Data.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Data.UnitOfWork
{
    
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _context;

        private IDbContextTransaction _transaction;

        public UnitOfWork(ECommerceDbContext context)
        {
            _context = context;
        }
        public async Task BeginTranssaction()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await _transaction.CommitAsync();
        }


        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        //Garbage collectora temizleme iznin verildiği kod
        public void Dispose()
        {
            _context.Dispose();
        }

    }
}

