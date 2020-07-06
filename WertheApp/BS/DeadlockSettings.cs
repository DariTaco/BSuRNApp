using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.Diagnostics;

namespace WertheApp.BS
{
    public class DeadlockSettings: ContentPage
    {
        private String cd, printer, usb, file, other;
        private String resource_vector;
        private Picker p_cd, p_printer, p_usb, p_file, p_other;
        private Label l_resource_vector;

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

            var l_available = new Label { Text = "available resources:" };
            stackLayout.Children.Add(l_available);

            // resource CD
            var sl_cd = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_cd = new Label { Text = " CD ROM Drives", VerticalOptions = LayoutOptions.Center };
            p_cd = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_cd.Items.Add(i.ToString());
            }
            p_cd.SelectedIndex = 2;
            p_cd.SelectedIndexChanged += VectorChanged;
            sl_cd.Children.Add(p_cd);
            sl_cd.Children.Add(l_cd);
            stackLayout.Children.Add(sl_cd);

            // resource printer
            var sl_printer = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_printer = new Label { Text = " Printers", VerticalOptions = LayoutOptions.Center };
            p_printer = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_printer.Items.Add(i.ToString());
            }
            p_printer.SelectedIndex = 2;
            p_printer.SelectedIndexChanged += VectorChanged; 
            sl_printer.Children.Add(p_printer);
            sl_printer.Children.Add(l_printer);
            stackLayout.Children.Add(sl_printer);

            // resource USB
            var sl_usb = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_usb = new Label { Text = " USB Ports", VerticalOptions = LayoutOptions.Center };
            p_usb = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_usb.Items.Add(i.ToString());
            }
            p_usb.SelectedIndex = 2;
            p_usb.SelectedIndexChanged += VectorChanged;
            sl_usb.Children.Add(p_usb);
            sl_usb.Children.Add(l_usb);
            stackLayout.Children.Add(sl_usb);

            // resource file
            var sl_file = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_file = new Label {
                Text = " Files",
                VerticalOptions = LayoutOptions.Center
            };
            p_file = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_file.Items.Add(i.ToString());
            }
            p_file.SelectedIndex = 2;
            p_file.SelectedIndexChanged += VectorChanged;
            sl_file.Children.Add(p_file);
            sl_file.Children.Add(l_file);
            stackLayout.Children.Add(sl_file);

            // resource other
            var sl_other = new StackLayout() { Orientation = StackOrientation.Horizontal};
            var l_other = new Label {
                Text = " Other Resources",
                VerticalOptions = LayoutOptions.Center
            };
            p_other = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_other.Items.Add(i.ToString());
            }
            p_other.SelectedIndex = 2;
            p_other.SelectedIndexChanged += VectorChanged;
            sl_other.Children.Add(p_other);
            sl_other.Children.Add(l_other);
            stackLayout.Children.Add(sl_other);

            // resource vector
            resource_vector = "22222";
            l_resource_vector = new Label { Text = "Vector E = (" + resource_vector + ")" };
            stackLayout.Children.Add(l_resource_vector);


            var b_Start = new Button { Text = "Start" };
            b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)


            stackLayout.Children.Add(b_Start);
        }

        /**********************************************************************
        *********************************************************************/
        void VectorChanged(object sender, EventArgs e)
        {
            cd = p_cd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            file = p_file.SelectedItem.ToString();
            other = p_other.SelectedItem.ToString();

            resource_vector = "";
            if(cd != "0"){ resource_vector = "" + resource_vector + "" + cd; }
            if (printer != "0") { resource_vector = "" + resource_vector + "" + printer; }
            if (usb != "0") { resource_vector = "" + resource_vector + "" + usb; }
            if (file != "0") { resource_vector = "" + resource_vector + "" + file; }
            if (other != "0") { resource_vector = "" + resource_vector + "" + other; }


            l_resource_vector.Text = "Vector E = (" + resource_vector + ")";
        }

        /**********************************************************************
        *********************************************************************/
        //If Button Start is clicked
        async void B_Start_Clicked(object sender, EventArgs e)
        {
            if(resource_vector.Length < 3) {
                await DisplayAlert("Alert", "Please define at least 3 resources", "OK");
            }
            else
            {
                if (!IsLandscape())
                {
                    await DisplayAlert("Alert", "Please hold your phone horizontally for landscape mode", "OK");
                }
                await Navigation.PushAsync(new Deadlock());
            }

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
