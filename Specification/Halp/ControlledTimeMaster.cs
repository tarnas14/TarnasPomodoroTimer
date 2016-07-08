namespace Specification.Halp
{
    using System;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class ControlledTimeMaster : TimeMaster
    {
        private Action _callback;
        private DateTime _utcNow;
        private TimeSpan _elapsedTime;
        public event EventHandler<TimeRemainingEventArgs> Tick;

        public bool Ticking { get; private set; }

        public void Stop()
        {
            Ticking = false;
        }

        public void SetTime(DateTime expectedTime)
        {
            _utcNow = expectedTime;
        }

        public DateTime UtcNow => _utcNow;

        public void SetElapsedTime(TimeSpan elapsed)
        {
            _elapsedTime = elapsed;
        }

        public TimeSpan ElapsedTime => _elapsedTime;

        public void Pass(TimeSpan timeInterval, Action callback)
        {
            _callback = callback;
            Ticking = true;
        }

        public void FinishLatestInterval()
        {
            _callback?.Invoke();

            _callback = null;
        }

        public void DoTick()
        {
            Tick?.Invoke(this, new TimeRemainingEventArgs{});
        }
    }
}