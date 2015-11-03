using System;
using System.Linq;
using System.Web.Http;
using Thinktecture.IdentityModel.Hawk.Core;
using Thinktecture.IdentityModel.Hawk.Core.Helpers;
using Thinktecture.IdentityModel.Hawk.Core.MessageContracts;
using Thinktecture.IdentityModel.Hawk.WebApi;

namespace RoboBraille.WebApi.Models
{
    /// <summary>
    /// currently not used
    /// </summary>
    public class HawkAuthenticator
    {
        public static void EnableHawkAuthentication(HttpConfiguration configuration)
        {
            var options = new Options
            {
                ClockSkewSeconds = 60,
                LocalTimeOffsetMillis = 0,
                CredentialsCallback = OnCredentialsCallback,
                ResponsePayloadHashabilityCallback = r => true,
                VerificationCallback = OnVerificationCallback
            };

            var handler = new HawkAuthenticationHandler(options);
            configuration.MessageHandlers.Add(handler);
        }

        private static bool OnVerificationCallback(IRequestMessage request, string ext)
        {
            if (string.IsNullOrEmpty(ext))
            {
                return true;
            }

            const string name = "X-Request-Header-To-Protect";
            return ext.Equals(name + ":" + request.Headers[name].First());
        }

        private static Credential OnCredentialsCallback(string id)
        {
            Guid userId;
            if (!Guid.TryParse(id, out userId))
                return null;

            using (var context = new RoboBrailleDataContext())
            {
                var user = context.ServiceUsers.FirstOrDefault(e => e.UserId.Equals(userId));
                if (user != null)
                {
                    return new Credential()
                    {
                        Id = id,
                        User = user.UserName,
                        Algorithm = SupportedAlgorithms.SHA256,
                        Key = user.ApiKey
                    };
                }
            }

            return null;
        }
    }
}