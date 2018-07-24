using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BotQQClient.Models.Utilities;

namespace BotQQClient.Constants
{
    /// <summary>
    /// The Smart QQ API <see cref="http://www.scienjus.com/webqq-analysis-2/"/>
    /// </summary>
    internal static class Api
    {
        public static readonly ApiUrl GetQrCode = new ApiUrl("https://ssl.ptlogin2.qq.com/ptqrshow?appid=501004106&e=0&l=M&s=5&d=72&v=4&t=0.1", "");

        public static readonly ApiUrl VerifyQrCode = new ApiUrl(
           "https://ssl.ptlogin2.qq.com/ptqrlogin?ptqrtoken={0}&webqq_type=10&remember_uin=1&login2qq=1&aid=501004106&u1=https%3A%2F%2Fw.qq.com%2Fproxy.html%3Flogin2qq%3D1%26webqq_type%3D10&ptredirect=0&ptlang=2052&daid=164&from_ui=1&pttype=1&dumy=&fp=loginerroralert&0-0-157510&mibao_css=m_webqq&t=undefined&g=1&js_type=0&js_ver=10184&login_sig=&pt_randsalt=3",
           "https://ui.ptlogin2.qq.com/cgi-bin/login?daid=164&target=self&style=16&mibao_css=m_webqq&appid=501004106&enable_qlogin=0&no_verifyimg=1&s_url=https%3A%2F%2Fw.qq.com%2Fproxy.html&f_url=loginerroralert&strong_login=1&login_state=10&t=20131024001");

        public static readonly ApiUrl GetPtwebqq = new ApiUrl("{0}", null);

        public static readonly ApiUrl GetVfwebqq = new ApiUrl(
            "https://s.web2.qq.com/api/getvfwebqq?ptwebqq={0}&clientid=53999199&psessionid=&t=0.1",
            "https://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");

        public static readonly ApiUrl GetUinAndPsessionid = new ApiUrl(
            "https://d1.web2.qq.com/channel/login2",
            "https://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");

        public static readonly ApiUrl GetGroupList = new ApiUrl(
            "https://s.web2.qq.com/api/get_group_name_list_mask2",
            "https://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");

        public static readonly ApiUrl PollMessage = new ApiUrl(
            "https://d1.web2.qq.com/channel/poll2",
            "https://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");

        public static readonly ApiUrl SendMessageToGroup = new ApiUrl(
            "https://d1.web2.qq.com/channel/send_qun_msg2",
            "https://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");

        public static readonly ApiUrl GetGroupInfo = new ApiUrl(
            "https://s.web2.qq.com/api/get_group_info_ext2?gcode={0}&vfwebqq={1}&t=0.1",
            "https://s.web2.qq.com/proxy.html?v=20130916001&callback=1&id=1");

        public static readonly ApiUrl GetFriendStatus = new ApiUrl(
           "https://d1.web2.qq.com/channel/get_online_buddies2?vfwebqq={0}&clientid=53999199&psessionid={1}&t=0.1",
           "https://d1.web2.qq.com/proxy.html?v=20151105001&callback=1&id=2");

        /// <summary>
        /// The response after QR verified looks like ptuiCB ('66','0','','0','http://ptlogin4.web2.qq.com/check_sig?xxxxxx','')
        /// Use this regex to match the URL part.
        /// </summary>
        public static readonly Regex GetPtwebqqPattern = new Regex(@"http(s?)[^']+?(?=')");
    }
}
