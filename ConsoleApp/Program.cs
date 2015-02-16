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
            var config = configFactory.GetConfig(new []{"1", "1", "1", "2"});
            if (args.Count() == 4)
            {
                config = configFactory.GetConfig(args);
            }

            var timeMaster = new SystemTimeMaster();
            var timer = new PomodoroTimer(timeMaster, config);

            _trayNotification = new TrayBubble();
            _trayNotification.Subscribe(timer);

            new SoundNotifications().Subscribe(timer);

            var consoleUi = new ConsoleUi();

            var ui = new Ui(timer, config);
            consoleUi.Subscribe(ui, Ui.NextCommand);
            consoleUi.Subscribe(ui, Ui.InterruptCommand);
            consoleUi.Subscribe(ui, Ui.RestartCommand);

            new InputLoop(consoleUi).Loop();

            Cleanup();
        }

        private static void Cleanup()
        {
            _trayNotification.Dispose();
        }
    }
}
