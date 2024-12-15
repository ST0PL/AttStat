using System.Text.RegularExpressions;
using System.Windows;
using AttStat.ViewModels;

namespace AttStat
{
    /// <summary>
    /// Логика взаимодействия для AddDataWindow.xaml
    /// </summary>
    public partial class AddDataWindow : Window
    {
        public bool IsClosed { get; set; }
        public AddDataWindow()
        {
            DataContext = new AddDataVM(() => { Close(); });
            InitializeComponent();
            Closed += (sender,args) => IsClosed = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
            => Close();

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
            => e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { try { DragMove(); } catch { } }
    }
}
