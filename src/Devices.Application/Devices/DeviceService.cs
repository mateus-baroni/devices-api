using Devices.Domain;
using Devices.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

    public async Task<DeviceResponse?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Devices
            .Where(d => d.Id == id)
            .Select(d => new DeviceResponse(
                d.Id,
                d.Name,
                d.Brand,
                d.State,
                d.CreatedAt))
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<DeviceResponse>> GetDevicesAsync(
        string? brand,
        DeviceState? state)
    {
        var query = _dbContext.Devices.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(d => d.Brand == brand);
        }

        if (state.HasValue)
        {
            query = query.Where(d => d.State == state.Value);
        }

        return await query
            .Select(d => new DeviceResponse(
                d.Id,
                d.Name,
                d.Brand,
                d.State,
                d.CreatedAt))
            .ToListAsync();
    }
}