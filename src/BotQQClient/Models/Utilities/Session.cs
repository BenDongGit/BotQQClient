using System;
using System.Net;

namespace BotQQClient.Models.Utilities
{
    [Serializable]
    public class Session
    {
        internal Session(Tokens tokens, CookieContainer cookies)
        {
            Tokens = tokens;
            Cookies = cookies;
        }

        internal Tokens Tokens { get; private set; }

        internal CookieContainer Cookies { get; private set; }
    }

    [Serializable]
    public class Tokens
    {
        public Tokens(string ptwebqq, string vfwebqq, ulong uin, string psessionId)
        {
            Ptwebqq = ptwebqq;
            Vfwebqq = vfwebqq;
            UIN = uin;
            PsessionId = psessionId;
        }

        public string Ptwebqq { get; private set; }

        public string Vfwebqq { get; private set; }

        public ulong UIN { get; private set; }

        public string PsessionId { get; private set; }
    }
}
