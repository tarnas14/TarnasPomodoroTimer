namespace ConsoleApp.Notifications
{
    using System.Media;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class SoundNotifications : PomodoroSubscriber
    {
        private void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            var soundStream = AppResources.BreakFinishedSound;

            if (e.Type == IntervalType.Productive)
            {
                soundStream = AppResources.ProductiveFinishedSound;
            }

            var soundPlayer = new SoundPlayer(soundStream);
            soundPlayer.Play();
        }

        public void Subscribe(PomodoroNotifier pomodoroNotifier)
        {
            pomodoroNotifier.IntervalFinished += IntervalFinished;
        }
    }
}