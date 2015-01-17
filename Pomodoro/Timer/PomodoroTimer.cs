namespace Pomodoro.Timer
{
    using System;

    public class PomodoroTimer
    {
        private readonly TimeMaster _timeMaster;
        private PomodoroConfig _pomodoroConfig;

        private IntervalType _currentInterval;
        private bool _currentIntervalFinished;
        private int _productiveIntervalsSinceLastLongBreak;

        public PomodoroTimer(TimeMaster timeMaster)
        {
            _timeMaster = timeMaster;
            _currentInterval = IntervalType.None;
        }

        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;

        public void Start(PomodoroConfig config)
        {
            if (_currentInterval != IntervalType.None)
            {
                throw new CannotStartPomodoroMultipleTimesException();
            }

            _pomodoroConfig = config;
            
            StartInterval(IntervalType.Productive);
        }

        private void OnIntervalEnd()
        {
            if (_currentInterval == IntervalType.Productive)
            {
                _productiveIntervalsSinceLastLongBreak++;
            }
            else if (_currentInterval == IntervalType.LongBreak)
            {
                _productiveIntervalsSinceLastLongBreak = 0;
            }

            _currentIntervalFinished = true;

            if (IntervalFinished != null)
            {
                IntervalFinished(this, new IntervalFinishedEventArgs{ Type = _currentInterval });
            }
        }

        public void StartNext()
        {
            if (!_currentIntervalFinished && _currentInterval != IntervalType.None)
            {
                return;
            }

            if (ShouldStartShortBreak())
            {
                StartInterval(IntervalType.ShortBreak);
            }
            else if (ShouldStartLongBreak())
            {
                StartInterval(IntervalType.LongBreak);
            }
            else if (ShouldStartProductiveInterval())
            {
                StartInterval(IntervalType.Productive);
            }
        }

        private bool ShouldStartProductiveInterval()
        {
            return _currentInterval != IntervalType.Productive;
        }

        private bool ShouldStartLongBreak()
        {
            return _currentInterval == IntervalType.Productive &&
                   _productiveIntervalsSinceLastLongBreak == _pomodoroConfig.LongBreakAfter;
        }

        private bool ShouldStartShortBreak()
        {
            return _currentInterval == IntervalType.Productive &&
                   _productiveIntervalsSinceLastLongBreak < _pomodoroConfig.LongBreakAfter;
        }

        private void StartInterval(IntervalType type)
        {
            _currentIntervalFinished = false;
            _currentInterval = type;
            switch (type)
            {
                case IntervalType.Productive:
                    _timeMaster.Pass(_pomodoroConfig.Productivity, OnIntervalEnd);
                    break;
                case IntervalType.ShortBreak:
                    _timeMaster.Pass(_pomodoroConfig.ShortBreak, OnIntervalEnd);
                    break;
                case IntervalType.LongBreak:
                    _timeMaster.Pass(_pomodoroConfig.LongBreak, OnIntervalEnd);
                    break;
            }
        }
    }
}