using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FirstWorkerService
{
    internal class SocketServerWorkerService : IHostedService, IDisposable
    {
        private ManualResetEvent allDone = new ManualResetEvent(false);
        private bool _isRunning;
        private Thread _thread;
        private readonly ILogger<SocketServerWorkerService> _logger;

        public SocketServerWorkerService(ILogger<SocketServerWorkerService> logger)
        {
            _logger = logger;
        }
        public void Dispose()
        {

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            ThreadStart start = new ThreadStart(StartListening);

            _thread = new Thread(start);
            _thread.Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _isRunning = false;
            allDone.Set(); //signal to main thread
            try
            {
                _thread.Join(500);
            }
            catch (Exception)
            {

            }

            return Task.CompletedTask;

        }

        public void StartListening()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = host.AddressList[0];
            IPEndPoint local = new IPEndPoint(ipAddress, 10888);

            //Create a TCP/IP socket
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(local);
                listener.Listen(10); // max pooling 10

                _logger.LogInformation("Server started. Waiting incoming client");

                while (_isRunning)
                {
                    //Set the event to nonsignaled state.
                    allDone.Reset();

                    //Waiting incoming client
                    if (_isRunning)
                    {
                        listener.BeginAccept(
                            new AsyncCallback(AcceptCallBack),
                            listener);
                    }
                    //Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception) { }
        }

        public void AcceptCallBack(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            //Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            _logger.LogInformation("A client was connected");

            //Create a thread to serve data communication
            var t = new Thread(() => Perform(handler));
            t.Start();
        }
        public void Perform(object obj)
        {
            string data = "";
            byte[] bytes = new byte[1024];

            Socket client = (Socket)obj;
            while (_isRunning)
            {
                int len = client.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, len);

                int index = data.IndexOf("\r\n");
                string line;
                if(index > -1)
                {
                    //Print to logger
                    line = data.Substring(0, index);
                    _logger.LogInformation("Recv: {str}", line);

                    //New data
                    data = data.Substring(index + 2);

                    //Exit if client sends "Exit"
                    if (line.Contains("Exit"))
                        break;

                }
            }
            client.Close();
        }
    }
}
