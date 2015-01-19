namespace ConsoleApp
{
    using System;
    using Pomodoro;

    public class ConfigFactory
    {
        public PomodoroConfig GetConfig(string[] args)
        {
            return new PomodoroConfig
            {
                Productivity = TimeSpan.FromMinutes(Convert.ToInt32(args[0])),
                ShortBreak = TimeSpan.FromMinutes(Convert.ToInt32(args[1])),
                LongBreak = TimeSpan.FromMinutes(Convert.ToInt32(args[2])),
                LongBreakAfter = Convert.ToInt32(args[3])
            };
        }
    }
}