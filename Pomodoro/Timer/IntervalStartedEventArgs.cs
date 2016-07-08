namespace Pomodoro.Timer
{
    using System;

    public class IntervalStartedEventArgs : EventArgs
    {
        public IntervalType Type;
        public TimeSpan Duration;
    }
}