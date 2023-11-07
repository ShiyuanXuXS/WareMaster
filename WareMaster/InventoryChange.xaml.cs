﻿using System;
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
    /// Interaction logic for InventoryChange.xaml
    /// </summary>
    public partial class InventoryChange : Window
    {
        private string option;
        private Transaction transaction;
        private User user;
        private Item item;
        public InventoryChange(String option)
        {
            this.option = option;
            InitializeComponent();
            Title = option;
            ConfirmButton.Content = option;

            user = new User
            {
                id = 1,
                Username = "testUser",
                Role = 0,
                Password = "password",
                Email = "test@email.com"
            };//for test only, to be switched to logged user

            TransactionInit();
            
        }

        private void TransactionInit()
        {
            DateTime transaction_date=(transaction != null) ?  transaction.Transaction_Date :  DateTime.Now.Date;
            
            transaction = new Transaction
            {
                Transaction_Date = transaction_date,
                User_Id = user.id,
                Item_Id=(item!=null)?item.id : 0,
            };
            ReBound();
        }
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //validation
            bool isValid = true;
            int quantity;
            decimal total;
            if (transaction.Item_Id == 0 )
            {
                itemValidation.Text = "Item is required.";
                isValid = false;
            }
            else
            {
                itemValidation.Text = "";
            }
            if (!int.TryParse(txtQuantity.Text, out quantity) || quantity <= 0)
            {
                quantityValidation.Text = "Quantity must be a positive integer.";
                isValid = false;
            }
            else
            {
                quantityValidation.Text = "";
            }
            if (!decimal.TryParse(txtTotal.Text, out total) || total <= 0)
            {
                totalValidation.Text = "Total must be a positive decimal.";
                isValid = false;
            }
            else
            {
                totalValidation.Text = "";
            }
            if (!isValid)
            {
                MessageBox.Show("Validation failed!");
                return;
            }
            //For outbound, Set Quantity and Total to their negative values
            if (option == "Outbound")
            {
                
                transaction.Quantity =(transaction.Quantity>0)? -transaction.Quantity: transaction.Quantity;
                transaction.Total = (transaction.Total>0) ?-transaction.Total: transaction.Total;
            }

            // Insert the transaction record
            Globals.wareMasterEntities.Transactions.Add(transaction);
            try
            {
                Globals.wareMasterEntities.SaveChanges();
                MessageBox.Show("Transaction saved successfully.");
                TransactionInit();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving transaction: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private List<Item> GetMatchingItems(string keyword)
        {
            List<Item> matchingItems = Globals.wareMasterEntities.Items
                .Where(item => item.Itemname.Contains(keyword))
                .ToList();
            return matchingItems;
        }
        private void txtSearchItem_TextChanged(object sender, TextChangedEventArgs e)
        {

            string searchKeyword = txtSearchItem.Text;
            List<Item> matchingItems = GetMatchingItems(searchKeyword);
            itemListBox.Items.Clear();
            foreach (var item in matchingItems)
            {
                ListBoxItem listBoxItem = new ListBoxItem();
                listBoxItem.Content = item.Itemname;
                listBoxItem.Tag = item;
                itemListBox.Items.Add(listBoxItem);
            }

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                itemListPopup.IsOpen = true;
            }
            else
            {
                itemListPopup.IsOpen = false;
            }

        }
        private void itemListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (itemListBox.SelectedItem != null)
            {
                ListBoxItem listBoxItem = (ListBoxItem)itemListBox.SelectedItem;
                Item selectedItem = (Item)listBoxItem.Tag;

                if (selectedItem != null)
                {
                    item = selectedItem;
                    transaction.Item_Id = selectedItem.id;
                    ReBound();
                }

                itemListPopup.IsOpen = false;
            }
        }
        private void ReBound()
        {
            DataContext = null;
            DataContext = transaction;

            if (item!=null)
            {
                txtItemId.Text = item.id.ToString();
                txtItemName.Text = item.Itemname;
            }
            
            txtUsername.Text = user.Username;

            itemValidation.Text = "";
            quantityValidation.Text = "";
            totalValidation.Text = "";
        }

    }
}