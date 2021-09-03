using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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

namespace MonitoringClient
{
    /// <summary>
    /// Логика взаимодействия для GenerateReport.xaml
    /// </summary>
    public partial class GenerateReport : Window
    {
        public Request Request_ListSensors;
        IList SelectedSensorsList;
        string IP_server, ArrayReport;
        int Port;
        string StartDate, FinalDate;
        string[] RequestArray;

        public GenerateReport(string ip_server, int port, Request request)
        {
            InitializeComponent();

            Request_ListSensors = request;
            IP_server = ip_server;
            Port = port;

            SelectedSensorsList = new List<string>();
            ListBoxSensor();
            //AppDomain.CurrentDomain.BaseDirectory + "template_report.xlsx";
        }

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartDatePicker.Text != null && StartDatePicker.Text != ""
                && FinalDatePicker.Text != null && FinalDatePicker.Text != ""
                && SensorsListBox.SelectedItems != null && SensorsListBox.SelectedIndex != -1)
            {
                if (StartDatePicker.SelectedDate < FinalDatePicker.SelectedDate)
                {
                    StartDate = Convert.ToDateTime(StartDatePicker.Text).ToString("yyyy-MM-dd");
                    FinalDate = Convert.ToDateTime(FinalDatePicker.Text).ToString("yyyy-MM-dd");
                    SelectedSensorsList = SensorsListBox.SelectedItems;

                    RequestArray = new string[SelectedSensorsList.Count + 2];
                    RequestArray[0] = StartDate + "%";
                    RequestArray[1] = FinalDate + "%";

                    for (int i = 2; i < SelectedSensorsList.Count + 2; i++)
                    {
                        RequestArray[i] = (string)SelectedSensorsList[i - 2];
                    }

                    try
                    {
                        Client client = new Client(IP_server, Port);
                        ArrayReport = client.message(6, RequestArray);
                        Request request_Report = new Request();
                        request_Report.ReportDataTable(ArrayReport);

                        SaveFileDialog dlg = new SaveFileDialog();
                        dlg.Filter = "Файлы Excel|*.xlsx;*.xls;*.xlsm";
                        dlg.FileName = "Отчет о нарушениях за период ( " + StartDatePicker.Text + " - " + FinalDatePicker.Text + " )";
                        dlg.Title = "Сохранить";
                        if (dlg.ShowDialog() == true)
                        {
                            string outputFile = dlg.FileName;

                            File.Copy(AppDomain.CurrentDomain.BaseDirectory + "template_report.xlsx", outputFile);

                            var workbook = new XLWorkbook(outputFile);
                            var worksheet = workbook.Worksheet(1);

                            worksheet.Cell("A1").Value += "( " + StartDatePicker.Text + " - " + FinalDatePicker.Text + " )";

                            for (int i = 0; i < request_Report.report_table.Rows.Count; i++)
                            {
                                worksheet.Cell("A" + (i + 3)).Value = request_Report.report_table.Rows[i][0];

                                worksheet.Cell("A" + (i + 3)).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                                worksheet.Cell("A" + (i + 3)).Style.Border.LeftBorderColor = XLColor.Black;

                                worksheet.Cell("B" + (i + 3)).Value = request_Report.report_table.Rows[i][1];
                                worksheet.Cell("C" + (i + 3)).Value = request_Report.report_table.Rows[i][2];
                                worksheet.Cell("D" + (i + 3)).Value = request_Report.report_table.Rows[i][3];
                                worksheet.Cell("E" + (i + 3)).Value = request_Report.report_table.Rows[i][4];
                                worksheet.Cell("F" + (i + 3)).Value = request_Report.report_table.Rows[i][5];

                                worksheet.Cell("F" + (i + 3)).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                                worksheet.Cell("F" + (i + 3)).Style.Border.RightBorderColor = XLColor.Black;
                            }

                            worksheet.Cell("A" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            worksheet.Cell("A" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorderColor = XLColor.Black;
                            worksheet.Cell("B" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            worksheet.Cell("B" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorderColor = XLColor.Black;
                            worksheet.Cell("C" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            worksheet.Cell("C" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorderColor = XLColor.Black;
                            worksheet.Cell("D" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            worksheet.Cell("D" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorderColor = XLColor.Black;
                            worksheet.Cell("E" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            worksheet.Cell("E" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorderColor = XLColor.Black;
                            worksheet.Cell("F" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                            worksheet.Cell("F" + (request_Report.report_table.Rows.Count + 2)).Style.Border.BottomBorderColor = XLColor.Black;

                            workbook.SaveAs(outputFile);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка формирования отчета. Пожалуйста, попробуйте снова или обратитесь к администратору.", "Ошибка");
                    }
                }
                else
                {
                    MessageBox.Show("Начальная дата не может быть позже конечной.", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Не все поля заполнены.", "Ошибка");
            }
        }

        private void ListBoxSensor()
        {
            foreach (string reader in Request_ListSensors.data)
            {
                SensorsListBox.Items.Add(reader);
            }
        }
    }
}