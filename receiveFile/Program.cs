using ApplicationForTransfer_Lib;
using System.Threading;

namespace receiveFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var receiveFile = new ReceiveFile(54000);
            receiveFile.Listen();
            while (true) 
            {
                Thread.Sleep(1000);
            }
        }
    }
}
