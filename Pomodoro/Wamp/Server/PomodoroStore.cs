namespace Pomodoro.Wamp.Server
{
    using Timer;

    public interface PomodoroStore
    {
        PomodoroIdentifier SetupNewPomodoro(PomodoroConfig config);
        PomodoroTimer this[PomodoroIdentifier pomodoroId] { get; }
    }
}