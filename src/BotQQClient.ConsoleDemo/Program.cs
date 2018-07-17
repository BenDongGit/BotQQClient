using System;
using System.Linq;
using BotBackendService;
using BotQQClient.Models.Utilities;

namespace BotQQClient.ConsoleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new BotQQClient();
            var botBackendClient = new BotBackendClient();
            client.QrLogin(new ConsolePrinter());

            while (true)
            {
                try
                {
                    var messages = client.Poll().ToList();

                    var catchedMsgs = messages.Where(msg => !string.IsNullOrEmpty(msg.Content) &&
                                                 msg.Content.Contains("@CDN") &&
                                                     msg.Type == Models.Message.SourceType.Group)
                                                        .ToList();
                    if (catchedMsgs.Any())
                    {
                        catchedMsgs.GroupBy(msg => msg.SourceId)
                            .ToList()
                            .ForEach(msgGroup =>
                        {
                            var group = client.Groups.FirstOrDefault(grp => grp.Value.Id == msgGroup.Key);
                            var contents = msgGroup.Select(m => m.Content).ToList();

                            botBackendClient.StartBotConversation(contents,retMsg => group.Value.Message(retMsg))
                                            .Wait();
                        });
                    }
                }
                catch (ApiException ex) when (ex.Code == 100100)
                {
                    Console.Write("*");
                }
            }
        }
    }
}
