namespace Pomodoro.Timer
{
    public class PreviousIntervalHasNotFinishedException : PomodoroException
    {
        public PreviousIntervalHasNotFinishedException() : base("Previous interval has not yet ended")
        {
        }
    }
}