using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuizApp.BL
{
    public abstract class AbstractViewModel : INotifyPropertyChanged
    {
        #region INOTIFY

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

