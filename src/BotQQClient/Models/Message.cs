using BotQQClient.Models.Abstract;

namespace BotQQClient.Models
{
    public class Message : IClientExclusive
    {
        public enum SourceType
        {
            Friend,
            Group,
            Discussion
        }

        public SourceType Type { get; internal set; }

        public ulong? Timestamp { get; internal set; }

        public string Content { get; internal set; }

        public ulong? SenderId { get; internal set; }

        public ulong? SourceId { get; internal set; }

        public BotQQClient Client { get; set; }
    }
}
