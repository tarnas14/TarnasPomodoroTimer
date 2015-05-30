namespace PomodoroServerApp
{
    using ConsoleApp;
    using ConsoleApp.Notifications;
    using Pomodoro;
    using Pomodoro.Server;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;
    using WampSharp.V2;

    class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        private void Run()
        {
            var wampHost = new DefaultWampHost(PomodoroServer.DefaultServer);
            wampHost.Open();

            var configFactory = new ConfigFactory();
            var config = configFactory.GetConfig(new []{"25", "5", "20", "4"});
            var pomodoro = new PomodoroTimer(new SystemTimeMaster(), config);
            new PomodoroServer(wampHost.RealmContainer.GetRealmByName(PomodoroServer.DefaultRealm), pomodoro);

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
