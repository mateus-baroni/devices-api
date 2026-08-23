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
                d.CreatedAt,
                d.UpdatedAt))
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
                d.CreatedAt,
                d.UpdatedAt))
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateDeviceRequest request)
    {
        var device = await _dbContext.Devices
            .FirstOrDefaultAsync(d => d.Id == id);

        if (device is null)
        {
            return false;
        }

        device.UpdateDetails(
            request.Name,
            request.Brand,
            request.State);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> PatchAsync(
        Guid id,
        PatchDeviceRequest request)
    {
        var device = await _dbContext.Devices
            .FirstOrDefaultAsync(d => d.Id == id);

        if (device is null)
        {
            return false;
        }

        device.UpdateDetails(
            request.Name ?? device.Name,
            request.Brand ?? device.Brand,
            request.State ?? device.State);

        await _dbContext.SaveChangesAsync();

        return true;
    }
    
}