using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Group = BotQQClient.Models.Group;

namespace BotQQClient.ConsoleDemo
{
    internal static class MockBotClient
    {
        public static readonly Regex GetChineseDatePattern = new Regex(@"\d{0,1}月\d{1,2}号");

        private static readonly List<Tuple<string, string, double>> Bandwidths = new List<Tuple<string, string, double>>
        {
            new Tuple<string, string, double>("test.com", "7月20号", 18.34),
            new Tuple<string, string, double>("test.com", "6月3号", 15.16),
            new Tuple<string, string, double>("test2.com", "7月20号", 29.31)
        };

        private static readonly string BandwidthResponseFormat = @"猜测您查询的是{0}域名{1}的峰值带宽，为{2} GB";
        private static readonly string StatusCodeResponse = @"抱歉，QQ暂不支持状态码查询，可使用AzureCDN助手小程序进行查询 ";
        private static readonly string DomainNotExistResponse = @"抱歉，您查询的域名不在系统内 ";
        private static readonly string UnknownMessageResponse = @"对不起，无法解读您的信息 ";
        private static readonly string ICMResponse = @"猜测您有紧急情况需要处理，QQ暂不支持工单填写，我们将立刻直接电联支持人员 ";

        internal static void StartBotConversation(List<string> messages, Group group)
        {
            messages.ForEach(msg =>
            {
                Thread.Sleep(1000); //Take some time to think.
                StartBotConversation(msg, group);
            });
        }

        internal static void StartBotConversation(string message, Group group)
        {
            StringBuilder response = new StringBuilder();

            if (message.Contains("带宽"))
            {
                var date = GetChineseDatePattern.Match(message).Value;
                if (string.IsNullOrEmpty(date))
                {
                    response.Append(@"抱歉，请检查是否提供了正确的日期");
                }
                else
                {
                    if (Bandwidths.Any(x => message.Contains(x.Item1)))
                    {
                        var bandwidths = Bandwidths.Where(x => message.Contains(x.Item1)).ToList();
                        var bandwidth = bandwidths.FirstOrDefault(x => x.Item2 == date);
                        if (bandwidth != null)
                        {
                            response.AppendFormat(BandwidthResponseFormat, bandwidth.Item1, bandwidth.Item2, bandwidth.Item3);
                        }
                        else
                        {
                            response.Append(@"抱歉，没有查询到当日的峰值带宽数据");
                        }
                    }
                    else
                    {
                        response.Append(DomainNotExistResponse);
                    }
                }

            }
            else if (message.Contains("状态码"))
            {
                response.Append(StatusCodeResponse);
            }
            else if (message.Contains("报警") || message.Contains("异常") || message.Contains("错误率偏高"))
            {
                response.Append(ICMResponse);
            }
            else if (message.Contains("谢谢"))
            {
                response.Append("You're very welcome");
            }
            else 
            {
                response.Append(UnknownMessageResponse);
            }
            

            group.Message(response.ToString());
        }
    }
}
