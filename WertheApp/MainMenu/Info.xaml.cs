using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace WertheApp
{
    public partial class Info : ContentPage
    {
        private string _currentVersion;
        public string currentVersion
        {
            get { return _currentVersion; }
            set
            {
                _currentVersion = value;
                OnPropertyChanged(nameof(currentVersion)); // Notify that there was a change on this property
            }
        }
        public Info()
        {
            InitializeComponent();
            BindingContext = this;
            // Current app version 
            currentVersion = VersionTracking.CurrentVersion;
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
