namespace Pomodoro.Timer
{
    public interface PomodoroSubscriber
    {
        void Subscribe(PomodoroNotifier pomodoroNotifier);
    }
}