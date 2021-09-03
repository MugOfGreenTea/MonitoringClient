using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringClient
{
    class Client
    {
        TcpClient Tcpclient;
        string ip;
        int port;

        public Client(string IP, int Port) { ip = IP; port = Port; }

        public string message(int id, string[] message)
        {
            string AutoStr = id.ToString() + ";";
            string responseData;

            //try
            //{
            TcpClient client = new TcpClient(ip, port);

            foreach (string item in message)
            {
                AutoStr += item + ";";
            }

            // Переводим наше сообщение в ASCII, а затем в массив Byte.
            byte[] data = System.Text.Encoding.UTF8.GetBytes(AutoStr);

            // Получаем поток для чтения и записи данных.
            NetworkStream stream = client.GetStream();

            // Отправляем сообщение нашему серверу. 
            stream.Write(data, 0, data.Length);
            //Console.WriteLine("Sent: {0}", AutoStr);

            // Получаем ответ от сервера.

            // Буфер для хранения принятого массива bytes.
            data = new Byte[4096];

            // Строка для хранения полученных ASCII данных.
            responseData = string.Empty;

            // Читаем первый пакет ответа сервера. 
            // Можно читать всё сообщение.
            // Для этого надо организовать чтение в цикле как на сервере.
            int bytes = stream.Read(data, 0, data.Length);
            responseData = Encoding.UTF8.GetString(data, 0, bytes);
            //Console.WriteLine("Received: {0}", responseData);

            // Закрываем всё.
            stream.Close();
            client.Close();

            return responseData;
        }
    }
}
