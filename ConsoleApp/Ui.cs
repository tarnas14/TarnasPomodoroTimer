namespace ConsoleApp
{
    using System;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class Ui
    {
        private readonly PomodoroTimer _timer;
        private readonly PomodoroConfig _config;

        public Ui(PomodoroTimer timer, PomodoroConfig config)
        {
            _timer = timer;
            _config = config;
            timer.IntervalFinished += IntervalFinished;
            timer.Tick += OnTick;
        }

        public void Start()
        {
            Announce();
            _timer.StartNext();
        }

        private void Announce()
        {
            Console.WriteLine("starting pomodoro timer configuration:");
            Console.WriteLine("Productivity - {0}", _config.Productivity);
            Console.WriteLine("ShortBreak - {0}", _config.ShortBreak);
            Console.WriteLine("LongBreak - {0}", _config.LongBreak);
            Console.WriteLine("Long break after {0} productive intervals", _config.LongBreakAfter);
            Console.WriteLine("Type 'quit' to... lol... quit");
        }

        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            Console.WriteLine(e.Type + " ended!");
            Console.ReadLine();
            _timer.StartNext();
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            Console.WriteLine("current interval remaining: {0}", e.TimeRemaining);
        }
    }
}