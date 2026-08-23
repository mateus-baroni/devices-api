using Devices.Domain;
using Devices.Domain.Exceptions;

namespace Devices.Tests.Domain;

public class DeviceTests
{
    [Fact]
    public void Create_ShouldInitializeDevice()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.Available);

        // Assert
        var after = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, device.Id);
        Assert.Equal("iPhone 15", device.Name);
        Assert.Equal("Apple", device.Brand);
        Assert.Equal(DeviceState.Available, device.State);

        Assert.InRange(device.CreatedAt, before, after);
        Assert.Equal(device.CreatedAt, device.UpdatedAt);
        Assert.Null(device.DeletedAt);
    }

    [Fact]
    public void UpdateDetails_ShouldUpdateDevice()
    {
        // Arrange
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.Available);

        var originalCreatedAt = device.CreatedAt;
        var originalUpdatedAt = device.UpdatedAt;

        // Act
        device.UpdateDetails(
            "iPhone 16",
            "Apple",
            DeviceState.InUse);

        // Assert
        Assert.Equal("iPhone 16", device.Name);
        Assert.Equal("Apple", device.Brand);
        Assert.Equal(DeviceState.InUse, device.State);

        Assert.Equal(originalCreatedAt, device.CreatedAt);
        Assert.True(device.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void UpdateDetails_WhenNothingChanges_ShouldNotUpdateUpdatedAt()
    {
        // Arrange
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.Available);

        var originalUpdatedAt = device.UpdatedAt;

        // Act
        device.UpdateDetails(
            "iPhone 15",
            "Apple",
            DeviceState.Available);

        // Assert
        Assert.Equal(originalUpdatedAt, device.UpdatedAt);
    }

    [Fact]
    public void UpdateDetails_WhenInUseAndNameChanges_ShouldThrow()
    {
        // Arrange
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.InUse);

        // Act
        var exception = Assert.Throws<DeviceUpdateConflictException>(() =>
            device.UpdateDetails(
                "iPhone 16",
                "Apple",
                DeviceState.InUse));

        // Assert
        Assert.Equal(
            "Name and brand cannot be updated while the device is in use.",
            exception.Message);
    }

    [Fact]
    public void UpdateDetails_WhenInUseAndBrandChanges_ShouldThrow()
    {
        // Arrange
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.InUse);

        // Act
        var exception = Assert.Throws<DeviceUpdateConflictException>(() =>
            device.UpdateDetails(
                "iPhone 15",
                "Samsung",
                DeviceState.InUse));

        // Assert
        Assert.Equal(
            "Name and brand cannot be updated while the device is in use.",
            exception.Message);
    }

    [Fact]
    public void UpdateDetails_WhenInUseAndOnlyStateChanges_ShouldSucceed()
    {
        // Arrange
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.InUse);

        // Act
        device.UpdateDetails(
            "iPhone 15",
            "Apple",
            DeviceState.Inactive);

        // Assert
        Assert.Equal("iPhone 15", device.Name);
        Assert.Equal("Apple", device.Brand);
        Assert.Equal(DeviceState.Inactive, device.State);
    }

    [Fact]
    public void Delete_WhenDeviceIsNotInUse_ShouldSetDeletedAt()
    {
        // Arrange
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.Available);

        // Act
        device.Delete();

        // Assert
        Assert.NotNull(device.DeletedAt);
    }

    [Fact]
    public void Delete_WhenDeviceIsInUse_ShouldThrow()
    {
        // Arrange
        var device = new Device(
            "iPhone 15",
            "Apple",
            DeviceState.InUse);

        // Act
        var exception = Assert.Throws<DeviceDeletionConflictException>(
            device.Delete);

        // Assert
        Assert.Equal(
            "In-use devices cannot be deleted.",
            exception.Message);

        Assert.Null(device.DeletedAt);
    }
}