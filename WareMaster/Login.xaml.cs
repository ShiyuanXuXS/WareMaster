using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace WareMaster
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void TblName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtName.Focus();
        }

        private void TxtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtName.Text) && TxtName.Text.Length > 0)
            {
                TblName.Visibility = Visibility.Collapsed;
            }
            else
            {
                TblName.Visibility = Visibility.Visible;
            }

        }

        private void TblPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TxtPassword.Focus();
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtPassword.Password) && TxtPassword.Password.Length > 0)
            {
                TblPassword.Visibility = Visibility.Collapsed;
            }
            else
            {
                TblPassword.Visibility = Visibility.Visible;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtName.Text) && !string.IsNullOrEmpty(TxtPassword.Password))
            {

            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
