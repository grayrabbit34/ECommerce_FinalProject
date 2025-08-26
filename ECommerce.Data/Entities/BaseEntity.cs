using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Entities
{
    public abstract class BaseEntity
    {
        //bir nesne oluşturduğumda oluşturduğum anın kayıt altına alınması için constructor kullanıyorum
        public BaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    //hangi propertylerin tabloya 
    //generic bir sınıf oluşturuyorum ama burada TEntity yerine gelebilecek sınıfları sınırlandırıyorum ve sadece BaseEntity ve tabiki ondan miras alanlar yazılabilecek
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        //bütün linq işlemlerim yani sorgulamalarımda  geçerli olacak filtre yazıyorum
        //soft delete atılmış verileri otomatik olarak filtrelemiş olacağım

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.ModifiedDate).IsRequired(false);
            builder.HasQueryFilter(x => x.IsDeleted == false);
        }
    }
}

