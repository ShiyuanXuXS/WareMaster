using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for UserManagementDialog.xaml
    /// </summary>
    public partial class UsersManagementDialog : Window
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPage = 0;
        private List<User> filterUsers = new List<User>();
        private List<User> allUsers = new List<User>();
        public UsersManagementDialog()
        {
            InitializeComponent();
            InitializeDgUsers();
        }

        private void InitializeDgUsers()
        {
            try
            {
                allUsers = Globals.wareMasterEntities.Users.ToList();
                DgUsers.ItemsSource = allUsers;
                TxblItemCount.Text = "Total " + allUsers.Count().ToString() + " Users";
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Fatal error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private bool IsMaximized = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;
                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                    //GridContent.Height = 920;
                    IsMaximized = true;
                }
            }

        }

        private void DgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User selectedUser = DgUsers.SelectedItem as User;
            if (selectedUser == null) return;

            AddEditUsersDialog dialog = new AddEditUsersDialog(selectedUser);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                InitializeDgUsers();
                LblMessage.Text = "User updated";
            }
        }

        private void BtnActivities_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            DgUsers_SelectionChanged(DgUsers, new SelectionChangedEventArgs(Selector.SelectedEvent, new List<object>(), new List<object>()));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = DgUsers.SelectedItem as User;
            if (selectedUser == null) return;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this user?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                User userToDelete = Globals.wareMasterEntities.Users.SingleOrDefault(user => user.id == selectedUser.id);
                if (userToDelete != null)
                {
                    Globals.wareMasterEntities.Users.Remove(userToDelete);
                    Globals.wareMasterEntities.SaveChanges();
                    InitializeDgUsers();
                }
            }
        }

        private void MenuItemAddItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEditUsersDialog dialog = new AddEditUsersDialog();
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    InitializeDgUsers();
                    LblMessage.Text = "User added";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
           
            if (txtFilter.Text == "")
            {
                filterUsers = allUsers;
                //currentPage = 1;
                //DisplayPage(currentPage);
            }
            else
            {
                filterUsers = new List<User>(from user in allUsers
                                                 where user.Username.ToLower().Contains(txtFilter.Text.Trim().ToLower())
                                             select user);
                //currentPage = 1;
                //DisplayPage(currentPage);
            }
            DgUsers.ItemsSource = filterUsers;
        }
    }
}
