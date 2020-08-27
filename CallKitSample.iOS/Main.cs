using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Foundation;
using UIKit;

namespace CallKitSample.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            try
            {
                UIApplication.Main(args, null, nameof(AppDelegate));

            }
            catch (Exception e)
            {
                try
                {
                    var data = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    var filename = Path.Combine(data, String.Format("crash.txt"));

                    Debug.WriteLine(e.Message);
                    Debug.WriteLine(e.StackTrace);
                    Debug.WriteLine(e.Source);
                    Debug.WriteLine(e.InnerException?.StackTrace);
                    Debug.WriteLine(e.InnerException?.Source);

                    File.WriteAllText(filename, e.Message + "\n-----\n" + e.StackTrace ?? "" + "\n-----\n" + e.Source ?? "" + "\n-----\n");
                }
                catch (DirectoryNotFoundException)
                {
                }
            }
        }
    }
}
