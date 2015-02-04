namespace Specification.Halp
{
    using System.Collections.Generic;
    using Pomodoro;
    using Pomodoro.Timer;
    using Pomodoro.Wamp.Server;

    internal class PomodoroEventHelper : PomodoroSubscriber
    {
        public IList<TimeRemainingEventArgs> Ticks { get; private set; }
        public IList<IntervalFinishedEventArgs> FinishedIntervals { get; private set; }
        public IList<IntervalStartedEventArgs> StartedIntervals { get; private set; }
        public IList<IntervalInterruptedEventArgs> InterruptedIntervals { get; private set; }

        public PomodoroEventHelper()
        {
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
    }
}