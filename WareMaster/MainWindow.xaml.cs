
using System;
using System.Collections.ObjectModel;
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
    public class ExitKey : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }

    public class ExitCommandContext
    {
        public ICommand ExitCommand
        {
            get
            {
                return new ExitKey();
            } 
        }
    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ExitCommandContext();
            //InitializeComponent();
            Globals.wareMasterEntities = new WareMasterEntities();
            InitializeLvInit();
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
                DgStorage.ItemsSource = query.ToList();
                TxblItemCount.Text = query.Count().ToString() + " Items";
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

                    IsMaximized = true;
                }
            }

        }

<<<<<<< Updated upstream
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
=======
        private void BtnItems_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemsManagementDialog dialog = new ItemsManagementDialog();
                dialog.Owner = this;
                Console.WriteLine("item management open");
                dialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
    
>>>>>>> Stashed changes
    }
}
