namespace PomodoroServerApp
{
    using System.Linq;
    using ConsoleApp;
    using Pomodoro;
    using Pomodoro.Configuration;
    using Pomodoro.Server;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;
    using WampSharp.V2;

    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            program.Run(args);
        }

        private void Run(string[] args)
        {
            var pomodoro = SetupPomodoro(args);

            var wampHost = SetupServer(pomodoro);

            var ui = new Ui();
            ui.Subscribe(pomodoro);
            var userInputController = new UserInputController(pomodoro, ui);

            var consoleUi = SetupUserInteraction(userInputController);

            while (true)
            {
                try
                {
                    new InputLoop(consoleUi).Loop();
                    break;
                }
                catch (TarnasConsoleUiException ex)
                {
                    if (ex is NobodyListensToThisCommand || ex is NotACommandException)
                    {
                        ui.ClearTheLineCommandIsOn();
                    }
                }
            }

            wampHost.Dispose();
        }

        private static PomodoroTimer SetupPomodoro(string[] args)
        {
            var configFactory = new ConfigFactory();
            PomodoroConfig pomodoroConfig;
            if (args.Count() == 4)
            {
                pomodoroConfig = configFactory.GetConfig(args.Take(4).ToArray());
            }
            else
            {
                pomodoroConfig = configFactory.GetConfig(new[] {"25", "5", "20", "4"});
            }
            var pomodoro = new PomodoroTimer(new SystemTimeMaster(), pomodoroConfig);
            return pomodoro;
        }

        private static DefaultWampHost SetupServer(PomodoroTimer pomodoro)
        {
            var serverConfig = ConfigurationFactory.FromFile<PomodoroServerConfig>("serverConfig.json");

            var wampHost = new DefaultWampHost(serverConfig.Server);
            wampHost.Open();

            new PomodoroServer(wampHost.RealmContainer.GetRealmByName(serverConfig.RealmName), pomodoro);
            return wampHost;
        }

        private static ConsoleUi SetupUserInteraction(UserInputController controller)
        {
            var consoleUi = new ConsoleUi();
            consoleUi.Subscribe(controller, UserInputController.NextCommand);
            consoleUi.Subscribe(controller, UserInputController.InterruptCommand);
            consoleUi.Subscribe(controller, UserInputController.RestartCommand);
            consoleUi.Subscribe(controller, UserInputController.ResetCommand);
            consoleUi.Subscribe(controller, UserInputController.HelpCommand);

            return consoleUi;
        }
    }
}
