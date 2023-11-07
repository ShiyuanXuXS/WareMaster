using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WareMaster
{
    /// <summary>
    /// Interaction logic for ItemsManagementDialog.xaml
    /// </summary>
    public partial class ItemsManagementDialog : Window
    {
        
        public ItemsManagementDialog()
        {
            InitializeComponent();
            InitializeLvItems();
        }

        private void InitializeLvItems()
        {
            try
            {
                var query = from item in Globals.wareMasterEntities.Items
                            join category in Globals.wareMasterEntities.Categories
                            on item.Category_Id equals category.id
                            select new
                            {
                                ItemId = item.id,
                                ItemName = item.Itemname,
                                Description = item.Description != null ? item.Description : string.Empty, 
                                CategoryName = category.Category_Name,
                                Unit = item.Unit != null ? item.Unit : string.Empty,
                                Location = item.Location != null ? item.Location : string.Empty
                            };
                LvItems.ItemsSource = query.ToList();
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Fatal error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }
        private void LvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Item currItem = LvItems.SelectedItem as Item;
            AddEditItemsDialog dialog = new AddEditItemsDialog();
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                InitializeLvItems();
                LblMessage.Text = "Item updated";
            }
        }

        private void MenuItemAddItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEditItemsDialog dialog = new AddEditItemsDialog();
                dialog.Owner = this;
                dialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
    }
    
}
