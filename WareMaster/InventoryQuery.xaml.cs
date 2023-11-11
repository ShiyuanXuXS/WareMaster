using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
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
    /// Interaction logic for InventoryQuery.xaml
    /// </summary>
    public partial class InventoryQuery : Window
    {
        public InventoryQuery()
        {
            InitializeComponent();
            DateBegin.SelectedDate = DateTime.Now;
            DateEnd.SelectedDate = DateTime.Now;
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            DataGridResult.Tag = "";
            switch (QueryFor.Text)
            {
                case "Inventory":
                    
                    if (GroupBy.Text != "Category")
                    {
                        //Inventory By Item
                        if (txtInputName.Tag == null)
                        {
                            DataGridResult.ItemsSource = Inventory.GetAllInventoriesByItem((DateTime)DateBegin.SelectedDate);
                        }
                        else
                        {
                            DataGridResult.ItemsSource = new List<InventoryData> { Inventory.GetInventoryByItem((Item)txtInputName.Tag, (DateTime)DateBegin.SelectedDate) };
                        }
                    }
                    else
                    {
                        //Inventory By Category
                        if (txtInputName.Tag == null)
                        {
                            DataGridResult.ItemsSource = Inventory.GetAllInventoriesByCategory((DateTime)DateBegin.SelectedDate);
                        }
                        else
                        {
                            DataGridResult.ItemsSource = new List<InventoryData> { Inventory.GetInventoryByCategory((Category)txtInputName.Tag, (DateTime)DateBegin.SelectedDate) };
                        }
                    };
                    break;
                case "Summary":
                    if (GroupBy.Text != "Category")
                    {
                        //Summary By Item
                        if (txtInputName.Tag == null)
                        {
                            DataGridResult.ItemsSource = Inventory.GetAllSummaryByItem((DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                        else
                        {
                            DataGridResult.ItemsSource = Inventory.GetSummaryByItem((Item)(txtInputName.Tag), (DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                    }
                    else
                    {
                            //Summary By Category
                            if (txtInputName.Tag == null)
                            {
                                DataGridResult.ItemsSource = Inventory.GetAllSummaryByCategory((DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                            }
                            else
                            {
                                DataGridResult.ItemsSource = Inventory.GetSummaryByCategory((Category)(txtInputName.Tag), (DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                            }
                        };
                    break;
                case "Details":
                    DataGridResult.Tag = "Details";
                    if (GroupBy.Text != "Category")
                    {
                        //Details By Item
                        if (txtInputName.Tag == null)
                        {
                            DataGridResult.ItemsSource = Inventory.GetAllInventoryChangeDetailsByItem((DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                        else
                        {
                            DataGridResult.ItemsSource = Inventory.GetInventoryChangeDetailsByItem((Item)(txtInputName.Tag), (DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                    }
                    else
                    {
                        //Details By Item
                        if (txtInputName.Tag == null)
                        {
                            DataGridResult.ItemsSource = Inventory.GetAllInventoryChangeDetailsByItem((DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                        else
                        {
                            DataGridResult.ItemsSource = Inventory.GetInventoryChangeDetailsByCategory((Category)(txtInputName.Tag), (DateTime)DateBegin.SelectedDate, (DateTime)DateEnd.SelectedDate);
                        }
                    };
                    break;
                default: break;
            }
            

            foreach (DataGridTextColumn column in DataGridResult.Columns)
            {
                column.Binding.StringFormat = column.Header.ToString() == "Date" ? "yyyy-MM-dd" : column.Header.ToString() == "Total" ? "${0:N2}" : null;
            }

            Mouse.OverrideCursor = null;
        }

        private void QueryFor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblTo!=null && DateEnd!=null)
            {
                lblTo.Visibility = (QueryFor.SelectedIndex ==0) ? Visibility.Hidden: Visibility.Visible;
                DateEnd.Visibility = (QueryFor.SelectedIndex == 0) ? Visibility.Hidden : Visibility.Visible;
            }
            
        }

        private void txtInputName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchKeyword = txtInputName.Text;
            switch (GroupBy.Text)
            {
                case "Item":
                    List<Item> matchingItems = Globals.wareMasterEntities.Items
                        .Where(item => item.Itemname.Contains(searchKeyword))
                        .ToList();
                    ListBoxNames.Items.Clear();
                    foreach (Item item in matchingItems)
                    {
                        ListBoxItem listBoxName=new ListBoxItem();
                        listBoxName.Content = item.Itemname;
                        listBoxName.Tag = item;
                        ListBoxNames.Items.Add(listBoxName);
                    }
                    break;
                case "Category":
                    List<Category> machingCategories=Globals.wareMasterEntities.Categories
                        .Where(c=>c.Category_Name.Contains(searchKeyword))
                        .ToList ();
                    ListBoxNames.Items.Clear();
                    foreach (Category cat in machingCategories)
                    {
                        ListBoxItem listBoxName=new ListBoxItem();
                        listBoxName.Content=cat.Category_Name;
                        listBoxName.Tag = cat;
                        ListBoxNames.Items.Add(listBoxName);
                    }
                    break;
                default:
                    break;
            }
            if (string.IsNullOrWhiteSpace(searchKeyword)||ListBoxNames.Items.Count==0)
            {
                ListPopup.IsOpen = false;
                txtInputName.Tag = null;
            }
            else
            {
                ListPopup.IsOpen = true;
            }
        }

        private void ListBoxNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBoxNames.SelectedItem==null) return;
            
            ListBoxItem listBoxItem = (ListBoxItem)ListBoxNames.SelectedItem;
            switch (GroupBy.Text)
            {
                case "Item":
                    Item selectedItem = (Item)listBoxItem.Tag;
                    txtInputName.Tag = selectedItem;
                    txtInputName.Text = selectedItem.Itemname;

                    break;
                case "Category":
                    Category selectedCategory = (Category)listBoxItem.Tag;
                    txtInputName.Tag = selectedCategory;
                    txtInputName.Text = selectedCategory.Category_Name;
                    break;
                default:
                    break;
            }
            ListBoxNames.Items.Clear();
            ListPopup.IsOpen = false;

        }

        private void GroupBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtInputName==null) return;
            txtInputName.Tag = null;
            txtInputName.Text = "";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
