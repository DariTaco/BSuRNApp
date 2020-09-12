using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using System.Collections.Generic;

namespace WertheApp.BS
{
    public class DeadlockSettings : ContentPage
    {
        private String dvd, usb, bluRay, printer, ijPrinter, printer3D;
        private String resourceVectorE, busyResourceVectorB, freeResourceVectorA, upcomingVectorC;
        private Picker p_dvd, p_usb, p_bluRay, p_printer, p_ijPrinter, p_runningprocesses, p_printer3D;

        private Label l_busyP1, l_busyP2, l_busyP3, l_busyP4, l_busyP5;
        private Label l_upcomingP1, l_upcomingP2, l_upcomingP3, l_upcomingP4, l_upcomingP5;

        private Picker p_p1_dvd, p_p2_dvd, p_p3_dvd, p_p4_dvd, p_p5_dvd;
        private Picker p_p1_usb, p_p2_usb, p_p3_usb, p_p4_usb, p_p5_usb;
        private Picker p_p1_bluRay, p_p2_bluRay, p_p3_bluRay, p_p4_bluRay, p_p5_bluRay;
        private Picker p_p1_printer, p_p2_printer, p_p3_printer, p_p4_printer, p_p5_printer;
        private Picker p_p1_ijprinter, p_p2_ijprinter, p_p3_ijprinter, p_p4_ijprinter, p_p5_ijprinter;
        private Picker p_p1_printer3D, p_p2_printer3D, p_p3_printer3D, p_p4_printer3D, p_p5_printer3D;
        // private int busy_dvd, busy_usb, busy_bluRay, busy_printer, busy_ijprinter, busy_printer3D;


        private Picker p_p1_upcoming_dvd, p_p2_upcoming_dvd, p_p3_upcoming_dvd, p_p4_upcoming_dvd, p_p5_upcoming_dvd;
        private Picker p_p1_upcoming_usb, p_p2_upcoming_usb, p_p3_upcoming_usb, p_p4_upcoming_usb, p_p5_upcoming_usb;
        private Picker p_p1_upcoming_bluRay, p_p2_upcoming_bluRay, p_p3_upcoming_bluRay, p_p4_upcoming_bluRay, p_p5_upcoming_bluRay;
        private Picker p_p1_upcoming_printer, p_p2_upcoming_printer, p_p3_upcoming_printer, p_p4_upcoming_printer, p_p5_upcoming_printer;
        private Picker p_p1_upcoming_ijprinter, p_p2_upcoming_ijprinter, p_p3_upcoming_ijprinter, p_p4_upcoming_ijprinter, p_p5_upcoming_ijprinter;
        private Picker p_p1_upcoming_printer3D, p_p2_upcoming_printer3D, p_p3_upcoming_printer3D, p_p4_upcoming_printer3D, p_p5_upcoming_printer3D;
        //private int upcoming_dvd, upcoming_usb, upcoming_bluRay, upcoming_printer, upcoming_ijprinter;

        private Label l_resourceVectorE, l_busyResourceVectorB, l_freeResourceVectorA, l_upcomingVectorC;
        private StackLayout sl_busyResources, sl_upcomingRequests;
        private StackLayout sl_busyProcess1, sl_busyProcess2, sl_busyProcess3, sl_busyProcess4, sl_busyProcess5;
        private StackLayout sl_upcomingProcess1, sl_upcomingProcess2, sl_upcomingProcess3, sl_upcomingProcess4, sl_upcomingProcess5;

        private List<Picker> busyResPickerList, upcomingResPickerList, resPickerList;
        //private int preset;
        private static Dictionary<int, String> vectorBProcesses, vectorCProcesses;


        public DeadlockSettings()
        {
            Title = "Deadlock";

            busyResPickerList = new List<Picker>();
            upcomingResPickerList = new List<Picker>();
            resPickerList = new List<Picker>();
            //preset = 0;

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

            var vectorLabelSize = 120;

            //EXISTING RESOURCES
            var l_resourcesExisting = new Label { Text = "Existing Resources ",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center
            };
            stackLayout.Children.Add(l_resourcesExisting);

            CreateExistingResourcesUI(stackLayout);

            //VECTOR E
            StackLayout stackLayoutVectorE = new StackLayout { Orientation = StackOrientation.Horizontal };
            Label l_existing = new Label
            {
                Text = "Existing ",
                TextColor = Color.Blue,
                VerticalOptions = LayoutOptions.Start,
                WidthRequest = vectorLabelSize
            };
            //stackLayoutVectorE.Children.Add(l_existing);
            resourceVectorE = "2    2    3";
            l_resourceVectorE = new Label
            {
                Text = "E = (    " + resourceVectorE + "    )",
                TextColor = Color.Blue,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.End
            };

            stackLayoutVectorE.Children.Add(l_resourceVectorE);
            stackLayout.Children.Add(stackLayoutVectorE);

            //VECTOR A
            StackLayout stackLayoutVectorA = new StackLayout { Orientation = StackOrientation.Horizontal };
            Label l_available = new Label { Text = "Available ",
                VerticalOptions = LayoutOptions.Start,
                TextColor = Color.Green,
                WidthRequest = vectorLabelSize
            };
            freeResourceVectorA = "2    2    3";
            l_freeResourceVectorA = new Label
            {
                Text = "A = (    " + freeResourceVectorA + "    )",
                TextColor = Color.Green,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            //stackLayoutVectorA.Children.Add(l_available);
            //stackLayoutVectorA.Children.Add(l_freeResourceVectorA);
            //stackLayout.Children.Add(stackLayoutVectorA);


            var l_Space0 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space0);

            StackLayout stackLayout2 = new StackLayout { Orientation = StackOrientation.Horizontal };

            //RUNNING PROCESSES
            var sl_runningProcesses = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_Processes = new Label
            {
                Text = "Running Processes",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center
            };
            p_runningprocesses = new Picker();
            for (int i = 2; i < 6; i++)
            {
                p_runningprocesses.Items.Add(i.ToString());
            }
            p_runningprocesses.SelectedIndex = 0;
            p_runningprocesses.SelectedIndexChanged += ProcessesChanged;
            sl_runningProcesses.Children.Add(l_Processes);
            sl_runningProcesses.Children.Add(p_runningprocesses);
            stackLayout.Children.Add(sl_runningProcesses);

            Label l_space8 = new Label { Text = " " };
            stackLayout.Children.Add(l_space8);

            // BUSY
            StackLayout stackLayoutBusy = new StackLayout { Orientation = StackOrientation.Horizontal };
            var l_resourcesBusy = new Label
            {
                Text = "Busy Resources ",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center
            };
            var b_ClearBusy = new Button {
                Text = "Clear",
                /*
                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                WidthRequest = 60,
                HeightRequest = 15
                */
            };
            b_ClearBusy.Clicked += B_ClearBusyClicked; //add Click Event(Method)
            stackLayoutBusy.Children.Add(l_resourcesBusy);
            var l_Space2 = new Label
            {
                Text = " ",
                WidthRequest = 88,
                HorizontalOptions = LayoutOptions.Center
            };
            stackLayoutBusy.Children.Add(l_Space2);
            stackLayoutBusy.Children.Add(b_ClearBusy);
            stackLayout.Children.Add(stackLayoutBusy);

            sl_busyResources = new StackLayout() { HorizontalOptions = LayoutOptions.Start };
            CreateBusyResourcesUI(sl_busyResources);
            stackLayout.Children.Add(sl_busyResources);

            StackLayout stackLayoutVectorB = new StackLayout { Orientation = StackOrientation.Horizontal };

            //Vector B
            busyResourceVectorB = "0    0    0";
            l_busyResourceVectorB = new Label
            {
                Text = "B = (    " + busyResourceVectorB + "    )",
                TextColor = Color.Red,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            stackLayoutVectorB.Children.Add(l_busyResourceVectorB);
            stackLayout.Children.Add(stackLayoutVectorB);


            var l_Space4 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space4);

            //upcomingVectorC REQUESTS
            StackLayout stackLayoutUpcoming = new StackLayout { Orientation = StackOrientation.Horizontal };
            var l_upcoming = new Label { Text = "Upcoming Requests ",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center
            };
            stackLayoutUpcoming.Children.Add(l_upcoming);

            var l_Space1 = new Label { Text = " ",
                WidthRequest = 50,
                HorizontalOptions = LayoutOptions.Center };
           
            stackLayoutUpcoming.Children.Add(l_Space1);

            var b_ClearUpcoming = new Button { Text = "Clear",
            
                /*FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                WidthRequest = 60,
                HeightRequest = 25
                */
            };
            b_ClearUpcoming.Clicked += B_ClearUpcoming_Clicked; //add Click Event(Method)
            stackLayoutUpcoming.Children.Add(b_ClearUpcoming);
            stackLayout.Children.Add(stackLayoutUpcoming);

            sl_upcomingRequests = new StackLayout() { };
            CreateUpcomingRequestsUI(sl_upcomingRequests);
            stackLayout.Children.Add(sl_upcomingRequests);

            //VECTOR C
            StackLayout stackLayoutVectorC = new StackLayout { Orientation = StackOrientation.Horizontal };
            upcomingVectorC = "0    0    0";
            l_upcomingVectorC = new Label
            {
                Text = "C = (    " + upcomingVectorC + "    )",
                TextColor = Color.Orange,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            stackLayoutVectorC.Children.Add(l_upcomingVectorC);
            stackLayout.Children.Add(stackLayoutVectorC);

            var l_Space5 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space5);

            StackLayout sl_buttons = new StackLayout { Orientation = StackOrientation.Horizontal };
            var b_ClearResources = new Button { Text = "Clear All", HorizontalOptions = LayoutOptions.Start };
            b_ClearResources.Clicked += B_ClearResources_Clicked; //add Click Event(Method)
            sl_buttons.Children.Add(b_ClearResources);

            //preset buttons
            Label l_space11 = new Label { Text = " ", WidthRequest = 40 };
            sl_buttons.Children.Add(l_space11);
            var b_preset1 = new Button { Text = "Preset 1", HorizontalOptions = LayoutOptions.Start };
            b_preset1.Clicked += B_Preset1_Clicked; //add Click Event(Method)
            sl_buttons.Children.Add(b_preset1);
            Label l_space13 = new Label { Text = " ", WidthRequest = 40 };
            sl_buttons.Children.Add(l_space13);
            var b_preset2 = new Button { Text = "Preset 2", HorizontalOptions = LayoutOptions.Start };
            b_preset2.Clicked += B_Preset2_Clicked; //add Click Event(Method)
            sl_buttons.Children.Add(b_preset2);
            Label l_space3 = new Label { Text = " ", WidthRequest = 40 };
            sl_buttons.Children.Add(l_space3);
            var b_preset3 = new Button { Text = "Preset 3", HorizontalOptions = LayoutOptions.Start };
            b_preset3.Clicked += B_Preset3_Clicked; //add Click Event(Method)
            sl_buttons.Children.Add(b_preset3);

            stackLayout.Children.Add(sl_buttons);

            var b_Start = new Button { Text = "Start" };
            b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            stackLayout.Children.Add(b_Start);
        }


