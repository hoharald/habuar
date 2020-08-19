using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace netcoreclient
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 500; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(ClientThread));
                thread.Start(i);
            }

            Console.WriteLine("Hit any key to exit");
            Console.ReadKey();
        }

        static void ClientThread(object i)
        {            
            var c = new UdpClient();

            //c.Client.Bind(new IPEndPoint(IPAddress.Any));
            var msg = $"NET CORE Device {(int)i}";
            var bytes = Encoding.UTF8.GetBytes(msg);
            c.BeginSend(bytes, bytes.Length, "255.255.255.255", 8888, new AsyncCallback(SendCallback), (Id: (int)i, Client: c));

            Console.WriteLine($"Client {i} started");            
        }

        static void SendCallback(IAsyncResult ar)
        {
            var c = ((int Id, UdpClient Client)) ar.AsyncState;               
            c.Client.EndSend(ar);
            Thread.Sleep(500);
            var msg = $"NET CORE Device {c.Id}";
            var bytes = Encoding.UTF8.GetBytes(msg);
            c.Client.BeginSend(bytes, bytes.Length, "255.255.255.255", 8888, new AsyncCallback(SendCallback), (c.Id, c.Client));
        }
    }
}
