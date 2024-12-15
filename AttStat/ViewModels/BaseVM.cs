using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AttStat.ViewModels
{
    internal class BaseVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string property = null)
            => PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(property));
    }
}
