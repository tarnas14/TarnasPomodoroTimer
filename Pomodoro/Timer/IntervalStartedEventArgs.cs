namespace Pomodoro.Timer
{
    using System;

    public class IntervalStartedEventArgs : EventArgs
    {
        public IntervalType Type { get; set; }
        public TimeSpan Duration { get; set; }
    }
}