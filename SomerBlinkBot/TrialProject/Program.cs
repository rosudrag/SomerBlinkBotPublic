using System;
using System.Threading.Tasks;
using SomerBlinkPromoBot.BotStateMachine;

namespace SomerBlinkPromoBot
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Task.Factory.StartNew(SomerBlinkStateMachine.Run, TaskCreationOptions.LongRunning);

            Console.ReadLine();
            Console.WriteLine("Preparing to cancel...");
            Console.ReadLine();
        }
    }
}
