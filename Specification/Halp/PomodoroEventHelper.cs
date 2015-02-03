namespace Specification.Halp
{
    using System.Collections.Generic;
    using Pomodoro;
    using Pomodoro.Timer;

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

        public void SubscribeToPomodoro(PomodoroTimer pomodoro)
        {
            pomodoro.IntervalStarted += StartOfInterval;
            pomodoro.IntervalInterrupted += OnInterruptedInterval;
            pomodoro.IntervalFinished += EndOfInterval;
            pomodoro.Tick += OnTick;
        }

        public void EndOfInterval(object sender, IntervalFinishedEventArgs e)
        {
            FinishedIntervals.Add(e);
        }

        public void StartOfInterval(object sender, IntervalStartedEventArgs e)
        {
            StartedIntervals.Add(e);
        }

        public void OnInterruptedInterval(object sender, IntervalInterruptedEventArgs e)
        {
            InterruptedIntervals.Add(e);
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            Ticks.Add(e);
        }
    }
}