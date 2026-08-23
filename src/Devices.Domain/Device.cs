using Devices.Domain.Exceptions;

namespace Devices.Domain;

public class Device
{
    private Device()
    {
        
    }

    public Device(
        string name,
        string brand,
        DeviceState state)
    {
        Id = Guid.NewGuid();
        Name = name;
        Brand = brand;
        State = state;
        
        var now = DateTime.UtcNow;

        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Brand { get; private set; }

    public DeviceState State { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public void UpdateDetails(
        string name,
        string brand,
        DeviceState state)
    {
        if (State == DeviceState.InUse &&
            (Name != name || Brand != brand))
        {
            throw new DeviceUpdateConflictException(
                "Name and brand cannot be updated while the device is in use.");
        }

        var changed =
            Name != name ||
            Brand != brand ||
            State != state;

        if (!changed)
        {
            return;
        }

        Name = name;
        Brand = brand;
        State = state;
        UpdatedAt = DateTime.UtcNow;
    }
}