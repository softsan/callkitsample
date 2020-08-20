using System;
using CallKitSample.iOS.CallKits;
using CallKitSample.iOS.Twilio;
using CallKitSample.Services;
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(CallService))]
namespace CallKitSample.iOS.CallKits
{
    public class CallService : ICallService 
    {
        NSUuid uuid;
        public CallService()
        {
        }

        public void StartCall(string phoneNumber)
        {
            uuid = new NSUuid();
            TwilioVoiceHelper.activeCallUuid = uuid;
            AppDelegate.Instance.CallManager.StartCall(phoneNumber,uuid);
        }
        public void EndCall()
        {
            AppDelegate.Instance.CallManager.EndCall(TwilioVoiceHelper.activeCallUuid);
        }
    }
}
