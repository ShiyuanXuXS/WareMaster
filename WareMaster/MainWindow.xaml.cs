
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WareMaster
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    //public class ExitKey : ICommand
    //{
    //    public event EventHandler CanExecuteChanged;

    //    public bool CanExecute(object parameter)
    //    {
    //        return true;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        Application.Current.Shutdown();
    //    }
    //}

    //public class ExitCommandContext
    //{
    //    public ICommand ExitCommand
    //    {
    //        get
    //        {
    //            return new ExitKey();
    //        } 
    //    }
    //}
    public partial class MainWindow : Window
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPage = 0;
        private List<ItemViewModel> allItems = new List<ItemViewModel>() ;
        private List<ItemViewModel> filterItems = new List<ItemViewModel>();
        private WareMasterEntities dbContext;
        public MainWindow()
        {
            InitializeComponent();
                //this.DataContext = new ExitCommandContext();
                //InitializeComponent();
                //Globals.wareMasterEntities = new WareMasterEntities();
            dbContext = Globals.DbContext;
            InitializeLvInit();
            totalPage = (int)Math.Ceiling((double)allItems.Count / pageSize);
            for (int i = 0; i < totalPage; i++)
            {
                Button newPageButton = new Button()
                {
                    Content = i+1,
                    Width = 15,
                    Height = 15,
                    FontSize = 10,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                newPageButton.Click += NewPageButton_Click;
                StackPaging.Children.Insert(i+2,newPageButton);
            }
        }

        private void NewPageButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton =(Button)sender;
            String page = clickedButton.Content.ToString();
            if (int.TryParse(page, out int currentPage))
            {
                DisplayPage(currentPage);
            }
            else
            {
                MessageBox.Show(this,"Somthing went wrong, will display first page of items.","error",MessageBoxButton.OK,MessageBoxImage.Exclamation);
                DisplayPage(1);
            }
        }

        private void DisplayPage(int page)
        {
            var itemsToDisplay = filterItems.Skip((page-1)*pageSize).Take(pageSize).ToList();
            DgStorage.ItemsSource = itemsToDisplay;
        }

        private void InitializeLvInit()
        {
            try
            {
                var query = from item in dbContext.Items
                            join settlement in dbContext.Settlements on item.id equals settlement.Item_Id into gj
                            from sub in gj.DefaultIfEmpty()
                            select new ItemViewModel
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
                allItems = query.ToList();
                filterItems = allItems;
                DisplayPage(currentPage);
                TxblItemCount.Text = "Total " + query.Count().ToString() + " Items";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryInit inventoryInit = new InventoryInit();
                inventoryInit.Owner = this;
                inventoryInit.ShowDialog();
            }catch (Exception ex) { MessageBox.Show(ex.Message); };
            
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
            if (e.ClickCount==2)
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

        private void MenuItemInventoryInit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryInit inventoryInit = new InventoryInit();
                inventoryInit.Owner = this;
                inventoryInit.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void MenuItemInventoryInbound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryChange inventoryInbound = new InventoryChange("Inbound");
                inventoryInbound.Owner = this;
                inventoryInbound.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void MenuItemInventoryOutbound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryChange inventoryOutbound = new InventoryChange("Outbound");
                inventoryOutbound.Owner = this;
                inventoryOutbound.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void MenuItemInventorySettle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventorySettle inventorySettle = new InventorySettle();
                inventorySettle.Owner = this;
                inventorySettle.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void MenuItemQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryQuery inventoryQuery = new InventoryQuery();
                inventoryQuery.Owner = this;
                inventoryQuery.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void CommandBindingNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void BtnManageItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemsManagementDialog itemDialog = new ItemsManagementDialog();
                itemDialog.Owner = this;
                itemDialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnManageUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UsersManagementDialog itemDialog = new UsersManagementDialog();
                itemDialog.Owner = this;
                itemDialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void BtnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                DisplayPage(currentPage);
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPage)
            {
                currentPage++;
                DisplayPage(currentPage);
            }
        }



        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtFilter.Text=="")
            {
                filterItems = allItems;
                currentPage = 1;
                DisplayPage(currentPage);
            }
            else
            {
                filterItems = new List<ItemViewModel>(from item in allItems
                              where item.ItemName.Contains(txtFilter.Text.Trim())
                              select item);
                currentPage = 1;
                DisplayPage(currentPage);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBox.Show("mainwindow closing");
        }
    }

    public class ItemViewModel
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
}
