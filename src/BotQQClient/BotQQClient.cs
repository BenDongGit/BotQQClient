using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using BotQQClient.Constants;
using BotQQClient.Helpers;
using BotQQClient.Models;
using BotQQClient.Models.Abstract;
using BotQQClient.Models.Utilities;
using RestSharp;
using RestSharp.Deserializers;
using SimpleJson;

/// <summary>
/// The bot client
/// </summary>
namespace BotQQClient
{
    public class BotQQClient
    {
        private static Session session;

        public BotQQClient()
        {
            RestClient.AddHandler("text/plain", new JsonDeserializer());
        }

        public RestClient RestClient { get; } =
            new RestClient { UserAgent = Miscellaneous.DefaultUserAgent, CookieContainer = new CookieContainer() };

        public Session Session
        {
            get { return session; }
            private set
            {
                session = value;
                RestClient.CookieContainer = value.Cookies;
                RestClient.Get(Api.GetFriendStatus, value.Tokens.Vfwebqq, value.Tokens.PsessionId);
            }
        }

        public Dictionary<ulong, Group> Groups { get; private set; }

        public IEnumerable<Message> Poll()
        {
            var response =
                RestClient.Post<PollingResponse>(Api.PollMessage, new JsonObject
                {
                    {@"ptwebqq", Session.Tokens.Ptwebqq},
                    {@"clientid", Miscellaneous.ClientId},
                    {@"psessionid", Session.Tokens.PsessionId},
                    {@"key", @""}
                });

            return response.Data.MessageList.Select(message => new Message
            {
                Client = this,
                Type = message.Type.ConvertToMessageType(),
                Content = string.Join(string.Empty, message.Data.Content.Skip(1)),
                SenderId = message.Data.SenderId,
                SourceId = message.Data.SourceId,
                Timestamp = message.Data.Timestamp
            });
        }

        public void QrLogin(IAuthStatusPrinter statusPrinter, int maxAttemps = 10)
        {
            Session ReadSession()
            {
                using (var stream = new FileStream(Miscellaneous.BotQQTempPath, FileMode.Open))
                {
                    return (Session)new BinaryFormatter().Deserialize(stream);
                }
            }

            Session QrAuthenticate()
            {
                var session = BotQQClient.QrAuthenticate((byte[] bytes) =>
                {
                    File.WriteAllBytes(@"qrcode", bytes);
                    Process.Start(@"qrcode");
                    statusPrinter.Print("Waiting for authentication...");
                }, maxAttemps);

                using (var stream = new FileStream(Miscellaneous.BotQQTempPath, FileMode.OpenOrCreate))
                {
                    new BinaryFormatter().Serialize(stream, session);
                }

                return session;
            }

            try
            {
                Session = File.Exists(Miscellaneous.BotQQTempPath) ? ReadSession() : QrAuthenticate();
                //Session = QrAuthenticate();
                Groups = GetGroupsList(Session);
                Groups.ToList().ForEach(gp => gp.Value.Client = this);
            }
            catch (Exception ex)
            {
                QrAuthenticate();
            }

            Console.WriteLine("Logged in!");
        }

        private static Session QrAuthenticate(Action<byte[]> imageCallback, int maxAttemps)
        {
            var client = new RestClient
            {
                UserAgent = Miscellaneous.DefaultUserAgent,
                CookieContainer = new CookieContainer()
            };

            client.AddHandler(@"text/plain", new JsonDeserializer());

            var qrResponse = client.Get(Api.GetQrCode);
            var qrsig = qrResponse.Cookies.First(x => x.Name == @"qrsig").Value;
            var ptqrtoken = Hash33(qrsig);

            imageCallback(qrResponse.RawBytes);

            string ptwebqqUrl, ptwebqq;
            while (true)
            {
                Thread.Sleep(1000);
                var message = client.Get(Api.VerifyQrCode, ptqrtoken);

                if (message.Content.Contains("已失效"))
                {
                    throw new TimeoutException("QR authentication timed out!");
                }

                if (!message.Content.Contains("成功"))
                {
                    continue;
                }

                ptwebqqUrl = Api.GetPtwebqqPattern.Match(message.Content).Value;
                ptwebqq = message.Cookies.First(x => x.Name == "ptwebqq").Value;
                break;
            }

            client.Get(Api.GetPtwebqq, ptwebqqUrl);

            VfwebqqResponse vfwebqqResponse = default(VfwebqqResponse);
            while (maxAttemps > 0)
            {
                var response = client.Get<VfwebqqResponse>(Api.GetVfwebqq, ptwebqq);
                if (response.IsSuccessful)
                {
                    vfwebqqResponse = response.Data;
                    break;
                }

                maxAttemps--;
            }

            if (vfwebqqResponse == null)
            {
                throw new HttpRequestException("QR authentication unsuccessful: maximum attemps reatched getting vfwebqq");
            }

            var vfwebqq = vfwebqqResponse.Result.Vfwebqq;
            var uinPsessionidResponse = client.Post<UinPsessionidResponse>(Api.GetUinAndPsessionid,
                new JsonObject
                {
                    {@"ptwebqq", ptwebqq },
                    {@"clientid", Miscellaneous.ClientId },
                    {@"psessionid", @"" },
                    {@"status", @"online" }
                });

            var uin = uinPsessionidResponse.Data.Result.Uin;
            var psessionid = uinPsessionidResponse.Data.Result.Psessionid;

            return new Session(new Tokens(ptwebqq, vfwebqq, uin, psessionid), client.CookieContainer);
        }

        private Dictionary<ulong, Group> GetGroupsList(Session session)
        {
            var hash = Hash((long)Session.Tokens.UIN, Session.Tokens.Ptwebqq);

            var groupsResponse = RestClient.Post<GroupsResponse>(Api.GetGroupList, new JsonObject
                {
                    {@"vfwebqq", session.Tokens.Vfwebqq},
                    {@"hash", hash}
                }).Data.Result;

            return groupsResponse.GroupList.ToDictionary(x => x.Id);
        }

        private static int Hash33(string s)
        {
            var e = 0;
            foreach (var t in s)
            {
                e += (e << 5) + t;
            }

            return int.MaxValue & e;
        }

        private static string Hash(long uin, string ptwebqq)
        {
            var n = new int[4];
            for (var T = 0; T < ptwebqq.Length; T++)
                n[T % 4] ^= ptwebqq[T];
            string[] u = { @"EC", @"OK" };
            var v = new long[4];
            v[0] = ((uin >> 24) & 255) ^ u[0][0];
            v[1] = ((uin >> 16) & 255) ^ u[0][1];
            v[2] = ((uin >> 8) & 255) ^ u[1][0];
            v[3] = (uin & 255) ^ u[1][1];

            var u1 = new long[8];

            for (var t = 0; t < 8; t++)
                u1[t] = t % 2 == 0 ? n[t >> 1] : v[t >> 1];

            string[] n1 = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
            var v1 = "";
            foreach (var aU1 in u1)
            {
                v1 += n1[(int)((aU1 >> 4) & 15)];
                v1 += n1[(int)(aU1 & 15)];
            }

            return v1;
        }
    }
}
