namespace Pomodoro.Timer
{
    using System;

    public class TimeRemainingEventArgs : EventArgs
    {
        public TimeRemainingEventArgs(TimeSpan timeRemaining)
        {
            TimeRemaining = timeRemaining;
        }

        public TimeSpan TimeRemaining { get; private set; }
    }
}