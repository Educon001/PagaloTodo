using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Exceptions;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
using UCABPagaloTodoMS.Application.Queries.Debtors;
using UCABPagaloTodoMS.Application.Queries.Services;
using UCABPagaloTodoMS.Application.Requests;
using UCABPagaloTodoMS.Application.Responses;
using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Core.Database;
using UCABPagaloTodoMS.Core.Entities;
using UCABPagaloTodoMS.Core.Enums;
using UCABPagaloTodoMS.Tests.MockData;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers;

[ExcludeFromCodeCoverage]
public class ServicesControllerTest
{
    private readonly ServicesController _controller;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IUCABPagaloTodoDbContext> _mockContext;

    public ServicesControllerTest()
    {
        var loggerMock = new Mock<ILogger<ServicesController>>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new ServicesController(loggerMock.Object, _mediatorMock.Object);
        _controller.ControllerContext = new ControllerContext();
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
        _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        _mockContext = new Mock<IUCABPagaloTodoDbContext>();
        DataSeed.DataSeed.SetupDbContextData(_mockContext);
    }

    /// <summary>
    ///     Prueba de metodo GetServices Ok
    /// </summary>
    [Fact]
    public async Task GetServices_Returns_OK()
    {
        var services = BuildDataContextFaker.BuildServicesList();

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetServicesQuery>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(services));

        var result = await _controller.GetServices();
        var response = Assert.IsType<OkObjectResult>(result.Result);

