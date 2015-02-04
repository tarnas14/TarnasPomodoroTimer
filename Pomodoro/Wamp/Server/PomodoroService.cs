namespace Pomodoro.Wamp.Server
{
    public interface PomodoroService
    {
        void StartNext(PomodoroIdentifier identifier);
        void Interrupt(PomodoroIdentifier identifier);
        void Restart(PomodoroIdentifier identifier);
    }
}