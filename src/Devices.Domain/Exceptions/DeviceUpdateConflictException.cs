namespace Devices.Domain.Exceptions;

public class DeviceUpdateConflictException : Exception
{
    public DeviceUpdateConflictException(string message)
        : base(message)
    {
    }
}