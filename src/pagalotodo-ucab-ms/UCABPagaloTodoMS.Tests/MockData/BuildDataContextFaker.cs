
using Bogus;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoMS.Tests.MockData
{
    public static class BuildDataContextFaker
    {
        /*public static Faker<ServiceRequest> BuildServicesRequest()
        {
            return new Faker<ServiceRequest>();
        }*/

        public static List<ServiceResponse> BuildServicesList()
        {
            var data = new List<ServiceResponse>()
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Jorge",
                    Description = "Servicio de Jorges",
                    ServiceStatus = ServiceStatusEnum.Activo.ToString(),
                    ServiceType = ServiceTypeEnum.Directo.ToString(),
                    Provider = new ProviderResponse()
                    {
                        AccountNumber = "12390491093109101",
                        Email = "jorgealeond@gmail.com",
                        Name = "Jorge",
                        LastName = "Leon",
                        Id = Guid.NewGuid(),
                        PasswordHash = "kfkslf2123lkjl232l",
                        Rif = "V19291292410",
                        Services = new List<ServiceResponse>(),
                        Status = true,
                        Username = "hola123"
                    }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Carol",
                    Description = "Servicio de Carols ",
                    ServiceStatus = ServiceStatusEnum.Inactivo.ToString(),
                    ServiceType = ServiceTypeEnum.Directo.ToString(),
                    Provider = new ProviderResponse()
                    {
                        AccountNumber = "12390491093109104",
                        Email = "jorgealeond@gmail.com",
                        Name = "Carol",
                        LastName = "El Souki",
                        Id = Guid.NewGuid(),
                        PasswordHash = "1fdsfa3e21ka0",
                        Rif = "V19914329",
                        Services = new List<ServiceResponse>(),
                        Status = true,
                        Username = "hola123"
                    }
                }
            };
            return data;
        }
    }
}
