namespace Pomodoro.Wamp.Server
{
    using Timer;

    public interface PomodoroStore
    {
        PomodoroIdentifier SetupNewPomodoro(PomodoroConfig config);
        void SubscribeToPomodoro(PomodoroIdentifier pomodoroId, PomodoroSubscriber pomodroSubscriber);
        void StartNext(PomodoroIdentifier identifier);
        void Interrupt(PomodoroIdentifier identifier);
        void Restart(PomodoroIdentifier identifier);
    }
}