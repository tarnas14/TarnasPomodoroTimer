namespace Pomodoro
{
    using System;

    public class IntervalFinishedEventArgs : EventArgs
    {
        public IntervalType Type { get; set; }
    }
}