/*
 https://docs.microsoft.com/en-us/xamarin/essentials/get-started?tabs=macos%2Candroid
 */
//TODO: App Indexing funz bei Android glaub noch nicht ganz
using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using WertheApp.OS.AllocationStrategies;
using Xamarin.Forms;
using WertheApp.CN;
using WertheApp.OS;
using Xamarin.Forms.Platform.Android.AppLinks;
using Firebase;

namespace WertheApp.Droid
{
    [IntentFilter(new[] { Intent.ActionView },
              Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
              DataScheme = "http",
              DataHost = "dariakern.de",
              DataPathPrefix = "/wertheapp/",
              AutoVerify = true)]
    [IntentFilter(new[] { Intent.ActionView },
              Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
              DataScheme = "https",
              DataHost = "dariakern.de",
              DataPathPrefix = "/wertheapp/",
              AutoVerify = true)]
    [Activity(Label = "WertheApp.Droid", Icon = "@drawable/Werthebach3", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.ScreenLayout) ]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            // Subscribe to Screen Orientation Messages
            MessagingCenter.Subscribe<object>(this, "Portrait", sender => { RequestedOrientation = ScreenOrientation.Portrait; });
            MessagingCenter.Subscribe<object>(this, "Unspecified", sender => { RequestedOrientation = ScreenOrientation.User; });
            MessagingCenter.Subscribe<object>(this, "Landscape", sender => { RequestedOrientation = ScreenOrientation.Landscape; });

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);

            // App Linking
            FirebaseApp.InitializeApp(this);
            AndroidAppLinks.Init(this);

            LoadApplication(new App());

        }
    }




}
