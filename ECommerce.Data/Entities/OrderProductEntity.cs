using System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Entities
{

    //çoka çok ilişki olduğu için ortaya çıkan ara tabloyu temsil edecek OrderProduct sınıfı
    public class OrderProductEntity : BaseEntity
    {
        
        //foreign key
        public int OrderId { get; set; }

        //foreign key
        public int ProductId { get; set; }

        public int Quantity { get; set; }


        //çoka çok ilişki olduğu için Sipariş ve Ürün tablolarını da property olarak burda tutuyorum.

        //burada yer verdiğim tabloların foreign keylerini yukarıda vermem gerekiyor.
        public OrderEntity? Order { get; set; }
        public ProductEntity? Product { get; set; }
    }

    public class OrderProductConfiguration : BaseConfiguration<OrderProductEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderProductEntity> builder)
        {

            //baseentityden gelen Id kolonunu görmezden gelmemiz gerekiyor aksi halde EF Id isminde olan kolonu otomatik olarak primary key seçer
            builder.Ignore(x => x.Id);


            //Composite key oluşturup yeni primary key olarak atadık


            builder.HasKey("OrderId", "ProductId");

            base.Configure(builder);
        }
    }
}



