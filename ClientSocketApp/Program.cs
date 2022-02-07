using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientSocketApp
    {
    class Program
{

        static void Main(string[] args)
        {
            Console.Write("Connecting to Server...");

            //Server
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint remoteServer = new IPEndPoint(ipAddress, 10888);

            //Create a TCP/IP socket.
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                client.Connect(remoteServer);
                Console.WriteLine("Connected");

                byte[] msg = Encoding.ASCII.GetBytes("This is message from client\r\n");
                //Send the data through the socket.
                client.Send(msg);
                Console.WriteLine("Sent: This is message from client");

                msg = Encoding.ASCII.GetBytes("Second Message\r\n");
                client.Send(msg);
                Console.WriteLine("Sent: Second message");

                //Send to exit
                msg = Encoding.ASCII.GetBytes("Exit\r\n");

                //Delay one Second
                Thread.Sleep(1000);

                client.Shutdown(SocketShutdown.Both);
                client.Close();

                Console.WriteLine("Close socket");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
}

}
