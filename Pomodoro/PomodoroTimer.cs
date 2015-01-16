namespace Pomodoro
{
    using System.Collections.Generic;

    public class PomodoroTimer
    {
        private readonly IList<ITimeInterval> _intervals;
        private bool _intervalInProgress;
        private int _currentIntervalId;

        public PomodoroTimer(List<ITimeInterval> intervals)
        {
            _intervals = intervals;
            _intervalInProgress = false;
            _currentIntervalId = -1;
        }

        public void StartNext()
        {
            if (_intervalInProgress)
            {
                throw new IntervalInProgressException();
            }

            if (_currentIntervalId >= 0)
            {
                _intervals[_currentIntervalId].Finished -= IntervalFinished;
            }

            var nextInterval = _intervals[++_currentIntervalId];
            nextInterval.Finished += IntervalFinished;
            nextInterval.Start();
            _intervalInProgress = true;
        }

        private void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            _intervalInProgress = false;
        }
    }
}