namespace Core.Common
{
    public class UserSettings
    {
        public long MaxBlinkBidIsk;
        public string proxyIp;
        public string proxyPort;
        public string proxyUser;
        public string proxyPass;
        public bool useProxy;
        public int proxyType;
        public string Username { get; set; }

        public string Password { get; set; }
        public int MinWaitTime { get; set; }
        public int MaxWaitTime { get; set; }
        public long MinBlinkBidIsk { get; set; }
        public int DownTime { get; set; }
        public int RunTime { get; set; }
        public int DebugMode { get; set; }
    }
}
