namespace Pomodoro.Wamp.Server
{
    using System;
    using Timer;

    public class PomodoroService
    {
        private readonly PomodoroStore _pomodoroStore;

        public PomodoroService(PomodoroStore pomodoroStore)
        {
            _pomodoroStore = pomodoroStore;
        }

        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;

        public PomodoroIdentifier SetupNewPomodoro(PomodoroConfig config)
        {
            var newPomodoroId = _pomodoroStore.SetupNewPomodoro(config);

            return newPomodoroId;
        }

        public void StartNext(PomodoroIdentifier identifier)
        {
            _pomodoroStore.StartNext(identifier);
        }

        public void Interrupt(PomodoroIdentifier identifier)
        {
            _pomodoroStore.Interrupt(identifier);
        }

        public void Restart(PomodoroIdentifier identifier)
        {
            _pomodoroStore.Restart(identifier);
        }
    }
}