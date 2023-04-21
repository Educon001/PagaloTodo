using BbcTravelMS.Models.Starter;
using BbcTravelMS.ServiceBus.Consumer;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace BbcTravelMS.Tests.UnitTests.ServicesBus.Consumer;

public class StarterConsumerTests
{
    private readonly StarterConsumer _starterConsumer;

    public StarterConsumerTests()
    {
        var logger = new Mock<ILogger<StarterConsumer>>();
        _starterConsumer = new StarterConsumer(logger.Object);
    }

    [Fact(DisplayName = "Consume starter queue")]
    public void ConsumeAsync()
    {
        var context = GetConsumeContext();
        var requestData = new StarterRequest { Id = Guid.NewGuid(), Name = "rlasso" };
        context.SetupGet(c => c.Message)
            .Returns(requestData);
        var task = _starterConsumer.Consume(context.Object);

        Assert.NotNull(task);
        task.GetAwaiter().GetResult();
    }

    private static Mock<ConsumeContext<StarterRequest>> GetConsumeContext()
    {
        return new();
    }
}
