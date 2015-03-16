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

        public static PomodoroConfig Standard
        {
            get
            {
                return new PomodoroConfig
                {
                    Productivity = TimeSpan.FromMinutes(25),
                    ShortBreak = TimeSpan.FromMinutes(5),
                    LongBreak = TimeSpan.FromMinutes(20),
                    LongBreakAfter = 4
                };
            }
        }
    }
}