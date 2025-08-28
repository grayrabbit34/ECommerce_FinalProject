using System.ComponentModel.DataAnnotations;
using System.Data;
using ECommerce.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Data.Entities;


//authentication ve authorization işlemlerimizde, oturum açma işlemlerimizde, yetki işlemlerimizde kullanılacak sınıf 
public class UserEntity : BaseEntity
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required, StringLength(50)]
    public string LastName { get; set; } = null!;
    //giriş işlemlerinde kullanılacak

    [Required, EmailAddress]
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    //giriş işlemlerinde kullanılacak
    public string Password { get; set; } = null!;

    ///kullanıcılar yönetici sıfatında da olabilir müşteri de olabilir
    ///bu nedenle kullanıcının türünü belirlemek için Roles tipinde nesneye ihtiyacım var

    public Role Role { get; set; } = Role.Customer;

    public ICollection<OrderEntity> Orders { get; set; } = new List<OrderEntity>();
}

public class UserConfiguration : BaseConfiguration<UserEntity>
{
    public override void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        //First name last name email ve password verileri zorunlu ve max 50 karakter uzunluğunda olmalı
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired();

        //emailin unique olması için gerekli kod
        builder.HasIndex(x => x.Email).IsUnique();

        builder.Property(x => x.Password)
            .IsRequired();

        base.Configure(builder);
    }
}