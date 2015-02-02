namespace Pomodoro.Timer
{
    using System;
    using Wamp.Server;

    public class IntervalStartedEventArgs : EventArgs
    {
        public IntervalType Type { get; set; }
        public TimeSpan Duration { get; set; }
        public PomodoroIdentifier Id { get; set; }
    }
}