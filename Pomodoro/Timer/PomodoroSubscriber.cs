namespace Pomodoro.Timer
{
    public interface PomodoroSubscriber
    {
        void EndOfInterval(object sender, IntervalFinishedEventArgs e);
        void StartOfInterval(object sender, IntervalStartedEventArgs e);
        void IntervalInterrupted(object sender, IntervalInterruptedEventArgs e);
        void OnTick(object sender, TimeRemainingEventArgs e);
    }
}