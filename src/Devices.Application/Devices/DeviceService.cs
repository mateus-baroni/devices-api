using Devices.Domain;
using Devices.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Devices.Application.Devices;

public class DeviceService
{
    private readonly DevicesDbContext _dbContext;
    private readonly ILogger<DeviceService> _logger;

    public DeviceService(
        DevicesDbContext dbContext,
        ILogger<DeviceService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> CreateAsync(CreateDeviceRequest request)
    {
        var device = new Device(
            request.Name,
            request.Brand,
            request.State);

        _dbContext.Devices.Add(device);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Device {DeviceId} created with brand {Brand} and state {State}",
            device.Id,
            device.Brand,
            device.State);

        return device.Id;
    }

    public async Task<DeviceResponse?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Devices
            .AsNoTracking()
            .Where(d => d.Id == id && d.DeletedAt == null)
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
        var query = _dbContext.Devices
            .AsNoTracking()
            .Where(d => d.DeletedAt == null)
            .AsQueryable();

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
            .FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);

        if (device is null)
        {
            _logger.LogWarning(
                "Device {DeviceId} was not found for update",
                id);

            return false;
        }

        device.UpdateDetails(
            request.Name,
            request.Brand,
            request.State);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Device {DeviceId} updated",
            id);

        return true;
    }

    public async Task<bool> PatchAsync(
        Guid id,
        PatchDeviceRequest request)
    {
        var device = await _dbContext.Devices
            .FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);

        if (device is null)
        {
            _logger.LogWarning(
                "Device {DeviceId} was not found for update",
                id);

            return false;
        }

        device.UpdateDetails(
            request.Name ?? device.Name,
            request.Brand ?? device.Brand,
            request.State ?? device.State);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Device {DeviceId} updated",
            id);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var device = await _dbContext.Devices
            .FirstOrDefaultAsync(d => d.Id == id && d.DeletedAt == null);

        if (device is null)
        {
             _logger.LogWarning(
                "Device {DeviceId} was not found for deletion",
                id);

            return false;
        }

        device.Delete();

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Device {DeviceId} soft deleted",
            id);
            
        return true;
    }
    
}