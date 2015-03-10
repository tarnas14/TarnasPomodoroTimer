namespace ConsoleApp
{
    using System.Linq;
    using Notifications;
    using Pomodoro;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;
    using Console = System.Console;

    class Program
    {
        private static TrayBubble _trayNotification;

        static void Main(string[] args)
        {
            var configFactory = new ConfigFactory();
            var config = configFactory.GetConfig(new []{"25", "5", "20", "4"});
            if (args.Count() == 4)
            {
                config = configFactory.GetConfig(args);
            }

            var timeMaster = new SystemTimeMaster();
            var timer = new PomodoroTimer(timeMaster, config);

            SubscribeNotifications(timer);

            DisplayConfiguration(config);

            var consoleUi = SetupUserInteraction(timer);

            new InputLoop(consoleUi).Loop();

            Cleanup();
        }

        private static void DisplayConfiguration(PomodoroConfig config)
        {
            Console.WriteLine("Productivity - {0}", config.Productivity);
            Console.WriteLine("ShortBreak - {0}", config.ShortBreak);
            Console.WriteLine("LongBreak - {0}", config.LongBreak);
            Console.WriteLine("Long break after {0} productive intervals", config.LongBreakAfter);
            Console.WriteLine("");
        }

        private static ConsoleUi SetupUserInteraction(PomodoroTimer timer)
        {
            var ui = new Ui();
            ui.Subscribe(timer);

            var controller = new UserInputController(timer, ui);

            var consoleUi = new ConsoleUi();
            consoleUi.Subscribe(controller, UserInputController.NextCommand);
            consoleUi.Subscribe(controller, UserInputController.InterruptCommand);
            consoleUi.Subscribe(controller, UserInputController.RestartCommand);
            consoleUi.Subscribe(controller, UserInputController.ResetCommand);

            return consoleUi;
        }

        private static void SubscribeNotifications(PomodoroNotifier timer)
        {
            _trayNotification = new TrayBubble();
            _trayNotification.Subscribe(timer);

            new SoundNotifications().Subscribe(timer);
        }

        private static void Cleanup()
        {
            _trayNotification.Dispose();
        }
    }
}