        /**********************************************************************
        *********************************************************************/
        void CreateBusyResourcesUI(StackLayout sl_busyResources)
        {
            // stacklayouts for every process (contain the pickers)
            sl_busyProcess1 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_busyProcess2 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_busyProcess3 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_busyProcess4 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_busyProcess5 = new StackLayout() { Orientation = StackOrientation.Horizontal };

            l_busyP1 = new Label { Text = "P1:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_busyP2 = new Label { Text = "P2:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_busyP3 = new Label { Text = "P3:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_busyP4 = new Label { Text = "P4:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_busyP5 = new Label { Text = "P5:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };

            p_p1_dvd = new Picker() { WidthRequest = 40 };
            p_p2_dvd = new Picker() { WidthRequest = 40 };
            p_p3_dvd = new Picker() { WidthRequest = 40 };
            p_p4_dvd = new Picker() { WidthRequest = 40 };
            p_p5_dvd = new Picker() { WidthRequest = 40 };

            p_p1_usb = new Picker() { WidthRequest = 40 };
            p_p2_usb = new Picker() { WidthRequest = 40 };
            p_p3_usb = new Picker() { WidthRequest = 40 };
            p_p4_usb = new Picker() { WidthRequest = 40 };
            p_p5_usb = new Picker() { WidthRequest = 40 };

            p_p1_bluRay = new Picker() { WidthRequest = 40 };
            p_p2_bluRay = new Picker() { WidthRequest = 40 };
            p_p3_bluRay = new Picker() { WidthRequest = 40 };
            p_p4_bluRay = new Picker() { WidthRequest = 40 };
            p_p5_bluRay = new Picker() { WidthRequest = 40 };

            p_p1_printer = new Picker() { WidthRequest = 40 };
            p_p2_printer = new Picker() { WidthRequest = 40 };
            p_p3_printer = new Picker() { WidthRequest = 40 };
            p_p4_printer = new Picker() { WidthRequest = 40 };
            p_p5_printer = new Picker() { WidthRequest = 40 };

            p_p1_ijprinter = new Picker() { WidthRequest = 40 };
            p_p2_ijprinter = new Picker() { WidthRequest = 40 };
            p_p3_ijprinter = new Picker() { WidthRequest = 40 };
            p_p4_ijprinter = new Picker() { WidthRequest = 40 };
            p_p5_ijprinter = new Picker() { WidthRequest = 40 };

            p_p1_printer3D = new Picker() { WidthRequest = 40 };
            p_p2_printer3D = new Picker() { WidthRequest = 40 };
            p_p3_printer3D = new Picker() { WidthRequest = 40 };
            p_p4_printer3D = new Picker() { WidthRequest = 40 };
            p_p5_printer3D = new Picker() { WidthRequest = 40 };

            busyResPickerList.Add(p_p1_dvd);
            busyResPickerList.Add(p_p2_dvd);
            busyResPickerList.Add(p_p3_dvd);
            busyResPickerList.Add(p_p4_dvd);
            busyResPickerList.Add(p_p5_dvd);

            busyResPickerList.Add(p_p1_usb);
            busyResPickerList.Add(p_p2_usb);
            busyResPickerList.Add(p_p3_usb);
            busyResPickerList.Add(p_p4_usb);
            busyResPickerList.Add(p_p5_usb);

            busyResPickerList.Add(p_p1_bluRay);
            busyResPickerList.Add(p_p2_bluRay);
            busyResPickerList.Add(p_p3_bluRay);
            busyResPickerList.Add(p_p4_bluRay);
            busyResPickerList.Add(p_p5_bluRay);

            busyResPickerList.Add(p_p1_printer);
            busyResPickerList.Add(p_p2_printer);
            busyResPickerList.Add(p_p3_printer);
            busyResPickerList.Add(p_p4_printer);
            busyResPickerList.Add(p_p5_printer);

            busyResPickerList.Add(p_p1_ijprinter);
            busyResPickerList.Add(p_p2_ijprinter);
            busyResPickerList.Add(p_p3_ijprinter);
            busyResPickerList.Add(p_p4_ijprinter);
            busyResPickerList.Add(p_p5_ijprinter);

            busyResPickerList.Add(p_p1_printer3D);
            busyResPickerList.Add(p_p2_printer3D);
            busyResPickerList.Add(p_p3_printer3D);
            busyResPickerList.Add(p_p4_printer3D);
            busyResPickerList.Add(p_p5_printer3D);

            AddItemsToBusyResPickers();
            SetBusyResPickersToZero();
            AddPickersToBusyRes();

            sl_busyResources.Children.Add(sl_busyProcess1);
            sl_busyResources.Children.Add(sl_busyProcess2);

        }
        /**********************************************************************
        *********************************************************************/
        void CreateUpcomingRequestsUI(StackLayout stackLayout)
        {
            // stacklayouts for every process (contain the pickers)
            sl_upcomingProcess1 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_upcomingProcess2 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_upcomingProcess3 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_upcomingProcess4 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_upcomingProcess5 = new StackLayout() { Orientation = StackOrientation.Horizontal };

            l_upcomingP1 = new Label { Text = "P1:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40 };
            l_upcomingP2 = new Label { Text = "P2:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_upcomingP3 = new Label { Text = "P3:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_upcomingP4 = new Label { Text = "P4:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_upcomingP5 = new Label { Text = "P5:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };

            //pickers for every resource /process combination
            p_p1_upcoming_dvd = new Picker() { WidthRequest = 40 };
            p_p2_upcoming_dvd = new Picker() { WidthRequest = 40 };
            p_p3_upcoming_dvd = new Picker() { WidthRequest = 40 };
            p_p4_upcoming_dvd = new Picker() { WidthRequest = 40 };
            p_p5_upcoming_dvd = new Picker() { WidthRequest = 40 };

            p_p1_upcoming_usb = new Picker() { WidthRequest = 40 };
            p_p2_upcoming_usb = new Picker() { WidthRequest = 40 };
            p_p3_upcoming_usb = new Picker() { WidthRequest = 40 };
            p_p4_upcoming_usb = new Picker() { WidthRequest = 40 };
            p_p5_upcoming_usb = new Picker() { WidthRequest = 40 };

            p_p1_upcoming_bluRay = new Picker() { WidthRequest = 40 };
            p_p2_upcoming_bluRay = new Picker() { WidthRequest = 40 };
            p_p3_upcoming_bluRay = new Picker() { WidthRequest = 40 };
            p_p4_upcoming_bluRay = new Picker() { WidthRequest = 40 };
            p_p5_upcoming_bluRay = new Picker() { WidthRequest = 40 };

            p_p1_upcoming_printer = new Picker() { WidthRequest = 40 };
            p_p2_upcoming_printer = new Picker() { WidthRequest = 40 };
            p_p3_upcoming_printer = new Picker() { WidthRequest = 40 };
            p_p4_upcoming_printer = new Picker() { WidthRequest = 40 };
            p_p5_upcoming_printer = new Picker() { WidthRequest = 40 };

            p_p1_upcoming_ijprinter = new Picker() { WidthRequest = 40 };
            p_p2_upcoming_ijprinter = new Picker() { WidthRequest = 40 };
            p_p3_upcoming_ijprinter = new Picker() { WidthRequest = 40 };
            p_p4_upcoming_ijprinter = new Picker() { WidthRequest = 40 };
            p_p5_upcoming_ijprinter = new Picker() { WidthRequest = 40 };

            p_p1_upcoming_printer3D = new Picker() { WidthRequest = 40 };
            p_p2_upcoming_printer3D = new Picker() { WidthRequest = 40 };
            p_p3_upcoming_printer3D = new Picker() { WidthRequest = 40 };
            p_p4_upcoming_printer3D = new Picker() { WidthRequest = 40 };
            p_p5_upcoming_printer3D = new Picker() { WidthRequest = 40 };

            upcomingResPickerList.Add(p_p1_upcoming_dvd);
            upcomingResPickerList.Add(p_p2_upcoming_dvd);
            upcomingResPickerList.Add(p_p3_upcoming_dvd);
            upcomingResPickerList.Add(p_p4_upcoming_dvd);
            upcomingResPickerList.Add(p_p5_upcoming_dvd);

            upcomingResPickerList.Add(p_p1_upcoming_usb);
            upcomingResPickerList.Add(p_p2_upcoming_usb);
            upcomingResPickerList.Add(p_p3_upcoming_usb);
            upcomingResPickerList.Add(p_p4_upcoming_usb);
            upcomingResPickerList.Add(p_p5_upcoming_usb);

            upcomingResPickerList.Add(p_p1_upcoming_bluRay);
            upcomingResPickerList.Add(p_p2_upcoming_bluRay);
            upcomingResPickerList.Add(p_p3_upcoming_bluRay);
            upcomingResPickerList.Add(p_p4_upcoming_bluRay);
            upcomingResPickerList.Add(p_p5_upcoming_bluRay);

            upcomingResPickerList.Add(p_p1_upcoming_printer);
            upcomingResPickerList.Add(p_p2_upcoming_printer);
            upcomingResPickerList.Add(p_p3_upcoming_printer);
            upcomingResPickerList.Add(p_p4_upcoming_printer);
            upcomingResPickerList.Add(p_p5_upcoming_printer);

            upcomingResPickerList.Add(p_p1_upcoming_ijprinter);
            upcomingResPickerList.Add(p_p2_upcoming_ijprinter);
            upcomingResPickerList.Add(p_p3_upcoming_ijprinter);
            upcomingResPickerList.Add(p_p4_upcoming_ijprinter);
            upcomingResPickerList.Add(p_p5_upcoming_ijprinter);

            upcomingResPickerList.Add(p_p1_upcoming_printer3D);
            upcomingResPickerList.Add(p_p2_upcoming_printer3D);
            upcomingResPickerList.Add(p_p3_upcoming_printer3D);
            upcomingResPickerList.Add(p_p4_upcoming_printer3D);
            upcomingResPickerList.Add(p_p5_upcoming_printer3D);

            AddItemsToUpcomingResPickers();
            SetVectorChangedEvents();
            SetUpcomingResPickersToZero();
            AddPickersToUpcomingRes();

            sl_upcomingRequests.Children.Add(sl_upcomingProcess1);
            sl_upcomingRequests.Children.Add(sl_upcomingProcess2);
        }

        /**********************************************************************
        *********************************************************************/
        void CreateExistingResourcesUI(StackLayout stackLayout)
        {
            var stackLayoutRight = new StackLayout();
            var stackLayoutLeft = new StackLayout();
            var stackLayoutRLContainer = new StackLayout() { Orientation = StackOrientation.Horizontal };

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
            p_dvd.SelectedIndexChanged += VectorChanged4;
            resPickerList.Add(p_dvd);

            sl_dvd.Children.Add(p_dvd);
            sl_dvd.Children.Add(l_dvd);
            stackLayoutLeft.Children.Add(sl_dvd);

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
            p_printer.SelectedIndexChanged += VectorChanged4;
            resPickerList.Add(p_printer);

            sl_printer.Children.Add(p_printer);
            sl_printer.Children.Add(l_printer);
            stackLayoutLeft.Children.Add(sl_printer);

            // resource USB
            var sl_usb = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_usb = new Label { Text = " USB Disk Drives", VerticalOptions = LayoutOptions.Center };
            p_usb = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_usb.Items.Add(i.ToString());
            }
            p_usb.SelectedIndex = 3;
            p_usb.SelectedIndexChanged += VectorChanged;
            p_usb.SelectedIndexChanged += VectorChanged4;
            resPickerList.Add(p_usb);

            sl_usb.Children.Add(p_usb);
            sl_usb.Children.Add(l_usb);
            stackLayoutLeft.Children.Add(sl_usb);

            // resource BluRay
            var sl_bluRay = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_bluRay = new Label
            {
                Text = " BluRay Drives",
                VerticalOptions = LayoutOptions.Center
            };
            p_bluRay = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_bluRay.Items.Add(i.ToString());
            }
            p_bluRay.SelectedIndex = 0;
            p_bluRay.SelectedIndexChanged += VectorChanged;
            p_bluRay.SelectedIndexChanged += VectorChanged4;
            resPickerList.Add(p_bluRay);

            sl_bluRay.Children.Add(p_bluRay);
            sl_bluRay.Children.Add(l_bluRay);
            stackLayoutRight.Children.Add(sl_bluRay);


            // resource inkjet printer
            var sl_ijPrinter = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_ijPrinter = new Label
            {
                Text = " Inkjet Printers",
                VerticalOptions = LayoutOptions.Center
            };
            p_ijPrinter = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_ijPrinter.Items.Add(i.ToString());
            }
            p_ijPrinter.SelectedIndex = 0;
            p_ijPrinter.SelectedIndexChanged += VectorChanged;
            p_ijPrinter.SelectedIndexChanged += VectorChanged4;
            resPickerList.Add(p_ijPrinter);

            sl_ijPrinter.Children.Add(p_ijPrinter);
            sl_ijPrinter.Children.Add(l_ijPrinter);
            stackLayoutRight.Children.Add(sl_ijPrinter);

            // resource 3D printer
            var sl_printer3D = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_printer3D = new Label { Text = " 3D Printers", VerticalOptions = LayoutOptions.Center };
            p_printer3D = new Picker();
            for (int i = 0; i < 10; i++)
            {
                p_printer3D.Items.Add(i.ToString());
            }
            p_printer3D.SelectedIndex = 0;
            p_printer3D.SelectedIndexChanged += VectorChanged;
            p_printer3D.SelectedIndexChanged += VectorChanged4;
            resPickerList.Add(p_printer3D);

            sl_printer3D.Children.Add(p_printer3D);
            sl_printer3D.Children.Add(l_printer3D);
            stackLayoutRight.Children.Add(sl_printer3D);

            stackLayoutRight.HorizontalOptions = LayoutOptions.CenterAndExpand;
            stackLayoutRLContainer.Children.Add(stackLayoutLeft);
            stackLayoutRLContainer.Children.Add(stackLayoutRight);
            stackLayout.Children.Add(stackLayoutRLContainer);
        }

        /**********************************************************************
        *********************************************************************/
        void B_ClearBusyClicked(object sender, EventArgs e)
        {
            SetBusyResPickersToZero();
        }

        void B_ClearUpcoming_Clicked(object sender, EventArgs e)
        {
            SetUpcomingResPickersToZero();
        }

        void B_ClearResources_Clicked(object sender, EventArgs e)
        {
            SetResPickersToZero();
            p_dvd.SelectedIndex = 2;
            p_printer.SelectedIndex = 2;
            p_usb.SelectedIndex = 3;
        }

        /**********************************************************************
        *********************************************************************/
        void B_Preset1_Clicked(object sender, EventArgs e)
        {
            /*
            SetResPickersToZero();
            SetUpcomingResPickersToZero();
            SetBusyResPickersToZero();

            preset++;
            int presetExample = preset % 3;
            switch (presetExample)
            {
                case 0: SetPreset1(); break;
                case 1: SetPreset1(); break;
                case 2: SetPreset1(); break;
            }*/
            SetPreset1();
        }

        void B_Preset2_Clicked(object sender, EventArgs e)
        {

            SetPreset2();
        }

        void B_Preset3_Clicked(object sender, EventArgs e)
        {

            SetPreset3();
        }

        /**********************************************************************
        *********************************************************************/

        void SetPreset1()
        {
            //dvd, printer, usb, bluRay, ijPrinter, printer3D
            // set resources
            p_dvd.SelectedIndex = 6;
            p_printer.SelectedIndex = 3;
            p_usb.SelectedIndex = 4;
            p_bluRay.SelectedIndex = 2;
            p_ijPrinter.SelectedIndex = 0;
            p_printer3D.SelectedIndex = 0;

            //set process total
            p_runningprocesses.SelectedIndex = 5;

            //set busy resources p1
            p_p1_dvd.SelectedIndex = 3;
            p_p1_usb.SelectedIndex = 1;
            p_p1_bluRay.SelectedIndex = 1;

            //set busy resources p2
            p_p2_printer.SelectedIndex = 1;

            //set busy resources p3
            p_p3_dvd.SelectedIndex = 1;
            p_p3_printer.SelectedIndex = 1;
            p_p3_usb.SelectedIndex = 1;

            //set busy resources p4
            p_p4_dvd.SelectedIndex = 1;
            p_p4_printer.SelectedIndex = 1;
            p_p4_bluRay.SelectedIndex = 1;

            //set busy resources p5
            //none


            //set future requests p1
            p_p1_upcoming_dvd.SelectedIndex = 1;
            p_p1_upcoming_printer.SelectedIndex = 1;

            //set future requests  p2
            p_p2_upcoming_printer.SelectedIndex = 1;
            p_p2_upcoming_usb.SelectedIndex = 1;
            p_p2_upcoming_bluRay.SelectedIndex = 2;

            //set future requests p3
            p_p3_upcoming_dvd.SelectedIndex = 3;
            p_p3_upcoming_printer.SelectedIndex = 1;

            //set future requests p4
            p_p4_upcoming_usb.SelectedIndex = 1;

            //set future requests p5
            p_p5_upcoming_dvd.SelectedIndex = 2;
            p_p5_upcoming_printer.SelectedIndex = 1;
            p_p5_upcoming_usb.SelectedIndex = 1;

        }

        void SetPreset2()
        {

        }

        void SetPreset3()
        {

        }

        /**********************************************************************
        *********************************************************************/
        void ProcessesChanged(object sender, EventArgs e)
        {
            sl_busyResources.Children.Clear();
            sl_upcomingRequests.Children.Clear();
            var total = Int16.Parse(p_runningprocesses.SelectedItem.ToString());

            if (total >= 2)
            {
                sl_busyResources.Children.Add(sl_busyProcess1);
                sl_busyResources.Children.Add(sl_busyProcess2);

                sl_upcomingRequests.Children.Add(sl_upcomingProcess1);
                sl_upcomingRequests.Children.Add(sl_upcomingProcess2);

            }
            if (total >= 3)
            {
                sl_busyResources.Children.Add(sl_busyProcess3);
                sl_upcomingRequests.Children.Add(sl_upcomingProcess3);

            }
            if (total >= 4)
            {
                sl_busyResources.Children.Add(sl_busyProcess4);
                sl_upcomingRequests.Children.Add(sl_upcomingProcess4);
            }
            if (total >= 5)
            {
                sl_busyResources.Children.Add(sl_busyProcess5);
                sl_upcomingRequests.Children.Add(sl_upcomingProcess5);

            }

        }

        //Vector changed für Vector E berechnen
        /**********************************************************************
        *********************************************************************/
        void VectorChanged(object sender, EventArgs e)
        {
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();
            printer3D = p_printer3D.SelectedItem.ToString();
            int total = 0;

            resourceVectorE = "";
            if (dvd != "0") { resourceVectorE = "" + resourceVectorE + "    " + dvd; total++; }
            if (printer != "0") { resourceVectorE = "" + resourceVectorE + "    " + printer; total++; }
            if (usb != "0") { resourceVectorE = "" + resourceVectorE + "    " + usb; total++; }
            if (bluRay != "0") { resourceVectorE = "" + resourceVectorE + "    " + bluRay; total++; }
            if (ijPrinter != "0") { resourceVectorE = "" + resourceVectorE + "    " + ijPrinter; total++; }
            if (printer3D != "0") { resourceVectorE = "" + resourceVectorE + "    " + printer3D; total++; }
            String vectorEText = "E = (" + resourceVectorE + "    )";

            string attention = "\nPick between 2 and 5 resources!";
            if (total < 2 || total > 5)
            {
                vectorEText = vectorEText + " " + attention;
            }
            vectorEText = vectorEText.Replace("\n", System.Environment.NewLine);
            l_resourceVectorE.Text = vectorEText;
        }

        //calculate Vector for busy ressources (VECTOR B) 
        void SetVectorB()
        {
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();
            printer3D = p_printer3D.SelectedItem.ToString();

            //Get number of processes
            var total = Int16.Parse(p_runningprocesses.SelectedItem.ToString());

            //calculate vector B
            int busy_dvd = 0;
            int busy_printer = 0;
            int busy_usb = 0;
            int busy_bluRay = 0;
            int busy_ijprinter = 0;
            int busy_printer3D = 0;

            if (total >= 2)
            {
                busy_dvd = busy_dvd + Int32.Parse(p_p1_dvd.SelectedItem.ToString())
                    + Int32.Parse(p_p2_dvd.SelectedItem.ToString());

                busy_printer = busy_printer + Int32.Parse(p_p1_printer.SelectedItem.ToString())
                    + Int32.Parse(p_p2_printer.SelectedItem.ToString());

                busy_usb = busy_usb + Int32.Parse(p_p1_usb.SelectedItem.ToString())
                  + Int32.Parse(p_p2_usb.SelectedItem.ToString());

                busy_bluRay = busy_bluRay + Int32.Parse(p_p1_bluRay.SelectedItem.ToString())
                  + Int32.Parse(p_p2_bluRay.SelectedItem.ToString());

                busy_ijprinter = busy_ijprinter + Int32.Parse(p_p1_ijprinter.SelectedItem.ToString())
                  + Int32.Parse(p_p2_ijprinter.SelectedItem.ToString());

                busy_printer3D = busy_printer3D + Int32.Parse(p_p1_printer3D.SelectedItem.ToString())
                    + Int32.Parse(p_p2_printer3D.SelectedItem.ToString());

            }
            if (total >= 3)
            {
                busy_dvd = busy_dvd + Int32.Parse(p_p3_dvd.SelectedItem.ToString());
                busy_printer = busy_printer + Int32.Parse(p_p3_printer.SelectedItem.ToString());
                busy_usb = busy_usb + Int32.Parse(p_p3_usb.SelectedItem.ToString());
                busy_bluRay = busy_bluRay + Int32.Parse(p_p3_bluRay.SelectedItem.ToString());
                busy_ijprinter = busy_ijprinter + Int32.Parse(p_p3_ijprinter.SelectedItem.ToString());
                busy_printer3D = busy_printer3D + Int32.Parse(p_p3_printer3D.SelectedItem.ToString());
            }
            if (total >= 4)
            {
                busy_dvd = busy_dvd + Int32.Parse(p_p4_dvd.SelectedItem.ToString());
                busy_printer = busy_printer + Int32.Parse(p_p4_printer.SelectedItem.ToString());
                busy_usb = busy_usb + Int32.Parse(p_p4_usb.SelectedItem.ToString());
                busy_bluRay = busy_bluRay + Int32.Parse(p_p4_bluRay.SelectedItem.ToString());
                busy_ijprinter = busy_ijprinter + Int32.Parse(p_p4_ijprinter.SelectedItem.ToString());
                busy_printer3D = busy_printer3D + Int32.Parse(p_p4_printer3D.SelectedItem.ToString());
            }
            if (total >= 5)
            {
                busy_dvd = busy_dvd + Int32.Parse(p_p5_dvd.SelectedItem.ToString());
                busy_printer = busy_printer + Int32.Parse(p_p5_printer.SelectedItem.ToString());
                busy_usb = busy_usb + Int32.Parse(p_p5_usb.SelectedItem.ToString());
                busy_bluRay = busy_bluRay + Int32.Parse(p_p5_bluRay.SelectedItem.ToString());
                busy_ijprinter = busy_ijprinter + Int32.Parse(p_p5_ijprinter.SelectedItem.ToString());
                busy_printer3D = busy_printer3D + Int32.Parse(p_p5_printer3D.SelectedItem.ToString());

            }

            busyResourceVectorB = "";
            if (dvd != "0") { busyResourceVectorB = "" + busyResourceVectorB + "    " + busy_dvd; }
            if (printer != "0") { busyResourceVectorB = "" + busyResourceVectorB + "    " + busy_printer; }
            if (usb != "0") { busyResourceVectorB = "" + busyResourceVectorB + "    " + busy_usb; }
            if (bluRay != "0") { busyResourceVectorB = "" + busyResourceVectorB + "    " + busy_bluRay; }
            if (ijPrinter != "0") { busyResourceVectorB = "" + busyResourceVectorB + "    " + busy_ijprinter; }
            if (printer3D != "0") { busyResourceVectorB = "" + busyResourceVectorB + "    " + busy_printer3D; }
            String vectorBText = "B = (" + busyResourceVectorB + "    )";

            string attention = "\nCannot occupy more resources than available!";
            if (busy_dvd > Int32.Parse(dvd) ||
                busy_printer > Int32.Parse(printer) ||
                busy_usb > Int32.Parse(usb) ||
                busy_bluRay > Int32.Parse(bluRay) ||
                busy_printer3D > Int32.Parse(printer3D) ||
                busy_ijprinter > Int32.Parse(ijPrinter)
                )
            {
                vectorBText = vectorBText + " " + attention;
            }

            vectorBText = vectorBText.Replace("\n", System.Environment.NewLine);
            l_busyResourceVectorB.Text = vectorBText;

        }

        //calculate Vector for busy ressources (VECTOR C) 
        void SetVectorC()
        {
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();
            printer3D = p_printer3D.SelectedItem.ToString();

            //Get number of processes
            var total = Int16.Parse(p_runningprocesses.SelectedItem.ToString());

            //calculate vector C
            int upcoming_dvd = 0;
            int upcoming_printer = 0;
            int upcoming_usb = 0;
            int upcoming_bluRay = 0;
            int upcoming_ijprinter = 0;
            int upcoming_printer3D = 0;

            if (total >= 2)
            {
                upcoming_dvd = upcoming_dvd + Int32.Parse(p_p1_upcoming_dvd.SelectedItem.ToString())
                    + Int32.Parse(p_p2_upcoming_dvd.SelectedItem.ToString());

                upcoming_printer = upcoming_printer + Int32.Parse(p_p1_upcoming_printer.SelectedItem.ToString())
                    + Int32.Parse(p_p2_upcoming_printer.SelectedItem.ToString());

                upcoming_usb = upcoming_usb + Int32.Parse(p_p1_upcoming_usb.SelectedItem.ToString())
                  + Int32.Parse(p_p2_upcoming_usb.SelectedItem.ToString());

                upcoming_bluRay = upcoming_bluRay + Int32.Parse(p_p1_upcoming_bluRay.SelectedItem.ToString())
                  + Int32.Parse(p_p2_upcoming_bluRay.SelectedItem.ToString());

                upcoming_ijprinter = upcoming_ijprinter + Int32.Parse(p_p1_upcoming_ijprinter.SelectedItem.ToString())
                  + Int32.Parse(p_p2_upcoming_ijprinter.SelectedItem.ToString());

                upcoming_printer3D = upcoming_printer3D + Int32.Parse(p_p1_upcoming_printer3D.SelectedItem.ToString())
                    + Int32.Parse(p_p2_upcoming_printer3D.SelectedItem.ToString());

            }
            if (total >= 3)
            {
                upcoming_dvd = upcoming_dvd + Int32.Parse(p_p3_upcoming_dvd.SelectedItem.ToString());
                upcoming_printer = upcoming_printer + Int32.Parse(p_p3_upcoming_printer.SelectedItem.ToString());
                upcoming_usb = upcoming_usb + Int32.Parse(p_p3_upcoming_usb.SelectedItem.ToString());
                upcoming_bluRay = upcoming_bluRay + Int32.Parse(p_p3_upcoming_bluRay.SelectedItem.ToString());
                upcoming_ijprinter = upcoming_ijprinter + Int32.Parse(p_p3_upcoming_ijprinter.SelectedItem.ToString());
                upcoming_printer3D = upcoming_printer3D + Int32.Parse(p_p3_upcoming_printer3D.SelectedItem.ToString());
            }
            if (total >= 4)
            {
                upcoming_dvd = upcoming_dvd + Int32.Parse(p_p4_upcoming_dvd.SelectedItem.ToString());
                upcoming_printer = upcoming_printer + Int32.Parse(p_p4_upcoming_printer.SelectedItem.ToString());
                upcoming_usb = upcoming_usb + Int32.Parse(p_p4_upcoming_usb.SelectedItem.ToString());
                upcoming_bluRay = upcoming_bluRay + Int32.Parse(p_p4_upcoming_bluRay.SelectedItem.ToString());
                upcoming_ijprinter = upcoming_ijprinter + Int32.Parse(p_p4_upcoming_ijprinter.SelectedItem.ToString());
                upcoming_printer3D = upcoming_printer3D + Int32.Parse(p_p4_upcoming_printer3D.SelectedItem.ToString());
            }
            if (total >= 5)
            {
                upcoming_dvd = upcoming_dvd + Int32.Parse(p_p5_upcoming_dvd.SelectedItem.ToString());
                upcoming_printer = upcoming_printer + Int32.Parse(p_p5_upcoming_printer.SelectedItem.ToString());
                upcoming_usb = upcoming_usb + Int32.Parse(p_p5_upcoming_usb.SelectedItem.ToString());
                upcoming_bluRay = upcoming_bluRay + Int32.Parse(p_p5_upcoming_bluRay.SelectedItem.ToString());
                upcoming_ijprinter = upcoming_ijprinter + Int32.Parse(p_p5_upcoming_ijprinter.SelectedItem.ToString());
                upcoming_printer3D = upcoming_printer3D + Int32.Parse(p_p5_upcoming_printer3D.SelectedItem.ToString());

            }

            upcomingVectorC = ""; 
            if (dvd != "0") { upcomingVectorC = "" + upcomingVectorC + "    " + upcoming_dvd; }
            if (printer != "0") { upcomingVectorC = "" + upcomingVectorC + "    " + upcoming_printer; }
            if (usb != "0") { upcomingVectorC = "" + upcomingVectorC + "    " + upcoming_usb; }
            if (bluRay != "0") { upcomingVectorC = "" + upcomingVectorC + "    " + upcoming_bluRay; }
            if (ijPrinter != "0") { upcomingVectorC = "" + upcomingVectorC + "    " + upcoming_ijprinter; }
            if (printer3D != "0") { upcomingVectorC = "" + upcomingVectorC + "    " + upcoming_printer3D; }
            String vectorCText = "C = (" + upcomingVectorC + "    )";

            l_upcomingVectorC.Text = vectorCText;

        }
        //calculate Vector for busy ressources (VECTOR C) 
        /**********************************************************************
        *********************************************************************/
        void VectorCChangedEvent(object sender, EventArgs e)
        {
            try
            {
                SetVectorC();
            }
            catch (System.NullReferenceException ex)
            {
                Debug.WriteLine(ex);
                //Exception is thrown because VectorChanged2 Event is also invoked when items are removed from picker  
            }

        }

        //calculate Vector for busy ressources (VECTOR B) 
        /**********************************************************************
        *********************************************************************/
        void VectorBChangedEvent(object sender, EventArgs e)
        {
            try
            {
                SetVectorB();
            }
            catch (System.NullReferenceException ex) {
                Debug.WriteLine(ex);
                //Exception is thrown because VectorChanged2 Event is also invoked when items are removed from picker  
            }

        }

        //calculate Vector for free ressources (VECTOR A) 
        /**********************************************************************
        *********************************************************************/
        void VectorAChangedEvent(object sender, EventArgs e)
        {
            try
            {
                SetVectorA();
            }
            catch (System.NullReferenceException ex)
            {
                Debug.WriteLine(ex);
                //Exception is thrown because VectorChanged3 Event is also invoked when items are removed from picker  
            }
        }

        void SetVectorA()
        {
            //get number of ressources
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();
            printer3D = p_printer3D.SelectedItem.ToString();

            //Get number of processes
            var total = Int16.Parse(p_runningprocesses.SelectedItem.ToString());

            //calculate vector B
            int busy_dvd = 0;
            int busy_printer = 0;
            int busy_usb = 0;
            int busy_bluRay = 0;
            int busy_ijprinter = 0;
            int busy_printer3D = 0;

            if (total >= 2)
            {
                busy_dvd = busy_dvd + Int32.Parse(p_p1_dvd.SelectedItem.ToString())
                    + Int32.Parse(p_p2_dvd.SelectedItem.ToString());

                busy_printer = busy_printer + Int32.Parse(p_p1_printer.SelectedItem.ToString())
                    + Int32.Parse(p_p2_printer.SelectedItem.ToString());

                busy_usb = busy_usb + Int32.Parse(p_p1_usb.SelectedItem.ToString())
                  + Int32.Parse(p_p2_usb.SelectedItem.ToString());

                busy_bluRay = busy_bluRay + Int32.Parse(p_p1_bluRay.SelectedItem.ToString())
                  + Int32.Parse(p_p2_bluRay.SelectedItem.ToString());

                busy_ijprinter = busy_ijprinter + Int32.Parse(p_p1_ijprinter.SelectedItem.ToString())
                  + Int32.Parse(p_p2_ijprinter.SelectedItem.ToString());

                busy_printer3D = busy_printer3D + Int32.Parse(p_p1_printer3D.SelectedItem.ToString())
                    + Int32.Parse(p_p2_printer3D.SelectedItem.ToString());

            }
            if (total >= 3)
            {
                busy_dvd = busy_dvd + Int32.Parse(p_p3_dvd.SelectedItem.ToString());
                busy_printer = busy_printer + Int32.Parse(p_p3_printer.SelectedItem.ToString());
                busy_usb = busy_usb + Int32.Parse(p_p3_usb.SelectedItem.ToString());
                busy_bluRay = busy_bluRay + Int32.Parse(p_p3_bluRay.SelectedItem.ToString());
                busy_ijprinter = busy_ijprinter + Int32.Parse(p_p3_ijprinter.SelectedItem.ToString());
                busy_printer3D = busy_printer3D + Int32.Parse(p_p3_printer3D.SelectedItem.ToString());
            }
            if (total >= 4)
            {
                busy_dvd = busy_dvd + Int32.Parse(p_p4_dvd.SelectedItem.ToString());
                busy_printer = busy_printer + Int32.Parse(p_p4_printer.SelectedItem.ToString());
                busy_usb = busy_usb + Int32.Parse(p_p4_usb.SelectedItem.ToString());
                busy_bluRay = busy_bluRay + Int32.Parse(p_p4_bluRay.SelectedItem.ToString());
                busy_ijprinter = busy_ijprinter + Int32.Parse(p_p4_ijprinter.SelectedItem.ToString());
                busy_printer3D = busy_printer3D + Int32.Parse(p_p4_printer3D.SelectedItem.ToString());

            }
            if (total >= 5)
            {
                busy_dvd = busy_dvd + Int32.Parse(p_p5_dvd.SelectedItem.ToString());
                busy_printer = busy_printer + Int32.Parse(p_p5_printer.SelectedItem.ToString());
                busy_usb = busy_usb + Int32.Parse(p_p5_usb.SelectedItem.ToString());
                busy_bluRay = busy_bluRay + Int32.Parse(p_p5_bluRay.SelectedItem.ToString());
                busy_ijprinter = busy_ijprinter + Int32.Parse(p_p5_ijprinter.SelectedItem.ToString());
                busy_printer3D = busy_printer3D + Int32.Parse(p_p5_printer3D.SelectedItem.ToString());

            }

            //calculate free ressources vector
            int free_dvd = Int32.Parse(dvd) - busy_dvd;
            int free_printer = Int32.Parse(printer) - busy_printer;
            int free_usb = Int32.Parse(usb) - busy_usb;
            int free_bluRay = Int32.Parse(bluRay) - busy_bluRay;
            int free_ijPrinter = Int32.Parse(ijPrinter) - busy_ijprinter;
            int free_printer3D = Int32.Parse(printer3D) - busy_printer3D;


            freeResourceVectorA = "";
            if (dvd != "0") { freeResourceVectorA = "" + freeResourceVectorA + "    " + free_dvd; }
            if (printer != "0") { freeResourceVectorA = "" + freeResourceVectorA + "    " + free_printer; }
            if (usb != "0") { freeResourceVectorA = "" + freeResourceVectorA + "    " + free_usb; }
            if (bluRay != "0") { freeResourceVectorA = "" + freeResourceVectorA + "    " + free_bluRay; }
            if (ijPrinter != "0") { freeResourceVectorA = "" + freeResourceVectorA + "    " + free_ijPrinter; }
            if (printer3D != "0") { freeResourceVectorA = "" + freeResourceVectorA + "    " + free_printer3D; }

            l_freeResourceVectorA.Text = "A = (" + freeResourceVectorA + "    )";

        }

        //show Ocuupied and Requested Ressources Picker
        void VectorChanged4(object sender, EventArgs e)
        {
            AddItemsToBusyResPickers();
            AddItemsToUpcomingResPickers();
            SetUpcomingResPickersToZero();
            SetBusyResPickersToZero();
            SetVectorB();
            SetVectorC();
            SetVectorA();
            AddPickersToBusyRes();
            AddPickersToUpcomingRes();

        }

        void SetVectorChangedEvents()
        {
            foreach (Picker picker in busyResPickerList)
            {
                picker.SelectedIndexChanged += VectorBChangedEvent;
                picker.SelectedIndexChanged += VectorAChangedEvent;
            }
            foreach (Picker picker in upcomingResPickerList)
            {
                picker.SelectedIndexChanged += VectorCChangedEvent;
            }
        }

        void SetResPickersToZero()
        {
            foreach (Picker picker in resPickerList)
            {
                picker.SelectedIndex = 0;
            }

        }

        void SetUpcomingResPickersToZero()
        {
            foreach (Picker picker in upcomingResPickerList)
            {
                picker.SelectedIndex = 0;
            }
               
        }

        void SetBusyResPickersToZero()
        {
            foreach (Picker picker in busyResPickerList)
            {
                picker.SelectedIndex = 0;
            }
        }

        void AddItemsToUpcomingResPickers()
        {
            //get number of ressources
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();
            printer3D = p_printer3D.SelectedItem.ToString();


            //first remove all items
            foreach (Picker picker in upcomingResPickerList)
            {
                picker.Items.Clear();
            }

            //add item for every ressource
            for (int i = 0; i <= UInt32.Parse(dvd); i++)
            {
                p_p1_upcoming_dvd.Items.Add(i.ToString());
                p_p2_upcoming_dvd.Items.Add(i.ToString());
                p_p3_upcoming_dvd.Items.Add(i.ToString());
                p_p4_upcoming_dvd.Items.Add(i.ToString());
                p_p5_upcoming_dvd.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(printer); i++)
            {
                p_p1_upcoming_printer.Items.Add(i.ToString());
                p_p2_upcoming_printer.Items.Add(i.ToString());
                p_p3_upcoming_printer.Items.Add(i.ToString());
                p_p4_upcoming_printer.Items.Add(i.ToString());
                p_p5_upcoming_printer.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(usb); i++)
            {
                p_p1_upcoming_usb.Items.Add(i.ToString());
                p_p2_upcoming_usb.Items.Add(i.ToString());
                p_p3_upcoming_usb.Items.Add(i.ToString());
                p_p4_upcoming_usb.Items.Add(i.ToString());
                p_p5_upcoming_usb.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(bluRay); i++)
            {
                p_p1_upcoming_bluRay.Items.Add(i.ToString());
                p_p2_upcoming_bluRay.Items.Add(i.ToString());
                p_p3_upcoming_bluRay.Items.Add(i.ToString());
                p_p4_upcoming_bluRay.Items.Add(i.ToString());
                p_p5_upcoming_bluRay.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(ijPrinter); i++)
            {
                p_p1_upcoming_ijprinter.Items.Add(i.ToString());
                p_p2_upcoming_ijprinter.Items.Add(i.ToString());
                p_p3_upcoming_ijprinter.Items.Add(i.ToString());
                p_p4_upcoming_ijprinter.Items.Add(i.ToString());
                p_p5_upcoming_ijprinter.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(printer3D); i++)
            {
                p_p1_upcoming_printer3D.Items.Add(i.ToString());
                p_p2_upcoming_printer3D.Items.Add(i.ToString());
                p_p3_upcoming_printer3D.Items.Add(i.ToString());
                p_p4_upcoming_printer3D.Items.Add(i.ToString());
                p_p5_upcoming_printer3D.Items.Add(i.ToString());
            }

        }
        void AddItemsToBusyResPickers()
        {
            //get number of ressources
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();
            printer3D = p_printer3D.SelectedItem.ToString();

            //first remove all items
            foreach (Picker picker in busyResPickerList)
            {
                picker.Items.Clear();
            }

            //add item for every ressource
            for (int i = 0; i <= UInt32.Parse(dvd); i++)
            {
                p_p1_dvd.Items.Add(i.ToString());
                p_p2_dvd.Items.Add(i.ToString());
                p_p3_dvd.Items.Add(i.ToString());
                p_p4_dvd.Items.Add(i.ToString());
                p_p5_dvd.Items.Add(i.ToString());

            }
            for (int i = 0; i <= UInt32.Parse(printer); i++)
            {
                p_p1_printer.Items.Add(i.ToString());
                p_p2_printer.Items.Add(i.ToString());
                p_p3_printer.Items.Add(i.ToString());
                p_p4_printer.Items.Add(i.ToString());
                p_p5_printer.Items.Add(i.ToString());

            }
            for (int i = 0; i <= UInt32.Parse(usb); i++)
            {
                p_p1_usb.Items.Add(i.ToString());
                p_p2_usb.Items.Add(i.ToString());
                p_p3_usb.Items.Add(i.ToString());
                p_p4_usb.Items.Add(i.ToString());
                p_p5_usb.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(bluRay); i++)
            {
                p_p1_bluRay.Items.Add(i.ToString());
                p_p2_bluRay.Items.Add(i.ToString());
                p_p3_bluRay.Items.Add(i.ToString());
                p_p4_bluRay.Items.Add(i.ToString());
                p_p5_bluRay.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(ijPrinter); i++)
            {
                p_p1_ijprinter.Items.Add(i.ToString());
                p_p2_ijprinter.Items.Add(i.ToString());
                p_p3_ijprinter.Items.Add(i.ToString());
                p_p4_ijprinter.Items.Add(i.ToString());
                p_p5_ijprinter.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(printer3D); i++)
            {
                p_p1_printer3D.Items.Add(i.ToString());
                p_p2_printer3D.Items.Add(i.ToString());
                p_p3_printer3D.Items.Add(i.ToString());
                p_p4_printer3D.Items.Add(i.ToString());
                p_p5_printer3D.Items.Add(i.ToString());

            }
        }


        void AddPickersToBusyRes()
        {
            // get values from ressource pickers
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();
            printer3D = p_printer3D.SelectedItem.ToString();

            //remove all process pickers 
            sl_busyProcess1.Children.Clear();
            sl_busyProcess2.Children.Clear();
            sl_busyProcess3.Children.Clear();
            sl_busyProcess4.Children.Clear();
            sl_busyProcess5.Children.Clear();

            // add labels to process layouts
            sl_busyProcess1.Children.Add(l_busyP1);
            sl_busyProcess2.Children.Add(l_busyP2);
            sl_busyProcess3.Children.Add(l_busyP3);
            sl_busyProcess4.Children.Add(l_busyP4);
            sl_busyProcess5.Children.Add(l_busyP5);
            
            if (dvd != "0")
            {
                sl_busyProcess1.Children.Add(p_p1_dvd);
                sl_busyProcess2.Children.Add(p_p2_dvd);
                sl_busyProcess3.Children.Add(p_p3_dvd);
                sl_busyProcess4.Children.Add(p_p4_dvd);
                sl_busyProcess5.Children.Add(p_p5_dvd);
            }
            if (printer != "0")
            {
                sl_busyProcess1.Children.Add(p_p1_printer);
                sl_busyProcess2.Children.Add(p_p2_printer);
                sl_busyProcess3.Children.Add(p_p3_printer);
                sl_busyProcess4.Children.Add(p_p4_printer);
                sl_busyProcess5.Children.Add(p_p5_printer);
            }
            if (usb != "0")
            {
                sl_busyProcess1.Children.Add(p_p1_usb);
                sl_busyProcess2.Children.Add(p_p2_usb);
                sl_busyProcess3.Children.Add(p_p3_usb);
                sl_busyProcess4.Children.Add(p_p4_usb);
                sl_busyProcess5.Children.Add(p_p5_usb);
            }
            if (bluRay != "0")
            {
                sl_busyProcess1.Children.Add(p_p1_bluRay);
                sl_busyProcess2.Children.Add(p_p2_bluRay);
                sl_busyProcess3.Children.Add(p_p3_bluRay);
                sl_busyProcess4.Children.Add(p_p4_bluRay);
                sl_busyProcess5.Children.Add(p_p5_bluRay);
            }
            if (ijPrinter != "0")
            {
                sl_busyProcess1.Children.Add(p_p1_ijprinter);
                sl_busyProcess2.Children.Add(p_p2_ijprinter);
                sl_busyProcess3.Children.Add(p_p3_ijprinter);
                sl_busyProcess4.Children.Add(p_p4_ijprinter);
                sl_busyProcess5.Children.Add(p_p5_ijprinter);
            }
            if (printer3D != "0")
            {
                sl_busyProcess1.Children.Add(p_p1_printer3D);
                sl_busyProcess2.Children.Add(p_p2_printer3D);
                sl_busyProcess3.Children.Add(p_p3_printer3D);
                sl_busyProcess4.Children.Add(p_p4_printer3D);
                sl_busyProcess5.Children.Add(p_p5_printer3D);
            }
        }
        void AddPickersToUpcomingRes()
        {
            // get values from ressource pickers
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();
            printer3D = p_printer3D.SelectedItem.ToString();


            //remove all process pickers
            sl_upcomingProcess1.Children.Clear();
            sl_upcomingProcess2.Children.Clear();
            sl_upcomingProcess3.Children.Clear();
            sl_upcomingProcess4.Children.Clear();
            sl_upcomingProcess5.Children.Clear();

            // add labels to process layouts
            sl_upcomingProcess1.Children.Add(l_upcomingP1);
            sl_upcomingProcess2.Children.Add(l_upcomingP2);
            sl_upcomingProcess3.Children.Add(l_upcomingP3);
            sl_upcomingProcess4.Children.Add(l_upcomingP4);
            sl_upcomingProcess5.Children.Add(l_upcomingP5);

            if (dvd != "0")
            {
                sl_upcomingProcess1.Children.Add(p_p1_upcoming_dvd);
                sl_upcomingProcess2.Children.Add(p_p2_upcoming_dvd);
                sl_upcomingProcess3.Children.Add(p_p3_upcoming_dvd);
                sl_upcomingProcess4.Children.Add(p_p4_upcoming_dvd);
                sl_upcomingProcess5.Children.Add(p_p5_upcoming_dvd);

            }
            if (printer != "0")
            {
                sl_upcomingProcess1.Children.Add(p_p1_upcoming_printer);
                sl_upcomingProcess2.Children.Add(p_p2_upcoming_printer);
                sl_upcomingProcess3.Children.Add(p_p3_upcoming_printer);
                sl_upcomingProcess4.Children.Add(p_p4_upcoming_printer);
                sl_upcomingProcess5.Children.Add(p_p5_upcoming_printer);
            }
            if (usb != "0")
            {
                sl_upcomingProcess1.Children.Add(p_p1_upcoming_usb);
                sl_upcomingProcess2.Children.Add(p_p2_upcoming_usb);
                sl_upcomingProcess3.Children.Add(p_p3_upcoming_usb);
                sl_upcomingProcess4.Children.Add(p_p4_upcoming_usb);
                sl_upcomingProcess5.Children.Add(p_p5_upcoming_usb);
            }
            if (bluRay != "0")
            {
                sl_upcomingProcess1.Children.Add(p_p1_upcoming_bluRay);
                sl_upcomingProcess2.Children.Add(p_p2_upcoming_bluRay);
                sl_upcomingProcess3.Children.Add(p_p3_upcoming_bluRay);
                sl_upcomingProcess4.Children.Add(p_p4_upcoming_bluRay);
                sl_upcomingProcess5.Children.Add(p_p5_upcoming_bluRay);
            }

            if (ijPrinter != "0")
            {
                sl_upcomingProcess1.Children.Add(p_p1_upcoming_ijprinter);
                sl_upcomingProcess2.Children.Add(p_p2_upcoming_ijprinter);
                sl_upcomingProcess3.Children.Add(p_p3_upcoming_ijprinter);
                sl_upcomingProcess4.Children.Add(p_p4_upcoming_ijprinter);
                sl_upcomingProcess5.Children.Add(p_p5_upcoming_ijprinter);
            }
            if (printer3D != "0")
            {
                sl_upcomingProcess1.Children.Add(p_p1_upcoming_printer3D);
                sl_upcomingProcess2.Children.Add(p_p2_upcoming_printer3D);
                sl_upcomingProcess3.Children.Add(p_p3_upcoming_printer3D);
                sl_upcomingProcess4.Children.Add(p_p4_upcoming_printer3D);
                sl_upcomingProcess5.Children.Add(p_p5_upcoming_printer3D);
            }
        }

        /**********************************************************************
        *********************************************************************/
        //If Button Start is clicked
        async void B_Start_Clicked(object sender, EventArgs e)
        {
            //check if resources between 2 and 5
            var countE = 0;
            foreach (Char c in resourceVectorE){
                if (Char.IsDigit(c))
                {
                    countE++;
                }
            }
            if (countE < 3) {
                await DisplayAlert("Alert", "Please define at least 3 resources", "OK");
            }
            else if (countE > 5)
            {
                await DisplayAlert("Alert", "Please define no more than 5 resources", "OK");
            }
            //check busy resources can't be higher than available ressources
            else if (l_busyResourceVectorB.Text.Contains("\nCannot occupy more resources than available!"))
            {
                await DisplayAlert("Alert", "Busy resources can't be higher than available ressources", "OK");
            }
            else
            {
                if (!IsLandscape())
                {
                    await DisplayAlert("Alert", "Please hold your phone horizontally for landscape mode", "OK");
                }

                var exResDict = GetExistingResDict();
                //todo: only use the numbers
                String VE = GetOnlyDigitsInString(l_resourceVectorE.Text);
                String VB = GetOnlyDigitsInString(l_busyResourceVectorB.Text);
                String VC = GetOnlyDigitsInString(l_upcomingVectorC.Text);
                String VA = GetOnlyDigitsInString(l_freeResourceVectorA.Text);
                int totalProcesses = Int16.Parse(p_runningprocesses.SelectedItem.ToString());
                await Navigation.PushAsync(new Deadlock(exResDict, VE, VB, VC, VA, totalProcesses, GetVectorBProcesses(), GetVectorCProcesses()));
            }

        }

        /**********************************************************************
        *********************************************************************/
        String GetOnlyDigitsInString(String givenString)
        {
            String digits = "";
            foreach (Char c in givenString)
            {
                if (Char.IsDigit(c))
                {
                    digits += c;
                }
            }
            return digits;
        }

        Dictionary<String, int> GetExistingResDict()
        {
            var exResDict = new Dictionary<string, int>(){};
            exResDict.Add("dvd", Int16.Parse(p_dvd.SelectedItem.ToString()));
            exResDict.Add("printer", Int16.Parse(p_printer.SelectedItem.ToString()));
            exResDict.Add("usb", Int16.Parse(p_usb.SelectedItem.ToString()));
            exResDict.Add("bluRay", Int16.Parse(p_bluRay.SelectedItem.ToString()));
            exResDict.Add("ijPrinter", Int16.Parse(p_ijPrinter.SelectedItem.ToString()));
            exResDict.Add("printer3D", Int16.Parse(p_printer3D.SelectedItem.ToString()));

            return exResDict;
        }

        public Dictionary<int, String> GetVectorBProcesses()
        {
         
            // create dictionary (keys= numbers 1 to 5 for processes 1 to 5)
            vectorBProcesses = new Dictionary<int, String>() { };

            // Vector values
            String textP1 = ""
                + p_p1_dvd.SelectedItem.ToString()
                + p_p1_printer.SelectedItem.ToString()
                + p_p1_usb.SelectedItem.ToString()
                + p_p1_bluRay.SelectedItem.ToString()
                + p_p1_ijprinter.SelectedItem.ToString()
                + p_p1_printer3D.SelectedItem.ToString();
            String textP2 = ""
                + p_p2_dvd.SelectedItem.ToString()
                + p_p2_printer.SelectedItem.ToString()
                + p_p2_usb.SelectedItem.ToString()
                + p_p2_bluRay.SelectedItem.ToString()
                + p_p2_ijprinter.SelectedItem.ToString()
                + p_p2_printer3D.SelectedItem.ToString();
            String textP3 = ""
                + p_p3_dvd.SelectedItem.ToString()
                + p_p3_printer.SelectedItem.ToString()
                + p_p3_usb.SelectedItem.ToString()
                + p_p3_bluRay.SelectedItem.ToString()
                + p_p3_ijprinter.SelectedItem.ToString()
                + p_p3_printer3D.SelectedItem.ToString();
            String textP4 = ""
                + p_p4_dvd.SelectedItem.ToString()
                + p_p4_printer.SelectedItem.ToString()
                + p_p4_usb.SelectedItem.ToString()
                + p_p4_bluRay.SelectedItem.ToString()
                + p_p4_ijprinter.SelectedItem.ToString()
                + p_p4_printer3D.SelectedItem.ToString();
            String textP5 = ""
                + p_p5_dvd.SelectedItem.ToString()
                + p_p5_printer.SelectedItem.ToString()
                + p_p5_usb.SelectedItem.ToString()
                + p_p5_bluRay.SelectedItem.ToString()
                + p_p5_ijprinter.SelectedItem.ToString()
                + p_p5_printer3D.SelectedItem.ToString();

            // get values from ressource pickers
            dvd = p_dvd.SelectedItem.ToString(); //index 0
            printer = p_printer.SelectedItem.ToString(); //index 1
            usb = p_usb.SelectedItem.ToString(); //index 2
            bluRay = p_bluRay.SelectedItem.ToString(); //index 3
            ijPrinter = p_ijPrinter.SelectedItem.ToString(); //index 4
            printer3D = p_printer3D.SelectedItem.ToString(); //index 5
            // remove unavailable ressources (starting from the last one)
            if (printer3D == "0") {
                textP1 = textP1.Remove(4, 1);
                textP2 = textP2.Remove(5, 1); textP3 = textP3.Remove(5, 1);
                textP4 = textP4.Remove(5, 1); textP5 = textP5.Remove(5, 1);
            }
            if (ijPrinter == "0") {
                textP1 = textP1.Remove(4, 1);
                textP2 = textP2.Remove(4, 1); textP3 = textP3.Remove(4, 1);
                textP4 = textP4.Remove(4, 1); textP5 = textP5.Remove(4, 1);
            }
            if (bluRay == "0") {
                textP1 = textP1.Remove(3, 1);
                textP2 = textP2.Remove(3, 1); textP3 = textP3.Remove(3, 1);
                textP4 = textP4.Remove(3, 1); textP5 = textP5.Remove(3, 1);
            }
            if (usb == "0") {
                textP1 = textP1.Remove(2, 1);
                textP2 = textP2.Remove(2, 1); textP3 = textP3.Remove(2, 1);
                textP4 = textP4.Remove(2, 1); textP5 = textP5.Remove(2, 1);
            }
            if (printer == "0") {
                textP1 = textP1.Remove(1, 1);
                textP2 = textP2.Remove(1, 1); textP3 = textP3.Remove(1, 1);
                textP4 = textP4.Remove(1, 1); textP5 = textP5.Remove(1, 1);
            }
            if (dvd == "0") {
                textP1 = textP1.Remove(0, 1);
                textP2 = textP2.Remove(0, 1); textP3 = textP3.Remove(0, 1);
                textP4 = textP4.Remove(0, 1); textP5 = textP5.Remove(0, 1);
            }

            // add key value pairs to dictionary
            int total = Int16.Parse(p_runningprocesses.SelectedItem.ToString());
            if(total >= 2)
            {
                vectorBProcesses.Add(1, textP1);
                vectorBProcesses.Add(2, textP2);
            }
            if(total >= 3){vectorBProcesses.Add(3, textP3);}
            if(total >= 4){vectorBProcesses.Add(4, textP4);}
            if(total >= 5){vectorBProcesses.Add(5, textP5);}

            return vectorBProcesses;
        }
        public Dictionary<int, String> GetVectorCProcesses()
        {
            // create dictionary (keys= numbers 1 to 5 for processes 1 to 5)
            vectorCProcesses = new Dictionary<int, String>() { };

            // Vector values
            String textP1 = ""
                + p_p1_upcoming_dvd.SelectedItem.ToString()
                + p_p1_upcoming_printer.SelectedItem.ToString()
                + p_p1_upcoming_usb.SelectedItem.ToString()
                + p_p1_upcoming_bluRay.SelectedItem.ToString()
                + p_p1_upcoming_ijprinter.SelectedItem.ToString()
                + p_p1_upcoming_printer3D.SelectedItem.ToString();
            String textP2 = ""
                + p_p2_upcoming_dvd.SelectedItem.ToString()
                + p_p2_upcoming_printer.SelectedItem.ToString()
                + p_p2_upcoming_usb.SelectedItem.ToString()
                + p_p2_upcoming_bluRay.SelectedItem.ToString()
                + p_p2_upcoming_ijprinter.SelectedItem.ToString()
                + p_p2_upcoming_printer3D.SelectedItem.ToString();
            String textP3 = ""
                + p_p3_upcoming_dvd.SelectedItem.ToString()
                + p_p3_upcoming_printer.SelectedItem.ToString()
                + p_p3_upcoming_usb.SelectedItem.ToString()
                + p_p3_upcoming_bluRay.SelectedItem.ToString()
                + p_p3_upcoming_ijprinter.SelectedItem.ToString()
                + p_p3_upcoming_printer3D.SelectedItem.ToString();
            String textP4 = ""
                + p_p4_upcoming_dvd.SelectedItem.ToString()
                + p_p4_upcoming_printer.SelectedItem.ToString()
                + p_p4_upcoming_usb.SelectedItem.ToString()
                + p_p4_upcoming_bluRay.SelectedItem.ToString()
                + p_p4_upcoming_ijprinter.SelectedItem.ToString()
                + p_p4_upcoming_printer3D.SelectedItem.ToString();
            String textP5 = ""
                + p_p5_upcoming_dvd.SelectedItem.ToString()
                + p_p5_upcoming_printer.SelectedItem.ToString()
                + p_p5_upcoming_usb.SelectedItem.ToString()
                + p_p5_upcoming_bluRay.SelectedItem.ToString()
                + p_p5_upcoming_ijprinter.SelectedItem.ToString()
                + p_p5_upcoming_printer3D.SelectedItem.ToString();

            // get values from ressource pickers
            dvd = p_dvd.SelectedItem.ToString(); //index 0
            printer = p_printer.SelectedItem.ToString(); //index 1
            usb = p_usb.SelectedItem.ToString(); //index 2
            bluRay = p_bluRay.SelectedItem.ToString(); //index 3
            ijPrinter = p_ijPrinter.SelectedItem.ToString(); //index 4
            printer3D = p_printer3D.SelectedItem.ToString(); //index 5

            // remove unavailable ressources (starting from the last one)
            if (printer3D == "0")
            {
                textP1 = textP1.Remove(5, 1);
                textP2 = textP2.Remove(5, 1); textP3 = textP3.Remove(5, 1);
                textP4 = textP4.Remove(5, 1); textP5 = textP5.Remove(5, 1);
            }
            if (ijPrinter == "0")
            {
                textP1 = textP1.Remove(4, 1);
                textP2 = textP2.Remove(4, 1); textP3 = textP3.Remove(4, 1);
                textP4 = textP4.Remove(4, 1); textP5 = textP5.Remove(4, 1);
            }
            if (bluRay == "0")
            {
                textP1 = textP1.Remove(3, 1);
                textP2 = textP2.Remove(3, 1); textP3 = textP3.Remove(3, 1);
                textP4 = textP4.Remove(3, 1); textP5 = textP5.Remove(3, 1);
            }
            if (usb == "0")
            {
                textP1 = textP1.Remove(2, 1);
                textP2 = textP2.Remove(2, 1); textP3 = textP3.Remove(2, 1);
                textP4 = textP4.Remove(2, 1); textP5 = textP5.Remove(2, 1);
            }
            if (printer == "0")
            {
                textP1 = textP1.Remove(1, 1);
                textP2 = textP2.Remove(1, 1); textP3 = textP3.Remove(1, 1);
                textP4 = textP4.Remove(1, 1); textP5 = textP5.Remove(1, 1);
            }
            if (dvd == "0")
            {
                textP1 = textP1.Remove(0, 1);
                textP2 = textP2.Remove(0, 1); textP3 = textP3.Remove(0, 1);
                textP4 = textP4.Remove(0, 1); textP5 = textP5.Remove(0, 1);
            }

            // add key value pairs to dictionary
            int total = Int16.Parse(p_runningprocesses.SelectedItem.ToString());
            if (total >= 2)
            {
                vectorCProcesses.Add(1, textP1);
                vectorCProcesses.Add(2, textP2);
            }
            if (total >= 3) { vectorCProcesses.Add(3, textP3); }
            if (total >= 4) { vectorCProcesses.Add(4, textP4); }
            if (total >= 5) { vectorCProcesses.Add(5, textP5); }

            return vectorCProcesses;
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
