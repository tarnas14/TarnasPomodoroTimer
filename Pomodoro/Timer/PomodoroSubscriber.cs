namespace Pomodoro.Timer
{
    using Wamp.Server;

    public interface PomodoroSubscriber
    {
        void Subscribe(PomodoroNotifier pomodoroNotifier);
    }
}