namespace ConsoleApp.Notifications
{
    using System.Drawing;
    using System.Windows.Forms;
    using Pomodoro;
    using Pomodoro.Timer;

    class TrayBubble
    {
        private readonly NotifyIcon _notifyIcon;

        public TrayBubble()
        {
            _notifyIcon = new NotifyIcon();
            SetupNotificationIcon();
        }

        private void SetupNotificationIcon()
        {
            _notifyIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            _notifyIcon.Text = "Tarnas pomodoro timer";
            _notifyIcon.Visible = true;
        }

        public void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            string bubbleTitle = string.Format("{0} finished!", e.Type);
            string bubbleText = string.Format("next up: {0}", e.NextIntervalType);
            _notifyIcon.ShowBalloonTip(5000, bubbleTitle, bubbleText, ToolTipIcon.Info);
            SetIconText(bubbleTitle);
        }

        private void SetIconText(string text)
        {
            _notifyIcon.Text = text;
        }

        public void OnTick(object sender, TimeRemainingEventArgs e)
        {
            SetIconText(string.Format("{0}; left: {1}", e.IntervalType, e.TimeRemaining));
        }
    }
}
