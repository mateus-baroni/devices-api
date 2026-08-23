using Devices.Domain;

namespace Devices.Application.Devices;

public sealed record CreateDeviceRequest(
    string Name,
    string Brand,
    DeviceState State);