namespace Pomodoro.Client
{
    using System;
    using Server;
    using Timer;
    using WampSharp.V2.Client;

    public class RemotePomodoroClient : PomodoroNotifier, IDisposable
    {
        private IDisposable _startSubscription;
        private IDisposable _finishSubscription;
        private IDisposable _interruptSubscription;
        private IDisposable _tickSubscription;

        public RemotePomodoroClient(IWampRealmProxy clientProxy)
        {
            SubscribeToSubjects(clientProxy);
        }

        private void SubscribeToSubjects(IWampRealmProxy clientProxy)
        {
            _startSubscription = clientProxy.Services.GetSubject<IntervalStartedEventArgs>(PomodoroServer.StartSubject)
                .Subscribe(OnStartedInterval);

            _finishSubscription = clientProxy.Services.GetSubject<IntervalFinishedEventArgs>(PomodoroServer.EndSubject).Subscribe(OnFinishedInterval);

            _interruptSubscription = clientProxy.Services.GetSubject<IntervalInterruptedEventArgs>(PomodoroServer.InterruptSubject)
                .Subscribe(OnInterruptedInterval);

            _tickSubscription = clientProxy.Services.GetSubject<TimeRemainingEventArgs>(PomodoroServer.TickSubject).Subscribe(OnTick);
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

        private void OnTick(TimeRemainingEventArgs timeRemaining)
        {
            if (Tick != null)
            {
                Tick(this, timeRemaining);
            }
        }

        public event EventHandler<IntervalStartedEventArgs> IntervalStarted;
        public event EventHandler<IntervalInterruptedEventArgs> IntervalInterrupted;
        public event EventHandler<IntervalFinishedEventArgs> IntervalFinished;
        public event EventHandler<TimeRemainingEventArgs> Tick;

        public void Dispose()
        {
            _startSubscription.Dispose();
            _finishSubscription.Dispose();
            _interruptSubscription.Dispose();
            _tickSubscription.Dispose();
        }
    }
}