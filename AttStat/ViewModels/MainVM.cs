using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using AttStat.Views;

namespace AttStat.ViewModels
{
    internal partial class MainVM : BaseVM
    {
        private Page _currentPage = null!;
        private SystemDataContext _context;
        public Page CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }
        private List<Page> _pages;
        public MainVM()
        {
            _pages = new List<Page>() { new TableView(), new ReportsView(), new ChartsView() };
            CurrentPage = _pages[0];
        }
        [RelayCommand]
        private void NavigateTo(string index)
        {
            int parsedIndex;
            if(int.TryParse(index, out parsedIndex) && _pages.IndexOf(_currentPage) != parsedIndex)
                CurrentPage = _pages[int.Parse(index)];
        }
    }
}
