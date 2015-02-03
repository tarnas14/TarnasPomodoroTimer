namespace Pomodoro.Wamp.Server
{
    using System;
    using Timer;

    public class PomodoroService : PomodoroSubscriber
    {
        private readonly PomodoroStore _pomodoroStore;

        public PomodoroService(PomodoroStore pomodoroStore)
        {
            _pomodoroStore = pomodoroStore;
        }

        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;

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

        public void EndOfInterval(object sender, IntervalFinishedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void StartOfInterval(object sender, IntervalStartedEventArgs e)
        {
            if (IntervalStarted != null)
            {
                IntervalStarted(sender, e);
            }
        }

        public void OnInterruptedInterval(object sender, IntervalInterruptedEventArgs e)
        {
            if (IntervalInterrupted != null)
            {
                IntervalInterrupted(sender, e);
            }
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}