namespace ConsoleApp
{
    using System.Media;
    using Pomodoro;

    internal class SoundNotifications
    {
        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            SystemSounds.Exclamation.Play();
        }
    }
}