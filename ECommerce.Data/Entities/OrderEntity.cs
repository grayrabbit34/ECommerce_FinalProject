using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ECommerce.Data.Entities
{
    public class OrderEntity : BaseEntity
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }


        //Relational Property
        public int UserId { get; set; }
        public UserEntity? User { get; set; }

        //relational property
        //veri tablosunda kolona dönüşmeyecek olan verileri diğerlerinden ayırmak için yazıyorum

        public ICollection<OrderProductEntity> OrderProducts { get; set; } = new List<OrderProductEntity>();
    }


    //Base Configuration'dan miras alıp Configure metodunu override ederek Base classdaki Modified date için yazdığım kural burada da geçerli oluyor. 
    public class OrderConfiguration : BaseConfiguration<OrderEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            //order date propertysini zorunlu hale getiriyorum
            builder.Property(x => x.OrderDate)
                 .IsRequired();

            base.Configure(builder);
        }
    }
}



