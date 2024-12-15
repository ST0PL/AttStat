using System.Windows.Controls;
using AttStat.ViewModels;


namespace AttStat.Views
{
    /// <summary>
    /// Логика взаимодействия для ChartsView.xaml
    /// </summary>
    public partial class ChartsView : Page
    {
        public ChartsView()
        {
            DataContext = new ChartsVM();
            InitializeComponent();
        }
    }
}
