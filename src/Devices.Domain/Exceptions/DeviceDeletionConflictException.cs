namespace Devices.Domain.Exceptions;

public class DeviceDeletionConflictException : Exception
{
    public DeviceDeletionConflictException(string message)
        : base(message)
    {
    }
}