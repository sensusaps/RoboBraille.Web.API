using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Http;
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.WebApi;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class HawkAuthenticator
    {
        public static void EnableHawkAuthentication(HttpConfiguration config)
        {
            var options = new Options
            {
                ClockSkewSeconds = 60,
                LocalTimeOffsetMillis = 0,
                CredentialsCallback = CredentialsCallback,
                ResponsePayloadHashabilityCallback = r => true,
                VerificationCallback = OnVerificationCallback
            };

            var handler = new HawkAuthenticationHandler(options);
            config.MessageHandlers.Add(handler);
        }

        private static bool OnVerificationCallback(IRequestMessage request, string ext)
        {
            if (string.IsNullOrEmpty(ext))
            {
                return true;
            }

            const string Name = "X-Request-Header-To-Protect";
            return true;// ext.Equals(Name + ":" + request.Headers[Name].First());
        }

        private static Credential CredentialsCallback(string id)
        {
            string dbid = "d2b97532-e8c5-e411-8270-f0def103cfd0";
            string userName = "TestUser";
            byte[] dbKey = Encoding.UTF8.GetBytes("7b76ae41-def3-e411-8030-0c8bfd2336cd");
            try
            {
                Guid guid = Guid.Parse(id);
                using (var context = new RoboBrailleDataContext())
                {
                    var user = context.ServiceUsers.FirstOrDefault(e => e.UserId.Equals(guid));
                    if (user != null)
                    {
                        if (user.ToDate >= DateTime.UtcNow || user.ToDate.Equals(user.FromDate))
                        {
                            dbid = user.UserId.ToString().ToLower().Trim();
                            userName = user.UserName.Trim();
                            dbKey = user.ApiKey;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            Credential credential = new Credential()
                {
                    Id = dbid,
                    Algorithm = SupportedAlgorithms.SHA256,
                    User = userName,
                    Key = dbKey
                };
            return credential;
        }

    }
}