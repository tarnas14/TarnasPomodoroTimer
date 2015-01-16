namespace Pomodoro
{
    using System;

    public class PomodoroTimer
    {
        private readonly PomodoroConfig _pomodoroConfig;
        private readonly TimeMaster _timeMaster;

        public PomodoroTimer(PomodoroConfig pomodoroConfig, TimeMaster timeMaster)
        {
            _pomodoroConfig = pomodoroConfig;
            _timeMaster = timeMaster;

        }

        public void Start()
        {
            _timeMaster.Pass(_pomodoroConfig.Productivity);
            OnProductivityIntervalEnd();
        }

        private void OnProductivityIntervalEnd()
        {
            if (EndOfProductivityInterval != null)
            {
                EndOfProductivityInterval(this, new EventArgs());
            }
        }

        public event EventHandler EndOfProductivityInterval;
    }
}