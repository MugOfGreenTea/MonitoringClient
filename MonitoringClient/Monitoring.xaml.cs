using System;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows.Threading;
using OxyPlot.Wpf;
using OxyPlot.Annotations;
using System.Windows.Media;
using System.Net.Sockets;

namespace MonitoringClient
{
    public partial class Monitoring : Window
    {
        //Request request = new Request("1;PID_1_1_a;PIT_1_1_b;PIT_1_2_a;"), ListSensorAnswer;

        public Request Request_ListSensors;
        Client client;
        double LastValue;
        OxyPlot.Series.LineSeries line;
        public DispatcherTimer timer1;
        string LastValueMessage, ListValueMessage, LastValueListSensorsMessage, ListSensorsMessage;
        DateTime Working_Hours;
        bool SolveTimer;

        Settings manager = new Settings(AppDomain.CurrentDomain.BaseDirectory + "MonitoringSetting.ini");
        string ip_server, port_server, UserName;
        List<SensorListItem> SensorListItem_List;

        public Monitoring(Request request, string login)
        {
            this.InitializeComponent();

            Request_ListSensors = request;
            UserName = login;

            Title += UserName;

            SetSettings();
            client = new Client(ip_server, Convert.ToInt32(port_server));
            SensorListItem_List = new List<SensorListItem>();

            try
            {
                ComboBoxSensor();
                FirstListSensor();
            }
            catch (SocketException)
            {
                MessageBox.Show("Кажется что-то не так с сервером. Пожалуйста переподключите соединение или обратитесь к администратору.", "Упс!");
                timer1.Stop();
                EllipseConnention.Fill = new SolidColorBrush(Color.FromRgb(230, 0, 0));
            }
            catch
            {

            }

            //FirstListSensor();
            //SensorListItem_List = ItemsList();

        }

        private void MenuItem_add_Click(object sender, RoutedEventArgs e)
        {
            Monitoring mw = new Monitoring(Request_ListSensors, UserName);
            mw.Show();
        }

        private void MenuItem_choice_user_Click(object sender, RoutedEventArgs e)
        {
            Autorization A = new Autorization();
            A.Show();

            foreach (Window w in App.Current.Windows)
            {
                if (w is Monitoring)
                    w.Close();
            }
        }

        private void MenuItem_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void ComboBoxChoiseSensors_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (timer1 != null && timer1.IsEnabled == true)
                timer1.Stop();

            LineDrawing();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (timer1 != null && timer1.IsEnabled == true)
                timer1.Stop();
        }

