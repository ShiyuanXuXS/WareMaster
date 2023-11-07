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
using System.Reflection;
using System.Printing;
using System.Windows.Markup;
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
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                // 创建一个FlowDocument来容纳ListView的内容
                FlowDocument document = new FlowDocument();

                // 创建一个Table用于显示ListView的数据
                System.Windows.Documents.Table table = new System.Windows.Documents.Table();
                //document.Blocks.Add(table);

                // Retrieve data from the ListView
                var data = LvInit.Items;
                if (data.Count > 0)
                {
                    // Retrieve column headers and create table columns
                    PropertyInfo[] columnTypes = data[0].GetType().GetProperties();
                    List<string> columnHeaders = columnTypes.Select(p => p.Name)
                        .Where(name => name != "SettlementId")
                        .ToList();

                    // Add table column headers
                    TableRowGroup headerGroup = new TableRowGroup();
                    System.Windows.Documents.TableRow headerRow = new System.Windows.Documents.TableRow();
                    foreach (string columnHeader in columnHeaders)
                    {
                        table.Columns.Add(new TableColumn());
                        table.Columns[table.Columns.Count - 1].Width = new GridLength(printDialog.PrintableAreaWidth / columnHeaders.Count);
                        headerRow.Cells.Add(new System.Windows.Documents.TableCell(new Paragraph(new Run(columnHeader))));
                    }
                    headerGroup.Rows.Add(headerRow);
                    table.RowGroups.Add(headerGroup);

                    // Add data rows
                    foreach (var item in data)
                    {
                        TableRowGroup dataGroup = new TableRowGroup();
                        System.Windows.Documents.TableRow dataRow = new System.Windows.Documents.TableRow();
                        foreach (string columnHeader in columnHeaders)
                        {
                            PropertyInfo property = item.GetType().GetProperty(columnHeader);
                            object value = property?.GetValue(item, null);
                            dataRow.Cells.Add(new System.Windows.Documents.TableCell(new Paragraph(new Run(value?.ToString() ?? string.Empty))));
                        }
                        dataGroup.Rows.Add(dataRow);
                        table.RowGroups.Add(dataGroup);
                    }
                    document.Blocks.Add(table);


                    //document.PageWidth = printDialog.PrintableAreaWidth;
                    //document.PageHeight = printDialog.PrintableAreaHeight;



                    // Set the print document's page size to fit A4 paper in portrait mode
                    IDocumentPaginatorSource paginator = document;
                    //paginator.DocumentPaginator.PageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

                    // Print the document
                    printDialog.PrintDocument(paginator.DocumentPaginator, "ListView Printing");
                }
                else
                {
                    MessageBox.Show("没有要打印的数据");
                }
            }
        }

      

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Initial Inventory Data");

                // get data from listview
                var data = LvInit.Items;
                if (data.Count <= 0)
                {
                    MessageBox.Show("No data to export");
                    return;
                }
                PropertyInfo[] columnTypes = data[0].GetType().GetProperties();

                // write data to excel
                for (int col = 1; col < columnTypes.Length; col++)
                {
                    worksheet.Cells[1,col].Value = columnTypes[col-1].Name;    
                    for (int row=2;row<data.Count+2;row++)
                    {
                        PropertyInfo property = columnTypes[col - 1];
                        var value = property.GetValue(data[row - 2], null);
                        if (property.PropertyType == typeof(DateTime)) {
                            worksheet.Cells[row,col].Value = ((DateTime)value).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            worksheet.Cells[row, col].Value = value;
                        }
                    }
                }
                using (var cells = worksheet.Cells[1, 1, 1, columnTypes.Length-1])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // 设置数据单元格的样式
                using (var cells = worksheet.Cells[2, 1, data.Count + 1, columnTypes.Length-1])
                {
                    cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // 自适应列宽
                worksheet.Cells.AutoFitColumns();

                // save Excel file
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
