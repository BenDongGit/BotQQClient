using System;
using System.Collections.Generic;
using System.Linq;
using BotBackendService;
using BotQQClient.Models;
using BotQQClient.Models.Utilities;
using Microsoft.Bot.Connector.DirectLine;

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
                                                 msg.Content.Contains("@Azure CDN助手") &&
                                                     msg.Type == Message.SourceType.Group)
                                                        .ToList();
                    if (catchedMsgs.Any())
                    {
                        catchedMsgs.GroupBy(msg => msg.SenderId)
                            .ToList()
                            .ForEach(msgGroup =>
                        {
                            var group = client.Groups.FirstOrDefault(grp => grp.Value.Id == msgGroup.Key);
                            var contents = msgGroup.Select(m => m.Content).ToList();
                            MockBotClient.StartBotConversation(contents, group.Value);
                            //botBackendClient.StartBotConversation(contents, act => BotActivityHandler(act, group.Value))
                            //                .Wait();
                        });
                    }
                }
                catch (ApiException ex) when (ex.Code == 100100)
                {
                    Console.Write("*");
                }
            }
        }

        static void BotActivityHandler(Activity act, Group qqGroup)
        {
            //qqGroup.Message(act.Text);
            throw new NotImplementedException();
        } 
    }
}
