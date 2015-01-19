namespace ConsoleApp
{
    using System;
    using System.Linq;
    using Pomodoro.Timer;

    class Program
    {
        static void Main(string[] args)
        {
            var configFactory = new ConfigFactory();
            var config = configFactory.GetConfig(new []{"25", "5", "15", "4"});
            if (args.Count() == 4)
            {
                config = configFactory.GetConfig(args);
            }

            var timeMaster = new SystemTimeMaster();
            var timer = new PomodoroTimer(timeMaster, config);

            new Ui(timer, config).Start();

            while (Console.ReadLine() != "quit"){}
        }
    }
}
