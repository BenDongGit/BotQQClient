using BotQQClient.Constants;
using BotQQClient.Helpers;
using BotQQClient.Models.Abstract;
using BotQQClient.Models.Utilities;
using RestSharp.Deserializers;
using SimpleJson;

namespace BotQQClient.Models
{
    public class Group : IMessageTarget, IClientExclusive
    {
        [DeserializeAs(Name = "gid")] public ulong Id { get; internal set; }

        [DeserializeAs(Name = "code")] public ulong PropertiesCode { get; internal set; }

        [DeserializeAs(Name = "name")] public string Name { get; internal set; }

        public BotQQClient Client { get; set; }

        public void Message(string content)
        {
            Client.RestClient.Post<MessageResponse>(Api.SendMessageToGroup,
                new JsonObject
                {
                    {"group_uin", Id},
                    {"content", new JsonArray {content, new JsonArray {@"font", Miscellaneous.Font}}.ToString()},
                    {"face", 573},
                    {"clientid", Miscellaneous.ClientId},
                    {"msg_id", Miscellaneous.MessageId},
                    {"psessionid", Client.Session.Tokens.PsessionId}
                });
        }
    }
}
