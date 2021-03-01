using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace WertheApp.OS
{
    public partial class PageReplacementStrategiesHelp : ContentPage
    {
        //Data Binding test
        /*private string _s1;
        public string s1
        {
            get { return _s1; }
            set
            {
                _s1 = value;
                OnPropertyChanged(nameof(s1)); // Notify that there was a change on this property
            }
        }
        */

        public PageReplacementStrategiesHelp()
        {
            InitializeComponent();
            BindingContext = this;
            /*s1 = "A virtual page must be mapped to a real page in memory (RAM) before execution. " +
                "Virtual and real memory pages have the same size. " +
                "The offset of the virtual memory address navigates directly in both pages.";
            */

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
