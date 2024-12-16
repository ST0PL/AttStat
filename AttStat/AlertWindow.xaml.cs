using System.Windows;
using System.Windows.Input;

namespace AttStat
{
    /// <summary>
    /// Логика взаимодействия для AlertWindow.xaml
    /// </summary>
    public partial class AlertWindow : Window
    {
        public string? UpTitle { get; set; }
        public string? Text { get; set; }
        public AlertWindow(string title, string text)
        {
            UpTitle = title;
            Text = text;
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
            => Close();
    }
}
