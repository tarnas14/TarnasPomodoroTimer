namespace ConsoleApp
{
    using System;
    using System.Linq;
    using Notifications;
    using Pomodoro;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;
    using Console = System.Console;

    class Program
    {
        private static TrayBubble _trayNotification;
        private const int ValidNumberOfArguments = 4;

        static void Main(string[] args)
        {
            if (InvalidNumberOfArguments(args.Count()))
            {
                Console.WriteLine("You specified {0} arguments, 4 are needed to configure pomodoro.", args.Count());
                Console.WriteLine("Friendly reminder: <productive interval> <short break> <long break> <long break after X intervals>");
                return;
            }

            Console.Clear();

            var configFactory = new ConfigFactory();
            var config = configFactory.GetConfig(new []{"25", "5", "20", "4"});
            if (args.Count() == ValidNumberOfArguments)
            {
                config = configFactory.GetConfig(args);
            }

            DisplayConfiguration(config);

            var timeMaster = new SystemTimeMaster();
            var timer = new PomodoroTimer(timeMaster, config);

            var ui = new Ui();
            ui.Subscribe(timer);

            var controller = new UserInputController(timer, ui);

            _trayNotification = new TrayBubble();
            _trayNotification.Subscribe(timer);

            var soundNotifications = new SoundNotifications();
            soundNotifications.Subscribe(timer);

            var consoleUi = SetupUserInteraction(controller, _trayNotification);

            while (true)
            {
                try
                {
                    new InputLoop(consoleUi).Loop();
                    break;
                }
                catch (NotACommandException)
                {
                    ui.ClearTheLineCommandIsOn();
                }
            }

            Cleanup();
        }

        private static bool InvalidNumberOfArguments(int count)
        {
            return count > 0 && count != ValidNumberOfArguments;
        }

        private static ConsoleUi SetupUserInteraction(UserInputController controller, TrayBubble trayNotification)
        {
            var consoleUi = new ConsoleUi();
            consoleUi.Subscribe(controller, UserInputController.NextCommand);
            consoleUi.Subscribe(controller, UserInputController.InterruptCommand);
            consoleUi.Subscribe(controller, UserInputController.RestartCommand);
            consoleUi.Subscribe(controller, UserInputController.ResetCommand);
            consoleUi.Subscribe(controller, UserInputController.HelpCommand);

            consoleUi.Subscribe(trayNotification, UserInputController.ResetCommand);

            return consoleUi;
        }

        private static void DisplayConfiguration(PomodoroConfig config)
        {
            Console.WriteLine("Productivity - {0}", config.Productivity);
            Console.WriteLine("ShortBreak - {0}", config.ShortBreak);
            Console.WriteLine("LongBreak - {0}", config.LongBreak);
            Console.WriteLine("Long break after {0} productive intervals", config.LongBreakAfter);
            Console.WriteLine("");
        }

        private static void Cleanup()
        {
            _trayNotification.Dispose();
        }
    }
}
