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
        public bool InProgress { get; private set; }

        public void Finish()
        {
            InProgress = false;
        }

        public void Start()
        {
            InProgress = true;
        }

        public void Interrupt()
        {
            InProgress = false;
        }
    }
}