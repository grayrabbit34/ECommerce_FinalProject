using System;
namespace ECommerce.Data.Context
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using ECommerce.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    //katmanlı mimari kullandığımız için Deisgn'ı ayrıca indirmemiz gerekiyor.

    public class ECommerceDbContext : DbContext
    {

        //Veritabanı bağlantısı için yaptığımız ilk işlem

        //options parametresini miras aldığımız DbContext sınıfın construtorına göndermemiz gerektiği için : base (options) yazıyoruz
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
        : base(options) { }

        //hangi sınıfların tablo olacağını belirttiğimiz kısım ve tbalo adlarını veriyorum
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<ProductEntity> Products => Set<ProductEntity>();
        public DbSet<OrderEntity> Orders => Set<OrderEntity>();
        public DbSet<OrderProductEntity> OrderProducts => Set<OrderProductEntity>();
        public DbSet<SettingEntity> Settings => Set<SettingEntity>();


        //entity yani tablolarımız veritabnına dönüşürken yapmak istediğimiz değişiklikleri burada gerçekleştiriyoruz
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent Api
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());

            // Seed single Setting row (Id = 1)
            modelBuilder.Entity<SettingEntity>().HasData(
                new SettingEntity
                { Id = 1, MaintenanceMode = false, CreatedDate = DateTime.UtcNow }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}

