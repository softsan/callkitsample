using System;
using Foundation;
using Twilio.Voice.iOS;

namespace CallKitSample.iOS.Twilio
{
    public class TwilioVoiceHelper : IDisposable
    {
        #region Fields

        private CallDelegate _callDelegate;
        private NotificationDelegate _notificationDelegate;
        private string _registeredAccessToken;
        private NSData _registeredDeviceToken;

        #endregion

        #region Properties

        public TVOCallInvite CallInvite { get; private set; }
        public TVOCall Call { get; private set; }
        public static NSUuid activeCallUuid { get; set; }
        #endregion

        #region Events

        public event EventHandler Registered;
        public event EventHandler<NSError> RegisteredWithError;
        public event EventHandler CallInviteReceived;
        public event EventHandler CallDidConnect;
        public event EventHandler<NSError> CallDidDisconnectWithError;
        public event EventHandler<NSError> CallDidFailToConnectWithError;
        public event EventHandler<(TVOCancelledCallInvite, NSError)> CancelledCallInviteReceived;

        #endregion
        public static bool IsVoipCall = false;
        #region Constructors

        public TwilioVoiceHelper()
        {
#if DEBUG
            //TwilioVoice.LogLevel = TVOLogLevel.Error;
#endif
            _callDelegate = CallDelegate.Instance;
            _notificationDelegate = NotificationDelegate.Instance;
            _callDelegate.CallDidConnectEvent -= CallDelegateOnCallDidConnectEvent;
            _callDelegate.CallDidDisconnectWithErrorEvent -= CallDelegateOnCallDidDisconnectWithError;
            _callDelegate.CallDidFailToConnectWithErrorEvent -= CallDelegateOnCallDidFailToConnectWithErrorEvent;
            _notificationDelegate.CallInviteReceivedEvent -= NotificationDelegateOnCallInviteReceivedEvent;
            _notificationDelegate.CancelledCallInviteReceivedEvent -= NotificationDelegateOnCancelledCallInviteReceivedEvent;

            _callDelegate.CallDidConnectEvent += CallDelegateOnCallDidConnectEvent;
            _callDelegate.CallDidDisconnectWithErrorEvent += CallDelegateOnCallDidDisconnectWithError;
            _callDelegate.CallDidFailToConnectWithErrorEvent += CallDelegateOnCallDidFailToConnectWithErrorEvent;
            _notificationDelegate.CallInviteReceivedEvent += NotificationDelegateOnCallInviteReceivedEvent;
            _notificationDelegate.CancelledCallInviteReceivedEvent += NotificationDelegateOnCancelledCallInviteReceivedEvent;
        }

        #endregion

        #region Methods

        public void Register(string accessToken, NSData deviceToken)
        {
            LogHelper.Call(nameof(TwilioVoiceHelper), nameof(Register), $"AccessToken: {accessToken}, DeviceToken: {deviceToken}");
            if (accessToken == null || deviceToken == null) return;
            TwilioVoice.RegisterWithAccessToken(accessToken, deviceToken,
                                                error =>
                                                {
                                                    if (error == null)
                                                    {
                                                        LogHelper.Info("Successfully registered for VoIP push notifications");
                                                        _registeredAccessToken = accessToken;
                                                        _registeredDeviceToken = deviceToken;
                                                        Registered?.Invoke(this, EventArgs.Empty);
                                                    }
                                                    else
                                                    {
                                                        LogHelper.Info($"An error occurred while registering: {error.LocalizedDescription}");
                                                        RegisteredWithError?.Invoke(this, error);
                                                    }
                                                });
        }

        public void Unregister()
        {
            if (_registeredAccessToken == null || _registeredDeviceToken == null) return;
            LogHelper.Call(nameof(TwilioVoiceHelper), nameof(Unregister));
            TwilioVoice.UnregisterWithAccessToken(_registeredAccessToken, _registeredDeviceToken,
                                                error =>
                                                {
                                                    if (error == null)
                                                    {
                                                        LogHelper.Info("Successfully unregistered for VoIP push notifications");
                                                        _registeredAccessToken = null;
                                                        _registeredDeviceToken = null;
                                                    }
                                                    else
                                                    {
                                                        LogHelper.Info($"An error occurred while unregistering: {error.LocalizedDescription}");
                                                    }
                                                });
        }

        public void MakeCall(string accessToken, NSDictionary<NSString, NSString> parameters)
        {
            LogHelper.Call(nameof(TwilioVoiceHelper), nameof(MakeCall));
            if (accessToken == null || Call != null) return;
            var connectionOptions = TVOConnectOptions.OptionsWithAccessToken(accessToken,
                (arg0) =>
                {
                    arg0.Params = parameters;
                });
            Call = TwilioVoice.ConnectWithOptions(connectionOptions, _callDelegate);

        }

