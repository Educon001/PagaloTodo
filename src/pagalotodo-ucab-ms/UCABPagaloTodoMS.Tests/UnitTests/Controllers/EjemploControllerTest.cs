using UCABPagaloTodoMS.Controllers;
using UCABPagaloTodoMS.Tests.MockData;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace UCABPagaloTodoMS.Tests.UnitTests.Controllers
{
    public class EjemploControllerTest
    {
        private readonly EjemploController _controller;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<EjemploController>> _loggerMock;

        public EjemploControllerTest()
        {
            _loggerMock = new Mock<ILogger<EjemploController>>();
            _mediatorMock = new Mock<IMediator>();
            _controller = new EjemploController(_loggerMock.Object, _mediatorMock.Object);
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();
            _controller.ControllerContext.ActionDescriptor = new ControllerActionDescriptor();
        }
    }
}
