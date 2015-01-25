namespace ConsoleApp
{
    using System.Linq;
    using Notifications;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;

    class Program
    {
        static void Main(string[] args)
        {
            var configFactory = new ConfigFactory();
            var config = configFactory.GetConfig(new []{"1", "1", "1", "4"});
            if (args.Count() == 4)
            {
                config = configFactory.GetConfig(args);
            }

            var timeMaster = new SystemTimeMaster();
            var timer = new PomodoroTimer(timeMaster, config);

            AddNotifications(timer);

            var consoleUi = new ConsoleUi(new CleverFactory());

            var ui = new Ui(timer, config);
            consoleUi.Subscribe(ui, Ui.StartCommand);
            consoleUi.Subscribe(ui, Ui.NextCommand);
            consoleUi.Subscribe(ui, Ui.InterruptCommand);
            consoleUi.Subscribe(ui, Ui.RestartCommand);

            string input = string.Empty;
            while (input != "/quit")
            {
                input = System.Console.ReadLine();
                consoleUi.UserInput(input);
            }
        }

        private static void AddNotifications(PomodoroTimer timer)
        {
            var trayNotification = new TrayBubble();
            timer.IntervalFinished += trayNotification.IntervalFinished;
            timer.Tick += trayNotification.OnTick;
            timer.IntervalInterrupted += trayNotification.IntervalInterrupted;
        }
    }
}
