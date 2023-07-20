using System.Diagnostics.CodeAnalysis;
using MockQueryable.Moq;
using Moq;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Enums;
using UCABPagaloTodoMS.Infrastructure.Utils;

namespace UCABPagaloTodoMS.Tests.DataSeed
{
    [ExcludeFromCodeCoverage]
    public static class DataSeed
    {
        public static void SetupDbContextData(this Mock<IUCABPagaloTodoDbContext> mockContext)
        {
            // Admins data
            var admins = new List<AdminEntity>
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = SecurePasswordHasher.Hash("Password."),
                    Email = "prueba@prueba.com",
                    Name = "Jhonny",
                    Status = true,
                },
                new ()
                {
                    Id = Guid.NewGuid(),
                    Username = "admin2",
                    PasswordHash = SecurePasswordHasher.Hash("Password."),
                    Email = "prueba@prueba.com",
                    Name = "Jhonny",
                    Status = false,
                }
                
            };
            
            //Providers data
            var providers = new List<ProviderEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Username = "prueba",
                    PasswordHash = SecurePasswordHasher.Hash("Password."),
                    Email = "prueba@prueba.com",
                    Name = "Jhonny",
                    Status = true,
                    Rif = "V123456789",
                    AccountNumber = "12345678909876543212",
                    Services = new List<ServiceEntity>()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Username = "test",
                    PasswordHash = SecurePasswordHasher.Hash("Password."),
                    Email = "test@test.com",
                    Name = "Juan",
                    Status = false,
                    Rif = "V123456729",
                    AccountNumber = "09876543211234567890",
                    Services = new List<ServiceEntity>()
                }
            };

            //Services data
            var services = new List<ServiceEntity>()
            {
                new()
                {
                    Id = new Guid("12345678-1234-1234-1234-1234567890AC"),
                    Name = "ferreLeon",
                    Description = "Ferreteria a domicilio",
                    ServiceStatus = ServiceStatusEnum.Inactivo,
                    ServiceType = ServiceTypeEnum.Directo,
                    Provider = providers[0],
                    Payments = new List<PaymentEntity>(),
                    ConciliationFormat = new List<FieldEntity>(),
                    PaymentFormat = new List<PaymentFieldEntity>()
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "directv Colombia",
                    Description = "television satelital en toda latinoamerica",
                    ServiceStatus = ServiceStatusEnum.Activo,
                    ServiceType = ServiceTypeEnum.PorConfirmacion,
                    Provider = providers[1],
                    Payments = new List<PaymentEntity>(),
                    ConfirmationList = new List<DebtorsEntity>(),
                    ConciliationFormat = new List<FieldEntity>(),
                    PaymentFormat = new List<PaymentFieldEntity>()
                }
            };
            providers[0].Services!.Add(services[0]);
            providers[1].Services!.Add(services[1]);

            //Fields data
            var fields = new List<FieldEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Field 1",
                    Format = "XXXXXXXXXX",
                    Length = 10,
                    AttrReference = "Payment.Id",
                    Service = services[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Field 2",
                    Format = "XXXXX.XX",
                    Length = 10,
                    AttrReference = "Consumer.LastName",
                    Service = services[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Field 3",
                    Format = "#.00",
                    Length = 10,
                    AttrReference = "Payment.Amount",
                    Service = services[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Field 4",
                    Format = "dd/MM/YYYY",
                    Length = 10,
                    AttrReference = "Payment.PaymentDate",
                    Service = services[0]
                }
            };
            services[0].ConciliationFormat!.AddRange(fields);

            //PaymentFields data
            var paymentFields = new List<PaymentFieldEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Service = services[0],
                    Name = "Test field",
                    Format = ""
                }
            };
            services[0].PaymentFormat!.AddRange(paymentFields);  
            
            //Debtors data
            var debtors = new List<DebtorsEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Amount = 10,
                    Identifier = "1234",
                    Status = false,
                    Service = services[1]
                }
            };
            services[1].ConfirmationList!.AddRange(debtors);

            //Consumers data
            var consumers = new List<ConsumerEntity>()
            {
                new ConsumerEntity()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Username = "prueba",
                    PasswordHash =  SecurePasswordHasher.Hash("Password."),
                    Email = "prueba@prueba.com",
                    Name = "Jhonny",
                    LastName = "Test",
                    Status = true,
                    ConsumerId = "V1234567",
                    Payments = new List<PaymentEntity>()
                },
                new ConsumerEntity()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Username = "test",
                    PasswordHash = SecurePasswordHasher.Hash("Password."),
                    Email = "test@test.com",
                    Name = "Juan",
                    LastName = "Parcial",
                    Status = false,
                    ConsumerId = "V12345678",
                    Payments = new List<PaymentEntity>()
                }
            };

            //Payments data
            var payments = new List<PaymentEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Amount = 25,
                    CardNumber = "4111111111111111",
                    ExpirationMonth = 8,
                    ExpirationYear = 2023,
                    CardholderName = "JHONNY TEST",
                    CardSecurityCode = "123",
                    TransactionId = Guid.NewGuid().ToString(),
                    PaymentStatus = PaymentStatusEnum.Pendiente,
                    Service = services[0],
                    Consumer = consumers[0]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Amount = 50,
                    CardNumber = "5111111111111118",
                    ExpirationMonth = 9,
                    ExpirationYear = 2024,
                    CardholderName = "JUAN PARCIAL",
                    CardSecurityCode = "456",
                    TransactionId = Guid.NewGuid().ToString(),
                    PaymentStatus = PaymentStatusEnum.Aprovado,
                    Service = services[0],
                    Consumer = consumers[1]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.Now,
                    Amount = 10,
                    Identifier = "1234",
                    CardNumber = "5111111111111118",
                    ExpirationMonth = 9,
                    ExpirationYear = 2024,
                    CardholderName = "JUAN PARCIAL",
                    CardSecurityCode = "456",
                    TransactionId = Guid.NewGuid().ToString(),
                    PaymentStatus = PaymentStatusEnum.Aprovado,
                    Service = services[1],
                    Consumer = consumers[1]
                }
            };
            services[0].Payments!.Add(payments[0]);
            services[0].Payments!.Add(payments[1]);
            services[1].Payments!.Add(payments[2]);
            consumers[0].Payments!.Add(payments[0]);
            consumers[1].Payments!.Add(payments[1]);
            consumers[1].Payments!.Add(payments[2]);

            //AccountingClose data
            var accountingClosures = new List<AccountingCloseEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ExecutedAt = new DateTime(2023, 6, 1)
                }
            };

            //PaymentDetails data
            var paymentDetails = new List<PaymentDetailEntity>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Payment = new PaymentEntity()
                }
            };
            
            //Admins setup
            mockContext.Setup(c => c.Admins).Returns(admins.AsQueryable().BuildMockDbSet().Object);
            
            //Providers setup
            mockContext.Setup(c => c.Providers).Returns(providers.AsQueryable().BuildMockDbSet().Object);

            //Services setup
            mockContext.Setup(c => c.Services).Returns(services.AsQueryable().BuildMockDbSet().Object);

            //Fields setup
            mockContext.Setup(c => c.Fields).Returns(fields.AsQueryable().BuildMockDbSet().Object);

            //PaymentFields setup
            mockContext.Setup(c => c.PaymentFields).Returns(paymentFields.AsQueryable().BuildMockDbSet().Object);

            //Debtors setup
            mockContext.Setup(c => c.Debtors).Returns(debtors.AsQueryable().BuildMockDbSet().Object);

            //Consumers setup
            mockContext.Setup(c => c.Consumers).Returns(consumers.AsQueryable().BuildMockDbSet().Object);

            //Payments setup
            mockContext.Setup(c => c.Payments).Returns(payments.AsQueryable().BuildMockDbSet().Object);
            
            //AccountingClose setup
            mockContext.Setup(c => c.AccountingClosures).Returns(accountingClosures.AsQueryable().BuildMockDbSet().Object);
            
            //PaymentFields setup
            mockContext.Setup(c => c.PaymentFields).Returns(paymentFields.AsQueryable().BuildMockDbSet().Object);
            
            //PaymentDetails setup
            mockContext.Setup(c => c.PaymentDetails).Returns(paymentDetails.AsQueryable().BuildMockDbSet().Object);
        }
    }
}