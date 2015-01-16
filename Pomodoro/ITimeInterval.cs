namespace Pomodoro
{
    using System;

    public interface ITimeInterval
    {
        event EventHandler<IntervalFinishedEventArgs> Finished;
        string Name { get; }
        void Start();
    }
}