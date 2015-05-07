namespace Pomodoro
{
    using System;
    using System.Diagnostics;
    using System.Timers;
    using Timer;

    public class SystemTimeMaster : TimeMaster
    {
        private readonly System.Timers.Timer _secondTimer;
        private TimeSpan _timeLeft;
        private readonly System.Timers.Timer _currentTimer;
        private Action _callback;
        private Stopwatch _stopwatch;

        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public TimeSpan ElapsedTime
        {
            get { return _stopwatch.Elapsed; }
        }

        public SystemTimeMaster()
        {
            _secondTimer = new System.Timers.Timer(1000) {AutoReset = true};
            _secondTimer.Elapsed += OnSecondElapsed;
            _currentTimer = new System.Timers.Timer
            {
                AutoReset = false
            };
            _currentTimer.Elapsed += OnIntervalTimerElapsed;
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
            _currentTimer.Stop();
            _currentTimer.Interval = timeInterval.TotalMilliseconds;
            _callback = callback;

            Start();
        }

        private void Start()
        {
            _secondTimer.Start();
            _currentTimer.Start();
            _stopwatch = Stopwatch.StartNew();
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
            _stopwatch.Stop();
        }
    }
}