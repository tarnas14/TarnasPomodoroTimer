namespace Pomodoro.Timer
{
    using System;

    public class IntervalInterruptedEventArgs : EventArgs
    {
        public IntervalType Type { get; set; }
        public TimeSpan Elapsed { get; set; }
    }
}
