using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace WertheApp.OS
{
    public partial class DeadlockHelp : ContentPage
    {
        public DeadlockHelp()
        {
            InitializeComponent();
        }
        public async void scroll()
        {
            //await scrollView.ScrollToAsync(0, 1500, true);
            await scrollView.ScrollToAsync(last, ScrollToPosition.End, true);


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            scroll();
        }
    }
}
