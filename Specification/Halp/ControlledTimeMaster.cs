namespace Specification.Halp
{
    using System;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class ControlledTimeMaster : TimeMaster
    {
        private Action _callback;
        public event EventHandler<TimeRemainingEventArgs> Tick;

        public void Pass(TimeSpan timeInterval, Action callback)
        {
            _callback = callback;
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