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

        public DateTime UtcNow
        {
            get { return _utcNow; }
        }

        public void SetElapsedTime(TimeSpan elapsed)
        {
            _elapsedTime = elapsed;
        }

        public TimeSpan ElapsedTime
        {
            get { return _elapsedTime; }
        }

        public void Pass(TimeSpan timeInterval, Action callback)
        {
            _callback = callback;
            Ticking = true;
        }

        public void FinishLatestInterval()
        {
            if (_callback != null)
            {
                _callback.Invoke();
            }

            _callback = null;
        }

        public void DoTick()
        {
            if (Tick != null)
            {
                Tick(this, new TimeRemainingEventArgs{});
            }
        }
    }
}