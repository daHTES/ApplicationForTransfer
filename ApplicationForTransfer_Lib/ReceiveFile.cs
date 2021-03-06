using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ApplicationForTransfer_Lib
{
    public class ReceiveFile
    {
        private bool _proccessHeader;
        private MemoryStream _ms;
        private byte[] _buffer = new byte[65555];

        private TcpListener _listener;
        private readonly int _port;

        private string _filename;
        private long _length;

        public event EventHandler TransferComplete;
        
        public ReceiveFile(int port)
        {
            _port = port;
        }

        public void Listen() 
        {
            var endPoint = new IPEndPoint(IPAddress.Any, _port);
            _listener = new TcpListener(endPoint);
            _listener.Start();
            _listener.BeginAcceptTcpClient(Client_Connected, _listener);
        }

        public void Stop() 
        {
            _listener.Stop();
            _listener = null;
        }

        private void Client_Connected(IAsyncResult result) 
        {
            if (result.IsCompleted) 
            {
                var listener = result.AsyncState as TcpListener;
                var client = listener.EndAcceptTcpClient(result);
                _ms = new MemoryStream();
                client.GetStream().BeginRead(_buffer, 0, _buffer.Length, Client_Received, client);
            }
        }

        private void Client_Received(IAsyncResult result) 
        {
            if (result.IsCompleted) 
            {
                var client = result.AsyncState as TcpClient;
                var bytesReceived = client.GetStream().EndRead(result);

                if (!_proccessHeader)
                {
                    var headerSize = GetHeaderSize(_buffer);
                    if (headerSize > 0)
                    {


                        var raw = Encoding.ASCII.GetString(_buffer, 0, headerSize);
                        var split = raw.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        var versionHeader = split[0];
                        _filename = split[1];
                        _length = long.Parse(split[2]);
                    }
                    _proccessHeader = true;

                    var lengthOfData = bytesReceived - headerSize;
                    _ms.Write(_buffer, headerSize + 1, lengthOfData);
                }
              else if (_ms.Length < _length) 
                {
                    _ms.Write(_buffer, 0, bytesReceived);
                }
                if (_ms.Length < _length)
                {
                    Array.Clear(_buffer, 0, _buffer.Length);
                    client.GetStream().BeginRead(_buffer, 0, _buffer.Length, Client_Received, client);
                }
                else 
                {
                    client.Close();
                    File.WriteAllBytes(_filename, _ms.ToArray());
                    _ms.Dispose();
                    _ms = null;
                    TransferComplete.Invoke(this, EventArgs.Empty);
                }

    
            }
        }

        private int GetHeaderSize(byte[] buffer) 
        {
            var length = -1;
            for (int i = 0; i < buffer.Length - 4; i++) 
            {
                char c1 = (char)buffer[i];
                char c2 = (char)buffer[i + 1];
                char c3 = (char)buffer[i + 2];
                char c4 = (char)buffer[i + 3];

                if (c1 == '\r' && c2 == '\n' && c3 == '\r' && c4 == '\n') 
                {
                    length = i + 3;
                    break;
                }
            }
            return length;
        }
    }
}
