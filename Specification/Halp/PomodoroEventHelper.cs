namespace Specification.Halp
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class PomodoroEventHelper : PomodoroSubscriber
    {
        public IList<TimeRemainingEventArgs> Ticks { get; private set; }
        public IList<IntervalFinishedEventArgs> FinishedIntervals { get; private set; }
        public IList<IntervalStartedEventArgs> StartedIntervals { get; private set; }
        public IList<IntervalInterruptedEventArgs> InterruptedIntervals { get; private set; }

        public int StartedIntervalsCounter;

        public PomodoroEventHelper()
        {
            StartedIntervalsCounter = 0;
            FinishedIntervals = new List<IntervalFinishedEventArgs>();
            Ticks = new List<TimeRemainingEventArgs>();
            StartedIntervals = new List<IntervalStartedEventArgs>();
            InterruptedIntervals = new List<IntervalInterruptedEventArgs>();
        }

        private void EndOfInterval(object sender, IntervalFinishedEventArgs e)
        {
            FinishedIntervals.Add(e);
        }

        private void StartOfInterval(object sender, IntervalStartedEventArgs e)
        {
            StartedIntervalsCounter++;
            StartedIntervals.Add(e);
        }

        private void OnInterruptedInterval(object sender, IntervalInterruptedEventArgs e)
        {
            InterruptedIntervals.Add(e);
        }

        private void OnTick(object sender, TimeRemainingEventArgs e)
        {
            Ticks.Add(e);
        }

        public void Subscribe(PomodoroNotifier pomodoroNotifier)
        {
            pomodoroNotifier.IntervalFinished += EndOfInterval;
            pomodoroNotifier.IntervalStarted += StartOfInterval;
            pomodoroNotifier.IntervalInterrupted += OnInterruptedInterval;
            pomodoroNotifier.Tick += OnTick;
        }

        public IList<IntervalType> TypesOfFinishedIntervals
        {
            get { return FinishedIntervals.Select(interval => interval.Type).ToList(); }
        }
    }
}