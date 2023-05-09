using Microsoft.EntityFrameworkCore;
using Moq;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;

namespace UCABPagaloTodoMS.Tests.DataSeed
{
    public static class DataSeed
    {
        public static Mock<DbSet<ProviderEntity>> mockSetProviderEntity = new Mock<DbSet<ProviderEntity>>();
        public static void SetupDbContextData(this Mock<IUCABPagaloTodoDbContext> mockContext)
        {
            var providers = new List<ProviderEntity>()
            {
                new ProviderEntity()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Username = "prueba",
                    PasswordHash = "Password",
                    Email = "prueba@prueba.com",
                    Name = "Jhonny",
                    LastName = "Test",
                    Status = true,
                    Rif = "V123456789",
                    AccountNumber = "012345678909876543212"
                },
                new ProviderEntity()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Username = "test",
                    PasswordHash = "Password",
                    Email = "test@test.com",
                    Name = "Juan",
                    LastName = "Parcial",
                    Status = true,
                    Rif = "V123456729",
                    AccountNumber = "012345578909876543212"
                }
            };
            
            mockSetProviderEntity.As<IQueryable<ProviderEntity>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
            mockSetProviderEntity.As<IQueryable<ProviderEntity>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
            mockSetProviderEntity.As<IQueryable<ProviderEntity>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
            mockSetProviderEntity.As<IQueryable<ProviderEntity>>().Setup(m => m.GetEnumerator()).Returns(providers.GetEnumerator());

            mockContext.Setup(c => c.Providers).Returns(mockSetProviderEntity.Object);
        }
    }
}
