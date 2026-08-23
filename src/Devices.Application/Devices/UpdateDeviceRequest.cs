using Devices.Domain;

namespace Devices.Application.Devices;

public record UpdateDeviceRequest(
    string Name,
    string Brand,
    DeviceState State);