namespace Pomodoro
{
    using System;
    using System.Timers;
    using Timer;

    public class SystemTimeMaster : TimeMaster
    {
        private readonly System.Timers.Timer _secondTimer;
        private TimeSpan _timeLeft;
        private System.Timers.Timer _currentTimer;
        private Action _callback;

        public SystemTimeMaster()
        {
            _secondTimer = new System.Timers.Timer(1000) {AutoReset = true};
            _secondTimer.Elapsed += OnSecondElapsed;
        }

        private void OnSecondElapsed(object sender, ElapsedEventArgs e)
        {
            if (_timeLeft == TimeSpan.Zero)
            {
                return;
            }

            _timeLeft -= TimeSpan.FromSeconds(1);

            if (Tick != null)
            {
                Tick(this, new TimeRemainingEventArgs
                {
                    TimeRemaining = _timeLeft
                });
            }
        }

        public void Pass(TimeSpan timeInterval, Action callback)
        {
            _timeLeft = timeInterval;
            _currentTimer = new System.Timers.Timer(timeInterval.TotalMilliseconds)
            {
                AutoReset = false
            };
            _currentTimer.Elapsed += OnIntervalTimerElapsed;
            _callback = callback;

            _secondTimer.Start();
            _currentTimer.Start();
        }

        private void OnIntervalTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _secondTimer.Stop();
            _callback.Invoke();
        }

        public event EventHandler<TimeRemainingEventArgs> Tick;
        public void Stop()
        {
            _secondTimer.Stop();
            _currentTimer.Stop();
        }
    }
}