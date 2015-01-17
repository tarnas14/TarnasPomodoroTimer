namespace Pomodoro
{
    using System;

    public class PomodoroTimer
    {
        private readonly TimeMaster _timeMaster;
        private PomodoroConfig _pomodoroConfig;

        private IntervalType _currentInterval;
        private bool _currentIntervalFinished;

        public PomodoroTimer(TimeMaster timeMaster)
        {
            _timeMaster = timeMaster;
            _currentInterval = IntervalType.None;
        }

        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;

        public void Start(PomodoroConfig config)
        {
            _pomodoroConfig = config;
            _currentInterval = IntervalType.Productive;
            _currentIntervalFinished = false;
            _timeMaster.Pass(_pomodoroConfig.Productivity, OnIntervalEnd);
        }

        private void OnIntervalEnd()
        {
            _currentIntervalFinished = true;
            if (IntervalFinished != null)
            {
                IntervalFinished(this, new IntervalFinishedEventArgs{ Type = _currentInterval });
            }
        }

        public void StartNext()
        {
            if (_currentIntervalFinished && _currentInterval == IntervalType.Productive)
            {
                _currentInterval = IntervalType.ShortBreak;
                _currentIntervalFinished = false;
                _timeMaster.Pass(_pomodoroConfig.ShortBreak, OnIntervalEnd);
            }
        }
    }
}