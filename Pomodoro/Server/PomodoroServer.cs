namespace Pomodoro.Server
{
    using System.Reactive.Subjects;
    using Timer;
    using WampSharp.V2.Realm;

    public class PomodoroServer
    {
        public const string StartSubject = "pomodoro.start";
        public const string EndSubject = "pomodoro.end";
        public const string InterruptSubject = "pomodoro.interrupt";
        public const string TickSubject = "pomodoro.tick";

        private readonly PomodoroNotifier _pomodoroNotifier;

        private ISubject<IntervalStartedEventArgs> _startSubject;
        private ISubject<IntervalFinishedEventArgs> _endSubject;
        private ISubject<IntervalInterruptedEventArgs> _interruptSubject;
        private ISubject<TimeRemainingEventArgs> _tickSubject;

        public PomodoroServer(IWampHostedRealm realm, PomodoroNotifier pomodoroNotifier)
        {
            SetupSubjects(realm);

            _pomodoroNotifier = pomodoroNotifier;
        }

        private void SetupSubjects(IWampHostedRealm realm)
        {
            _startSubject = realm.Services.GetSubject<IntervalStartedEventArgs>(StartSubject);
            _endSubject = realm.Services.GetSubject<IntervalFinishedEventArgs>(EndSubject);
            _interruptSubject = realm.Services.GetSubject<IntervalInterruptedEventArgs>(InterruptSubject);
            _tickSubject = realm.Services.GetSubject<TimeRemainingEventArgs>(TickSubject);
        }

        private void Started(object sender, IntervalStartedEventArgs e)
        {
            _startSubject.OnNext(e);
        }

        private void Finished(object sender, IntervalFinishedEventArgs e)
        {
            _endSubject.OnNext(e);
        }

        private void Interrupted(object sender, IntervalInterruptedEventArgs e)
        {
            _interruptSubject.OnNext(e);
        }

        private void Tick(object sender, TimeRemainingEventArgs e)
        {
            _tickSubject.OnNext(e);
        }

        public void Stop()
        {
            _pomodoroNotifier.IntervalStarted -= Started;
            _pomodoroNotifier.IntervalFinished -= Finished;
            _pomodoroNotifier.IntervalInterrupted -= Interrupted;
            _pomodoroNotifier.Tick -= Tick;
        }

        public void Start()
        {
            _pomodoroNotifier.IntervalStarted += Started;
            _pomodoroNotifier.IntervalFinished += Finished;
            _pomodoroNotifier.IntervalInterrupted += Interrupted;
            _pomodoroNotifier.Tick += Tick;
        }
    }
}