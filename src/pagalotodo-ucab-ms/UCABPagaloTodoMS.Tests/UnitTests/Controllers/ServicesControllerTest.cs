using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using UCABPagaloTodoMS.Application.Commands.Services;
using UCABPagaloTodoMS.Application.Mappers;
using UCABPagaloTodoMS.Application.Queries;
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

    [Fact]
    public async Task GetServices_Returns_Error()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetServicesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        var result = await _controller.GetServices();
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }

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

    [Fact]
    public async Task GetServicesByProviderId_Returns_Error()
    {
        var expectedException = new Exception("Test Exception");
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicesByProviderIdQuery>(), CancellationToken.None))
            .ThrowsAsync(expectedException);
        var response = await _controller.GetServiceByProviderId(Guid.NewGuid());
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        var ex = Assert.IsType<string>(badRequestResult.Value);
        Assert.Contains("Test Exception", ex);
    }

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

    [Fact]
    public async Task GetServiceById_Returns_Error()
    {
        Guid id = Guid.NewGuid();
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetServiceByIdQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        var result = await _controller.GetServiceById(id);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }

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

    [Fact]
    public async Task DeleteService_Returns_Error()
    {
        var id = Guid.NewGuid();
        _mediatorMock.Setup(x => x.Send(It.IsAny<DeleteServiceCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        var response = await _controller.DeleteService(id);
        var badResult = Assert.IsType<BadRequestObjectResult>(response.Result);
        Assert.Equal(400, badResult.StatusCode);
        _mediatorMock.Verify();
    }

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

    [Fact]
    public async Task CreateService_Returns_Error()
    {
        ServiceRequest service = new()
            {Name = "My Service #2", Description = "Whatever # 2", Provider = Guid.NewGuid()};
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateServiceCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        var result = await _controller.CreateService(service);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }

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

    [Fact]
    public async Task UpdateService_Returns_Error()
    {
        Guid id = Guid.NewGuid();
        ServiceRequest service = new()
        {
            Name = "ferreLeon",
            Description = "Ferreteria a domicilio", Provider = new Guid("12345678-1234-1234-1234-1234567890AB"),
            ServiceType = ServiceTypeEnum.Directo, ServiceStatus = ServiceStatusEnum.Inactivo
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateServiceCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        var result = await _controller.UpdateService(service, id);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }


    [Fact]
    public async Task CreateFormat_Returns_Ok()
    {
        List<FieldRequest> fieldRequests = new List<FieldRequest>
        {
            new()
            {
                Name = "Campo#1", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "Payment.Id",
                Service = new Guid("12345678-1234-1234-1234-1234567890AC")
            },
            new()
            {
                Name = "Campo#2", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "Consumer.Id",
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

    [Fact]
    public async Task CreateFormat_Returns_Error()
    {
        List<FieldRequest> fieldRequests = new List<FieldRequest>
        {
            new()
            {
                Name = "Campo#1", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "Payment.Id",
                Service = new Guid("12345678-1234-1234-1234-1234567890AC")
            },
            new()
            {
                Name = "Campo#2", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "Consumer.Id",
                Service = new Guid("12345678-1234-1234-1234-1234567890AC")
            }
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        var result = await _controller.CreateFormat(fieldRequests);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }

    [Fact]
    public async Task Update_Field_Returns_Ok()
    {
        Guid id = new Guid("12345678-1234-1234-1234-1234567890AC");
        FieldRequest fieldRequest = new()
        {
            Name = "Campo#1", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "Payment.Id",
            Service = new Guid("12345678-1234-1234-1234-1234567890AC")
        };
        ServiceEntity serviceE = new ServiceEntity() {Id = new Guid("12345678-1234-1234-1234-1234567890AC")};
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

    [Fact]
    public async Task Update_Field_Returns_Error()
    {
        Guid id = new Guid("12345678-1234-1234-1234-1234567890AC");
        FieldRequest fieldRequest = new()
        {
            Name = "Campo#1", Length = 10, Format = "XXXXXXXXXXX", AttrReference = "Payment.Id",
            Service = new Guid("12345678-1234-1234-1234-1234567890AC")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
        var result = await _controller.UpdateField(fieldRequest, id);
        var response = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, response.StatusCode);
        _mediatorMock.Verify();
    }
}