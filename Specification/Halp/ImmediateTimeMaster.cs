namespace Specification.Halp
{
    using System;
    using Pomodoro;

    internal class ImmediateTimeMaster : TimeMaster
    {
        public void Pass(TimeSpan timeInterval, Action callback)
        {
            callback.Invoke();
        }
    }
}