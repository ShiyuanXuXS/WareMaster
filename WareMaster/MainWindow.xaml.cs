using MahApps.Metro.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WareMaster
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeComponent();
            Globals.wareMasterEntities = new WareMasterEntities();
        }

        private void InventoryInit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryInit inventoryInit = new InventoryInit();
                inventoryInit.Owner = this;
                inventoryInit.ShowDialog();
            }catch (Exception ex) { MessageBox.Show(ex.Message); };
            
        }

      

        private void items_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ItemsManagementDialog dialog = new ItemsManagementDialog();
                dialog.Owner = this;
                dialog.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void InventoryInbound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryChange inventoryChange = new InventoryChange("Inbound");
                inventoryChange.Owner = this;
                inventoryChange.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }

        private void InventoryOutbound_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                InventoryChange inventoryChange = new InventoryChange("Outbound");
                inventoryChange.Owner = this;
                inventoryChange.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); };
        }
    }
}
