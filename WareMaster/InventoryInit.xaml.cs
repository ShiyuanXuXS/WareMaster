using Microsoft.Win32;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
            try
            {
                Globals.wareMasterEntities.Transactions.RemoveRange(Globals.wareMasterEntities.Transactions);
                Globals.wareMasterEntities.Settlements.RemoveRange(Globals.wareMasterEntities.Settlements);
                Globals.wareMasterEntities.SaveChanges();
                InitializeLvInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private class ViewItem
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public string CategoryName { get; set; }
            public string Unit { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
            public int Quantity { get; set; }
            public decimal Total { get; set; }
            public DateTime SettleDate { get; set; }
            public int SettlementId { get; set; }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Inventory Data");

                // 获取 ListView 的数据源
                var data = LvInit.ItemsSource;

                MessageBox.Show(data.ToString());
                return;
                int row = 1;
                int col = 1;

                // 写入列标题
                worksheet.Cells[row, 1].Value = "Item ID";
                worksheet.Cells[row, 2].Value = "Item Name";
                worksheet.Cells[row, 3].Value = "Category Name";
                worksheet.Cells[row, 4].Value = "Unit";
                worksheet.Cells[row, 5].Value = "Location";
                worksheet.Cells[row, 6].Value = "Description";
                worksheet.Cells[row, 7].Value = "Quantity";
                worksheet.Cells[row, 8].Value = "Total";
                worksheet.Cells[row, 9].Value = "Settle Date";


                // 写入数据
                //row++;
                //foreach (var item in data)
                //{
                    
                //    col = 1;
                //    worksheet.Cells[row, col].Value = item.ItemId;
                //    col++;
                //    worksheet.Cells[row, col].Value = item.ItemName;
                //    col++;
                //    worksheet.Cells[row, col].Value = item.CategoryName;
                //    col++;
                //    worksheet.Cells[row, col].Value = item.Unit;
                //    col++;
                //    worksheet.Cells[row, col].Value = item.Location;
                //    col++;
                //    worksheet.Cells[row, col].Value = item.Description;
                //    col++;
                //    worksheet.Cells[row, col].Value = item.Quantity;
                //    col++;
                //    worksheet.Cells[row, col].Value = item.Total;
                //    col++;
                //    worksheet.Cells[row, col].Value = item.SettleDate.ToString("yyyy-MM-dd");
                //    row++;
                //}

                // 保存 Excel 文件
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx|All Files|*.*",
                    DefaultExt = "xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var newFile = new FileInfo(saveFileDialog.FileName);
                    package.SaveAs(newFile);
                    MessageBox.Show("Data exported successfully!", "Export Data", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

    }
}
