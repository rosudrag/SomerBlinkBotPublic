using System;
using System.Linq;
using Core.Logging;
using ServiceStack;
using System.Threading.Tasks;
using SomerBlinkPromoBot.BotStateMachine;
using SomerBlinkSpecific.Blink;

namespace AreThereAnyPromosRESTAPI
{
    public static class Program
    {
        /// <summary>
        /// Gets the current promo.
        /// </summary>
        /// <value>
        /// The current promo.
        /// </value>
        private static Blink CurrentPromo
        {
            get { return SomerBlinkStateMachine.CurrentPromo; }
        }

        [Route("/promorunning")]
        private class PromoRunning
        {
        }


        /// <summary>
        /// PromoResponse json
        /// </summary>
        private class PromoResponse
        {
            public bool Result { get; set; }
            public string Name { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="PromoResponse"/> class.
            /// </summary>
            public PromoResponse()
            {
                Result = CurrentPromo != null;
                if (CurrentPromo != null)
                {
                    Name = CurrentPromo.Name;
                }
            }
        }


        private class PromoService : Service
        {
            public object Any(PromoRunning x)
            {
                return new PromoResponse();
            }
        }


        //Define the Web Services AppHost
        private class AppHost : AppSelfHostBase
        {
            public AppHost()
                : base("HttpListener Self-Host", typeof(PromoService).Assembly) { }

            public override void Configure(Funq.Container container) { }
        }


        
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            var listeningOn = args.Length == 0 ? "http://*:1337/" : args[0];
            var appHost = new AppHost()
                            .Init()
                            .Start(listeningOn);

            Logger.LogMessage("AppHost Created at {0}, listening on {1}", 
            DateTime.Now, listeningOn);

            Task.Factory.StartNew(SomerBlinkStateMachine.RunPromoCheck, TaskCreationOptions.LongRunning);

            Console.ReadLine();
            Console.Write("Preparing to cancel...");
            Console.ReadLine();
        }
    }
}
