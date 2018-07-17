namespace BotQQClient.Models.Utilities
{
    internal class ApiUrl
    {
        internal ApiUrl(string url, string referer)
        {
            Url = url;
            Referer = referer;
        }

        internal string Url { get; private set; }

        internal string Referer { get; private set; }
    }
}
