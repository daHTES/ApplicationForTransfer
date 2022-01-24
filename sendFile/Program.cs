using ApplicationForTransfer_Lib;

namespace sendFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostname = args[0];
            var file = args[1];

            var transferFile = new TransferFille(file, hostname);
            transferFile.Start();
        }
    }
}
