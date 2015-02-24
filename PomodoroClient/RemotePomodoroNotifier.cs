namespace PomodoroClient
{
    using System;
    using Pomodoro;
    using Pomodoro.Timer;
    using Pomodoro.Wamp.Server;
    using WampSharp.V2.Client;

    public class RemotePomodoroNotifier : PomodoroNotifier
    {
        private readonly TimeMaster _timeMaster;
        private Interval _currentIterval;

        public RemotePomodoroNotifier(PomodoroIdentifier pomodoroIdentifier, IWampRealmProxy realmProxy, TimeMaster timeMaster)
        {
            _timeMaster = timeMaster;
            SubscribeToTopics(pomodoroIdentifier, realmProxy);
        }

        private void SubscribeToTopics(PomodoroIdentifier pomodoroIdentifier, IWampRealmProxy realmProxy)
        {
            realmProxy.Services.GetSubject<IntervalInterruptedEventArgs>(pomodoroIdentifier.GetTopic(TopicType.interrupted)).Subscribe(Interrupted);

            realmProxy.Services.GetSubject<IntervalStartedEventArgs>(pomodoroIdentifier.GetTopic(TopicType.started)).Subscribe(Started);
        }

        private void Interrupted(IntervalInterruptedEventArgs interrupted)
        {
            if (IntervalInterrupted != null)
            {
                IntervalInterrupted(this, interrupted);
            }
        }

        private void Started(IntervalStartedEventArgs started)
        {
            _currentIterval = new Interval(started.Type, started.Duration);
            _timeMaster.Pass(_currentIterval.TimeSpan, OnIntervalEnd);

            if (IntervalStarted != null)
            {
                IntervalStarted(this, started);
            }
        }

        private void OnIntervalEnd()
        {
            if (IntervalFinished != null)
            {
                IntervalFinished(this, new IntervalFinishedEventArgs
                {
                    Type = _currentIterval.Type
                });
            }
        }

        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;
        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        public event EventHandler<TimeRemainingEventArgs> Tick;
    }
}