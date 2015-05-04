# TarnasPomodoroTimer

##How to use:

Run this from command line (all intervals in minutes)
if you don't provide any arguments it defaults to:

-productive interval: 25min
-short break: 5min
-long break: 20min
-long break after *4* productive intervals

```
ConsoleApp.exe [<productive interval> <short break> <long break> <long break after X productive intervals>]
```

type /help for instructions as follows:


```
Available commands:

/help    - no comments...

/next    - starts next interval in the configuration

/stahp   - stops current interval

/restart - restarts last interval (if stopped) or current (if in progress)

/reset   - resets the pomodoro to the first interval and resets finished intervals counter
```