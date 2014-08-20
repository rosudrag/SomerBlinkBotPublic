using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using Core.Extensions;
using Core.Logging;
using OpenQA.Selenium;
using SomerBlinkSpecific.Blink;

namespace SomerBlinkSpecific.CogdevDotNet
{
    public class CogdevDotNet
    {
        public const string MainPage = @"http://cogdev.net/blink/";

        /// <summary>
        ///     Initializes a new instance of the <see cref="CogdevDotNet" /> class.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="account">The account.</param>
        /// <exception cref="System.ArgumentNullException">
        ///     webDriver
        ///     or
        ///     account
        /// </exception>
        public CogdevDotNet(IWebDriver webDriver, Account.Account account, string name)
        {
            if (webDriver == null)
            {
                throw new ArgumentNullException("webDriver");
            }
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            WebDriver = webDriver;
            Account = account;
            Name = name;
        }

        public IWebDriver WebDriver { get; set; }
        public Account.Account Account { get; set; }
        public string Name { get; set; }

        public IEnumerable<Blink.Blink> BlinkList { get; set; }

        /// <summary>
        ///     Logins the specified web driver.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        public void Login()
        {
            WebDriver.Url = MainPage;

            IWebElement signInLink = WebDriver.FindElements(By.TagName("a"))[1];

            signInLink.Click();

            IWebElement UsernameField = WebDriver.FindElement(By.Name("username"));
            IWebElement PasswordField = WebDriver.FindElement(By.Name("password"));

            UsernameField.SendKeys(Account.Username);
            PasswordField.SendKeys(Account.Password);

            IWebElement loginButton = WebDriver.FindElement(By.Name("login_me"));
            loginButton.Click();
        }

        /// <summary>
        ///     Reads the main meta data.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="account">The account.</param>
        public void ReadMainMetaDataIntoAccount()
        {
            WebDriver.Navigate().GoToUrl(MainPage);

            Logger.LogMessage("{0} reading metadata into account", Name);

            string iskString = WebDriver.FindElement(By.Id("isk_amount")).Text;
            string tokenString = WebDriver.FindElement(By.Id("promo_token_amount")).Text;

            Account.Isk = iskString.ConvertToIsk();
            Account.Tokens = tokenString.ConvertToToken();
        }

        /// <summary>
        ///     Ams the i logged in.
        /// </summary>
        /// <param name="webDriver">The web driver.</param>
        /// <param name="account">The account.</param>
        /// <returns></returns>
        private bool AmILoggedIn()
        {
            bool validLogin = WebDriver.PageSource.Contains(Account.Username);

            return validLogin;
        }

        /// <summary>
        ///     Reads the blinks and assigns them to a local BlinkList
        /// </summary>
        public void ReadBlinks()
        {
            Logger.LogMessage("{0} started reading blinks", Name);

            WebDriver.Url = MainPage;

            ReadOnlyCollection<IWebElement> formList = WebDriver.FindElements(By.ClassName("blink_form"));

            var blinkList = new List<Blink.Blink>();

            foreach (IWebElement form in formList)
            {
                ReadOnlyCollection<IWebElement> promos = form.FindElements(By.Id("promo_description"));

                blinkList.Add(promos.Any() ? CreatePromoBlink(form) : CreateBlink(form));
            }

            if (BlinkList == null)
            {
                BlinkList = blinkList;
            }
            else
            {
                BlinkList = blinkList.Where(x => x != null);
            }
        }

        /// <summary>
        /// Reads the promo.
        /// </summary>
        public bool IsRunningPromosActiveQuery()
        {
            Logger.LogMessage("{0} started searching for promo", Name);

            WebDriver.Url = MainPage;

            ReadOnlyCollection<IWebElement> formList = WebDriver.FindElements(By.ClassName("blink_form"));

            ReadOnlyCollection<IWebElement> promos = formList[0].FindElements(By.Id("promo_description"));

            if (promos.Any())
                return true;
            return false;
        }

