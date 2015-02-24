namespace ConsoleApp
{
    using System.Linq;
    using Notifications;
    using Pomodoro;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;

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

            var consoleUi = SetupUserInteraction(config, timer);

            new InputLoop(consoleUi).Loop();

            Cleanup();
        }

        private static ConsoleUi SetupUserInteraction(PomodoroConfig config, PomodoroTimer timer)
        {
            var ui = new Ui(config);
            ui.Subscribe(timer);
            var controller = new PomodoroController(timer, ui);

            var consoleUi = new ConsoleUi();
            consoleUi.Subscribe(controller, PomodoroController.NextCommand);
            consoleUi.Subscribe(controller, PomodoroController.InterruptCommand);
            consoleUi.Subscribe(controller, PomodoroController.RestartCommand);

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
