namespace Pomodoro.Timer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PomodoroTimer
    {
        private readonly TimeMaster _timeMaster;

        private int _currentInterval;
        private IList<Interval> _pomodoros;

        private Interval CurrentInterval
        {
            get
            {
                return _pomodoros[_currentInterval];
            }
        }

        public PomodoroTimer(TimeMaster timeMaster)
        {
            _timeMaster = timeMaster;
        }

        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;

        public void Start(PomodoroConfig config)
        {
            if (_pomodoros != null)
            {
                throw new CannotStartPomodoroMultipleTimesException();
            }

            PreparePomodoros(config);

            StartCurrent();
        }

        private void StartCurrent()
        {
            _timeMaster.Pass(CurrentInterval.TimeSpan, OnIntervalEnd);
        }

        private void PreparePomodoros(PomodoroConfig config)
        {
            _pomodoros = new List<Interval>{new Interval(IntervalType.Productive, config.Productivity)};

            for (int i = 0; i < config.LongBreakAfter - 1; ++i)
            {
                _pomodoros.Add(new Interval(IntervalType.ShortBreak, config.ShortBreak));
                _pomodoros.Add(new Interval(IntervalType.Productive, config.Productivity));
            }

            _pomodoros.Add(new Interval(IntervalType.LongBreak, config.LongBreak));
        }

        private void OnIntervalEnd()
        {
            CurrentInterval.Passed();

            if (IntervalFinished != null)
            {
                IntervalFinished(this, new IntervalFinishedEventArgs{ Type = CurrentInterval.Type });
            }
        }

        public void StartNext()
        {
            _currentInterval++;

            if (_currentInterval == _pomodoros.Count)
            {
                _currentInterval = 0;
            }

            StartCurrent();
        }
    }
}