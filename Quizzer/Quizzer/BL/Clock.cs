using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Presentation;

namespace BL;

public class Clock
{
    DispatcherTimer? timer;
    QuizWindow quizWindow;
    int elapsedSeconds = 0;
    int allotedSeconds = 0;
    public bool isRunning { get; private set; } = false;

    public Clock(int allotedSec, QuizWindow quizWin) // circular dependency -- to fix
    {
        allotedSeconds = allotedSec;
        quizWindow = quizWin;
    }

    public void Start()
    {
        // if Start() is being called for the first time
        if (timer == null)
        {
            timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(timer_Ticked);
        }
        timer.Start();
        isRunning = true;
    }

    // delegate
    public void Stop()
    {
        timer.Stop();
        timer = null;
        elapsedSeconds = 0; 
        isRunning = false;

    }

    private void timer_Ticked(object? sender, EventArgs e)
    {
        if (elapsedSeconds + 1 > allotedSeconds)
        {
            Stop();
        }

        quizWindow.RemainingSeconds = allotedSeconds - ++elapsedSeconds;

        // Forcing the CommandManager to raise the RequerySuggested event
        CommandManager.InvalidateRequerySuggested();
    }

    public int EllapsedTime() => elapsedSeconds;
}