        public void MuteAudio(bool isMute)
        {
            if (Call != null)
                Call.Muted = isMute;


        }
        public void RejectCallInvite()
        {
            LogHelper.Call(nameof(TwilioVoiceHelper), nameof(RejectCallInvite));
            CallInvite?.Reject();
            CallInvite = null;
        }

        public void IgnoreCallInvite()
        {
            LogHelper.Call(nameof(TwilioVoiceHelper), nameof(IgnoreCallInvite));
            CallInvite = null;
        }

        public void AcceptCallInvite()
        {
            if (CallInvite == null) return;
            LogHelper.Call(nameof(TwilioVoiceHelper), nameof(AcceptCallInvite));
            try
            {

                var options = TVOAcceptOptions.OptionsWithCallInvite(CallInvite,
                                (TVOAcceptOptionsBuilder arg0) =>
                                {
                                    arg0.SetUuid(CallInvite.GetUuid());//activeCallUuid); // 
                                });

                Call = CallInvite?.AcceptWithOptions(options, _callDelegate);


            }
            catch (Exception ex)
            {
                LogHelper.Info($"Error: {ex.Message} {ex.StackTrace}");
            }



            //Call = CallInvite?.AcceptWithDelegate(_callDelegate);
            CallInvite = null;
        }

        public void SetNotificationPayload(PushKit.PKPushPayload payload)
        {
            
            TwilioVoice.HandleNotification(payload.DictionaryPayload, _notificationDelegate, delegateQueue: null);
            //TwilioVoice.AudioDevice

            

        }

        public void Dispose()
        {
            if (CallInvite != null)
            {
                CallInvite.Reject();
                CallInvite.Dispose();
                CallInvite = null;
            }

            if (Call != null)
            {
                Call.Disconnect();
                Call.Dispose();
                Call = null;
            }

            if (_callDelegate != null)
            {
                _callDelegate.CallDidConnectEvent -= CallDelegateOnCallDidConnectEvent;
                _callDelegate.CallDidDisconnectWithErrorEvent -= CallDelegateOnCallDidDisconnectWithError;
                _callDelegate.CallDidFailToConnectWithErrorEvent -= CallDelegateOnCallDidFailToConnectWithErrorEvent;
                _callDelegate = null;
            }

            if (_notificationDelegate != null)
            {
                _notificationDelegate.CallInviteReceivedEvent -= NotificationDelegateOnCallInviteReceivedEvent;
                _notificationDelegate.CancelledCallInviteReceivedEvent += NotificationDelegateOnCancelledCallInviteReceivedEvent;
                _notificationDelegate = null;
            }
        }

        #endregion

        #region Event handlers

        private void CallDelegateOnCallDidConnectEvent(object sender, TVOCall e)
        {
            Call = e;
            IsVoipCall = true;
            CallDidConnect?.Invoke(this, EventArgs.Empty);
        }

        private void CallDelegateOnCallDidDisconnectWithError(object sender, (TVOCall call, NSError error) e)
        {
            if (e.error == null)
                LogHelper.Info("Call disconnected");
            else
                LogHelper.Info($"Call failed: {e.error.LocalizedDescription}");

            AppDelegate.Instance.CallManager.EndCall(activeCallUuid);
            Call = null;
            CallDidDisconnectWithError?.Invoke(this, e.error);
        }

        private void CallDelegateOnCallDidFailToConnectWithErrorEvent(object sender, (TVOCall call, NSError error) e)
        {
            Call = null;
            CallDidFailToConnectWithError?.Invoke(this, e.error);
        }

        private void NotificationDelegateOnCallInviteReceivedEvent(object sender, TVOCallInvite e)
        {
            LogHelper.Call(nameof(TwilioVoiceHelper), nameof(NotificationDelegateOnCallInviteReceivedEvent));
            if (CallInvite != null)
            {
                LogHelper.Info($"Already a pending call invite. Ignoring incoming call invite from {e.From}");
                return;
            }
            if (Call != null && Call.State == TVOCallState.Connected)
            {
                LogHelper.Info($"Already an active call. Ignoring incoming call invite from {e.From}");
                return;
            }

            CallInvite = e;
            activeCallUuid = e.GetUuid();
            AppDelegate.Instance.CallProviderDelegate.ReportIncomingCall(activeCallUuid, e.From);

            CallInviteReceived?.Invoke(this, EventArgs.Empty);
        }

        

        private void NotificationDelegateOnCancelledCallInviteReceivedEvent(object sender, (TVOCancelledCallInvite e, NSError error) invite)
        {
            // We've already accepted the invite.
            if (Call != null) return;
            CancelledCallInviteReceived?.Invoke(this, (invite.e, invite.error));
        }

        #endregion
    }
}
