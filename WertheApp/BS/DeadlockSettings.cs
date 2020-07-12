using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using System.Collections.Generic;

namespace WertheApp.BS
{
    public class DeadlockSettings: ContentPage
    {
        private String dvd, usb, bluRay, printer, ijPrinter;
        private String resourceVectorE, occupiedResourceVectorB, freeResourceVectorA;
        private Picker p_dvd, p_usb, p_BluRay, p_printer, p_ijPrinter, p_total;
        private Label l_resourceVectorE, l_occupiedResourceVectorB, l_freeResourceVectorA;
        private Entry e_occuP1, e_occuP2, e_occuP3, e_occuP4, e_occuP5;
        private StackLayout sl_occupiedResources, sl_requestedRessources;
        private List<Entry> occupiedPEntriesList;

        public DeadlockSettings()
        {
            Title = "Deadlock";
            CreateContent();
            occupiedPEntriesList = new List<Entry>();

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
            var stackLayoutLeft = new StackLayout();
            var stackLayoutRight = new StackLayout();
            var stackLayoutRLContainer = new StackLayout() { Orientation = StackOrientation.Horizontal };


            this.Content = scrollView;
            scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content

            var l_available = new Label { Text = "Resources", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))};
            stackLayout.Children.Add(l_available);

            // resource DVD
            var sl_dvd = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_dvd = new Label { Text = " DVD Drives", VerticalOptions = LayoutOptions.Center };
            p_dvd = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_dvd.Items.Add(i.ToString());
            }
            p_dvd.SelectedIndex = 2;
            p_dvd.SelectedIndexChanged += VectorChanged;
            p_dvd.SelectedIndexChanged += VectorChanged2;
            sl_dvd.Children.Add(p_dvd);
            sl_dvd.Children.Add(l_dvd);
            stackLayoutLeft.Children.Add(sl_dvd);


            // resource USB
            var sl_usb = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_usb = new Label { Text = " USB Disk Drives", VerticalOptions = LayoutOptions.Center };
            p_usb = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_usb.Items.Add(i.ToString());
            }
            p_usb.SelectedIndex = 2;
            p_usb.SelectedIndexChanged += VectorChanged;
            p_usb.SelectedIndexChanged += VectorChanged2;
            sl_usb.Children.Add(p_usb);
            sl_usb.Children.Add(l_usb);
            stackLayoutLeft.Children.Add(sl_usb);

            // resource BluRay
            var sl_bluRay = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_bluRay = new Label {
                Text = " BluRay Drives",
                VerticalOptions = LayoutOptions.Center
            };
            p_BluRay = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_BluRay.Items.Add(i.ToString());
            }
            p_BluRay.SelectedIndex = 2;
            p_BluRay.SelectedIndexChanged += VectorChanged;
            p_BluRay.SelectedIndexChanged += VectorChanged2;
            sl_bluRay.Children.Add(p_BluRay);
            sl_bluRay.Children.Add(l_bluRay);
            stackLayoutLeft.Children.Add(sl_bluRay);

            // resource Laser printer
            var sl_printer = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_printer = new Label { Text = " Laser Printers", VerticalOptions = LayoutOptions.Center };
            p_printer = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_printer.Items.Add(i.ToString());
            }
            p_printer.SelectedIndex = 2;
            p_printer.SelectedIndexChanged += VectorChanged;
            p_printer.SelectedIndexChanged += VectorChanged2;
            sl_printer.Children.Add(p_printer);
            sl_printer.Children.Add(l_printer);
            stackLayoutRight.Children.Add(sl_printer);

            // resource inkjet printer
            var sl_ijPrinter = new StackLayout() { Orientation = StackOrientation.Horizontal};
            var l_ijPrinter = new Label {
                Text = " Inkjet Printers",
                VerticalOptions = LayoutOptions.Center
            };
            p_ijPrinter = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_ijPrinter.Items.Add(i.ToString());
            }
            p_ijPrinter.SelectedIndex = 2;
            p_ijPrinter.SelectedIndexChanged += VectorChanged;
            p_ijPrinter.SelectedIndexChanged += VectorChanged2;
            sl_ijPrinter.Children.Add(p_ijPrinter);
            sl_ijPrinter.Children.Add(l_ijPrinter);
            stackLayoutRight.Children.Add(sl_ijPrinter);

            stackLayoutRight.HorizontalOptions = LayoutOptions.CenterAndExpand;
            stackLayoutRLContainer.Children.Add(stackLayoutLeft);
            stackLayoutRLContainer.Children.Add(stackLayoutRight);
            stackLayout.Children.Add(stackLayoutRLContainer);

            // resource vector
            resourceVectorE = "22222";
            l_resourceVectorE = new Label { Text = "Vector E = (" + resourceVectorE + ")" };
            stackLayout.Children.Add(l_resourceVectorE);

            var l_Space1 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space1);

            var l_Processes = new Label { Text = "Processes", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            stackLayout.Children.Add(l_Processes);
            var sl_total = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_total = new Label { Text = "Total: ", VerticalOptions = LayoutOptions.Center };
            p_total = new Picker();
            for (int i = 2; i < 6; i++)
            {
                p_total.Items.Add(i.ToString());
            }
            p_total.SelectedIndex = 2;
            p_total.SelectedIndexChanged += ProcessesChanged;
            sl_total.Children.Add(l_total);
            sl_total.Children.Add(p_total);
            stackLayout.Children.Add(sl_total);

            // occupied resources
            sl_occupiedResources = new StackLayout();
            var l_resource_occupied = new Label { Text = "Occupied Ressources" };
            stackLayout.Children.Add(l_resource_occupied);

            occupiedResourceVectorB = "00000";
            l_occupiedResourceVectorB = new Label { Text = "Vector B = (" + occupiedResourceVectorB + ")" };
            stackLayout.Children.Add(l_occupiedResourceVectorB);

            e_occuP1 = new Entry { Keyboard = Keyboard.Telephone, Text = "0,1,0,1,0" };
            e_occuP2 = new Entry { Keyboard = Keyboard.Telephone, Text = "0,1,0,1,0" };
            e_occuP3 = new Entry { Keyboard = Keyboard.Telephone, Text = "0,1,0,1,0" };
            e_occuP4 = new Entry { Keyboard = Keyboard.Telephone, Text = "0,1,0,1,0" };
            e_occuP5 = new Entry { Keyboard = Keyboard.Telephone, Text = "0,1,0,1,0" };
            occupiedPEntriesList.Add(e_occuP1);
            occupiedPEntriesList.Add(e_occuP2);
            occupiedPEntriesList.Add(e_occuP3);
            occupiedPEntriesList.Add(e_occuP4);
            occupiedPEntriesList.Add(e_occuP5);

            // requested resources
            var l_resource_request = new Label { Text = "Resource Requests" };
            stackLayout.Children.Add(l_resource_request);
            freeResourceVectorA = "22222";
            l_freeResourceVectorA = new Label { Text = "Vector A = (" + freeResourceVectorA + ")" };
            stackLayout.Children.Add(l_freeResourceVectorA);



            var l_Space2 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space2);
            var b_Default = new Button { Text = "Set Default", HorizontalOptions = LayoutOptions.Start };
            b_Default.Clicked += B_Default_Clicked; //add Click Event(Method)
            stackLayout.Children.Add(b_Default);

            var b_Start = new Button { Text = "Start" };
            b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            var l_Space = new Label { Text = " " };
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(b_Start);
        }

        /**********************************************************************
        *********************************************************************/
        //TODO:sets default values
        void B_Default_Clicked(object sender, EventArgs e)
        {
            
        }

        /**********************************************************************
        *********************************************************************/
        void ProcessesChanged(object sender, EventArgs e)
        {
            sl_occupiedResources.Children.Clear();
            var total = Int16.Parse(p_total.SelectedItem.ToString());

            int i = 0;
            foreach (Entry entry in occupiedPEntriesList)
            {
                sl_occupiedResources.Children.Add(entry);
                i++;
                if(i == total) { break; }

            }
            
        }

        /**********************************************************************
        *********************************************************************/
        void VectorChanged(object sender, EventArgs e)
        {
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_BluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();

            resourceVectorE = "";
            if(dvd != "0"){ resourceVectorE = "" + resourceVectorE + "" + dvd; }
            if (printer != "0") { resourceVectorE = "" + resourceVectorE + "" + printer; }
            if (usb != "0") { resourceVectorE = "" + resourceVectorE + "" + usb; }
            if (bluRay != "0") { resourceVectorE = "" + resourceVectorE + "" + bluRay; }
            if (ijPrinter != "0") { resourceVectorE = "" + resourceVectorE + "" + ijPrinter; }


            l_resourceVectorE.Text = "Vector E = (" + resourceVectorE + ")";
        }

        /**********************************************************************
        *********************************************************************/
        void VectorChanged2(object sender, EventArgs e)
        {
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_BluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();

            //TODO: Ausrechnen Occupied
            var occu_dvd = dvd;
            var occu_printer = printer;
            var occu_usb = usb;
            var occu_bluRay = bluRay;
            var occu_ijPrinter = ijPrinter;

            occupiedResourceVectorB = "";
            if (dvd != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_dvd; }
            if (printer != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_printer; }
            if (usb != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_usb; }
            if (bluRay != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_bluRay; }
            if (ijPrinter != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_ijPrinter; }


            l_occupiedResourceVectorB.Text = "Vector B = (" + occupiedResourceVectorB + ")";
        }

        //TODO: Vector changed3 für free ressources berechnet aus total ressources und occupied
        /**********************************************************************
        *********************************************************************/
        //If Button Start is clicked
        async void B_Start_Clicked(object sender, EventArgs e)

            //TODO: check occupied ressources can't be higher than available ressources
        {
            if(resourceVectorE.Length < 3) {
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
