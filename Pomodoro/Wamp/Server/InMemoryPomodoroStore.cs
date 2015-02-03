namespace Pomodoro.Wamp.Server
{
    using System.Collections.Generic;
    using Timer;

    public class InMemoryPomodoroStore : PomodoroStore
    {
        private readonly IList<PomodoroTimer> _pomodoros;
        private readonly TimeMaster _timeMaster;

        public InMemoryPomodoroStore(TimeMaster timeMaster)
        {
            _timeMaster = timeMaster;
            _pomodoros = new List<PomodoroTimer>();
        }

        public PomodoroIdentifier SetupNewPomodoro(PomodoroConfig config)
        {
            var newTimer = new PomodoroTimer(_timeMaster, config);
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
            this[pomodoroId].IntervalFinished += subscriber.EndOfInterval;
            this[pomodoroId].IntervalStarted += subscriber.StartOfInterval;
            this[pomodoroId].IntervalInterrupted += subscriber.IntervalInterrupted;
            this[pomodoroId].Tick+= subscriber.OnTick;
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