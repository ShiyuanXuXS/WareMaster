using MahApps.Metro.Controls;
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
    /// Interaction logic for AddEditItemsDialog.xaml
    /// </summary>
    
    public partial class AddEditItemsDialog : Window
    {
        private Item currItem;
        private string errorMessage;

        public AddEditItemsDialog(Item currItem = null)
        {
            this.currItem = currItem;
            Console.WriteLine(currItem);
            InitializeComponent();
            InitializeCategory();
            if (currItem != null) // update, load select values
            {
                ItemId.Content = currItem.id;
                ItemNameInput.Text = currItem.Itemname;
                DescriptionInput.Text = currItem.Description;
                List<Category> categories = Globals.wareMasterEntities.Categories.ToList();
                CategoryComboBox.SelectedItem = categories.FirstOrDefault(category => category.id == currItem.Category_Id);
                foreach (ComboBoxItem item in UnitComboBox.Items)
                {
                    if (item.Content.ToString() == currItem.Unit)
                    {
                        item.IsSelected = true;
                        break;
                    }
                }
                LocationInput.Text = currItem.Location;
            }
        }

        private void InitializeCategory()
        {
            try
            {
                List<Category> categories = Globals.wareMasterEntities.Categories.ToList();
                CategoryComboBox.ItemsSource = categories;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Fatal error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                // Close();
                Environment.Exit(1);
            }
            
        }

        
       private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            

            
        }

        private void ItemNameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Item.IsItemNameValid(ItemNameInput.Text, out errorMessage))
            {
                currItem.Itemname = ItemNameInput.Text;
            }
            else
            {
                LblErrItemName.Visibility= Visibility.Visible;
                LblErrItemName.Content = errorMessage;
            }
        }

        private void DescriptionInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Item.IsDescriptionValid(DescriptionInput.Text, out errorMessage))
            {
                currItem.Description = DescriptionInput.Text;
            }
            else
            {
                LblErrDescription.Visibility = Visibility.Visible;
                LblErrDescription.Content = errorMessage;
            }

        }

        private void CategoryComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Item.IsCategoryValid(CategoryComboBox.SelectedItem.ToString(), out errorMessage))
            {
                //currItem.Description = CategoryComboBox.SelectedItem;
            }
            else
            {
                LblErrCategory.Visibility = Visibility.Visible;
                LblErrCategory.Content = errorMessage;
            }
        }
    }
}
