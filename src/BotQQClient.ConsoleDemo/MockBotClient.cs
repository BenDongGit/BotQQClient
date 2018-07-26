using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Group = BotQQClient.Models.Group;

namespace BotQQClient.ConsoleDemo
{
    internal static class MockBotClient
    {
        private static readonly Regex GetChineseDatePattern = new Regex(@"\d{0,1}月\d{0,2}号{0,1}日{0,1}");

        private static readonly List<string> MockDomainList = new List<string>
        {
            "a.com",
            "b.com",
            "c.com",
            "d.com",
            "e.com"
        };

        private static readonly string AllBandwidthResponseFormat = @"为您查询到所有域名在{0}的峰值带宽，为{1}Gbps。";
        private static readonly string DomainBandwidthResponseFormat = @"为您查询到{0}在{1}的峰值带宽，为{2}Gbps。";
        private static readonly string AllFlowResponseFormat = @"为您查询到所有域名在{0}的流量，为{1}PB。";
        private static readonly string DomainFlowResponseFormat = @"为您查询到{0}在{1}的流量，为{2}PB。";
        private static readonly string StatusCodeResponse = @"非常抱歉，QQ CDN助手暂不支持状态码查询。";
        private static readonly string DomainNotExistResponse = @"非常抱歉，未检测到您提供的域名。";
        private static readonly string UnknownMessageResponse = @"暂时无法解析您的问题。";
        private static readonly string ICMResponse = @"猜测您有紧急情况需要处理，我们将立即联系支持人员。";
        private static readonly string HelpResponse = 
            @"您可以@我并输入:
                 1) 查询xxx(所有)域名xx月xx号(流量)带宽数据
                 2) 域名xxx正在持续报警 
                 3）查询域名列表
              我们将为您进行相关操作
             (目前QQ助手暂不支持状态码查询以及工单填写，如有相关需求请关注AzureCDN助手微信小程序)";

        internal static void StartBotConversation(List<string> messages, Group group)
        {
            messages.ForEach(msg =>
            {
                ConversationWithQQMessage(msg, group);
            });
        }

        private static void ConversationWithQQMessage(string message, Group group)
        {
            StringBuilder response = new StringBuilder();
            if (message.Contains("带宽") || message.Contains("流量"))
            {
                var date = GetChineseDatePattern.Match(message).Value;
                if (string.IsNullOrEmpty(date))
                {
                    response.Append("请确认您提供了查询日期以及域名，如xxx(所有)域名xx月xx号带宽(流量)数据");
                }
                else
                {
                    if (message.Contains("全部") || message.Contains("所有"))
                    {
                        if (MockDomainList.Any(x => message.Contains(x)))
                        {
                            response.Append("非常抱歉，我不清楚您打算要查询指定域名还是全部域名,请再次确认");
                        }
                        else
                        {
                            if (message.Contains("带宽"))
                            {
                                response.AppendFormat(
                                AllBandwidthResponseFormat,
                                date,
                                GetRandomBandwidthOrFlow(300, 500)
                                .ToString());
                            }

                            if (message.Contains("流量"))
                            {
                                response.AppendFormat(
                                AllFlowResponseFormat,
                                date,
                                GetRandomBandwidthOrFlow(30, 50)
                                .ToString());
                            }
                        }
                    }
                    else if(MockDomainList.Any(x => message.Contains(x)))
                    {
                        var domains = string.Join(";",
                            MockDomainList
                              .Where(x => message.Contains(x))
                                  .ToList());

                        if (!string.IsNullOrEmpty(domains))
                        {
                            if (message.Contains("带宽"))
                            {
                                response.AppendFormat(
                                    DomainBandwidthResponseFormat, 
                                    domains, 
                                    date, 
                                    GetRandomBandwidthOrFlow(20, 100).ToString());
                            }

                            if (message.Contains("流量"))
                            {
                                response.AppendFormat(
                                    DomainFlowResponseFormat,
                                    domains,
                                    date,
                                    GetRandomBandwidthOrFlow(2, 10).ToString());
                            }
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
            else if (message.Contains("报警") ||
                     message.Contains("异常") ||
                     message.Contains("错误率偏高"))
            {
                response.Append(ICMResponse);
            }
            else if (message.Contains("谢谢"))
            {
                response.Append("非常感谢您对CDN助手的支持！");
            }
            else if (message.Contains("帮助") || message.Contains("你好"))
            {
                response.Append("您好，非常感谢您使用CDN QQ助手。").Append(HelpResponse);
            }
            else if (message.Contains("域名列表"))
            {
                response.AppendFormat("域名列表为：{0}", string.Join(";", MockDomainList));
            }
            else
            {
                response.Append(UnknownMessageResponse).Append(HelpResponse);
            }

            group.Message(response.ToString());
        }

        private static double GetRandomBandwidthOrFlow(double minimum, double maximum)
        {
            Random random = new Random();
            return Math.Round(random.NextDouble() * (maximum - minimum) + minimum, 2);
        }
    }
}
