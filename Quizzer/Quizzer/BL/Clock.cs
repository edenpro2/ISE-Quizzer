using System;
using System.Windows;
using System.Windows.Threading;

namespace QuizApp.BL
{
    public class Clock : AbstractViewModel
    {
        private DispatcherTimer? _timer;
        private int _elapsedSeconds;
        private int _allottedSeconds;
        private int _remainingSeconds;

        public int RemainingSeconds
        {
            get => _remainingSeconds;
            set
            {
                _remainingSeconds = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning { get; private set; }

        public Clock(int allottedSec = 0)
        {
            _allottedSeconds = allottedSec;
        }

        public void SetAllotted(int allottedSec)
        {
            _allottedSeconds = allottedSec;
        }

        public void Start()
        {
            // if Start() is being called for the first time
            if (_timer == null)
            {
                _timer = new DispatcherTimer(DispatcherPriority.Normal, Application.Current.Dispatcher)
                {
                    Interval = new TimeSpan(0, 0, 1)
                };
                _timer.Tick += Timer_Ticked;
            }

            _timer.Start();
            IsRunning = true;
        }

        public void Pause()
        {
            if (_timer == null)
                return;

            _timer.Stop();
            _timer = null;
            _elapsedSeconds = 0;
            IsRunning = false;
        }

        public void Rewind()
        {
            RemainingSeconds = _allottedSeconds;
        }

        private void Timer_Ticked(object? sender, EventArgs e)
        {
            if (_elapsedSeconds + 1 > _allottedSeconds)
                Pause();

            RemainingSeconds = _allottedSeconds - ++_elapsedSeconds;
        }
    }
}