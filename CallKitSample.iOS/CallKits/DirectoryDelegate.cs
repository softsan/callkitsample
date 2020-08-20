using System;
using CallKit;
using Foundation;

namespace CallKitSample.CallKits
{
    public class DirectoryDelegate : CXCallDirectoryProvider
    {
        public override void BeginRequestWithExtensionContext(NSExtensionContext context)
        {
            var ctx = context as CXCallDirectoryExtensionContext;

            ctx.AddIdentificationEntry(18052483024, "AG : Nic");
            ctx.AddIdentificationEntry(15157714893, "AG : Casey");

            //TODO Populate this


            base.BeginRequestWithExtensionContext(context);
        }
    }

}
