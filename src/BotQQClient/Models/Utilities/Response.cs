using System.Collections.Generic;
using RestSharp.Deserializers;

namespace BotQQClient.Models.Utilities
{
    internal class Response
    {
        [DeserializeAs(Name = "retcode")]
        internal int? Code { get; set; }
    }

    internal class MessageResponse : Response
    {
    }

    internal class PollingResponse : Response
    {
        [DeserializeAs(Name = "result")]
        public List<MessageWrapper> MessageList { get; set; } = new List<MessageWrapper>();

        public class MessageWrapper
        {
            [DeserializeAs(Name = "poll_type")]
            public string Type { get; internal set; }

            [DeserializeAs(Name = "value")] public Wrapper Data { get; set; }

            public class Wrapper
            {
                [DeserializeAs(Name = "send_uin")]
                public ulong SenderIdGroupDiscussion
                {
                    set => SenderId = value;
                }

                [DeserializeAs(Name = "time")] public ulong? Timestamp { get; internal set; }

                [DeserializeAs(Name = "content")] public List<string> Content { get; internal set; }

                [DeserializeAs(Name = "from_uin")] public ulong? SenderId { get; internal set; }

                [DeserializeAs(Name = "group_code")] public ulong? SourceId { get; internal set; }

                [DeserializeAs(Name = "send_uin")] public ulong? SenderUin { get; internal set; }

                [DeserializeAs(Name = "to_uin")] public ulong? GroupUin { get; internal set; }
            }
        }
    }

    internal class VfwebqqResponse : Response
    {
        [DeserializeAs(Name = "result")] public Wrapper Result { get; set; }

        public class Wrapper
        {
            [DeserializeAs(Name = "vfwebqq")] public string Vfwebqq { get; set; }
        }
    }

    internal class UinPsessionidResponse : Response
    {
        [DeserializeAs(Name = "result")] public Wrapper Result { get; set; }

        public class Wrapper
        {
            [DeserializeAs(Name = "uin")] public ulong Uin { get; set; }

            [DeserializeAs(Name = "psessionid")] public string Psessionid { get; set; }
        }
    }

    internal class GroupsResponse : Response
    {
        [DeserializeAs(Name = "result")] public Wrapper Result { get; set; }

        public class Wrapper
        {
            [DeserializeAs(Name = "gnamelist")] public List<Group> GroupList { get; set; }
        }
    }
}

