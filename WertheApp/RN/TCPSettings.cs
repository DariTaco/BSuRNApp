using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;

namespace WertheApp.RN
{
    public class TCPSettings : ContentPage
    {
        //VARIABLES
        Picker p_Example;//has to be definded here instead of Constructor because value is also needed in method


        //CONSTRUCTOR
        public TCPSettings()
        {
            Title = "TCP";
            CreateContent();
        }

        //METHODS
        void CreateContent()
        {
            var scrollView = new ScrollView
            {
                Margin = new Thickness(10)
            };
            var stackLayout = new StackLayout();

            this.Content = scrollView;
            scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            //define elements
            var l_Choose = new Label { Text = "Choose an example:" };
            p_Example = new Picker();
            p_Example.Items.Add("Reno Fast Recovery");
            p_Example.Items.Add("Ack Generation");
            p_Example.SelectedIndex = 0;
            var l_Space2 = new Label { Text = "  " };
            var b_Start = new Button { Text = "Start" };
            b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)
            //add elemnts to stackLayout
            stackLayout.Children.Add(l_Choose);
            stackLayout.Children.Add(p_Example);
            stackLayout.Children.Add(l_Space2);
            stackLayout.Children.Add(b_Start);

        }

        /**********************************************************************
        *********************************************************************/
        //If Button Start is clicked
        async void B_Start_Clicked(object sender, EventArgs e)
        {
          
            if (IsLandscape())
            {
                await DisplayAlert("Alert", "Please hold your phone vertically for portrait mode", "OK");
            }

            // start selected example
            String selectedExample = p_Example.SelectedItem.ToString();
            await Navigation.PushAsync(new TCP(selectedExample));

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
