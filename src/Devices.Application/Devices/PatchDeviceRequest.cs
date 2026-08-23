using Devices.Domain;

namespace Devices.Application.Devices;

public record PatchDeviceRequest(
    string? Name,
    string? Brand,
    DeviceState? State);