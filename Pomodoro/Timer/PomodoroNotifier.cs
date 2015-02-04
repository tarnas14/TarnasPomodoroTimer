namespace Pomodoro.Timer
{
    using System;

    public interface PomodoroNotifier
    {
        event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;
        event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        event EventHandler<TimeRemainingEventArgs> Tick;
    }
}