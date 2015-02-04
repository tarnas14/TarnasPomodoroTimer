namespace Pomodoro.Wamp.Server
{
    using System.Collections.Generic;
    using Timer;

    public class InMemoryPomodoroStore : PomodoroStore
    {
        private readonly IList<PomodoroTimer> _pomodoros;
        private readonly TimeMasterFactory _timeMasterFactory;

        public InMemoryPomodoroStore() : this(new SystemTimeMasterFactory())
        {
            
        }

        public InMemoryPomodoroStore(TimeMasterFactory timeMasterFactory)
        {
            _timeMasterFactory = timeMasterFactory;
            _pomodoros = new List<PomodoroTimer>();
        }

        public PomodoroIdentifier SetupNewPomodoro(PomodoroConfig config)
        {
            var newTimer = new PomodoroTimer(_timeMasterFactory.GetTimeMaster(), config);
            _pomodoros.Add(newTimer);

            newTimer.Id = new PomodoroIdentifier(_pomodoros.Count);

            return newTimer.Id;
        }

        private PomodoroTimer this[PomodoroIdentifier pomodoroId]
        {
            get { return _pomodoros[pomodoroId.Id - 1]; }
        }

        public void SubscribeToPomodoro(PomodoroIdentifier pomodoroId, PomodoroSubscriber subscriber)
        {
            subscriber.Subscribe(this[pomodoroId]);
        }

        public void StartNext(PomodoroIdentifier identifier)
        {
            this[identifier].StartNext();
        }

        public void Interrupt(PomodoroIdentifier identifier)
        {
            this[identifier].Interrupt();
        }

        public void Restart(PomodoroIdentifier identifier)
        {
            this[identifier].Restart();
        }
    }
}