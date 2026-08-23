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
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string Brand { get; private set; }

    public DeviceState State { get; private set; }

    public DateTime CreatedAt { get; private set; }
}