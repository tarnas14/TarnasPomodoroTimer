namespace Pomodoro.Timer
{
    using System;

    public class IntervalInterruptedEventArgs : EventArgs
    {
        public IntervalType Type;
        public TimeSpan Elapsed;
    }
}
