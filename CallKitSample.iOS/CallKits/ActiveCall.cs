using System;
using CoreFoundation;
using Foundation;

namespace CallKitSample.CallKits
{
    public class ActiveCall
    {
        #region Private Variables
        private bool isConnecting;
        private bool isConnected;
        private bool isOnhold;
        private bool isMuted;
        #endregion

        #region Computed Properties
        public NSUuid UUID { get; set; }
        public bool isOutgoing { get; set; }
        public string Handle { get; set; }
        public DateTime StartedConnectingOn { get; set; }
        public DateTime ConnectedOn { get; set; }
        public DateTime EndedOn { get; set; }

        public bool IsConnecting
        {
            get { return isConnecting; }
            set
            {
                isConnecting = value;
                if (isConnecting) StartedConnectingOn = DateTime.Now;
                RaiseStartingConnectionChanged();
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                if (isConnected)
                {
                    ConnectedOn = DateTime.Now;
                }
                else
                {
                    EndedOn = DateTime.Now;
                }
                RaiseConnectedChanged();
            }
        }

        public bool IsOnHold
        {
            get { return isOnhold; }
            set
            {
                isOnhold = value;
            }
        }

        public bool IsMuted
        {
            get { return isMuted; }
            set
            {
                isMuted = value;
                RaiseConnectedChanged();
            }
        }

        #endregion

        #region Constructors
        public ActiveCall()
        {
        }

        public ActiveCall(NSUuid uuid, string handle, bool outgoing)
        {
            // Initialize
            this.UUID = uuid;
            this.Handle = handle;
            this.isOutgoing = outgoing;
        }
        #endregion

        #region Public Methods

        public void MuteCall(ActiveCallbackDelegate completionHandler)
        {
            iOS.Twilio.TwilioService.MuteCall(!IsMuted);
            //iOS.AppDelegate.Instance.complete();
            // Simulate the call ending
            IsMuted = !IsMuted;
            completionHandler(true);
        }

        public void StartCall(ActiveCallbackDelegate completionHandler)
        {
            iOS.Twilio.TwilioService.ConnectCall(this.Handle);
            // Simulate the call starting successfully
            completionHandler(true);

            // Simulate making a starting and completing a connection
            DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 3000), () => {
                // Note that the call is starting
                IsConnecting = true;

                // Simulate pause before connecting
                DispatchQueue.MainQueue.DispatchAfter(new DispatchTime(DispatchTime.Now, 1500), () => {
                    // Note that the call has connected
                    IsConnecting = false;
                    IsConnected = true;
                });
            });
        }

        public void AnswerCall(ActiveCallbackDelegate completionHandler)
        {
            iOS.Twilio.TwilioService.AnswerIncomingCall();
            //iOS.AppDelegate.Instance.complete();
            // Simulate the call being answered
            IsConnected = true;
            completionHandler(true);
        }

        public void EndCall(ActiveCallbackDelegate completionHandler)
        {
            iOS.Twilio.TwilioService.EndCall();
            //iOS.AppDelegate.Instance.complete();
            // Simulate the call ending
            IsConnected = false;
            completionHandler(true);
        }
        public void DeclineCallInvite(ActiveCallbackDelegate completionHandler)
        {
            iOS.Twilio.TwilioService.DeclineCallInvite();
            //iOS.AppDelegate.Instance.complete();
            // Simulate the call ending
            IsMuted = !IsMuted;
            completionHandler(true);
        }
        #endregion

        #region Events
        public delegate void ActiveCallbackDelegate(bool successful);
        public delegate void ActiveCallStateChangedDelegate(ActiveCall call);

        public event ActiveCallStateChangedDelegate StartingConnectionChanged;
        internal void RaiseStartingConnectionChanged()
        {
            if (this.StartingConnectionChanged != null) this.StartingConnectionChanged(this);
        }

        public event ActiveCallStateChangedDelegate ConnectedChanged;
        internal void RaiseConnectedChanged()
        {
            if (this.ConnectedChanged != null) this.ConnectedChanged(this);
        }
        #endregion
    }

}
