using System;
using CallKitSample.CallKits;
using CallKitSample.iOS.Twilio;
using CoreFoundation;
using Foundation;
using PushKit;
using Twilio.Voice.iOS;
using UIKit;

namespace CallKitSample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IPKPushRegistryDelegate
    {
        public static AppDelegate Instance;
        public ActiveCallManager CallManager { get; set; }
        public ProviderDelegate CallProviderDelegate { get; set; }
        public DirectoryDelegate CallDirectoryDelegate { get; set; }
        string deviceToken;

       
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Instance = this;
            
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            //OneSignalNotification();
            RegisterVoip();
            CallManager = new ActiveCallManager();
            CallProviderDelegate = new ProviderDelegate(CallManager);
            CallDirectoryDelegate = new DirectoryDelegate();
            var audio = TVODefaultAudioDevice.AudioDevice();
            TwilioVoice.AudioDevice = audio;
            return base.FinishedLaunching(app, options);
        }
        
        public async void DidUpdatePushCredentials(PKPushRegistry registry, PKPushCredentials credentials, string type)
        {
            if (credentials != null && credentials.Token != null)
            {
                var fullToken = credentials.Token.DebugDescription;
                deviceToken = fullToken.Trim('<').Trim('>').Replace(" ", string.Empty);
                Console.WriteLine("Token is " + deviceToken);
                await TwilioService.Register(deviceToken);
            }
        }

        public void DidReceiveIncomingPush(PKPushRegistry registry, PKPushPayload payload, string type)
        {
            Console.WriteLine("My push is coming!");
            // var aps = payload.DictionaryPayload.ObjectForKey(new NSString("callerID"));
            var callerid = payload.DictionaryPayload["twi_from"].ToString();
            //CallProviderDelegate.ReportIncomingCall(new NSUuid(), callerid);
            TwilioService.Setnotification(payload);
        }

        [Export("pushRegistry:didReceiveIncomingPushWithPayload:forType:withCompletionHandler:")]
        [Preserve(Conditional = true)]
        public void DidReceiveIncomingPush(PKPushRegistry registry, PKPushPayload payload, string type,Action completion)
        {
            try
            {
                Console.WriteLine("My push is coming (Inside Action method!");
                var callerid = payload.DictionaryPayload["twi_from"].ToString();
                Console.WriteLine($"from: {callerid}");
                
                
                if (payload != null)
                {

                    TwilioService.Setnotification(payload);
                    //TwilioVoiceHelper.activeCallUuid = new NSUuid();
                    //CallProviderDelegate.ReportIncomingCall(TwilioVoiceHelper.activeCallUuid, callerid);

                }
                completion();

                //DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default).DispatchAsync(() =>
                //{
                //    DispatchQueue.MainQueue.DispatchAsync(() =>
                //    {
                //        completion();
                //    });
                //});

            }
            catch (Exception ex)
            {
                LogHelper.Info($"Inside DidReceiveIncomingPush:: Error:: {ex.Message} {ex.StackTrace}");
            }
        }

        void RegisterVoip()
        {
            var mainQueue = DispatchQueue.MainQueue;
            PKPushRegistry voipRegistry = new PKPushRegistry(mainQueue);
            voipRegistry.Delegate = this;  
            voipRegistry.DesiredPushTypes = new NSSet(new string[] { PushKit.PKPushType.Voip });
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // Get handle from url
            var handle = StartCallRequest.CallHandleFromURL(url);

            //// Found?
            if (handle != null)
            {
                // Yes, start call and inform system
                CallManager.StartCall(handle, TwilioVoiceHelper.activeCallUuid);
                return true;
            }

            return base.OpenUrl(application, url, sourceApplication, annotation);
        }
        
    }


    public class PushRegistry : PKPushRegistryDelegate
    {
        public override void DidReceiveIncomingPush(PKPushRegistry registry, PKPushPayload payload, string type, Action completion)
        {
            Console.WriteLine("My push is coming!");
            // var aps = payload.DictionaryPayload.ObjectForKey(new NSString("callerID"));
            var callerid = payload.DictionaryPayload["twi_from"].ToString();
            Console.WriteLine($"from: {callerid}");
            // CallProviderDelegate.ReportIncomingCall(new NSUuid(), callerid);
            TwilioService.Setnotification(payload);
            completion();
        }

        public override void DidReceiveIncomingPush(PKPushRegistry registry, PKPushPayload payload, string type)
        {
            Console.WriteLine("My push is coming!");
            var callerid = payload.DictionaryPayload["twi_from"].ToString();
            Console.WriteLine($"from: {callerid}");
            TwilioService.Setnotification(payload);
        }

        public override async void DidUpdatePushCredentials(PKPushRegistry registry, PKPushCredentials credentials, string type)
        {
            if (credentials != null && credentials.Token != null)
            {
                var fullToken = credentials.Token.DebugDescription;
                var deviceToken = fullToken.Trim('<').Trim('>').Replace(" ", string.Empty);
                Console.WriteLine("Token is " + deviceToken);
                await TwilioService.Register(deviceToken);
            }
        }
    }
}
