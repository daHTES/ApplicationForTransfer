using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ApplicationForTransfer_Lib
{
    public class TransferFille
    {
        private static readonly string VersionInfo = "FileTranfer 1.0";
        private TcpClient _client;
        private readonly string _hostName;
        private readonly string _fileName;
        private readonly int _port;

        public event EventHandler TransferComplete;

 ////////////////////////////////////////////////////////////////////////////////////
        public TransferFille(string hostName, string fileName, int port = 54000)
        {
            _hostName = hostName;
            _fileName = fileName;
            _port = port;
        }
////////////////////////////////////////////////////////////////////////////////////
        public void Start() 
        {
            _client = new TcpClient();
            _client.Connect(_hostName, _port);

             var buffer = CreateBuffer();

            _client.GetStream().BeginWrite(buffer, 0, buffer.Length, Write_Result, _client);
        }
////////////////////////////////////////////////////////////////////////////////////
        private void Write_Result(IAsyncResult result) 
        {
            if (result.IsCompleted) 
            {
                var client = result.AsyncState as TcpClient;
                client.GetStream().EndWrite(result);
                client.Close();
                TransferComplete?.Invoke(this, EventArgs.Empty);
            }
        }
////////////////////////////////////////////////////////////////////////////////////
        private byte[] CreateBuffer() 
        {
            byte[] buffer = null;
            using (MemoryStream ms = new MemoryStream()) 
            {
                FileInfo myFileInfo = new FileInfo(_fileName);
                WriteString(ms, $"{VersionInfo} 1.0\r\n");
                WriteString(ms, Path.GetFileName(_fileName)+ "\r\n");
                WriteString(ms, myFileInfo.ToString() + "\r\n\r\n");

                var fileContents = File.ReadAllBytes(_fileName);
                ms.Write(fileContents, 0, fileContents.Length);
                buffer = ms.ToArray();
            }
            return buffer;
        }
////////////////////////////////////////////////////////////////////////////////////
        private void WriteString(MemoryStream ms, string s) 
        {
            var bytes = Encoding.ASCII.GetBytes(s);
            ms.Write(bytes, 0, bytes.Length);
        }
    }
}
