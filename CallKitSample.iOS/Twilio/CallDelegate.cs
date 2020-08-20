using System;
using Foundation;
using Twilio.Voice.iOS;

namespace CallKitSample.iOS.Twilio
{
    internal class CallDelegate : TVOCallDelegate
    {
        #region Fields

        private static CallDelegate _instance;

        #endregion

        #region Properties

        public static CallDelegate Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CallDelegate();
                }

                return _instance;
            }
        }

        #endregion

        #region Events

        public event EventHandler<TVOCall> CallDidConnectEvent;
        public event EventHandler<(TVOCall call, NSError error)> CallDidDisconnectWithErrorEvent;
        public event EventHandler<(TVOCall call, NSError error)> CallDidFailToConnectWithErrorEvent;
        public event EventHandler<TVOCall> CallDidStartRingingEvent;

        #endregion

        #region Methods

        [Export("callDidConnect:")]
        public override void CallDidConnect(TVOCall call)
        {
            LogHelper.Call(nameof(CallDelegate), nameof(CallDidConnect));
            CallDidConnectEvent?.Invoke(this, call);
        }

        [Export("call:didDisconnectWithError:")]
        public override void CallDidDisconnectWithError(TVOCall call, NSError error)
        {
            LogHelper.Call(nameof(CallDelegate), nameof(CallDidDisconnectWithError));
            CallDidDisconnectWithErrorEvent?.Invoke(this, (call, error));
        }

        [Export("call:didFailToConnectWithError:")]
        public override void CallDidFailToConnectWithError(TVOCall call, NSError error)
        {
            LogHelper.Call(nameof(CallDelegate), nameof(CallDidFailToConnectWithError));
            CallDidFailToConnectWithErrorEvent?.Invoke(this, (call, error));
        }

        [Export("callDidStartRinging:")]
        public override void CallDidStartRinging(TVOCall call)
        {
            LogHelper.Call(nameof(CallDelegate), nameof(CallDidStartRinging));
            CallDidStartRingingEvent?.Invoke(this, call);
        }
        #endregion
    }


    internal class LogHelper
    {
        static string TAG = "CallkitSample";
        public static void Call(string c, string m)
        {
            Console.WriteLine(c + ":" + m);
        }
        public static void Call(string c, string m, string a)
        {
            Console.WriteLine(c + ":" + m + " " + a);
        }
        public static void Info(string m)
        {
            Console.WriteLine($"{TAG} : {m}");
        }

    }
}
