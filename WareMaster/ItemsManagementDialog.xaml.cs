using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                            select new ViewItem
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

        private class ViewItem
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string CategoryName { get; set; }
            public string Unit { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
            public override string ToString()
            {
                return $"ItemId: {ItemId}, ItemName: {ItemName}, CategoryName: {CategoryName}, Unit: {Unit}, Location: {Location}, Description: {Description}";
            }
        }

        private void LvItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewItem selectedItem = LvItems.SelectedItem as ViewItem;
            if (selectedItem == null) return;
            Item currItem = new Item();
                currItem.id = selectedItem.ItemId;
                currItem.Itemname = selectedItem.ItemName;
                currItem.Description = selectedItem.Description;
                currItem.Unit = selectedItem.Unit;
                currItem.Location = selectedItem.Location;
                currItem.Category_Id = Globals.wareMasterEntities.Items.Where(item => item.id == selectedItem.ItemId).Select(item => item.Category_Id).SingleOrDefault();
            
            AddEditItemsDialog dialog = new AddEditItemsDialog(currItem);
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
                //dialog.ShowDialog();
                if (dialog.ShowDialog() == true)
                {
                    InitializeLvItems();
                    LblMessage.Text = "Item updated";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            LvItems_SelectionChanged(LvItems, new SelectionChangedEventArgs(Selector.SelectedEvent, new List<object>(), new List<object>()));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            ViewItem selectedItem = LvItems.SelectedItem as ViewItem;
            if (selectedItem == null) return;
            
            
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this item?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
                {
                Item itemToDelete = Globals.wareMasterEntities.Items.SingleOrDefault(item => item.id == selectedItem.ItemId);
                if(itemToDelete != null)
                {
                    Globals.wareMasterEntities.Items.Remove(itemToDelete);                 
                    Globals.wareMasterEntities.SaveChanges();
                    InitializeLvItems();
                }
            }
        }
    }
    
}
