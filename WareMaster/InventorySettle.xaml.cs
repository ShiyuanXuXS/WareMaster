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
            if (int.TryParse(txtNumber.Text, out int numOfRecords) && numOfRecords>0)
            {
                ShowSettletDates(numOfRecords);
            }
            else
            {
                MessageBox.Show("Please enter a valid number of records.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //在这里实现：如果LVSettle没有选中项，则提示；如果有选中项，则要求用户确认；如果用户取消则返回，如果用户确认，则删除所有这个日期的Settlements的记录并保存，然后提示操作是否成功
            if (LVSettle.SelectedItem == null)
            {
                MessageBox.Show("Please select a settlement date to delete.");
                return;
            }
            if(MessageBoxResult.No== MessageBox.Show("Are you sure you want to delete the selected settlement date?", "Confirmation", MessageBoxButton.YesNo))
            {
                return;
            }
            DateTime selectedDate = (DateTime)LVSettle.SelectedItem;
            try
            {
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
        }

        private void Settle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
