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
    /// Interaction logic for InventoryQuery.xaml
    /// </summary>
    public partial class InventoryQuery : Window
    {
        public InventoryQuery()
        {
            InitializeComponent();
            DateBegin.SelectedDate = DateTime.Now;
            DateEnd.SelectedDate = DateTime.Now;
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(QueryFor.Text + "/" + GroupBy.Text + "/" + txtInputName.Text + "/"+(txtInputName.Tag==null).ToString());
            if (QueryFor.Text == "Inventory")
            {
                if (GroupBy.Text == "Category")
                {
                    if (txtInputName.Tag==null)
                    {
                        DataGridResult.ItemsSource = Inventory.GetAllInventoriesByCategory((DateTime)DateBegin.SelectedDate);
                    }
                    else
                    {
                        //DataGridResult.ItemsSource = Inventory.GetInventoryByCategory(txtInputName.Tag,(DateTime)DateBegin.SelectedDate);
                    }
                }
                else
                {
                    if (txtInputName.Tag == null)
                    {
                        DataGridResult.ItemsSource = Inventory.GetAllInventoriesByItem((DateTime)DateBegin.SelectedDate);
                    }
                    else
                    {
                        //DataGridResult.ItemsSource = Inventory.GetInventoryByItem(txtInputName.Tag,(DateTime)DateBegin.SelectedDate);
                    }
                }
            }
        }

        private void QueryFor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblTo!=null && DateEnd!=null)
            {
                lblTo.Visibility = (QueryFor.SelectedIndex ==0) ? Visibility.Hidden: Visibility.Visible;
                DateEnd.Visibility = (QueryFor.SelectedIndex == 0) ? Visibility.Hidden : Visibility.Visible;
            }
            
        }
    }
}
