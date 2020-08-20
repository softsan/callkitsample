using System;
namespace CallKitSample.Services
{
    public interface ICallService
    {
        void StartCall(string phoneNumber);
        void EndCall();
    }

    
}
