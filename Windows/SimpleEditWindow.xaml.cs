using System.Windows;

namespace pract_15.Windows
{
    public partial class SimpleEditWindow : Window
    {
        public string Value { get; private set; }

        public SimpleEditWindow(string title, string value = "")
        {
            InitializeComponent();
            TitleText.Text = title;
            ValueBox.Text = value;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}