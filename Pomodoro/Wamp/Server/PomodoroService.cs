namespace Pomodoro.Wamp.Server
{
    public class PomodoroService
    {
        private readonly PomodoroStore _pomodoroStore;

        public PomodoroService(PomodoroStore pomodoroStore)
        {
            _pomodoroStore = pomodoroStore;
        }

        public PomodoroIdentifier SetupNewPomodoro(PomodoroConfig config)
        {
            return _pomodoroStore.SetupNewPomodoro(config);
        }

        public void StartNext(PomodoroIdentifier identifier)
        {
            _pomodoroStore[identifier].StartNext();
        }

        public void Interrupt(PomodoroIdentifier identifier)
        {
            _pomodoroStore[identifier].Interrupt();
        }

        public void Restart(PomodoroIdentifier identifier)
        {
            _pomodoroStore[identifier].Restart();
        }
    }
}