using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Core.Database
{
    public interface IUCABPagaloTodoDbContext
    {
        DbSet<ValoresEntity> Valores { get; }
        DbSet<AdminEntity> Admins { get; }
        DbSet<ConsumerEntity> Consumers { get; }
        DbSet<DebtorsEntity> Debtors { get; }
        DbSet<FieldEntity> Fields { get; }
        DbSet<PaymentEntity> Payments { get; }
        DbSet<ProviderEntity> Providers { get; }
        DbSet<ServiceEntity> Services { get; }

        DbContext DbContext
        {
            get;
        }

        IDbContextTransactionProxy BeginTransaction();

        void ChangeEntityState<TEntity>(TEntity entity, EntityState state);

        Task<bool> SaveEfContextChanges(string user, CancellationToken cancellationToken = default);

    }
}
