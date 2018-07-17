using System;
using BotQQClient.Models.Abstract;

namespace BotQQClient.Models.Utilities
{
    public class ConsolePrinter : IAuthStatusPrinter
    {
        public void Print(string authMessage)
        {
            Console.WriteLine(authMessage);
        }
    }
}
