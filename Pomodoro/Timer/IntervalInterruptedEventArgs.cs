namespace Pomodoro.Timer
{
    using System;
    using Wamp.Server;

    public class IntervalInterruptedEventArgs : EventArgs
    {
        public IntervalType Type { get; set; }
        public PomodoroIdentifier Id { get; set; }
    }
}
