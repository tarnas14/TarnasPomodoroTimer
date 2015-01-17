namespace Specification.Halp
{
    using System.Collections.Generic;
    using Pomodoro;

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
    }
}