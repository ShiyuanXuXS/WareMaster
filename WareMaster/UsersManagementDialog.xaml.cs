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
                List<User> allUsers = Globals.wareMasterEntities.Users.ToList();
                foreach (User user in allUsers)
                {
                    Console.WriteLine($"User ID: {user.id}");
                    Console.WriteLine($"Username: {user.Username}");
                    Console.WriteLine($"Role: {user.Role}");
                    Console.WriteLine($"Email: {user.Email}");
                    Console.WriteLine(); // Add a blank line for separation
                }
                DgUsers.ItemsSource = allUsers;
                filterUsers = allUsers;
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
            //ViewItem selectedItem = LvItems.SelectedItem as ViewItem;
            //if (selectedItem == null) return;
            //Item currItem = new Item();
            //currItem.id = selectedItem.ItemId;
            //currItem.Itemname = selectedItem.ItemName;
            //currItem.Description = selectedItem.Description;
            //currItem.Unit = selectedItem.Unit;
            //currItem.Location = selectedItem.Location;
            //currItem.Category_Id = Globals.wareMasterEntities.Items.Where(item => item.id == selectedItem.ItemId).Select(item => item.Category_Id).SingleOrDefault();

            //AddEditItemsDialog dialog = new AddEditItemsDialog(currItem);
            //dialog.Owner = this;
            //if (dialog.ShowDialog() == true)
            //{
            //    InitializeLvItems();
            //    LblMessage.Text = "Item updated";
            //}
        }

        private void BtnActivities_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //LvItems_SelectionChanged(LvItems, new SelectionChangedEventArgs(Selector.SelectedEvent, new List<object>(), new List<object>()));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            //ViewItem selectedItem = LvItems.SelectedItem as ViewItem;
            //if (selectedItem == null) return;


            //MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (result == MessageBoxResult.Yes)
            //{
            //    Item itemToDelete = Globals.wareMasterEntities.Items.SingleOrDefault(item => item.id == selectedItem.ItemId);
            //    if (itemToDelete != null)
            //    {
            //        Globals.wareMasterEntities.Items.Remove(itemToDelete);
            //        Globals.wareMasterEntities.SaveChanges();
            //        InitializeLvItems();
            //    }
            //}
        }

        private void MenuItemAddItems_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    AddEditItemsDialog dialog = new AddEditItemsDialog();
            //    dialog.Owner = this;
            //    //dialog.ShowDialog();
            //    if (dialog.ShowDialog() == true)
            //    {
            //        InitializeLvItems();
            //        LblMessage.Text = "Item updated";
            //    }
            //}
            //catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (txtFilter.Text == "")
            //{
            //    filterItems = allItems;
            //    //currentPage = 1;
            //    //DisplayPage(currentPage);
            //}
            //else
            //{
            //    filterItems = new List<ViewItem>(from item in allItems
            //                                     where item.ItemName.Contains(txtFilter.Text.Trim())
            //                                     select item);
            //    //currentPage = 1;
            //    //DisplayPage(currentPage);
            //}
        }
    }
}
