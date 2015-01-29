namespace ConsoleApp.Notifications
{
    using System.Media;
    using Pomodoro;

    internal class SoundNotifications
    {
        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            PlayNotification(e.Type);
        }

        private void PlayNotification(IntervalType intervalType)
        {
            var soundStream = AppResources.BreakFinishedSound;

            if (intervalType == IntervalType.Productive)
            {
                soundStream = AppResources.ProductiveFinishedSound;
            }

            var soundPlayer = new SoundPlayer(soundStream);
            soundPlayer.Play();
        }
    }
}