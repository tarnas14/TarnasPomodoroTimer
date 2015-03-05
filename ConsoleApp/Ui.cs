﻿namespace ConsoleApp
{
    using Pomodoro;
    using Pomodoro.Timer;
    using Console = System.Console;

    public class Ui : PomodoroSubscriber
    {
        private readonly int _infoRowIndex;
        private int _savedRow;
        private int _savedColumn;
        private int _remainingErrorTicks = -1;
        private int _productiveFinishedCount;

        public Ui()
        {
            _productiveFinishedCount = 0;
            _infoRowIndex = Console.CursorTop;

            InitDisplayArea();
        }

        private void InitDisplayArea()
        {
            Console.Write("\n\n\n");
            DisplayFinishedProductiveCount();
        }

        private void DisplayFinishedProductiveCount()
        {
            string productiveFinishedText = string.Format("Productive intervals finished: {0}", _productiveFinishedCount);
            AnnounceOnLine(productiveFinishedText, 0);
        }

        private void IntervalInterrupted(object sender, IntervalInterruptedEventArgs e)
        {
            AnnounceOnLine(string.Format("{0} has been interrupted!", e.Type), 1);
        }

        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            if (e.Type == IntervalType.Productive)
            {
                ++_productiveFinishedCount;
                DisplayFinishedProductiveCount();
            }
            AnnounceOnLine(string.Format("{0} has ended!\n", e.Type), 1);
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            AnnounceOnLine(string.Format("{0} remaining: {1}", e.IntervalType, e.TimeRemaining), 1);

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

        private void AnnounceOnLine(string announcement, int lineIndex)
        {
            SaveCursorAndRewindConsoleTo(_infoRowIndex + lineIndex);
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
            AnnounceOnLine(errorMessage, 2);

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

        public void ResetCounter()
        {
            _productiveFinishedCount = 0;
            DisplayFinishedProductiveCount();
        }
    }
}