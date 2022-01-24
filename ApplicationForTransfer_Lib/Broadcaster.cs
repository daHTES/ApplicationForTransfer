using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ApplicationForTransfer_Lib
{
    public class Broadcaster
    {
        public const string HELLO = nameof(HELLO); // Say hello
        public const string CONFIRM = nameof(CONFIRM); // Initi file transfer
        public const string ASKING = nameof(ASKING); // Asking hello
        public const string SND = nameof(SND); // Send Request
        public const string SOK = nameof(SOK); // Send ACKknowledge

        private readonly UdpClient _client;
        private readonly int _port;

        public EventHandler<BroadcastPayload> MessageReceived;

        public Broadcaster(int port = 54000) 
        {
            _port = port;
            _client = new UdpClient(_port);
        }

        public void SayHello() 
        {
            var HelloString = Encoding.ASCII.GetBytes(HELLO);
            _client.Send(HelloString, 
                HelloString.Length, 
                new IPEndPoint(IPAddress.Broadcast, _port));
        }

        public void Listen() 
        {
            _client.BeginReceive(ClientMessageReceived, _client);

        }

        public void Asknowledge(IPEndPoint client) 
        {
            _client.Send(Encoding.ASCII.GetBytes(ASKING), ASKING.Length, client);
        }

        public void SendFileRequest(IPEndPoint client, string hostAndUser, string filename) 
        {

            string trimmedFilename = Path.GetFileName(filename);


            string msg = $"{SND}\r\n{hostAndUser}\r\n{trimmedFilename}";
            _client.Send(Encoding.ASCII.GetBytes(msg), msg.Length, client);
        }

        public void SendFileAcknowledgement(IPEndPoint client, string filename) 
        {
            string msg = $"{SOK}\r\n{filename}";
            _client.Send(Encoding.ASCII.GetBytes(msg), msg.Length, client);
        }



        public void InitiatingTransfer(IPEndPoint client) 
        {
            _client.Send(Encoding.ASCII.GetBytes(CONFIRM), CONFIRM.Length, client);
        }

        private void ClientMessageReceived(IAsyncResult result) 
        {
            if (result.IsCompleted) 
            {
                var sender = new IPEndPoint(IPAddress.Any, 0);
                var client = result.AsyncState as UdpClient;
                var received = client.EndReceive(result, ref sender);

                if (received.Length > 0) 
                {
                    var msg = Encoding.ASCII.GetString(received);

                    var msgSplit = msg.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


                    switch (msgSplit[0]) 
                    {
                        case CONFIRM:
                            OnMessageReceived(BroadcastMessage.Confirm, sender);
                            break;
                        case ASKING:
                            OnMessageReceived(BroadcastMessage.HelloAsknowledge, sender);
                            break;
                        case SND:
                            OnMessageReceived(BroadcastMessage.SendRequest, sender, msgSplit[1]);
                            break;
                        case SOK:
                            OnMessageReceived(BroadcastMessage.SendAcknowledge, sender, msgSplit[1]);
                            break;
                        default:
                            OnMessageReceived(BroadcastMessage.Hello, sender);
                            break;
                    }
                }
                client.BeginReceive(ClientMessageReceived, client);
            }
        }

        private void OnMessageReceived(BroadcastMessage message, 
            IPEndPoint client, 
            string hostname = "", 
            string filename = "") 
        {
            MessageReceived.Invoke(this, new BroadcastPayload(message, client, hostname, filename));
        }
    }
}
