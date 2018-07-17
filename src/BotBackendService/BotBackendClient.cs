using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;

namespace BotBackendService
{
    public class BotBackendClient
    {
        private static readonly string BotAppSecret = "2Xdli3fWpdI.cwA.enY.rfHBISkIuIYKfNtA5D5U5QEQ-Kr3U1cv87ymioAYSmM";
        private static readonly string BotAppId = "MS_China_CDN_Hackthon";
        private static readonly string ChannelName = "AzureChinaCDN";

        public async Task StartBotConversation(IEnumerable<string> inputMsgs, Action<string> activityCallback)
        {
            var client = new DirectLineClient(BotAppSecret);
            var conversation = await client.Conversations.StartConversationAsync();

            Console.WriteLine("Bot considering...");

            new Thread(
                async () =>
                {
                    await ReadBotMessagesAsync(
                        client,
                        conversation.ConversationId, 
                        activityCallback);

                }).Start();

            foreach (var msg in inputMsgs)
            {
                if (msg.Length > 0)
                {
                    Activity message = new Activity
                    {
                        From = new ChannelAccount(ChannelName),
                        Text = msg,
                        Type = ActivityTypes.Message
                    };

                    await client.Conversations.PostActivityAsync(conversation.ConversationId, message);
                }
            }
        }

        private static async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId, Action<string> activityCallback)
        {
            while (true)
            {
                var activitySet = await client
                    .Conversations.GetActivitiesAsync(conversationId);

                activitySet.Activities
                   .Where(act => act.From.Id == BotAppId)
                       .ToList()
                           .ForEach(act=> activityCallback(act.Text));
            }
        }
    }
}
