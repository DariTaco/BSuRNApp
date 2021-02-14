/*
 https://docs.microsoft.com/en-us/xamarin/essentials/get-started?tabs=macos%2Candroid
 */
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

namespace WertheApp.Droid
{
    [Activity(Label = "WertheApp.Droid", Icon = "@drawable/Werthebach3", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);


            LoadApplication(new App());

            // Subscribe to Screen Orientation Messages
            MessagingCenter.Subscribe<object>(this, "Portrait", sender => { RequestedOrientation = ScreenOrientation.Portrait; });
            MessagingCenter.Subscribe<object>(this, "Unspecified", sender => { RequestedOrientation = ScreenOrientation.Sensor; });
            MessagingCenter.Subscribe<object>(this, "Landscape", sender => { RequestedOrientation = ScreenOrientation.Landscape; });
          
        }
    }




}
