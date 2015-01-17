namespace Specification.Halp
{
    using System;
    using Pomodoro;

    internal class ControlledTimeMaster : TimeMaster
    {
        private Action _callback;

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
    }
}