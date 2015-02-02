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

            Subscribe(newPomodoroId);

            return newPomodoroId;
        }

        private void Subscribe(PomodoroIdentifier newPomodoroId)
        {
            var pomodoro = _pomodoroStore[newPomodoroId];

            pomodoro.IntervalStarted += OnIntervalStarted;
        }

        private void OnIntervalStarted(object sender, IntervalStartedEventArgs e)
        {
            var pomodoro = sender as PomodoroTimer;
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