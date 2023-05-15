using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Tests.DataSeed
{
    public static class DataSeed
    {
        public static Mock<DbSet<ProviderEntity>> mockSetProviderEntity = new Mock<DbSet<ProviderEntity>>();
        public static void SetupDbContextData(this Mock<IUCABPagaloTodoDbContext> mockContext)
        {
            var providers = new List<ProviderEntity>()
            {
                new()
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
                new()
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
            
            var services = new List<ServiceEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "ferreLeon",
                    Description = "Ferreteria a domicilio",
                    ServiceStatus = ServiceStatusEnum.Activo,
                    ServiceType = ServiceTypeEnum.Directo,
                    Provider = new ProviderEntity()
                    {
                        Id = Guid.NewGuid(),
                        Username = "prueba",
                        PasswordHash = "Password",
                        Email = "prueba@prueba.com",
                        Name = "Jesus Fermin",
                        LastName = "Fermin",
                        Status = true,
                        Rif = "V12123311",
                        AccountNumber = "012345678909876543212"
                    },
                    Payments = new List<PaymentEntity>(),
                    ConfirmationList = new List<DebtorsEntity>(),
                    ConciliationFormat = new List<FieldEntity>()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "directv Colombia",
                    Description = "television satelital en toda latinoamerica",
                    ServiceStatus = ServiceStatusEnum.Activo,
                    ServiceType = ServiceTypeEnum.Directo,
                    Provider = new ProviderEntity()
                    {
                        Id = Guid.NewGuid(),
                        Username = "provider1",
                        PasswordHash = "provider1212",
                        Email = "probandoshit@outlook.com",
                        Name = "Fernando",
                        LastName = "Delgado",
                        Status = true,
                        Rif = "V12123313",
                        AccountNumber = "012345678909876041912"
                    },
                    Payments = new List<PaymentEntity>(),
                    ConfirmationList = new List<DebtorsEntity>(),
                    ConciliationFormat = new List<FieldEntity>()
                }
            };
            
            mockSetProviderEntity.As<IQueryable<ProviderEntity>>().Setup(m => m.Provider).Returns(providers.AsQueryable().Provider);
            mockSetProviderEntity.As<IQueryable<ProviderEntity>>().Setup(m => m.Expression).Returns(providers.AsQueryable().Expression);
            mockSetProviderEntity.As<IQueryable<ProviderEntity>>().Setup(m => m.ElementType).Returns(providers.AsQueryable().ElementType);
            mockSetProviderEntity.As<IQueryable<ProviderEntity>>().Setup(m => m.GetEnumerator()).Returns(providers.GetEnumerator());

            mockContext.Setup(c => c.Providers).Returns(mockSetProviderEntity.Object);
            mockContext.Setup(c => c.Services).Returns(services.AsQueryable().BuildMockDbSet().Object);
        }
    }
}
