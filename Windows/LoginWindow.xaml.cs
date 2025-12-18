
using System.Windows;

namespace pract_15.Windows
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (PinBox.Password == "1234")
                OpenMain(true);
            else
                MessageBox.Show("Неверный пин-код");
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            OpenMain(false);
        }

        private void OpenMain(bool isManager)
        {
            var main = new MainWindow(isManager);
            main.Show();
            Close();
        }
    }
}
