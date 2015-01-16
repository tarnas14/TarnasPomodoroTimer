namespace Specification.Halp
{
    using System;
    using System.Collections.Generic;
    using Pomodoro;

    internal class PomodoroEventHelper
    {
        public PomodoroEventHelper()
        {
            FinishedIntervals = new List<IntervalType>();
        }

        public void EndOfProductivityInterval(object sender, EventArgs e)
        {
            FinishedIntervals.Add(IntervalType.Productive);
        }

        public IList<IntervalType> FinishedIntervals { get; set; }
    }
}