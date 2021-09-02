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

            ctx.AddIdentificationEntry(123456789, "AG : John");
            ctx.AddIdentificationEntry(531236789, "AG : Michel");

            //TODO Populate this


            base.BeginRequestWithExtensionContext(context);
        }
    }

}
