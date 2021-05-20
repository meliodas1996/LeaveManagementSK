using DotNetOpenAuth.GoogleOAuth2;
using Microsoft.AspNet.Membership.OpenAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveManagementSystemValueCreed.App_Start
{
    public class AuthConfig
    {
        public static void RegisterAuth()
        {
            GoogleOAuth2Client clientGoog = new GoogleOAuth2Client("32215653599-rd2ji5a90fno2d8b5p2qfcboc4kfnvan.apps.googleusercontent.com", "xINBInKvU3s6GzS5oSNzZmPo");
            IDictionary<string, string> extraData = new Dictionary<string, string>();
            OpenAuth.AuthenticationClients.Add("google", () => clientGoog, extraData);
        }
    }
}