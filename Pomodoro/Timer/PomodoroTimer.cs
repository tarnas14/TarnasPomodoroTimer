namespace Pomodoro.Timer
{
    using System;
    using System.Collections.Generic;

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
            if (Tick == null)
            {
                return;
            }

            e.IntervalType = CurrentInterval.Type;
            Tick(this, e);
        }

        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        public event EventHandler<TimeRemainingEventArgs> Tick;
        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;

        private void StartCurrent()
        {
            _timeMaster.Pass(CurrentInterval.TimeSpan, OnIntervalEnd);
            CurrentInterval.Start();

            IntervalStarted?.Invoke(this, new IntervalStartedEventArgs
            {
                Type = CurrentInterval.Type,
                Duration = CurrentInterval.TimeSpan
            });
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

            IntervalFinished?.Invoke(this, new IntervalFinishedEventArgs
            {
                Type = CurrentInterval.Type,
                NextIntervalType = NextQueuedInterval,
                UtcFinishedAt = _timeMaster.UtcNow
            });
        }

        public void StartNext()
        {
            if (CurrentInterval.InProgress)
            {
                StopCurrentInterval();
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

        private Interval CurrentInterval => _pomodoros[_currentInterval];

        public void Interrupt()
        {
            if (!CurrentInterval.InProgress)
            {
                return;
            }

            StopCurrentInterval();

            IntervalInterrupted?.Invoke(this, new IntervalInterruptedEventArgs
            {
                Elapsed = _timeMaster.ElapsedTime,
                Type = CurrentInterval.Type
            });
        }

        private void StopCurrentInterval()
        {
            CurrentInterval.Interrupt();
            _timeMaster.Stop();
        }

        public void RestartInterval()
        {
            StartCurrent();
        }

        public void Reset()
        {
            CurrentInterval.Interrupt();
            _currentInterval = 0;
            _started = false;

            _timeMaster.Stop();
        }
    }
}