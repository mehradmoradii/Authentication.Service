using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Command.Handler.Helpers
{
    public static class LoginHelper
    {
        public static string? GenerateDisplayName(string? UserAgent)
        {
            if (UserAgent == null)
                return null;
            var uaParser = UAParser.Parser.GetDefault();
            var userAgentInfo = uaParser.Parse(UserAgent);

            var os = userAgentInfo.OS.Family;           // "Windows", "iOS", "Android", "Mac OS"
            var osVersion = userAgentInfo.OS.Major;     // "10", "16", "13"
            var browser = userAgentInfo.UA.Family;      // "Chrome", "Safari", "Edge"

            var displayName = $"{os} {osVersion} - {browser}";
            return displayName ;
        }
    }
}
