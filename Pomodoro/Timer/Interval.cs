namespace Pomodoro.Timer
{
    using System;

    internal class Interval
    {
        public Interval(IntervalType intervalType, TimeSpan timeSpan)
        {
            Type = intervalType;
            TimeSpan = timeSpan;
        }

        public TimeSpan TimeSpan { get; private set; }
        public IntervalType Type { get; private set; }

        public void Passed()
        {
            
        }
    }
}