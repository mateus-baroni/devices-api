using Devices.Domain;

namespace Devices.Tests.Integration;

public class DevicesDbTests(PostgresFixture fixture)
    : IClassFixture<PostgresFixture>
{
    [Fact]
    public async Task ShouldPersistDevice()
    {
        // Arrange
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.Available);

        // Act
        fixture.DbContext.Devices.Add(device);
        await fixture.DbContext.SaveChangesAsync();

        var result = await fixture.DbContext.Devices
            .FindAsync(device.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(device.Id, result.Id);
        Assert.Equal("iPhone 15", result.Name);
        Assert.Equal("Apple", result.Brand);
        Assert.Equal(DeviceState.Available, result.State);
    }
}