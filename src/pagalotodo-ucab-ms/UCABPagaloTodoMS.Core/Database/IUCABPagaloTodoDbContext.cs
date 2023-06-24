using Microsoft.EntityFrameworkCore;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Core.Database
{
    public interface IUCABPagaloTodoDbContext
    {
        DbSet<AccountingCloseEntity> AccountingClosures { get; set; }
        DbSet<AdminEntity> Admins { get; set;  }
        DbSet<ConsumerEntity> Consumers { get; set; }
        DbSet<DebtorsEntity> Debtors { get; set;  }
        DbSet<FieldEntity> Fields { get; set;  }
        DbSet<PaymentEntity> Payments { get; set; }
        DbSet<ProviderEntity> Providers { get; set; }
        DbSet<ServiceEntity> Services { get; set;  }

        DbContext DbContext
        {
            get;
        }

        IDbContextTransactionProxy BeginTransaction();

        void ChangeEntityState<TEntity>(TEntity entity, EntityState state);

        Task<bool> SaveEfContextChanges(string user, CancellationToken cancellationToken = default);

    }
}
