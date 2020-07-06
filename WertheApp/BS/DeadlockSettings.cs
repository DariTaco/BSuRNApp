using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace WertheApp.BS
{
    public class DeadlockSettings: ContentPage
    {
        public DeadlockSettings()
        {
            Title = "Deadlock";
            CreateContent();
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            var scrollView = new ScrollView
            {
                Margin = new Thickness(10)
            };
            var stackLayout = new StackLayout();

            this.Content = scrollView;
            scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            var b_Start = new Button { Text = "Start" };
            b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            stackLayout.Children.Add(b_Start);
        }

        /**********************************************************************
        *********************************************************************/
        //If Button Start is clicked
        async void B_Start_Clicked(object sender, EventArgs e)
        {
            if (!IsLandscape())
            {
                await DisplayAlert("Alert", "Please hold your phone horizontally for landscape mode", "OK");
            }
            await Navigation.PushAsync(new Deadlock());
        }

        /**********************************************************************
        *********************************************************************/
        static bool IsLandscape()
        {
            bool isLandscape = false;
            if (Application.Current.MainPage.Width > Application.Current.MainPage.Height)
            {
                isLandscape = true;
            }
            else
            {
                isLandscape = false;
            }
            return isLandscape;
        }
    }
}
