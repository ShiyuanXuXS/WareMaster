using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPage = 0;
        private List<ViewItem> filterItems = new List<ViewItem>();
        private List<ViewItem> allItems = new List<ViewItem>();

        public ItemsManagementDialog()
        {
            InitializeComponent();
            InitializeLvItems();
            totalPage = (int)Math.Ceiling((double)allItems.Count / pageSize);
            for (int i = 0; i < totalPage; i++)
            {
                Button newPageButton = new Button()
                {
                    Content = i + 1,
                    Width = 15,
                    Height = 15,
                    FontSize = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                newPageButton.Click += NewPageButton_Click;
                StackPaging.Children.Insert(i + 2, newPageButton);
            }
        }

        private void NewPageButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;
            String page = clickedButton.Content.ToString();
            if (int.TryParse(page, out int currentPage))
            {
                DisplayPage(currentPage);
            }
            else
            {
                MessageBox.Show(this, "Somthing went wrong, will display first page of items.", "error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                DisplayPage(1);
            }
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                DisplayPage(currentPage);
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPage)
            {
                currentPage++;
                DisplayPage(currentPage);
            }
        }

        private void DisplayPage(int page)
        {
            var itemsToDisplay = filterItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            LvItems.ItemsSource = itemsToDisplay;
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
                allItems = query.ToList();
                LvItems.ItemsSource= allItems;
                TxblItemCount.Text = "Total " + query.Count().ToString() + " Items";
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
                    LblMessage.Text = "Item added";
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
                if (itemToDelete != null)
                {
                    Globals.wareMasterEntities.Items.Remove(itemToDelete);
                    Globals.wareMasterEntities.SaveChanges();
                    LblMessage.Text = "Item deleted";
                    InitializeLvItems();
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtFilter.Text == "")
            {
                filterItems = allItems;
                //currentPage = 1;
                //DisplayPage(currentPage);
            }
            else
            {
                filterItems = new List<ViewItem>(from item in allItems
                                                      where item.ItemName.ToLower().Contains(txtFilter.Text.Trim().ToLower())
                                                 select item);
                //currentPage = 1;
                //DisplayPage(currentPage);
            }
            LvItems.ItemsSource = filterItems;
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


    }

}
