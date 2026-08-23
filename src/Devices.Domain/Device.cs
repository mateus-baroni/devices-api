namespace Devices.Domain;

public class Device
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Brand { get; private set; }

    public DeviceState State { get; private set; }

    public DateTime CreatedAt { get; private set; }
}