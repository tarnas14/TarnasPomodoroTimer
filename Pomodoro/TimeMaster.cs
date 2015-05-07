namespace Pomodoro
{
    using System;
    using Timer;

    public interface TimeMaster
    {
        void Pass(TimeSpan timeInterval, Action callback);
        event EventHandler<TimeRemainingEventArgs> Tick;
        void Stop();
        DateTime UtcNow { get; }
        TimeSpan ElapsedTime { get; }
    }
}
