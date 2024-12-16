using AttStat.ViewModels;
using System.Windows.Controls;

namespace AttStat.Views
{
    /// <summary>
    /// Логика взаимодействия для TableView.xaml
    /// </summary>
    public partial class TableView : Page
    {
        public TableView()
        {
            DataContext = new TableVM();
            InitializeComponent();
        }
    }
}
