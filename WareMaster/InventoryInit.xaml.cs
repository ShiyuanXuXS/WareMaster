using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Interaction logic for InventoryInit.xaml
    /// </summary>
    public partial class InventoryInit : Window
    {
        private bool hasTransactions = Globals.wareMasterEntities.Transactions.Any();
        public InventoryInit()
        {
            InitializeComponent();
            InitializeLvInit();

        }

        private void LvInit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            showEdit();
        }
        private void LvInit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            showEdit();
        }

        private void showEdit()
        {
            if (hasTransactions) { 
                return; 
            }

            if (LvInit.SelectedItem != null)
            {
                var selectedData = (dynamic)LvInit.SelectedItem;
                int itemId = selectedData.ItemId;
                InventoryInitEdit editWindow = new InventoryInitEdit(selectedData);
                editWindow.Closed += (s, args) =>
                {
                    InitializeLvInit();
                };
                editWindow.ShowDialog();

            }
        }

        private void InitializeLvInit()
        {
            try
            {
                var query = from item in Globals.wareMasterEntities.Items
                            join settlement in Globals.wareMasterEntities.Settlements on item.id equals settlement.Item_Id into gj
                            from sub in gj.DefaultIfEmpty()
                            select new
                            {
                                ItemId = item.id,
                                ItemName = item.Itemname,
                                CategoryName = item.Category.Category_Name,
                                Unit = item.Unit != null ? item.Unit : string.Empty,
                                Location = item.Location != null ? item.Location : string.Empty,
                                Description = item.Description != null ? item.Description : string.Empty,
                                Quantity = sub != null ? sub.Quantity : 0,
                                Total = sub != null ? sub.Total : 0,
                                SettleDate = sub != null ? sub.Settle_Date : DateTime.Now,
                                SettlementId = sub != null ? sub.id : -1


                            };
                LvInit.ItemsSource = query.ToList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
        private void InitializeButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete all inbound and outbound records and settlement data?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                Globals.wareMasterEntities.Transactions.RemoveRange(Globals.wareMasterEntities.Transactions);
                Globals.wareMasterEntities.Settlements.RemoveRange(Globals.wareMasterEntities.Settlements);
                Globals.wareMasterEntities.SaveChanges();
                InitializeLvInit();
            }
        }
    }
}
