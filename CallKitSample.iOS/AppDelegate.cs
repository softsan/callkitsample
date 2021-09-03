using System;
using System.Diagnostics;
using System.IO;
using AVFoundation;
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
       
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Instance = this;
            
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            RegisterVoip();
            CallManager = new ActiveCallManager();
            CallProviderDelegate = new ProviderDelegate(CallManager);
            CallDirectoryDelegate = new DirectoryDelegate();
            return base.FinishedLaunching(app, options);
        }
        
        public async void DidUpdatePushCredentials(PKPushRegistry registry, PKPushCredentials credentials, string type)
        {
            if (credentials != null && credentials.Token != null)
            {
                var fullToken = credentials.Token.DebugDescription;
                Console.WriteLine("Token is " + fullToken);
                await TwilioService.Register(credentials.Token);
            }
        }

        public void DidReceiveIncomingPush(PKPushRegistry registry, PKPushPayload payload, string type)
        {
            Console.WriteLine("My push is coming!");
            if (payload != null)
            {
                TwilioService.Setnotification(payload);
            }
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

        void RegisterVoip()
        {
            var mainQueue = DispatchQueue.MainQueue;
            PKPushRegistry voipRegistry = new PKPushRegistry(mainQueue);
            voipRegistry.Delegate = this;
            voipRegistry.DesiredPushTypes = new NSSet(new string[] { PushKit.PKPushType.Voip });
        }
    }


    public static class LoggerService
    {
        public static void Log(string type,string message)
        {
            try
            {
                var data = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var filename = Path.Combine(data, String.Format("CallKit_logs.txt"));

                Debug.WriteLine($"{type} {message}");
                var dt = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                var msg = $"{dt}: {type} {message} \n-----\n";

                File.AppendAllText(filename, msg);
            }
            catch (DirectoryNotFoundException)
            {
            }
             
        }
    }
}
