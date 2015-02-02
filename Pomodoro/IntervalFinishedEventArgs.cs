namespace Pomodoro
{
    using System;
    using Wamp.Server;

    public class IntervalFinishedEventArgs : EventArgs
    {
        public IntervalType Type { get; set; }
        public IntervalType NextIntervalType { get; set; }
        public PomodoroIdentifier Id { get; set; }
    }
}