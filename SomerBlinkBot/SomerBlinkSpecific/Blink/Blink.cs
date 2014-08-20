using System.Collections.Generic;
using OpenQA.Selenium;

namespace SomerBlinkSpecific.Blink
{
    public class Blink
    {
        public long BuyoutValue { get; set; }
        public long BidValue { get; set; }
        public BlinkType Type { get; set; }
        public string Name { get; set; }

        public IEnumerable<IWebElement> Buttons { get; set; } 
        public IEnumerable<IWebElement> PeopleThatBid { get; set; }

        public Blink(long buyoutValue, long bidValue, BlinkType type, string name, IEnumerable<IWebElement> buttons,
            IEnumerable<IWebElement> peopleThatBid)
        {
            BuyoutValue = buyoutValue;
            BidValue = bidValue;
            Type = type;
            Name = name;
            Buttons = buttons;
            PeopleThatBid = peopleThatBid;
        }
    }
}
