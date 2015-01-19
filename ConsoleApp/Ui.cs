namespace ConsoleApp
{
    using System;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class Ui
    {
        private readonly PomodoroTimer _timer;
        private readonly PomodoroConfig _config;
        private int _row;

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
            Console.WriteLine("");
            _row = Console.CursorTop;
        }

        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            Console.SetCursorPosition(0, _row);
            Console.Write("\r{0} has ended!          ", e.Type);
            Console.ReadLine();
            _timer.StartNext();
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            Console.SetCursorPosition(0, _row);
            Console.Write("\r{0} remaining: {1}", _timer.CurrentInterval.Type, e.TimeRemaining);
        }
    }
}