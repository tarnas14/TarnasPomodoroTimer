namespace ConsoleApp.Notifications
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Pomodoro;
    using Pomodoro.Timer;
    using Tarnas.ConsoleUi;

    internal class TrayBubble : IDisposable, PomodoroSubscriber, Subscriber
    {
        private readonly NotifyIcon _notifyIcon;

        public TrayBubble()
        {
            _notifyIcon = new NotifyIcon();
            SetupNotificationIcon();
        }

        private void SetupNotificationIcon()
        {
            SetVoidIcon();
            _notifyIcon.Text = @"Tarnas pomodoro timer";
            _notifyIcon.Visible = true;
        }

        private void IntervalFinished(object sender, IntervalFinishedEventArgs e)
        {
            SetVoidIcon();
            string bubbleTitle = $"{e.Type} finished!";
            string bubbleText = $"next up: {e.NextIntervalType}";
            ShowPopup(bubbleTitle, bubbleText);
            SetIconText(bubbleTitle);
        }

        private void SetVoidIcon()
        {
            _notifyIcon.Icon = Icon.FromHandle(AppResources.tomatoIcon.GetHicon());
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
            SetIconText($"{e.IntervalType}; left: {e.TimeRemaining}");
        }

        private void IntervalInterrupted(object sender, IntervalInterruptedEventArgs e)
        {
            SetVoidIcon();
            string bubbleTitle = $"{e.Type} interrupted!";
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
            pomodoroNotifier.IntervalStarted += IntervalStarted;
        }

        private void IntervalStarted(object sender, IntervalStartedEventArgs e)
        {
            SetStateIcon(e.Type);
            string bubbleTitle = $"{e.Type} started!";
            string bubbleText = $"duration: {e.Duration}";
            ShowPopup(bubbleTitle, bubbleText);
            SetIconText(bubbleTitle + " " + bubbleText);
        }

        private void SetStateIcon(IntervalType type)
        {
            if (type == IntervalType.Productive)
            {
                _notifyIcon.Icon = Icon.FromHandle(AppResources.pomodoroProductive.GetHicon());
                return;
            }
            
            _notifyIcon.Icon = Icon.FromHandle(AppResources.pomodoroBreak.GetHicon());
        }

        public void Execute(UserCommand userCommand)
        {
            if (userCommand.Name == UserInputController.ResetCommand)
            {
                SetVoidIcon();
            }
        }
    }
}
