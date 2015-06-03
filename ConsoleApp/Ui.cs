namespace ConsoleApp
{
    using Pomodoro;
    using Pomodoro.Timer;
    using Console = System.Console;

    public class Ui : PomodoroSubscriber
    {
        private int _remainingErrorTicks = -1;
        private int _productiveFinishedCount;
        private readonly LineDisplay _statusDisplay;
        private readonly LineDisplay _helpDisplay;

        private const int EventsLine = 1;
        private const int ErrorLine = 2;

        public Ui()
        {
            _productiveFinishedCount = 0;
            _statusDisplay = new LineDisplay(Console.CursorTop, 3);
            _helpDisplay = new LineDisplay(Console.CursorTop + 6, 6);
            SetupHelp();
            DisplayFinishedProductiveCount();
            Console.SetCursorPosition(0, Console.CursorTop + 4);
        }

        private void DisplayFinishedProductiveCount()
        {
            string productiveFinishedText = string.Format("Productive intervals finished: {0}", _productiveFinishedCount);
            AnnounceOnLine(productiveFinishedText, 0);
        }

        private void IntervalInterrupted(object sender, IntervalInterruptedEventArgs e)
        {
            AnnounceOnLine(string.Format("{0} has been interrupted after {1}!", e.Type, e.Elapsed.ToString(@"hh\:mm\:ss")), EventsLine);
        }

        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            if (e.Type == IntervalType.Productive)
            {
                ++_productiveFinishedCount;
                DisplayFinishedProductiveCount();
            }
            AnnounceOnLine(string.Format("{0} has ended at {1}!\n", e.Type, e.UtcFinishedAt.ToLocalTime()), EventsLine);
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            AnnounceOnLine(string.Format("{0} remaining: {1}", e.IntervalType, e.TimeRemaining), EventsLine);

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
            _remainingErrorTicks = 0;
            _statusDisplay.ClearLine(ErrorLine);
            _statusDisplay.Update();
        }

        private void AnnounceOnLine(string announcement, int lineIndex)
        {
            _statusDisplay.UpdateLine(lineIndex, announcement);
        }

        public void DisplayErrorForXTicks(string errorMessage, int tickCount)
        {
            AnnounceOnLine(errorMessage, ErrorLine);

            _remainingErrorTicks = tickCount;
        }

        public void ClearTheLineCommandIsOn()
        {
            Console.SetCursorPosition(0, Console.CursorTop-1);
            ClearCurrentLine();
        }

        private static void ClearCurrentLine()
        {
            Console.Write("\r                                                                                                 \r");
        }

        public void Subscribe(PomodoroNotifier pomodoroNotifier)
        {
            pomodoroNotifier.IntervalFinished += IntervalFinished;
            pomodoroNotifier.IntervalInterrupted += IntervalInterrupted;
            pomodoroNotifier.Tick += OnTick;
        }

        public void Reset()
        {
            ClearEvents();
            ClearError();
            _productiveFinishedCount = 0;
            DisplayFinishedProductiveCount();
        }

        private void ClearEvents()
        {
            _statusDisplay.ClearLine(EventsLine);
            _statusDisplay.Update();
        }

        private void SetupHelp()
        {
            _helpDisplay.SetLine(0, "Available commands:");
            _helpDisplay.SetLine(1, "/help    - no comments...");
            _helpDisplay.SetLine(2, "/next    - starts next interval in the configuration");
            _helpDisplay.SetLine(3, "/stahp   - stops current interval");
            _helpDisplay.SetLine(4, "/restart - restarts last interval (if stopped) or current (if in progress)");
            _helpDisplay.SetLine(5, "/reset   - resets the pomodoro to the first interval and resets finished intervals counter");
        }

        public void DisplayHelp()
        {
            _helpDisplay.Update();
        }

        public void ClearHelp()
        {
            _helpDisplay.Hide();
        }
    }
}