        //El tipo es de lista de ServiceResponse
        Assert.IsType<List<ServiceResponse>>(response.Value);
        //La respuesta es la esperada
        Assert.Equal(services, response.Value);
        //Status code = 200
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo GetServices retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async Task GetServices_Returns_Error(Type exceptionType)
    {
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetServicesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        var result = await _controller.GetServices();
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo GetServiceByProviderId Ok
    /// </summary>
    [Fact]
    public async Task GetServicesByProviderId_Returns_OK()
    {
        var entity = _mockContext.Object.Services.First();
        var expectedResponse = new List<ServiceResponse>();
        expectedResponse.Add(ServiceMapper.MapEntityToResponse(entity, false));
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicesByProviderIdQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResponse);
        var response = await _controller.GetServiceByProviderId(entity.Provider!.Id);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<List<ServiceResponse>>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo GetServiceByProviderId retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async Task GetServicesByProviderId_Returns_Error(Type exceptionType)
    {
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicesByProviderIdQuery>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.GetServiceByProviderId(Guid.NewGuid());
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }

    /// <summary>
    ///     Prueba de metodo GetServiceById Ok
    /// </summary>
    [Fact]
    public async Task GetServiceById_Returns_Ok()
    {
        Guid id = Guid.NewGuid();
        ServiceResponse expectedResponse = new()
            {Name = "My Service #1", Description = "hola", Id = id};

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetServiceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.GetServiceById(id);
        var response = Assert.IsType<OkObjectResult>(result.Result);
        //El tipo es de lista de ServiceResponse
        Assert.IsType<ServiceResponse>(response.Value);
        Assert.Equal(expectedResponse, response.Value);
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }
    
    /// <summary>
    ///     Prueba de metodo GetServiceById retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async Task GetServiceById_Returns_Error(Type exceptionType)
    {
        Guid id = Guid.NewGuid();
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetServiceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var result = await _controller.GetServiceById(id);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo DeleteService Ok
    /// </summary>
    [Fact]
    public async Task DeleteService_Returns_OK()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteServiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(id);
        var response = await _controller.DeleteService(id);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Equal(id, okResult.Value);
        Assert.Equal(200, okResult.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo DeleteService retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async Task DeleteService_Returns_Error(Type exceptionType)
    {
        var id = Guid.NewGuid();
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteServiceCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        var response = await _controller.DeleteService(id);
        var badResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.Equal(400, badResult.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo CreateService retorna Ok
    /// </summary>
    [Fact]
    public async Task CreateService_Returns_OK()
    {
        ServiceRequest service = new()
        {
            Name = "My Service", Description = "Whatever", ServiceStatus = ServiceStatusEnum.Activo,
            ServiceType = ServiceTypeEnum.Directo, Provider = Guid.NewGuid()
        };
        Guid expectedResponse = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateServiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        var result = await _controller.CreateService(service);
        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, response.Value);
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo CreateService retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async Task CreateService_Returns_Error(Type exceptionType)
    {
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        ServiceRequest service = new()
            {Name = "My Service", Description = "Whatever", ServiceStatus = ServiceStatusEnum.Activo,
                ServiceType = ServiceTypeEnum.Directo, Provider = Guid.NewGuid()};
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateServiceCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        var result = await _controller.CreateService(service);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo UpdateService retorna Ok
    /// </summary>
    [Fact]
    public async Task UpdateService_Returns_Ok()
    {
        Guid id = new Guid("12345678-1234-1234-1234-1234567890AC");
        ServiceRequest service = new()
        {
            Name = "ferreLeon",
            Description = "Ferreteria a domicilio", Provider = new Guid("12345678-1234-1234-1234-1234567890AB"),
            ServiceType = ServiceTypeEnum.Directo, ServiceStatus = ServiceStatusEnum.Inactivo
        };
        ProviderEntity providerE = new ProviderEntity() {Id = new Guid("12345678-1234-1234-1234-1234567890AB")};
        ServiceResponse expectedResponse =
            ServiceMapper.MapEntityToResponse(ServiceMapper.MapRequestToEntity(service, providerE), false);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateServiceCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        var result = await _controller.UpdateService(service, id);
        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, response.Value);
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo UpdateService retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async Task UpdateService_Returns_Error(Type exceptionType)
    {
        Guid id = Guid.NewGuid();
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        ServiceRequest service = new()
        {
            Name = "ferreLeon",
            Description = "Ferreteria a domicilio", Provider = new Guid("12345678-1234-1234-1234-1234567890AB"),
            ServiceType = ServiceTypeEnum.Directo, ServiceStatus = ServiceStatusEnum.Inactivo
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateServiceCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        var result = await _controller.UpdateService(service, id);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }

    
    /// <summary>
    ///     Prueba de metodo CreateFormat Ok
    /// </summary>
    [Fact]
    public async Task CreateFormat_Returns_Ok()
    {
        List<FieldRequest> fieldRequests = new List<FieldRequest>
        {
            new()
            {
                Name = "Campo#1", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "payment.id",
                Service = new Guid("12345678-1234-1234-1234-1234567890AC")
            },
            new()
            {
                Name = "Campo#2", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "consumer.id",
                Service = new Guid("12345678-1234-1234-1234-1234567890AC")
            }
        };

        List<Guid> expectedResponse = new() {new Guid(new byte[16]), new Guid(new byte[16])};
        _mediatorMock.Setup(m => m.Send(It.IsAny<List<CreateFieldCommand>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        var result = await _controller.CreateFormat(fieldRequests);
        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, response.Value);
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo CreateFormat retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async Task CreateFormat_Returns_Error(Type exceptionType)
    {
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        List<FieldRequest> fieldRequests = new List<FieldRequest>
        {
            new()
            {
                Name = "Campo#1", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "payment.id",
                Service = new Guid("12345678-1234-1234-1234-1234567890AC")
            },
            new()
            {
                Name = "Campo#2", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "consumer.id",
                Service = new Guid("12345678-1234-1234-1234-1234567890AC")
            }
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        var result = await _controller.CreateFormat(fieldRequests);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }
    
    /// <summary>
    ///     Prueba de metodo UpdateField Ok
    /// </summary>
    [Fact]
    public async Task Update_Field_Returns_Ok()
    {
        Guid id = new Guid("12345678-1234-1234-1234-1234567890AC");
        FieldRequest fieldRequest = new()
        {
            Name = "Campo#1", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "payment.amount",
            Service = new Guid("12345678-1234-1234-1234-1234567890AC")
        };
        ServiceEntity serviceE = new ServiceEntity{Id = new Guid("12345678-1234-1234-1234-1234567890AC")};
        FieldResponse expectedResponse =
            FieldMapper.MapEntityToResponse(FieldMapper.MapRequestToEntity(fieldRequest, serviceE));
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);
        var result = await _controller.UpdateField(fieldRequest, id);
        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedResponse, response.Value);
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }

    /// <summary>
    ///     Prueba de metodo UpdateField retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async Task Update_Field_Returns_Error(Type exceptionType)
    {
        Guid id = new Guid("12345678-1234-1234-1234-1234567890AC");
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        FieldRequest fieldRequest = new()
        {
            Name = "Campo#1", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "Payment.Id",
            Service = new Guid("12345678-1234-1234-1234-1234567890AC")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        var result = await _controller.UpdateField(fieldRequest, id);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }
    
    /// <summary>
    ///     Prueba de metodo UploadConfirmationList Ok
    /// </summary>
    [Fact]
    public async void UploadConfirmationList_Returns_Ok()
    {
        var mockFile = new Mock<IFormFile>();
        var expectedResponse = "El archivo fue agregado a la cola";
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<MemoryStream>(), default))
            .Returns(Task.CompletedTask);
        _mediatorMock.Setup(m => m.Send(It.IsAny<UploadDebtorsRequest>(), default))
            .ReturnsAsync(true);
        var response = await _controller.UploadConfirmationList(Guid.Empty, mockFile.Object);
        var okResult = Assert.IsType<OkObjectResult>(response.Result);
        Assert.IsType<string>(okResult.Value);
        Assert.Equal(expectedResponse, okResult.Value);
    }

    /// <summary>
    ///     Prueba de metodo UploadConciliationResponse BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async void UploadConfirmationList_Returns_BadRequest(Type exceptionType)
    {
        var mockFile = new Mock<IFormFile>();
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        mockFile.Setup(f => f.CopyToAsync(It.IsAny<MemoryStream>(),default))
            .Throws(expectedException);
        var response = await _controller.UploadConfirmationList(Guid.Empty, mockFile.Object);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.IsType<string>(badRequestResult.Value);
        Assert.Equal(expectedException.Message, badRequestResult.Value);
    }

    ///<summary>
    ///     Prueba de metodo de GetFieldById
    /// </summary>
    [Fact]
    public async void GetFieldById_Returns_Ok()
    {
        Guid id = Guid.NewGuid();
        FieldResponse expectedResponse = new()
            {Name = "My Service #1", Id = id};

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetFieldByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.GetFieldById(id);
        var response = Assert.IsType<OkObjectResult>(result.Result);
        //El tipo es de lista de ServiceResponse
        Assert.IsType<FieldResponse>(response.Value);
        Assert.Equal(expectedResponse, response.Value);
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }
    
    ///<summary>
    ///     Prueba de metodo de GetFieldById Retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async void GetFieldById_Returns_BadRequest(Type exceptionType)
    {
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        Guid id = Guid.NewGuid();
        FieldResponse expectedResponse = new()
            {Name = "My Service #1", Id = id};

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetFieldByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);
        
        var response  = await _controller.GetFieldById(id);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
        _mediatorMock.Verify();
    }

    ///<summary>
    ///     Prueba de metodo de GetDebtorsByServiceId retorna Ok
    /// </summary>
    [Fact]
    public async void GetDebtorsByServiceId_Returns_Ok()
    {
        Guid id = Guid.NewGuid();
        List<DebtorsResponse> expectedResponse = new List<DebtorsResponse>
            {new(){Id = id, Amount = 200, Identifier = "hola"}};

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetDebtorsByServiceIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var result = await _controller.GetDebtorsByServiceIdQuery(id);
        var response = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsType<List<DebtorsResponse>>(response.Value);
        Assert.Equal(expectedResponse, response.Value);
        Assert.Equal(200, response.StatusCode);
        _mediatorMock.Verify();
    }
    
    ///<summary>
    ///     Prueba de metodo de GetDebtorsByServiceId retorna BadRequest
    /// </summary>
    [Theory]
    [InlineData(typeof(Exception))]
    [InlineData(typeof(CustomException))]
    public async void GetDebtorsByServiceId_Returns_BadRequest(Type exceptionType)
    {
        var expectedException = (Exception)Activator.CreateInstance(exceptionType, "Test Exception");
        Guid id = Guid.NewGuid();
        List<DebtorsResponse> expectedResponse = new List<DebtorsResponse>
            {new(){Id = id, Amount = 200, Identifier = "hola"}};

        _mediatorMock.Setup(x => x.Send(It.IsAny<GetDebtorsByServiceIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        var response  = await _controller.GetDebtorsByServiceIdQuery(id);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
        _mediatorMock.Verify();
    }
}