        private void MenuItem_settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsAutorization SA = new SettingsAutorization();
            SA.ShowDialog();
        }

        private void MenuItem_generate_report_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport GR = new GenerateReport(ip_server, Convert.ToInt32(port_server), Request_ListSensors);
            GR.Show();
        }

        private void ManualInputButton_Click(object sender, RoutedEventArgs e)
        {
            ManualInput MI = new ManualInput(ip_server, port_server, Request_ListSensors, UserName);
            MI.ShowDialog();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (timer1 != null && timer1.IsEnabled == true)
                    timer1.Stop();

                LineDrawing();
                FirstListSensor();

                EllipseConnention.Fill = new SolidColorBrush(Color.FromRgb(0, 240, 30));
            }
            catch(SocketException)
            {
                MessageBox.Show("Кажется что-то не так с сервером. Пожалуйста переподключите соединение или обратитесь к администратору.", "Упс!");
                timer1.Stop();
                EllipseConnention.Fill = new SolidColorBrush(Color.FromRgb(230, 0, 0));
            }
            catch
            {

            }
        }


        private void ComboBoxSensor()
        {
            foreach (string reader in Request_ListSensors.data)
            {
                ComboBoxChoiseSensors.Items.Add(reader);
            }

            ComboBoxChoiseSensors.Text = (string)ComboBoxChoiseSensors.Items.GetItemAt(0);
        }

        public void LineDrawing()
        {
            timer1 = null;
            timer1 = new DispatcherTimer();

            line = new OxyPlot.Series.LineSeries() { };

            line.Points.Clear();

            FirstRequest();
            if (timer1 != null && timer1.IsEnabled == false && SolveTimer)
                StartTimer();
            else
                MessageBox.Show("Нет значений.");
        }

        private void FirstListSensor()
        {
            ListBoxChoiseSensors.Items.Clear();

            for (int i = 0; i < Request_ListSensors.data.Length; i++)
            {
                ListSensorsMessage = client.message(4, new string[1] { ComboBoxChoiseSensors.Items[i].ToString() });

                Request request_ListSensorsMessage = new Request();
                request_ListSensorsMessage.SensorListItemData(ListSensorsMessage);

                SensorListItem sensorListItem = new SensorListItem(request_ListSensorsMessage.name_sensor,
                    request_ListSensorsMessage.maxvalue.ToString(), request_ListSensorsMessage.minvalue.ToString(),
                    request_ListSensorsMessage.value + " " + request_ListSensorsMessage.unit, this);
                ListBoxChoiseSensors.Items.Add(sensorListItem);
                SensorListItem_List.Add(sensorListItem);
            }
        }

        public void FirstRequest()
        {
            FormLineSeries.DataContext = null;

            ListValueMessage = client.message(2, new string[1] { ComboBoxChoiseSensors.SelectedItem.ToString() });
            Request request_ListValueMessage = new Request();
            request_ListValueMessage.SensorData(ListValueMessage);

            if (request_ListValueMessage.data[0] != "no_value")
            {
                double j = 0;
                for (int i = request_ListValueMessage.data.Length - 1; i >= 0; i--)
                {
                    line.Points.Add(new DataPoint(j, Convert.ToDouble(request_ListValueMessage.data[i])));
                    j++;
                    Debug.WriteLine(j + " " + Convert.ToDouble(request_ListValueMessage.data[i]));
                }

                LabelNameFalicity.Content = request_ListValueMessage.facilities;
                LabelNameSensor.Content = ComboBoxChoiseSensors.SelectedItem.ToString();
                LabelValue.Content = line.Points[line.Points.Count - 1].Y;
                LabelValueMax.Content = request_ListValueMessage.maxvalue;
                LabelValueMin.Content = request_ListValueMessage.minvalue;
                LabelDate.Content = DateTime.Today.ToString("D");
                LabelTime.Content = DateTime.Now.ToLongTimeString();

                Working_Hours = Working_Hours.AddSeconds(1);
                LabelTimeSeanse.Content = Working_Hours.ToString("mm:ss");

                OxyPlot.Wpf.LineAnnotation annotationMax = new OxyPlot.Wpf.LineAnnotation()
                {
                    StrokeThickness = 1,
                    Type = LineAnnotationType.Horizontal,
                    X = 0.0,
                    Y = (double)request_ListValueMessage.maxvalue
                };

                OxyPlot.Wpf.LineAnnotation annotationMin = new OxyPlot.Wpf.LineAnnotation()
                {
                    StrokeThickness = 1,
                    Type = LineAnnotationType.Horizontal,
                    X = 0.0,
                    Y = (double)request_ListValueMessage.minvalue
                };

                Plot1.Annotations.Add(annotationMax);

                FormLineSeries.DataContext = line;

                SolveTimer = true;
            }
            else
            {
                SolveTimer = false;
            }
        }

        public void StartTimer()
        {
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += ValueRequest;
            timer1.Start();
        }

        private void NotificationsPanel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NotificationsPanel.Visibility = Visibility.Hidden;
        }

        public void ValueRequest(object sender, EventArgs e)
        {
            ValueRequestGraphic();
            ValueRequestListSensors();
        }

        private void ValueRequestGraphic()
        {
            try
            {
                LastValueMessage = client.message(3, new string[1] { ComboBoxChoiseSensors.SelectedItem.ToString() });

                Request request = new Request(LastValueMessage);
                LastValue = Convert.ToDouble(request.data[0]);

                line.Points.Add(new DataPoint(line.Points.Count + 1, LastValue));
                if (line.Points.Count >= 60)
                    line.Points.RemoveAt(0);

                double j = 0;
                for (int i = 0; i < line.Points.Count - 1; i++)
                {
                    line.Points[i] = (new DataPoint(i, line.Points[i].Y));
                    j++;
                }

                LabelDate.Content = DateTime.Today.ToString("D");
                LabelTime.Content = DateTime.Now.ToLongTimeString();
                LabelValue.Content = LastValue;

                Working_Hours = Working_Hours.AddSeconds(1);
                LabelTimeSeanse.Content = Working_Hours.ToString("mm:ss");
                Plot1.InvalidatePlot(true);
            }
            catch (SocketException)
            {
                timer1.Stop();
                return;
            }
            catch
            {

            }
        }

        private void ValueRequestListSensors()
        {
            try
            {
                LastValueListSensorsMessage = client.message(5, Request_ListSensors.data);

                Request request_LastValueListSensorsMessage = new Request(LastValueListSensorsMessage);

                for (int i = 0; i < request_LastValueListSensorsMessage.data.Length; i++)
                {
                    SensorListItem_List[i].ValueItem = request_LastValueListSensorsMessage.data[i];
                    SensorListItem_List[i].ValueLabel.Content = SensorListItem_List[i].ValueItem;
                    Debug.WriteLine(SensorListItem_List[i].NameItem + "  " + SensorListItem_List[i].ValueItem);
                }
            }
            catch (SocketException)
            {
                MessageBox.Show("Кажется что-то не так с сервером. Пожалуйста переподключите соединение или обратитесь к администратору.", "Упс!");
                timer1.Stop();
                EllipseConnention.Fill = new SolidColorBrush(Color.FromRgb(230, 0, 0));
                return;
            }
            catch
            {

            }

        }

        private List<SensorListItem> ItemsList()
        {
            List<SensorListItem> itemsList = new List<SensorListItem>();
            for (int i = 0; i < ListBoxChoiseSensors.Items.Count; i++)
            {
                if (ListBoxChoiseSensors.Items[i] is SensorListItem)
                {
                    itemsList.Add((SensorListItem)ListBoxChoiseSensors.Items[i]);
                }
            }
            return itemsList;
        }

        public void SetSettings()
        {
            ip_server = manager.GetPrivateString("SERVER", "ip_server");
            port_server = manager.GetPrivateString("SERVER", "port");
        }
    }
}