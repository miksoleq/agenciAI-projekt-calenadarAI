using CalendarAI.Core.Abstractions;

namespace CalendarAI.Core.Services;

public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}