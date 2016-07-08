namespace Pomodoro
{
    using System;

    public class PomodoroConfig
    {
        public PomodoroConfig()
        {
            LongBreakAfter = 4;
        }

        public TimeSpan Productivity;
        public TimeSpan ShortBreak;
        public TimeSpan LongBreak;
        public int LongBreakAfter;

        public static PomodoroConfig Standard => new PomodoroConfig
        {
            Productivity = TimeSpan.FromMinutes(25),
            ShortBreak = TimeSpan.FromMinutes(5),
            LongBreak = TimeSpan.FromMinutes(20),
            LongBreakAfter = 4
        };
    }
}