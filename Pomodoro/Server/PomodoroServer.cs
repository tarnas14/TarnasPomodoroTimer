namespace Pomodoro.Server
{
    using System.Reactive.Subjects;
    using Timer;
    using WampSharp.V2.Realm;

    public class PomodoroServer
    {
        private ISubject<IntervalStartedEventArgs> _startSubject;
        private ISubject<IntervalFinishedEventArgs> _endSubject;
        private ISubject<IntervalInterruptedEventArgs> _interruptSubject;
        public const string StartSubject = "pomodoro.start";
        public const string EndSubject = "pomodoro.end";
        public const string InterruptSubject = "pomodoro.interrupt";

        public PomodoroServer(IWampHostedRealm realm, PomodoroNotifier pomodoroNotifier)
        {
            SetupSubjects(realm);

            pomodoroNotifier.IntervalStarted += Started;
            pomodoroNotifier.IntervalFinished += Finished;
            pomodoroNotifier.IntervalInterrupted += Interrupted;
        }

        private void SetupSubjects(IWampHostedRealm realm)
        {
            _startSubject = realm.Services.GetSubject<IntervalStartedEventArgs>(StartSubject);
            _endSubject = realm.Services.GetSubject<IntervalFinishedEventArgs>(EndSubject);
            _interruptSubject = realm.Services.GetSubject<IntervalInterruptedEventArgs>(InterruptSubject);
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
    }
}