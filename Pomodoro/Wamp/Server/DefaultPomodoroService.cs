namespace Pomodoro.Wamp.Server
{
    using System;
    using Timer;

    public class DefaultPomodoroService : PomodoroSubscriber, PomodoroNotifier, PomodoroService
    {
        private readonly PomodoroStore _pomodoroStore;

        public DefaultPomodoroService() : this(new InMemoryPomodoroStore())
        {
            
        }

        public DefaultPomodoroService(PomodoroStore pomodoroStore)
        {
            _pomodoroStore = pomodoroStore;
        }

        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;
        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        public event EventHandler<TimeRemainingEventArgs> Tick;

        public PomodoroIdentifier SetupNewPomodoro(PomodoroConfig config)
        {
            var newPomodoroId = _pomodoroStore.SetupNewPomodoro(config);

            _pomodoroStore.SubscribeToPomodoro(newPomodoroId, this);

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

        private void StartOfInterval(object sender, IntervalStartedEventArgs e)
        {
            if (IntervalStarted != null)
            {
                IntervalStarted(sender, e);
            }
        }

        private void OnInterruptedInterval(object sender, IntervalInterruptedEventArgs e)
        {
            if (IntervalInterrupted != null)
            {
                IntervalInterrupted(sender, e);
            }
        }

        public void Subscribe(PomodoroNotifier pomodoroNotifier)
        {
            pomodoroNotifier.IntervalStarted += StartOfInterval;
            pomodoroNotifier.IntervalInterrupted += OnInterruptedInterval;
        }
    }
}