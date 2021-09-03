using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient
{
    public class Request
    {
        public int id;
        public string[] data;
        public int maxvalue, minvalue;
        public string facilities, name_sensor, unit, value;
        public string[,] report_array;
        public DataTable report_table;


        public Request() { }

        public Request(string message)
        {
            string[] str_request = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string[] Data = new string[str_request.Length - 1];
            id = Convert.ToInt32(str_request[0]);

            for (int i = 1; i < str_request.Length; i++)
            {
                Data[i - 1] = str_request[i];
            }
            data = Data;
        }

        public void SensorData(string message)
        {
            string[] str_request = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            id = Convert.ToInt32(str_request[0]);
            facilities = str_request[1];
            unit = str_request[2];
            maxvalue = Convert.ToInt32(str_request[3]);
            minvalue = Convert.ToInt32(str_request[4]);

            string[] Data = new string[str_request.Length - 5];
            for (int i = 5, j = 0; i < str_request.Length; i++, j++)
            {
                Data[j] = str_request[i];
            }
            data = Data;
        }

        public void SensorListItemData(string message)
        {
            string[] str_request = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            id = Convert.ToInt32(str_request[0]);
            name_sensor = str_request[1];
            unit = str_request[2];
            maxvalue = Convert.ToInt32(str_request[3]);
            minvalue = Convert.ToInt32(str_request[4]);
            value = str_request[5];
        }

        public void ReportArray(string message)
        {
            string[] str_request = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            int n = 0;

            for (int i = 0; i < str_request.Length / 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    report_array[i, j] = str_request[n];
                    n++;
                }
            }
        }

        public void ReportDataTable(string message)
        {
            string[] str_request = message.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            int n = 0;

            report_table = new DataTable();
            report_table.Columns.Add("Date");
            report_table.Columns.Add("IdFacility");
            report_table.Columns.Add("NameFacility");
            report_table.Columns.Add("IdSensor");
            report_table.Columns.Add("NameSensor");
            report_table.Columns.Add("CountViolation");

            for (int i = 0; i < str_request.Length / 6; i++)
            {
                report_table.Rows.Add(str_request[n + 1], str_request[n + 2], str_request[n + 3], 
                    str_request[n + 4], str_request[n + 5], str_request[n + 6]);
                n += 6;
            }
        }
    }
}

