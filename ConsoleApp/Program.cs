namespace ConsoleApp
{
    using System.Linq;
    using global::Ui;
    using Pomodoro.Timer;

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

            var consoleUi = new ConsoleUi(new CleverFactory());

            var ui = new Ui(timer, config);
            consoleUi.Subscribe(ui, Ui.StartCommand);
            consoleUi.Subscribe(ui, Ui.NextCommand);
            consoleUi.Subscribe(ui, Ui.InterruptCommand);
            consoleUi.Subscribe(ui, Ui.RestartCommand);

            string input = string.Empty;
            while (input != "/quit")
            {
                input = global::System.Console.ReadLine();
                consoleUi.UserInput(input);
            }
        }
    }
}
