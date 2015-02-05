namespace Pomodoro.Wamp.Server
{
    using WampSharp.V2.Rpc;

    public interface PomodoroService
    {
        [WampProcedure("com.tarnas.pomodoro.setupNew")]
        PomodoroIdentifier SetupNewPomodoro(PomodoroConfig config);

        [WampProcedure("com.tarnas.pomodoro.startNext")]
        void StartNext(PomodoroIdentifier identifier);

        [WampProcedure("com.tarnas.pomodoro.interrupt")]
        void Interrupt(PomodoroIdentifier identifier);

        [WampProcedure("com.tarnas.pomodoro.restart")]
        void Restart(PomodoroIdentifier identifier);
    }
}