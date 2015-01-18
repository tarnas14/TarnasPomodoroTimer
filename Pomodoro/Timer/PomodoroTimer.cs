namespace Pomodoro.Timer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PomodoroTimer
    {
        public PomodoroTimer(TimeMaster timeMaster, PomodoroConfig config)
        {
            _currentInterval = -1;
            _timeMaster = timeMaster;
            PreparePomodoros(config);
        }

        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;

        private void StartCurrent()
        {
            _timeMaster.Pass(CurrentInterval.TimeSpan, OnIntervalEnd);
            CurrentInterval.Start();
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
            if (!CurrentInterval.InProgress)
            {
                return;
            }

            CurrentInterval.Finish();

            if (IntervalFinished != null)
            {
                IntervalFinished(this, new IntervalFinishedEventArgs{ Type = CurrentInterval.Type });
            }
        }

        public void StartNext()
        {
            if (CurrentInterval != null && CurrentInterval.InProgress)
            {
                throw new PreviousIntervalHasNotFinishedException();
            }
            
            SetNextInterval();

            StartCurrent();
        }

        private void SetNextInterval()
        {
            _currentInterval++;

            if (_currentInterval == _pomodoros.Count)
            {
                _currentInterval = 0;
            }
        }

        private readonly TimeMaster _timeMaster;
        private int _currentInterval;
        private IList<Interval> _pomodoros;
        private Interval CurrentInterval
        {
            get
            {
                return _currentInterval == -1 ? null : _pomodoros[_currentInterval];
            }
        }

        public void Interrupt()
        {
            CurrentInterval.Interrupt();
        }

        public void Restart()
        {
            StartCurrent();
        }
    }
}