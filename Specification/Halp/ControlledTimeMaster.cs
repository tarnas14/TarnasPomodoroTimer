namespace Specification.Halp
{
    using System;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class ControlledTimeMaster : TimeMaster
    {
        private Action _callback;
        private DateTime _utcNow;
        public event EventHandler<TimeRemainingEventArgs> Tick;

        public bool Ticking { get; private set; }

        public void Stop()
        {
            Ticking = false;
        }

        public DateTime UtcNow
        {
            get { return _utcNow; }
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

        public void SetTime(DateTime expectedTime)
        {
            _utcNow = expectedTime;
        }
    }
}