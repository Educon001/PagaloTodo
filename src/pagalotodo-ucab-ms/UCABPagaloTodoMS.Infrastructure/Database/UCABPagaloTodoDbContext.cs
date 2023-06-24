using System.Linq.Expressions;
using System.Reflection;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Infrastructure.Database;

public class UCABPagaloTodoDbContext : DbContext, IUCABPagaloTodoDbContext
{
    public UCABPagaloTodoDbContext(DbContextOptions<UCABPagaloTodoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountingCloseEntity> AccountingClosures { get; set; } = null!;
    public virtual DbSet<AdminEntity> Admins { get; set; } = null!;
    public virtual DbSet<ConsumerEntity> Consumers { get; set; } = null!;
    public virtual DbSet<DebtorsEntity> Debtors { get; set; } = null!;
    public virtual DbSet<FieldEntity> Fields { get; set; } = null!;
    public virtual DbSet<PaymentEntity> Payments { get; set; } = null!;
    public virtual DbSet<ProviderEntity> Providers { get; set; } = null!;
    public virtual DbSet<ServiceEntity> Services { get; set; } = null!;

    public DbContext DbContext
    {
        get { return this; }
    }

    public IDbContextTransactionProxy BeginTransaction()
    {
        return new DbContextTransactionProxy(this);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        //IsDeleted Query Filter
        modelBuilder.Entity<AdminEntity>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ConsumerEntity>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<DebtorsEntity>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<FieldEntity>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<PaymentEntity>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ProviderEntity>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ServiceEntity>().HasQueryFilter(e => !e.IsDeleted);
        //Default admin
        modelBuilder.Entity<AdminEntity>().HasData(new AdminEntity()
        {
            Username = "admin",
            PasswordHash = SecurePasswordHasher.Hash("admin"),
            Name = "admin",
            Email = "pagalotodoucabaf@gmail.com",
            Id = Guid.NewGuid(),
            Status = true,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            CreatedBy = "APP"
        });
    }

    virtual public void SetPropertyIsModifiedToFalse<TEntity, TProperty>(TEntity entity,
        Expression<Func<TEntity, TProperty>> propertyExpression) where TEntity : class
    {
        Entry(entity).Property(propertyExpression).IsModified = false;
    }

    virtual public void ChangeEntityState<TEntity>(TEntity entity, EntityState state)
    {
        if (entity != null)
        {
            Entry(entity).State = state;
        }
    }

    public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity) entityEntry.Entity).CreatedAt = DateTime.UtcNow;
                ((BaseEntity) entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
                ((BaseEntity) entityEntry.Entity).IsDeleted = false;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                ((BaseEntity) entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
                Entry((BaseEntity) entityEntry.Entity).Property(x => x.CreatedAt).IsModified = false;
                Entry((BaseEntity) entityEntry.Entity).Property(x => x.CreatedBy).IsModified = false;
            }

            if (entityEntry.State == EntityState.Deleted)
            {
                entityEntry.State = EntityState.Modified;
                ((BaseEntity) entityEntry.Entity).IsDeleted = true;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(string user, CancellationToken cancellationToken = default)
    {
        var state = new List<EntityState> {EntityState.Added, EntityState.Modified, EntityState.Deleted};

        var entries = ChangeTracker.Entries().Where(e =>
            e.Entity is BaseEntity && state.Any(s => e.State == s)
        );

        var dt = DateTime.UtcNow;

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity) entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = dt;
                entity.CreatedBy = user;
                Entry(entity).Property(x => x.UpdatedAt).IsModified = false;
                Entry(entity).Property(x => x.UpdatedBy).IsModified = false;
                entity.IsDeleted = false;
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entity.UpdatedAt = dt;
                entity.UpdatedBy = user;
                Entry(entity).Property(x => x.CreatedAt).IsModified = false;
                Entry(entity).Property(x => x.CreatedBy).IsModified = false;
            }

            if (entityEntry.State == EntityState.Deleted)
            {
                entityEntry.State = EntityState.Modified;
                entity.IsDeleted = true;
                if (entity.GetType().IsSubclassOf(typeof(UserEntity)))
                {
                    ((UserEntity) entity).Username = null;
                    ((UserEntity) entity).Email = null;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> SaveEfContextChanges(CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(cancellationToken) >= 0;
    }

    public async Task<bool> SaveEfContextChanges(string user, CancellationToken cancellationToken = default)
    {
        return await SaveChangesAsync(user, cancellationToken) >= 0;
    }
}