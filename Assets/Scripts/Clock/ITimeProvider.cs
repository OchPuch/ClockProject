using System;

namespace Clock
{
    public interface ITimeProvider
    {
        public event Action<DateTime> TimeUpdated;
        public DateTime GetTime();
    }
}