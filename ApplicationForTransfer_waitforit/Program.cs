using ApplicationForTransfer_Lib;
using System;
using System.Threading;


namespace ApplicationForTransfer_waitforit
{
    class Program
    {
        static void Main(string[] args)
        {
            var broadcaster = new Broadcaster();
            broadcaster.MessageReceived += Message_Received;
            broadcaster.Listen();
            while (true) 
            {
                Thread.Sleep(1000);
            }
        }
        static void Message_Received(object sender, BroadcastPayload payload) 
        {
            Console.WriteLine($"{payload.Message} - { payload.Client}");
        }
    }
}
