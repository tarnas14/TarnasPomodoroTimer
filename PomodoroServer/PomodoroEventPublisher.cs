namespace PomodoroServer
{
    using Pomodoro.Timer;
    using Pomodoro.Wamp.Server;
    using WampSharp.V2.Client;


    class PomodoroEventPublisher : PomodoroSubscriber
    {
        private readonly IWampRealmProxy _proxyRealm;

        public PomodoroEventPublisher(IWampRealmProxy proxyRealm)
        {
            _proxyRealm = proxyRealm;
        }

        public void Subscribe(PomodoroNotifier pomodoroNotifier)
        {
            pomodoroNotifier.IntervalInterrupted += OnIntervalInterrupted;
            pomodoroNotifier.IntervalStarted += OnIntervalStarted;
        }

        private void OnIntervalInterrupted(object sender, IntervalInterruptedEventArgs e)
        {
            var subject = _proxyRealm.Services.GetSubject<IntervalInterruptedEventArgs>(e.Id.GetTopic(TopicType.interrupted));

            subject.OnNext(e);
        }

        private void OnIntervalStarted(object sender, IntervalStartedEventArgs e)
        {
            var subject = _proxyRealm.Services.GetSubject<IntervalStartedEventArgs>(e.Id.GetTopic(TopicType.started));

            subject.OnNext(e);
        }
    }
}