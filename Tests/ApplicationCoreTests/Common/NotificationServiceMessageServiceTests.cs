using ApplicationCore.Common;
using Xunit;

namespace Tests.ApplicationCoreTests.Common;

public class NotificationServiceMessageServiceTests
{
    [Fact]
    public void Create_NotificationMessage_Success()
    {
        var service = new NotificationMessageService();
        var eventWasRaised = false;
        service.Notify += (sender, args) => eventWasRaised = true;

        // Act
        service.Create("", "", NotificationType.Success);

        // Assert
        Assert.True(eventWasRaised);
    }
}
