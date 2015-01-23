namespace ConsoleApp
{
    using System;
    using System.Timers;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class SystemTimeMaster : TimeMaster
    {
        private readonly Timer _secondTimer;
        private TimeSpan _timeLeft;
        private Timer _currentTimer;
        private Action _callback;

        public SystemTimeMaster()
        {
            _secondTimer = new Timer(1000) {AutoReset = true};
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
            _currentTimer = new Timer(timeInterval.TotalMilliseconds)
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
    }
}