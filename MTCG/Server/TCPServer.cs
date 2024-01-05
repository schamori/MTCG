
using System.Net;
using System.Text;
using System.Net.Sockets;
using MTCG.Interface;
using MTCG.Models;

namespace MTCG.Server
{
    public class TCPServer 
    {
        private IPEndPoint _ipEndPoint;
        const int BUFFERSIZE = 1024;
        private readonly IHttpService _httpService;
        public TCPServer(int port, IHttpService httpService)
        {
            IPAddress ipAddress = IPAddress.Loopback;
            _ipEndPoint = new IPEndPoint(ipAddress, port);
            _httpService = httpService;
        }
        public async Task Listen()
        {
            
            using Socket listener = new(
                _ipEndPoint.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            listener.Bind(_ipEndPoint);
            listener.Listen(100);

            Console.WriteLine($"Server started on {_ipEndPoint.Address}:{_ipEndPoint.Port}");
            Socket handler;
            while (true)

            {
                handler = await listener.AcceptAsync();
                _ = Task.Run(() => AcceptClient(handler));
            }

        }
        private async Task AcceptClient(Socket handler)
        {
            var buffer = new byte[BUFFERSIZE];
            StringBuilder completeMessage = new StringBuilder();
            do
            {
                int size = await handler.ReceiveAsync(buffer, SocketFlags.None);
                completeMessage.Append(Encoding.UTF8.GetString(buffer, 0, size));
            } while (handler.Available > 0);



            HttpRequest? httpRequest = _httpService.Parse(completeMessage.ToString());
            Console.WriteLine(completeMessage.ToString());
            string response = _httpService.Route(httpRequest);
            Console.WriteLine("Response" + response);

            byte[] responseBytes = Encoding.UTF8.GetBytes(response);

            await handler.SendAsync(responseBytes, SocketFlags.None);
            handler.Shutdown(SocketShutdown.Both);
        }
    }


}

