namespace ConsoleApp
{
    using Pomodoro;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;
    using Console = System.Console;

    internal class Ui : PomodoroSubscriber
    {
        private readonly PomodoroConfig _config;
        private int _infoRowIndex;
        private int _savedRow;
        private int _savedColumn;
        private int _remainingErrorTicks = -1;

        public Ui(PomodoroConfig config)
        {
            _config = config;

            Introduction();
        }

        private void IntervalInterrupted(object sender, IntervalInterruptedEventArgs e)
        {
            Announce(string.Format("{0} has been interrupted!", e.Type));
        }

        private void Introduction()
        {
            Console.WriteLine("Productivity - {0}", _config.Productivity);
            Console.WriteLine("ShortBreak - {0}", _config.ShortBreak);
            Console.WriteLine("LongBreak - {0}", _config.LongBreak);
            Console.WriteLine("Long break after {0} productive intervals", _config.LongBreakAfter);
            Console.WriteLine("");
            _infoRowIndex = Console.CursorTop;
            Console.WriteLine("");
            Console.WriteLine("");
        }

        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            Announce(string.Format("{0} has ended!\n", e.Type));
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            Announce(string.Format("{0} remaining: {1}", e.IntervalType, e.TimeRemaining));

            if (_remainingErrorTicks == 0)
            {
                ClearError();
                _remainingErrorTicks = -1;
            }
            else if (_remainingErrorTicks > 0)
            {
                _remainingErrorTicks--;
            }
        }

        private void ClearError()
        {
            SaveCursorAndRewindConsoleTo(_infoRowIndex + 1);
            ClearCurrentLine();
            RevertCursor();
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

        public void DisplayErrorForXTicks(string errorMessage, int tickCount)
        {
            SaveCursorAndRewindConsoleTo(_infoRowIndex);
            Console.WriteLine("\n" + errorMessage);
            RevertCursor();

            _remainingErrorTicks = tickCount;
        }

        public void ClearTheLineCommandIsOn()
        {
            Console.SetCursorPosition(0, Console.CursorTop-1);
            ClearCurrentLine();
        }

        public void Subscribe(PomodoroNotifier pomodoroNotifier)
        {
            pomodoroNotifier.IntervalFinished += IntervalFinished;
            pomodoroNotifier.IntervalInterrupted += IntervalInterrupted;
            pomodoroNotifier.Tick += OnTick;
        }
    }
}