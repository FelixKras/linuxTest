using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LinuxTest
{
    class Program
    {
        static readonly AutoResetEvent are = new AutoResetEvent(false);
        private static UdpClient udpSend;
        private static UdpClient udpReceive;

        static void Main(string[] args)
        {
            udpReceive = new UdpClient(new IPEndPoint(IPAddress.Loopback, 1000));
            udpSend = new UdpClient(new IPEndPoint(IPAddress.Loopback, 1010));

            Thread thrSend = new Thread(() =>
            {
                DateTime dt = DateTime.UtcNow;
                for (int i = 0; i <= 500; i++)
                {
                    TimeSpan delta = DateTime.UtcNow - dt;
                    dt = DateTime.UtcNow;
                    are.WaitOne(TimeSpan.FromMilliseconds(10));
                    string sMsg = delta.ToString() + " sent";
                    byte[] by1Dgram = Encoding.ASCII.GetBytes(sMsg);
                    udpSend.Send(by1Dgram, by1Dgram.Length, new IPEndPoint(IPAddress.Loopback, 1000));
                    Console.WriteLine("sent " + i);
                }
            }) {IsBackground = false, Priority = ThreadPriority.Highest};
            thrSend.Start();


            Thread thrReceive = new Thread(() =>
            {
                IPEndPoint remote = null;
                for (int i = 0; i <= 500; i++)
                {
                    are.WaitOne(TimeSpan.FromMilliseconds(10));
                    string sMsg = Encoding.ASCII.GetString(udpReceive.Receive(ref remote));
                    Console.WriteLine("received: " + sMsg);
                }
            }) {IsBackground = false, Priority = ThreadPriority.Highest};
            thrReceive.Start();

            thrSend.Join();
            thrReceive.Join();
            Console.ReadKey();
        }
    }
}
