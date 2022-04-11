using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Foundation;

namespace CallKitSample.iOS.Twilio
{
    public class TwilioService
    {
        public static string identity = "sudhir";
        //public static string accessToken = string.Empty;
        public static string url = "http://f5aba9130569.ngrok.io/";

        static TwilioVoiceHelper helper;
        static TwilioService()
        {
            helper = new TwilioVoiceHelper();
        }

        public static async Task Register(NSData deviceToken)
        {
            var accessToken = await GetAccessToken();
            helper.Register(accessToken, deviceToken);
        }

        public static void Setnotification(PushKit.PKPushPayload payload)
        {
            if (payload == null)
            {
                LoggerService.Log("Info", "TwilioService.cs->SetNotification:: Payload is null");
                return;
            }
            if (helper == null)
            {
                LoggerService.Log("Info", "TwilioService.cs->SetNotification:: Helper is null");
                return;
            }
            helper.SetNotificationPayload(payload);
        }

        public static void MuteCall(bool isMuted)
        {
            if (helper.Call != null)
                helper.MuteAudio(isMuted);
        }

        public static void AnswerIncomingCall()
        {
            helper.AcceptCallInvite();
        }

        public static async void ConnectCall(string to)
        {
            var accessToken = await GetAccessToken(to);
            var args = new Dictionary<string, string>();
            args.Add("to", to);
            TwilioVoiceHelper.activeCallUuid = new NSUuid();
            AppDelegate.Instance.CallManager.StartCall(args["to"], TwilioVoiceHelper.activeCallUuid);

            var nsArgs = NSDictionary<NSString, NSString>
               .FromObjectsAndKeys(args.Values.ToArray(), args.Keys.ToArray());

            helper.MakeCall(accessToken, nsArgs);
        }

        public static  void EndCall()
        {
            if(helper.Call!=null)
                helper.Call.Disconnect();
        }

        public static void DeclineCallInvite()
        {
            helper.RejectCallInvite();
        }

        private static async Task<string> GetAccessToken(string to="")
        {
            var client = new HttpClient();
            var twilioTokenUrl = $"{url}accessToken";

            if(!string.IsNullOrWhiteSpace(to))
            {
                twilioTokenUrl = $"{twilioTokenUrl}?identity={to}";
            }
            else if (!string.IsNullOrWhiteSpace(identity))
            {
                twilioTokenUrl = $"{twilioTokenUrl}?identity={identity}";
            }
            var response = await client.GetAsync(twilioTokenUrl);
            var result = await response.Content.ReadAsStringAsync();
            var accessToken = result;
            Console.WriteLine(result);
            return accessToken;
        }
    }
}
