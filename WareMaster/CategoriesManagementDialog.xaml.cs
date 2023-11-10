﻿using Microsoft.Win32;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for CategoriesManagementDialog.xaml
    /// </summary>
    public partial class CategoriesManagementDialog : Window
    {
        private List<Category> filterCategories = new List<Category>();
        private List<Category> allCategories = new List<Category>();
        private string loginUser = Globals.Username;
        public CategoriesManagementDialog()
        {
            InitializeComponent();
            InitializeDgCategories();
            TbUserName.Text = loginUser;
        }


        private void InitializeDgCategories()
        {
            try
            {
                allCategories = Globals.wareMasterEntities.Categories.ToList();
                DgCategories.ItemsSource = allCategories;
                //TxblItemCount.Text = "Total " + query.Count().ToString() + " Items";
            }
            catch (SystemException ex)
            {
                MessageBox.Show(this, "Error reading from database\n" + ex.Message, "Fatal error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
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
        private void BtnToHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow dialog = new MainWindow();
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    InitializeDgCategories();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Users Data");

                var data = DgCategories.Items;
                if (data.Count <= 0)
                {
                    MessageBox.Show("No data to export");
                    return;
                }
                PropertyInfo[] columnTypes = data[0].GetType().GetProperties();

                // write data to excel
                for (int col = 1; col < columnTypes.Length; col++)
                {
                    worksheet.Cells[1, col].Value = columnTypes[col - 1].Name;
                    for (int row = 2; row < data.Count + 2; row++)
                    {
                        PropertyInfo property = columnTypes[col - 1];
                        var value = property.GetValue(data[row - 2], null);
                        if (property.PropertyType == typeof(DateTime))
                        {
                            worksheet.Cells[row, col].Value = ((DateTime)value).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            worksheet.Cells[row, col].Value = value;
                        }
                    }
                }
                using (var cells = worksheet.Cells[1, 1, 1, columnTypes.Length - 1])
                {
                    cells.Style.Font.Bold = true;
                    cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // set column style
                using (var cells = worksheet.Cells[2, 1, data.Count + 1, columnTypes.Length - 1])
                {
                    cells.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                // set width and height
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

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = DgCategories.SelectedItem as User;
            if (selectedUser == null) return;

            AddEditUsersDialog dialog = new AddEditUsersDialog(selectedUser);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
            {
                InitializeDgCategories();
                //LblMessage.Text = "User updated";
            }
            //DgUsers_SelectionChanged(DgUsers, new SelectionChangedEventArgs(Selector.SelectedEvent, new List<object>(), new List<object>()));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = DgCategories.SelectedItem as User;
            if (selectedUser == null) return;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this category?", "Delete Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                User userToDelete = Globals.wareMasterEntities.Users.SingleOrDefault(user => user.id == selectedUser.id);
                if (userToDelete != null)
                {
                    Globals.wareMasterEntities.Users.Remove(userToDelete);
                    Globals.wareMasterEntities.SaveChanges();
                    InitializeDgCategories();
                }
            }
        }

        private void BtnAddItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddEditUsersDialog dialog = new AddEditUsersDialog();
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    InitializeDgCategories();
                    //LblMessage.Text = "User added";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (txtFilter.Text == "")
            {
                filterCategories = allCategories;
                //currentPage = 1;
                //DisplayPage(currentPage);
            }
            else
            {
                filterCategories = new List<Category>(from category in allCategories
                                                  where category.Category_Name.ToLower().Contains(txtFilter.Text.Trim().ToLower())
                                             select category);
                //currentPage = 1;
                //DisplayPage(currentPage);
            }
            DgCategories.ItemsSource = filterCategories;
        }
    }
}
