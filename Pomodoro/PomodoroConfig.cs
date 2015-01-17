namespace Pomodoro
{
    using System;

    public class PomodoroConfig
    {
        public PomodoroConfig()
        {
            LongBreakAfter = 4;
        }

        public TimeSpan Productivity { get; set; }
        public TimeSpan ShortBreak { get; set; }
        public TimeSpan LongBreak { get; set; }
        public int LongBreakAfter { get; set; }
    }
}