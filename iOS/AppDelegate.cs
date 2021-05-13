using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms;
using WertheApp.OS.AllocationStrategies;
using WertheApp.OS;
using WertheApp.CN;

namespace WertheApp.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            // Subscribe to Screen Orientation Messages
            MessagingCenter.Subscribe<object>(this, "Portrait", sender => {
                UIDevice.CurrentDevice.SetValueForKey(NSNumber.FromNInt((int)(UIInterfaceOrientation.Portrait)), new NSString("orientation"));
            });
            MessagingCenter.Subscribe<object>(this, "Landscape", sender => {
                UIDevice.CurrentDevice.SetValueForKey(NSNumber.FromNInt((int)(UIInterfaceOrientation.LandscapeRight)), new NSString("orientation"));
            });
            MessagingCenter.Subscribe<object>(this, "Unspecified", sender => {
                UIDevice.CurrentDevice.SetValueForKey(NSNumber.FromNInt((int)(UIInterfaceOrientation.Unknown)), new NSString("orientation"));
            });
            return base.FinishedLaunching(app, options);
        }
    }
}

/*
 (UIDeviceOrientation.Portrait)), new NSString("orientation"));
 (UIInterfaceOrientation.Portrait)), new NSString("orientation"));
UIInterfaceOrientationMask

 */