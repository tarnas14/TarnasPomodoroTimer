namespace Pomodoro.Timer
{
    using System;

    public class TimeRemainingEventArgs : EventArgs
    {
        public TimeSpan TimeRemaining { get; set; }
        public IntervalType IntervalType { get; set; }
    }
}