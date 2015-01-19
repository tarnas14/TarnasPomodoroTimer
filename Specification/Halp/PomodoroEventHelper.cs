namespace Specification.Halp
{
    using System.Collections.Generic;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class PomodoroEventHelper
    {
        public PomodoroEventHelper()
        {
            FinishedIntervals = new List<IntervalType>();
        }

        public IList<IntervalType> FinishedIntervals { get; set; }

        public void EndOfInterval(object sender, IntervalFinishedEventArgs e)
        {
            FinishedIntervals.Add(e.Type);
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            TicksCount++;
        }

        public int TicksCount { get; private set; }
    }
}