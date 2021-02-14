using System;

using Xamarin.Forms;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategies : ContentPage
    {
        private double width = 0;
        private double height = 0;

        public AllocationStrategies()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Hello ContentPage" }
                }
            };
        }


        /**********************************************************************
        ********************************************************************/
        protected override void OnAppearing()
        {
            base.OnAppearing();
          
            MessagingCenter.Send<object>(this, "Landscape");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<object>(this, "Unspecified"); 
        }

        //this method is called everytime the device is rotated
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                MessagingCenter.Send<object>(this, "Landscape");
            }
        }

    }
}

