using System.Windows.Controls;
using AttStat.ViewModels;
namespace AttStat.Views
{
    /// <summary>
    /// Логика взаимодействия для ReportsView.xaml
    /// </summary>
    public partial class ReportsView : Page
    {
        public ReportsView()
        {
            DataContext = new ReportsVM();
            InitializeComponent();
        }
    }
}
