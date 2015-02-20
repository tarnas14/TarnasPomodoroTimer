﻿namespace ConsoleApp
{
    using Pomodoro;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;

    internal class PomodoroController : Subscriber
    {
        private readonly PomodoroTimer _timer;
        private readonly Ui _ui;

        public PomodoroController(PomodoroTimer timer, Ui ui)
        {
            _timer = timer;
            _ui = ui;
        }

        public void Execute(UserCommand userCommand)
        {
            try
            {
                _ui.ClearTheLineCommandIsOn();
                switch (userCommand.Name)
                {
                    case NextCommand:
                        _timer.StartNext();
                        break;
                    case InterruptCommand:
                        _timer.Interrupt();
                        break;
                    case RestartCommand:
                        _timer.Restart();
                        break;
                }
            }
            catch (PomodoroException exception)
            {
                _ui.DisplayErrorForXTicks("ERROR: " + exception.Message, 3);
            }
        }

        public const string NextCommand = "next";
        public const string InterruptCommand = "stahp";
        public const string RestartCommand = "restart";
    }
}