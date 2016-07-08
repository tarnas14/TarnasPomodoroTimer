namespace Pomodoro
{
    using System;

    public class IntervalFinishedEventArgs : EventArgs
    {
        public IntervalType Type;
        public IntervalType NextIntervalType;
        public DateTime UtcFinishedAt;
    }
}