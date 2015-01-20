namespace ConsoleApp
{
    using global::Ui;
    using Pomodoro;
    using Pomodoro.Timer;
    using Console = System.Console;

    internal class Ui : Subscriber
    {
        public const string StartCommand = "start";
        public const string NextCommand = "next";
        public const string InterruptCommand = "stahp";
        public const string RestartCommand = "restart";

        private readonly PomodoroTimer _timer;
        private readonly PomodoroConfig _config;
        private int _infoRowIndex;
        private int _savedRow;
        private int _savedColumn;

        public Ui(PomodoroTimer timer, PomodoroConfig config)
        {
            _timer = timer;
            _config = config;
            timer.IntervalFinished += IntervalFinished;
            timer.Tick += OnTick;

            Introduction();
        }

        private void Introduction()
        {
            Console.WriteLine("starting pomodoro timer configuration:");
            Console.WriteLine("Productivity - {0}", _config.Productivity);
            Console.WriteLine("ShortBreak - {0}", _config.ShortBreak);
            Console.WriteLine("LongBreak - {0}", _config.LongBreak);
            Console.WriteLine("Long break after {0} productive intervals", _config.LongBreakAfter);
            Console.WriteLine("Type 'quit' to... lol... quit");
            Console.WriteLine("");
            _infoRowIndex = Console.CursorTop;
            Console.WriteLine("");
            Console.WriteLine("");
        }


        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            Announce(string.Format("{0} has ended!", e.Type));
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            Announce(string.Format("{0} remaining: {1}", _timer.CurrentInterval.Type, e.TimeRemaining));
        }

        private void Announce(string announcement)
        {
            SaveCursorAndRewindConsoleTo(_infoRowIndex);
            ClearCurrentLine();
            Console.WriteLine(announcement);
            RevertCursor();
        }

        private static void ClearCurrentLine()
        {
            Console.Write("\r                                                         \r");
        }

        private void SaveCursorAndRewindConsoleTo(int infoRowIndex)
        {
            _savedRow = Console.CursorTop;
            _savedColumn = Console.CursorLeft;
            Console.SetCursorPosition(0, infoRowIndex);
        }

        private void RevertCursor()
        {
            Console.SetCursorPosition(_savedColumn, _savedRow);
        }

        public void Execute(UserCommand userCommand)
        {
            try
            {
                ClearTheLineCommandIsOn();
                switch (userCommand.Name)
                {
                    case StartCommand:
                        StartNext();
                        break;
                    case NextCommand:
                        StartNext();
                        break;
                    case InterruptCommand:
                        Interrupt();
                        break;
                    case RestartCommand:
                        Restart();
                        break;
                }
            }
            catch (PomodoroException exception)
            {
                SaveCursorAndRewindConsoleTo(_infoRowIndex);
                Console.WriteLine("ERROR:");
                Console.WriteLine(exception.Message);
            }
        }

        private void ClearTheLineCommandIsOn()
        {
            Console.SetCursorPosition(0, Console.CursorTop-1);
            ClearCurrentLine();
        }

        private void Restart()
        {
            _timer.Restart();
        }

        private void Interrupt()
        {
            _timer.Interrupt();
        }

        private void StartNext()
        {
            _timer.StartNext();
        }
    }
}