        /// <summary>
        ///     Creates the blink.
        /// </summary>
        /// <param name="form">The form.</param>
        private Blink.Blink CreateBlink(IWebElement form)
        {
            try
            {
                IWebElement blinkPrizeDescription = form.FindElement(By.ClassName("blink_prize_image"));

                ReadOnlyCollection<IWebElement> blinkNameContainer =
                    blinkPrizeDescription.FindElements(By.TagName("strong"));
                string blinkName = blinkNameContainer[0].Text;

                IWebElement blinkBidIskString = blinkPrizeDescription.FindElement(By.ClassName("blinkme"));
                long blinkBidIsk = blinkBidIskString.Text.ConvertToIsk();

                ReadOnlyCollection<IWebElement> buttons = form.FindElements(By.Name("ticket"));
                List<IWebElement> peopleThatBid =
                    form.FindElements(By.TagName("strong")).Except(new List<IWebElement>(blinkNameContainer)).ToList();

                int totalSpots = buttons.Count + peopleThatBid.Count;

                BlinkType type;

                switch (totalSpots)
                {
                    case 8:
                        type = BlinkType.Normal;
                        break;
                    case 16:
                        type = BlinkType.Mega;
                        break;
                    case 11:
                        type = BlinkType.Roulette;
                        break;
                    default:
                        type = BlinkType.Other;
                        break;
                }

                return new Blink.Blink(1, blinkBidIsk, type, blinkName, buttons, peopleThatBid);
            }
            catch (Exception e)
            {
                //Add logging here
                return null;
            }
            
        }

        /// <summary>
        ///     Creates the promo blink.
        /// </summary>
        /// <param name="promoForm">The promo form.</param>
        /// <returns></returns>
        private Blink.Blink CreatePromoBlink(IWebElement promoForm)
        {
            string promoName = promoForm.FindElement(By.XPath("/html/body/div/div[6]/form/div/div[2]/div/h2")).Text;

            ReadOnlyCollection<IWebElement> buttons = promoForm.FindElements(By.Name("ticket"));

            ReadOnlyCollection<IWebElement> peopleThatBid = promoForm.FindElements(By.TagName("strong"));

            return new Blink.Blink(1, 1, BlinkType.Promo, promoName, buttons, peopleThatBid);
        }

        /// <summary>
        ///     Promoses the active.
        /// </summary>
        /// <returns></returns>
        public bool IsPromoActiveFromCacheList()
        {
            if (BlinkList == null || !BlinkList.Any())
            {
                return false;
            }

            IEnumerable<Blink.Blink> promos = BlinkList.Where(x => x.Type == BlinkType.Promo);

            return promos.Any();
        }

        /// <summary>
        ///     Bids the on blinks.
        /// </summary>
        /// <param name="minIsk">The minimum isk.</param>
        /// <param name="maxIsk">The maximum isk.</param>
        public void BidOnBlinks(long minIsk, long maxIsk)
        {
            if (BlinkList == null || !AmILoggedIn())
            {
                return;
            }

            Logger.LogMessage("{0} trying to bid on blinks", Name);


            Blink.Blink[] blinkToBidOn = BlinkList.Where(x => x.BidValue <= maxIsk &&
                                                              x.BidValue >= minIsk &&
                                                              Account.Isk >= x.BidValue &&
                                                              !x.PeopleThatBid.Select(y => y.Text)
                                                                  .Contains(Account.Username))
                .ToArray();

            if (blinkToBidOn.Any())
            {
                SubmitBid(blinkToBidOn[0]);
            }
            else
            {
                Logger.LogMessage("{0} no blinks to bid on", Name);
            }
            if (IsPromoActiveFromCacheList())
            {
                Blink.Blink promo = BlinkList.Where(x => x.Type == BlinkType.Promo).ToArray()[0];

                SubmitBid(promo);
            }
        }

        /// <summary>
        ///     Submits the bid.
        /// </summary>
        /// <param name="blink">The blink.</param>
        private void SubmitBid(Blink.Blink blink)
        {
            var randomiser = new Random();

            IWebElement[] buttons = blink.Buttons.ToArray();

            int buttonCount = buttons.Count();

            if (buttonCount <= 0)
            {
                Logger.LogMessage("{0} no buttons left on blink {1}", Name, blink.Name);
                return;
            }

            int randomisedButtonCount = randomiser.Next(buttonCount);

            Logger.LogMessage("{0} bidding on blink: {1} type {2}", Name, blink.Name, blink.Type);

            buttons[randomisedButtonCount].Click();
        }

        /// <summary>
        /// Gets the promos.
        /// </summary>
        /// <returns></returns>
        public Blink.Blink GetPromos()
        {
            try
            {
                return BlinkList.FirstOrDefault(x => x.Type == BlinkType.Promo);

            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}