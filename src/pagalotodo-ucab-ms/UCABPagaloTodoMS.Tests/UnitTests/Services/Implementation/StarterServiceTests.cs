using BbcTravelMS.Entities;
using BbcTravelMS.Services.Implementation;
using Xunit;

namespace BbcTravelMS.Tests.UnitTests.Services.Implementation;

public class StarterServiceTests
{
    [Fact]
    public void Check()
    {
        var service = new StarterService();

        service.GetStarterList();
        service.GetStarterById(Guid.NewGuid());
        service.CreateStarterAsync(new StarterEntity());
        service.UpdateStarterAsync(Guid.NewGuid(), new StarterEntity());
        service.DeleteStarter(new StarterEntity());
        service.CalculateAgeRate(DateTime.Now);

        Assert.Null(null);
    }
}
