namespace Pomodoro.Client
{
    using System;
    using Server;
    using Timer;
    using WampSharp.V2.Client;

    public class RemotePomodoroClient : PomodoroNotifier
    {
        public RemotePomodoroClient(IWampRealmProxy clientProxy)
        {
            SubscribeToSubjects(clientProxy);
        }

        private void SubscribeToSubjects(IWampRealmProxy clientProxy)
        {
            clientProxy.Services.GetSubject<IntervalStartedEventArgs>(PomodoroServer.StartSubject)
                .Subscribe(OnStartedInterval);

            clientProxy.Services.GetSubject<IntervalFinishedEventArgs>(PomodoroServer.EndSubject).Subscribe(OnFinishedInterval);

            clientProxy.Services.GetSubject<IntervalInterruptedEventArgs>(PomodoroServer.InterruptSubject)
                .Subscribe(OnInterruptedInterval);
        }

        private void OnStartedInterval(IntervalStartedEventArgs intervalStartedEventArgs)
        {
            if (IntervalStarted != null)
            {
                IntervalStarted(this, intervalStartedEventArgs);
            }
        }

        private void OnFinishedInterval(IntervalFinishedEventArgs intervalFinishedEventArgs)
        {
            if (IntervalFinished != null)
            {
                IntervalFinished(this, intervalFinishedEventArgs);
            }
        }

        private void OnInterruptedInterval(IntervalInterruptedEventArgs interruptedEventArgs)
        {
            if (IntervalInterrupted != null)
            {
                IntervalInterrupted(this, interruptedEventArgs);
            }
        }

        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;
        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        public event EventHandler<TimeRemainingEventArgs> Tick;
    }
}