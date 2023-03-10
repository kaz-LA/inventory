using System.Reflection;
using Aine.Inventory.Core.CategoryAggregate;
using Aine.Inventory.Core.ProductAggregate;
using Aine.Inventory.Core.ProductInventoryAggregate;
using Aine.Inventory.SharedKernel;
using Aine.Inventory.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aine.Inventory.Infrastructure.Data;

public class AppDbContext : DbContext
{
  private readonly IDomainEventDispatcher? _dispatcher;

  public AppDbContext(DbContextOptions<AppDbContext> options,
    IDomainEventDispatcher? dispatcher)
      : base(options)
  {
    _dispatcher = dispatcher;
  }

  public DbSet<Product> Products => Set<Product>();
  public DbSet<ProductCategory> Categories => Set<ProductCategory>();
  public DbSet<ProductInventory> ProductInventories => Set<ProductInventory>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
  {
    int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    // ignore events if no dispatcher provided
    if (_dispatcher == null) return result;

    // dispatch events only if save was successful
    var entitiesWithEvents = ChangeTracker.Entries<EntityBase>()
        .Select(e => e.Entity)
        .Where(e => e.DomainEvents.Any())
        .ToArray();

    await _dispatcher.DispatchAndClearEvents(entitiesWithEvents);

    return result;
  }

  public override int SaveChanges()
  {
    return SaveChangesAsync().GetAwaiter().GetResult();
  }
}

