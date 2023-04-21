using BbcTravelMS.BusinessLogic.Starter;
using BbcTravelMS.Models.Starter;
using Xunit;

namespace BbcTravelMS.Tests.UnitTests.BusinessLogic.Starter;

public class GetStarterDetailTests
{
    [Fact(DisplayName = "CalculateAgeRateOk Should Return OK")]
    public void CalculateAgeRateOk()
    {
        var result = 0;

        var requestData = new AgeRateRequest { BirthDay = DateTime.Now };

        result = GetStarterDetail.CalculateAgeRate(requestData.BirthDay);
        Assert.IsType<int>(result);
    }
}
