namespace ConsoleApp.Notifications
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Pomodoro;
    using Pomodoro.Timer;

    internal class TrayBubble : IDisposable, PomodoroSubscriber
    {
        private readonly NotifyIcon _notifyIcon;

        public TrayBubble()
        {
            _notifyIcon = new NotifyIcon();
            SetupNotificationIcon();
        }

        private void SetupNotificationIcon()
        {
            _notifyIcon.Icon = Icon.FromHandle(AppResources.tomatoIcon.GetHicon());
            _notifyIcon.Text = "Tarnas pomodoro timer";
            _notifyIcon.Visible = true;
        }

        private void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            string bubbleTitle = string.Format("{0} finished!", e.Type);
            string bubbleText = string.Format("next up: {0}", e.NextIntervalType);
            ShowPopup(bubbleTitle, bubbleText);
            SetIconText(bubbleTitle);
        }

        private void ShowPopup(string bubbleTitle, string bubbleText)
        {
            _notifyIcon.ShowBalloonTip(5000, bubbleTitle, bubbleText, ToolTipIcon.Info);
        }

        private void SetIconText(string text)
        {
            _notifyIcon.Text = text;
        }

        private void OnTick(object sender, TimeRemainingEventArgs e)
        {
            SetIconText(string.Format("{0}; left: {1}", e.IntervalType, e.TimeRemaining));
        }

        private void IntervalInterrupted(object sender, IntervalInterruptedEventArgs e)
        {
            string bubbleTitle = string.Format("{0} interrupted!", e.Type);
            const string bubbleText = "retart or go to next";
            ShowPopup(bubbleTitle, bubbleText);
            SetIconText(bubbleTitle + " " + bubbleText);
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }

        public void Subscribe(PomodoroNotifier pomodoroNotifier)
        {
            pomodoroNotifier.IntervalFinished += IntervalFinished;
            pomodoroNotifier.Tick += OnTick;
            pomodoroNotifier.IntervalInterrupted += IntervalInterrupted;
        }
    }
}
