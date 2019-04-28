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

namespace PersonalFinanceManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DatabaseInfo databaseInfo = new DatabaseInfo();
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            DateTime start = startPicker.SelectedDate.Value;
            DateTime end = endPicker.SelectedDate.Value;

            
            listBox.Visibility = Visibility.Visible;
            var summaries = DataCalculation.Calculate(start, end);           

            foreach (var summary in summaries)
            {
                listBox.Items.Add(summary.Day + "\t" + summary.Total);
            }
        }
    }
}
