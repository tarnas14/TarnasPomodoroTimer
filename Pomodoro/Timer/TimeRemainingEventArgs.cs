namespace Pomodoro.Timer
{
    using System;

    public class TimeRemainingEventArgs : EventArgs
    {
        public TimeSpan TimeRemaining;
        public IntervalType IntervalType;
    }
}