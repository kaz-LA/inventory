using Aine.Inventory.Core.CategoryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aine.Inventory.Infrastructure.Data.Config;

public class CategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
  public void Configure(EntityTypeBuilder<ProductCategory> builder)
  {
    builder.ToTable("product_category");

    builder.Property(t => t.Id).HasColumnName("id");

    builder.HasIndex(p => p.Name).IsUnique();

    builder.HasKey(p => p.Id);

    builder.Property(t => t.Name)
      .HasColumnName("name")
      .HasMaxLength(ProductCategory.MAX_NAME_LENGTH)
        .IsRequired();

    builder.Property(t => t.Description)
        .HasColumnName("description")
        .HasMaxLength(250)
        .IsRequired(false);
  }
}

