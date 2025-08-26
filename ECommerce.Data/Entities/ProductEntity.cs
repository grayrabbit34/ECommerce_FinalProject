using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Entities;

public class ProductEntity : BaseEntity
{
    public string ProductName { get; set; } = null!;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }

    //realtional property
    //veri tablosunda kolona dönüşmeyecek olan verileri diğerlerinden ayırmak için yazıyorum

    public ICollection<OrderProductEntity> OrderProducts { get; set; } = new List<OrderProductEntity>();
}

public class ProductConfiguration : BaseConfiguration<ProductEntity>
{
    public override void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        //Product name zorunlu olsun ve max 50 karakter uzunluğunda olsun
        builder.Property(x => x.ProductName).
            IsRequired()
            .HasMaxLength(50);

        base.Configure(builder);
    }
}


