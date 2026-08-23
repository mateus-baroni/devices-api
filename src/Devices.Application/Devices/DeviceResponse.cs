using Devices.Domain;

namespace Devices.Application.Devices;

public sealed record DeviceResponse(
    Guid Id,
    string Name,
    string Brand,
    DeviceState State,
    DateTime CreatedAt,
    DateTime UpdatedAt);