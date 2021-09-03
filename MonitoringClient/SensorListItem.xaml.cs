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

namespace MonitoringClient
{
    /// <summary>
    /// Логика взаимодействия для SensorListItem.xaml
    /// </summary>
    public partial class SensorListItem : UserControl
    {
        public string NameItem, MaxItem, MinItem, ValueItem;
        public Monitoring monitoring;

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            monitoring.ComboBoxChoiseSensors.Text = NameItem;
        }

        public SensorListItem(string name, string max, string min, string value, Monitoring monitor)
        {
            NameItem = name;
            MaxItem = max;
            MinItem = min;
            ValueItem = value;
            monitoring = monitor;

            InitializeComponent();

            NameLabel.Content = NameItem;
            MaxLabel.Content = MaxItem;
            MinLabel.Content = MinItem;
            ValueLabel.Content = ValueItem;
        }
    }
}
