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
    /// Interaction logic for InventorySettle.xaml
    /// </summary>
    public partial class InventorySettle : Window
    {
        public InventorySettle()
        {
            InitializeComponent();
            ShowSettletDates(5);
        }
        private void ShowSettletDates(int numOfRecords)
        {
            List<DateTime> recentSettlementDates = Globals.wareMasterEntities
                .Settlements
                .OrderByDescending(s => s.Settle_Date)
                .Select(s => s.Settle_Date)
                .Distinct()
                .Take(numOfRecords)
                .ToList();
            LVSettle.ItemsSource = recentSettlementDates;

        }
        private void GetSettleHistory_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (int.TryParse(txtNumber.Text, out int numOfRecords) && numOfRecords>0)
            {
                ShowSettletDates(numOfRecords);
            }
            else
            {
                MessageBox.Show("Please enter a valid number of records.");
            }
            Mouse.OverrideCursor = null;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (LVSettle.SelectedItem == null)
            {
                MessageBox.Show("Please select a settlement date to delete.");
                Mouse.OverrideCursor = null;
                return;
            }
            if(MessageBoxResult.No== MessageBox.Show("Are you sure you want to delete the selected settlement date?", "Confirmation", MessageBoxButton.YesNo))
            {
                Mouse.OverrideCursor = null;
                return;
            }
            DateTime selectedDate = (DateTime)LVSettle.SelectedItem;
            try
            {
                DateTime minDate = Globals.wareMasterEntities.Settlements
                    .Select(s => s.Settle_Date)
                    .DefaultIfEmpty(DateTime.MaxValue) 
                    .Min();

                if (selectedDate <= minDate)
                {
                    MessageBox.Show("Cannot delete the earlist settlement data.");
                    Mouse.OverrideCursor = null;
                    return;
                }

                var settlementsToDelete = Globals.wareMasterEntities.Settlements
                    .Where(s => s.Settle_Date == selectedDate)
                    .ToList();

                Globals.wareMasterEntities.Settlements.RemoveRange(settlementsToDelete);
                Globals.wareMasterEntities.SaveChanges();
                if (int.TryParse(txtNumber.Text, out int numOfRecords) && numOfRecords > 0)
                {
                    ShowSettletDates(numOfRecords);
                }
                else
                {
                    ShowSettletDates(5);
                }
                MessageBox.Show("Settlement deleted successfully.");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error deleting settlement: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }

        private void Settle_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            DateTime lastSettleDate = Globals.wareMasterEntities.Settlements
                .Select(s => s.Settle_Date)
                .DefaultIfEmpty(DateTime.MinValue)
                .Max();
            if ( dpSettleDate.SelectedDate <= lastSettleDate)
            {
                MessageBox.Show("Cannot settle on or before the last settlement date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBoxResult.No==MessageBox.Show("Do you want to settle inventories?", "Confirmation", MessageBoxButton.YesNo))
            {
                return;
            }
            
            foreach (var item in Globals.wareMasterEntities.Items)
            {
                // find last settlement of the item
                Settlement lastSettlement = Globals.wareMasterEntities.Settlements
                    .Where(s => s.Item_Id == item.id)
                    .OrderByDescending(s => s.Settle_Date)
                    .FirstOrDefault();

                if (lastSettlement != null)
                {
                    // get sum of transactions
                    List<Transaction> transactionsAfterLastSettle = Globals.wareMasterEntities.Transactions
                        .Where(t => t.Item_Id == item.id && t.Transaction_Date > lastSettlement.Settle_Date)
                        .ToList();

                    int totalQuantity = transactionsAfterLastSettle.Sum(transaction => transaction.Quantity);
                    decimal totalTotal = transactionsAfterLastSettle.Sum(transaction => transaction.Total);


                    // new settlement
                    Settlement newSettlement = new Settlement
                    {
                        Item_Id = item.id,
                        Settle_Date = dpSettleDate.SelectedDate.Value,
                        Quantity = lastSettlement.Quantity + totalQuantity,
                        Total = lastSettlement.Total + totalTotal
                    };
                    Globals.wareMasterEntities.Settlements.Add(newSettlement);
                }
                else
                {
                    //get sum of transactions
                    List<Transaction> transactionsAfterLastSettle = Globals.wareMasterEntities.Transactions
                        .Where(t => t.Item_Id == item.id )
                        .ToList();

                    int totalQuantity = transactionsAfterLastSettle.Sum(transaction => transaction.Quantity);
                    decimal totalTotal = transactionsAfterLastSettle.Sum(transaction => transaction.Total);
                    // new settlement
                    Settlement newSettlement = new Settlement
                    {
                        Item_Id = item.id,
                        Settle_Date = dpSettleDate.SelectedDate.Value,
                        Quantity = totalQuantity, 
                        Total = totalTotal 
                    };
                    Globals.wareMasterEntities.Settlements.Add(newSettlement);
                }
            }

            // save changes
            try
            {
                Globals.wareMasterEntities.SaveChanges();
                if (int.TryParse(txtNumber.Text, out int numOfRecords) && numOfRecords > 0)
                {
                    ShowSettletDates(numOfRecords);
                }
                else
                {
                    ShowSettletDates(5);
                }
                MessageBox.Show("Settlement completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error settling inventories: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Mouse.OverrideCursor = null;
        }
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
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
