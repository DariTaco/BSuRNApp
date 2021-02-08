using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace WertheApp.CN
{
    public partial class RenoFastRecoveryHelp : ContentPage
    {
        public RenoFastRecoveryHelp()
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
