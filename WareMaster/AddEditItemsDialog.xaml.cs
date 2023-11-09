﻿using FluentValidation;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
        Item currItem = new Item();
        string errorMessage;
        List<Category> categories = Globals.wareMasterEntities.Categories.ToList();
        ItemInputValidator validator = new ItemInputValidator();

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
                CategoryComboBox.ItemsSource = categories;
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Fatal error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

        }


        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string selectedCategoryName = (CategoryComboBox.SelectedItem as Category)?.Category_Name;
            Category selectedCategory = categories.FirstOrDefault(category => category.Category_Name == selectedCategoryName);
            try
            {
                if (string.IsNullOrWhiteSpace(ItemNameInput.Text) ||
                    string.IsNullOrWhiteSpace(DescriptionInput.Text) ||
                    string.IsNullOrWhiteSpace(LocationInput.Text) ||
                    CategoryComboBox.SelectedItem == null ||
                    UnitComboBox.SelectedItem == null)
                {                  
                    throw new ArgumentException("Input incompleted");
                }

                if (currItem != null)
                { // update
                    Console.WriteLine("ENTER EDIT");
                    var itemToUpdate = Globals.wareMasterEntities.Items.FirstOrDefault(item => item.id == currItem.id);
                    itemToUpdate.Itemname = ItemNameInput.Text;
                    itemToUpdate.Description = DescriptionInput.Text;
                    itemToUpdate.Category_Id = selectedCategory.id;
                    itemToUpdate.Location = LocationInput.Text;
                    itemToUpdate.Unit =(UnitComboBox.SelectedItem as ComboBoxItem)?.Tag as string;
                    Console.WriteLine(currItem);
                    var result = validator.Validate(itemToUpdate);
                    if (!result.IsValid)
                    {
                        throw new ArgumentException(result.ToString(Environment.NewLine));
                    }

                }
                else // add
                {
                    Console.WriteLine("enter add");
                    Item newItem = new Item
                    {
                        Itemname = ItemNameInput.Text,
                        Description = DescriptionInput.Text,

                        Category_Id = selectedCategory.id,
                        Location = LocationInput.Text,
                        Unit = (UnitComboBox.SelectedItem as ComboBoxItem)?.Tag as string

                    };
                    var result = validator.Validate(newItem);
                    if (!result.IsValid)
                    {
                        throw new ArgumentException(result.ToString(Environment.NewLine));
                    }
                    Console.WriteLine(newItem);
                    Globals.wareMasterEntities.Items.Add(newItem);
                }
                Globals.wareMasterEntities.SaveChanges();
                this.DialogResult = true; // dismiss the dialog
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(this, ex.Message, "Input error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Database error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        private void ItemNameInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Item.IsItemNameValid(ItemNameInput.Text, out errorMessage))
            {
                LblErrItemName.Visibility = Visibility.Visible;
                LblErrItemName.Content = errorMessage;
            }
            else
            {
                LblErrItemName.Visibility = Visibility.Hidden;
            }
        }

        private void DescriptionInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Item.IsDescriptionValid(DescriptionInput.Text, out errorMessage))
            {
                LblErrDescription.Visibility = Visibility.Visible;
                LblErrDescription.Content = errorMessage;
            }
            else
            {
                LblErrDescription.Visibility = Visibility.Hidden;
            }
        }

        private void CategoryComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Item.IsCategoryValid(CategoryComboBox.SelectedItem.ToString(), out errorMessage))
            {
                LblErrCategory.Visibility = Visibility.Visible;
                LblErrCategory.Content = errorMessage;
            }
            else
            {
                LblErrCategory.Visibility = Visibility.Hidden;
            }
        }

        private void LocationInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Item.IsLocationValid(LocationInput.Text, out errorMessage))
            {
                LblErrLocation.Visibility = Visibility.Visible;
                LblErrLocation.Content = errorMessage;
            }
            else
            {
                LblErrLocation.Visibility = Visibility.Hidden;
            }
        }

        private void UnitComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!Item.IsUnitValid(UnitComboBox.SelectedItem.ToString(), out errorMessage))
            {
                LblErrUnit.Visibility = Visibility.Visible;
                LblErrUnit.Content = errorMessage;
            }
            else
            {
                LblErrUnit.Visibility = Visibility.Hidden;
            }
        }
    }

}
