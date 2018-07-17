using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using BotQQClient.Models.Utilities;
using System.Text.RegularExpressions;
using System.Net.Http;
using SimpleJson;
using BotQQClient.Models;

namespace BotQQClient.Helpers
{
    internal static class ExtensionMethods
    {
        #region Rest Client Extensions

        private static readonly Regex UrlPattern = new Regex(@"^(?<domain>https?://.*?)/(?<path>.*)$");
        private static readonly string[] ContentTypeInsepctionTargets = { @"json", @"text/plain" };

        internal static IRestResponse<Response> Get(this RestClient client, ApiUrl api, params object[] parameters)
        {
            return client.Get<Response>(api, parameters);
        }

        internal static IRestResponse<T> Get<T>(this RestClient client, ApiUrl api, params object[] parameters)
            where T : Response, new()
        {
            var domainPath = LookupDomainPath(string.Format(api.Url, parameters));

            var request = new RestRequest(domainPath.Item2).AddHeader("Connection", "Keep-Alive");
            if (api.Referer != null) request.AddHeader("Referer", api.Referer);

            client.BaseUrl = new Uri(domainPath.Item1);

            return client.Get<T>(request).Inspect();
        }

        internal static IRestResponse<Response> Post(this RestClient client, ApiUrl api, JsonObject json)
        {
            return client.Post<Response>(api, json);
        }

        internal static IRestResponse<T> Post<T>(this RestClient client, ApiUrl api, JsonObject json)
            where T : Response, new()
        {
            var domainPath = LookupDomainPath(string.Format(api.Url));
            client.BaseUrl = new Uri(domainPath.Item1);

            var request = new RestRequest(domainPath.Item2)
                    .AddHeader(@"Referer", api.Referer)
                        .AddHeader(@"Origin", api.Url.Substring(0, api.Url.LastIndexOf('/')))
                            .AddParameter(@"r", json, ParameterType.GetOrPost);

            return client.Post<T>(request).Inspect();
        }

        #endregion

        internal static Message.SourceType ConvertToMessageType(this string pollType)
        {
            var type = default(Message.SourceType);

            switch (pollType)
            {
                case @"message":
                    type = Message.SourceType.Friend;
                    break;
                case @"group_message":
                    type = Message.SourceType.Group;
                    break;
                case @"discu_message":
                    type = Message.SourceType.Discussion;
                    break;
                default:
                    break;
            }

            return type;
        }

        private static IRestResponse<T> Inspect<T>(this IRestResponse<T> response) where T : Response
        {
            if (!ContentTypeInsepctionTargets.Any(x => response.ContentType.Contains(x))) return response;

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException(
                   $"HTTP request unsuccessful: status code {response.StatusCode}. See inner exception (if exists) for details.",
                   response.ErrorException);
            }

            if ((response.Data?.Code ?? 0) != 0)
            {
                throw new ApiException($"Request unsuccessful: returned {response.Data?.Code}", response.Data?.Code,
                    response.ErrorException);
            }

            return response;
        }

        private static Tuple<string, string> LookupDomainPath(string url)
        {
            var match = UrlPattern.Match(url);

            string domain = match.Groups["domain"].Value;
            string path = match.Groups["path"].Value;

            return new Tuple<string, string>(domain, path);
        }
    }
}
