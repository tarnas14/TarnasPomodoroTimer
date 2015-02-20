namespace Pomodoro.Timer
{
    using System;
    using System.Collections.Generic;
    using Wamp.Server;

    public class PomodoroTimer : PomodoroNotifier
    {
        private readonly TimeMaster _timeMaster;
        private int _currentInterval;
        private IList<Interval> _pomodoros;
        private bool _started;

        public PomodoroTimer(TimeMaster timeMaster, PomodoroConfig config)
        {
            _timeMaster = timeMaster;
            _timeMaster.Tick += OnTick;
            PreparePomodoros(config);
        }

        private void OnTick(object sender, TimeRemainingEventArgs e)
        {
            if (Tick != null)
            {
                e.IntervalType = CurrentInterval.Type;
                Tick(this, e);
            }
        }

        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        public event EventHandler<TimeRemainingEventArgs> Tick;
        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;

        private void StartCurrent()
        {
            _timeMaster.Pass(CurrentInterval.TimeSpan, OnIntervalEnd);
            CurrentInterval.Start();

            if (IntervalStarted != null)
            {
                IntervalStarted(this, new IntervalStartedEventArgs
                {
                    Type = CurrentInterval.Type,
                    Duration = CurrentInterval.TimeSpan,
                    Id = Id
                });
            }
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
                IntervalFinished(this, new IntervalFinishedEventArgs
                {
                    Type = CurrentInterval.Type,
                    NextIntervalType = NextQueuedInterval,
                    Id = Id
                });
            }
        }

        public void StartNext()
        {
            if (CurrentInterval.InProgress)
            {
                throw new PreviousIntervalHasNotFinishedException();
            }

            if (_started)
            {
                SetNextInterval();
            }
            else
            {
                _started = true;
            }

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

        private IntervalType NextQueuedInterval
        {
            get
            {
                var nextInterval = _currentInterval + 1;

                if (nextInterval == _pomodoros.Count)
                {
                    nextInterval = 0;
                }

                return _pomodoros[nextInterval].Type;
            }
        }

        private Interval CurrentInterval
        {
            get
            {
                return _pomodoros[_currentInterval];
            }
        }

        public PomodoroIdentifier Id { get; set; }

        public void Interrupt()
        {
            if (!CurrentInterval.InProgress)
            {
                return;
            }

            CurrentInterval.Interrupt();
            _timeMaster.Stop();

            if (IntervalInterrupted != null)
            {
                IntervalInterrupted(this, new IntervalInterruptedEventArgs
                {
                    Type = CurrentInterval.Type,
                    Id = Id
                });
            }
        }

        public void Restart()
        {
            StartCurrent();
        }
    }
}