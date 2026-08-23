using Devices.Domain;
using Devices.Infrastructure.Persistence;

namespace Devices.Application.Devices;

public class DeviceService
{
    private readonly DevicesDbContext _dbContext;

    public DeviceService(DevicesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> CreateAsync(CreateDeviceRequest request)
    {
        var device = new Device(
            request.Name,
            request.Brand,
            request.State);

        _dbContext.Devices.Add(device);

        await _dbContext.SaveChangesAsync();

        return device.Id;
    }
}