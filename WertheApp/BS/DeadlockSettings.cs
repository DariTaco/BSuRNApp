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
        private Picker p_dvd, p_usb, p_bluRay, p_printer, p_ijPrinter, p_total;

        private Label l_occuP1, l_occuP2, l_occuP3, l_occuP4, l_occuP5;
        private Label l_reqP1, l_reqP2, l_reqP3, l_reqP4, l_reqP5;

        private Picker p_p1_dvd, p_p2_dvd, p_p3_dvd, p_p4_dvd, p_p5_dvd;
        private Picker p_p1_usb, p_p2_usb, p_p3_usb, p_p4_usb, p_p5_usb;
        private Picker p_p1_bluRay, p_p2_bluRay, p_p3_bluRay, p_p4_bluRay, p_p5_bluRay;
        private Picker p_p1_printer, p_p2_printer, p_p3_printer, p_p4_printer, p_p5_printer;
        private Picker p_p1_ijprinter, p_p2_ijprinter, p_p3_ijprinter, p_p4_ijprinter, p_p5_ijprinter;
        private int occu_dvd, occu_usb, occu_bluRay, occu_printer, occu_ijprinter;


        private Picker p_p1_req_dvd, p_p2_req_dvd, p_p3_req_dvd, p_p4_req_dvd, p_p5_req_dvd;
        private Picker p_p1_req_usb, p_p2_req_usb, p_p3_req_usb, p_p4_req_usb, p_p5_req_usb;
        private Picker p_p1_req_bluRay, p_p2_req_bluRay, p_p3_req_bluRay, p_p4_req_bluRay, p_p5_req_bluRay;
        private Picker p_p1_req_printer, p_p2_req_printer, p_p3_req_printer, p_p4_req_printer, p_p5_req_printer;
        private Picker p_p1_req_ijprinter, p_p2_req_ijprinter, p_p3_req_ijprinter, p_p4_req_ijprinter, p_p5_req_ijprinter;
        private int req_dvd, req_usb, req_bluRay, req_printer, req_ijprinter;

        private Label l_resourceVectorE, l_occupiedResourceVectorB, l_freeResourceVectorA;
        private Entry e_occuP1, e_occuP2, e_occuP3, e_occuP4, e_occuP5,
            e_reqP1, e_reqP2, e_reqP3, e_reqP4, e_reqP5;
        private StackLayout sl_occupiedResources, sl_requestedResources;
        private StackLayout sl_OccuProcess1, sl_OccuProcess2, sl_OccuProcess3, sl_OccuProcess4, sl_OccuProcess5;
        private StackLayout sl_ReqProcess1, sl_ReqProcess2, sl_ReqProcess3, sl_ReqProcess4, sl_ReqProcess5;

        private List<Picker> occuResPickerList, reqResPickerList;


        public DeadlockSettings()
        {
            Title = "Deadlock";

            occuResPickerList = new List<Picker>();
            reqResPickerList = new List<Picker>();

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

            var l_available = new Label { Text = "Resources", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))};
            stackLayout.Children.Add(l_available);

            CreateResourcesUI(stackLayout);

            var l_Space2 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space2);

            StackLayout stackLayout2 = new StackLayout { Orientation = StackOrientation.Horizontal };

            // occupied resources
            var l_resource_occupied = new Label { Text = "Occupied Resources:" };
            stackLayout.Children.Add(l_resource_occupied);

            sl_occupiedResources = new StackLayout() { HorizontalOptions = LayoutOptions.Start};
            CreateProcessesOccupiedResourcesUI(sl_occupiedResources);
            stackLayout.Children.Add(sl_occupiedResources);

            occupiedResourceVectorB = "0000";
            l_occupiedResourceVectorB = new Label { Text = "Vector B = (" + occupiedResourceVectorB + ")", TextColor = Color.Red };
            sl_occupiedResources.Children.Add(l_occupiedResourceVectorB);
            freeResourceVectorA = "2231";
            stackLayout.Children.Add(l_occupiedResourceVectorB);

            var l_Space4 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space4);

            // requested resources
            var l_requestedResources = new Label { Text = "Resource Requests:" };
            stackLayout.Children.Add(l_requestedResources);

            sl_requestedResources = new StackLayout() { };
            CreateProcessesRequestedResourcesUI(sl_requestedResources);
            stackLayout.Children.Add(sl_requestedResources);

            var l_Space3 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space3);
            var b_Default = new Button { Text = "Set Default", HorizontalOptions = LayoutOptions.Start };
            b_Default.Clicked += B_Default_Clicked; //add Click Event(Method)
            stackLayout.Children.Add(b_Default);

            var l_Space5 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space5);

            var l_freeResources = new Label { Text = "Free Resources", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label) )};
            stackLayout.Children.Add(l_freeResources);
            l_freeResourceVectorA = new Label { Text = "Vector A = (" + freeResourceVectorA + ")", TextColor = Color.Green};
            sl_requestedResources.Children.Add(l_freeResourceVectorA);
            stackLayout.Children.Add(l_freeResourceVectorA);


            var b_Start = new Button { Text = "Start" };
            b_Start.Clicked += B_Start_Clicked; //add Click Event(Method)

            var l_Space = new Label { Text = " " };
            stackLayout.Children.Add(l_Space);
            stackLayout.Children.Add(b_Start);
        }


        /**********************************************************************
        *********************************************************************/
        void CreateProcessesOccupiedResourcesUI(StackLayout sl_occupiedResources)
        {
            // stacklayouts for every process (contain the pickers)
            sl_OccuProcess1 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_OccuProcess2 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_OccuProcess3 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_OccuProcess4 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_OccuProcess5 = new StackLayout() { Orientation = StackOrientation.Horizontal };

            l_occuP1 = new Label { Text = "P1:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_occuP2 = new Label { Text = "P2:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_occuP3 = new Label { Text = "P3:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_occuP4 = new Label { Text = "P4:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_occuP5 = new Label { Text = "P5:  ",
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

            occuResPickerList.Add(p_p1_dvd);
            occuResPickerList.Add(p_p2_dvd);
            occuResPickerList.Add(p_p3_dvd);
            occuResPickerList.Add(p_p4_dvd);
            occuResPickerList.Add(p_p5_dvd);

            occuResPickerList.Add(p_p1_usb);
            occuResPickerList.Add(p_p2_usb);
            occuResPickerList.Add(p_p3_usb);
            occuResPickerList.Add(p_p4_usb);
            occuResPickerList.Add(p_p5_usb);

            occuResPickerList.Add(p_p1_bluRay);
            occuResPickerList.Add(p_p2_bluRay);
            occuResPickerList.Add(p_p3_bluRay);
            occuResPickerList.Add(p_p4_bluRay);
            occuResPickerList.Add(p_p5_bluRay);

            occuResPickerList.Add(p_p1_printer);
            occuResPickerList.Add(p_p2_printer);
            occuResPickerList.Add(p_p3_printer);
            occuResPickerList.Add(p_p4_printer);
            occuResPickerList.Add(p_p5_printer);

            occuResPickerList.Add(p_p1_ijprinter);
            occuResPickerList.Add(p_p2_ijprinter);
            occuResPickerList.Add(p_p3_ijprinter);
            occuResPickerList.Add(p_p4_ijprinter);
            occuResPickerList.Add(p_p5_ijprinter);

            AddItemsToOccuResPickers();
            SetOccuResPickersToZero();
            SetVectorChangedEvents();
            AddPickersToOccuRes();

            sl_occupiedResources.Children.Add(sl_OccuProcess1);
            sl_occupiedResources.Children.Add(sl_OccuProcess2);
            sl_occupiedResources.Children.Add(sl_OccuProcess3);

        }
        /**********************************************************************
        *********************************************************************/
        void CreateProcessesRequestedResourcesUI(StackLayout stackLayout)
        {
            // stacklayouts for every process (contain the pickers)
            sl_ReqProcess1 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_ReqProcess2 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_ReqProcess3 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_ReqProcess4 = new StackLayout() { Orientation = StackOrientation.Horizontal };
            sl_ReqProcess5 = new StackLayout() { Orientation = StackOrientation.Horizontal };

            l_reqP1 = new Label { Text = "P1:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40 };
            l_reqP2 = new Label { Text = "P2:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_reqP3 = new Label { Text = "P3:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_reqP4 = new Label { Text = "P4:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };
            l_reqP5 = new Label { Text = "P5:  ",
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = 40
            };

            //pickers for every resource /process combination
            p_p1_req_dvd = new Picker() { WidthRequest = 40 };
            p_p2_req_dvd = new Picker() { WidthRequest = 40 };
            p_p3_req_dvd = new Picker() { WidthRequest = 40 };
            p_p4_req_dvd = new Picker() { WidthRequest = 40 };
            p_p5_req_dvd = new Picker() { WidthRequest = 40 };

            p_p1_req_usb = new Picker() { WidthRequest = 40 };
            p_p2_req_usb = new Picker() { WidthRequest = 40 };
            p_p3_req_usb = new Picker() { WidthRequest = 40 };
            p_p4_req_usb = new Picker() { WidthRequest = 40 };
            p_p5_req_usb = new Picker() { WidthRequest = 40 };

            p_p1_req_bluRay = new Picker() { WidthRequest = 40 };
            p_p2_req_bluRay = new Picker() { WidthRequest = 40 };
            p_p3_req_bluRay = new Picker() { WidthRequest = 40 };
            p_p4_req_bluRay = new Picker() { WidthRequest = 40 };
            p_p5_req_bluRay = new Picker() { WidthRequest = 40 };

            p_p1_req_printer = new Picker() { WidthRequest = 40 };
            p_p2_req_printer = new Picker() { WidthRequest = 40 };
            p_p3_req_printer = new Picker() { WidthRequest = 40 };
            p_p4_req_printer = new Picker() { WidthRequest = 40 };
            p_p5_req_printer = new Picker() { WidthRequest = 40 };

            p_p1_req_ijprinter = new Picker() { WidthRequest = 40 };
            p_p2_req_ijprinter = new Picker() { WidthRequest = 40 };
            p_p3_req_ijprinter = new Picker() { WidthRequest = 40 };
            p_p4_req_ijprinter = new Picker() { WidthRequest = 40 };
            p_p5_req_ijprinter = new Picker() { WidthRequest = 40 };

            reqResPickerList.Add(p_p1_req_dvd);
            reqResPickerList.Add(p_p2_req_dvd);
            reqResPickerList.Add(p_p3_req_dvd);
            reqResPickerList.Add(p_p4_req_dvd);
            reqResPickerList.Add(p_p5_req_dvd);

            reqResPickerList.Add(p_p1_req_usb);
            reqResPickerList.Add(p_p2_req_usb);
            reqResPickerList.Add(p_p3_req_usb);
            reqResPickerList.Add(p_p4_req_usb);
            reqResPickerList.Add(p_p5_req_usb);

            reqResPickerList.Add(p_p1_req_bluRay);
            reqResPickerList.Add(p_p2_req_bluRay);
            reqResPickerList.Add(p_p3_req_bluRay);
            reqResPickerList.Add(p_p4_req_bluRay);
            reqResPickerList.Add(p_p5_req_bluRay);

            reqResPickerList.Add(p_p1_req_printer);
            reqResPickerList.Add(p_p2_req_printer);
            reqResPickerList.Add(p_p3_req_printer);
            reqResPickerList.Add(p_p4_req_printer);
            reqResPickerList.Add(p_p5_req_printer);

            reqResPickerList.Add(p_p1_req_ijprinter);
            reqResPickerList.Add(p_p2_req_ijprinter);
            reqResPickerList.Add(p_p3_req_ijprinter);
            reqResPickerList.Add(p_p4_req_ijprinter);
            reqResPickerList.Add(p_p5_req_ijprinter);

            AddItemsToReqResPickers();
            SetReqResPickersToZero();
            AddPickersToReqRes();

            sl_requestedResources.Children.Add(sl_ReqProcess1);
            sl_requestedResources.Children.Add(sl_ReqProcess2);
            sl_requestedResources.Children.Add(sl_ReqProcess3);
        }

        /**********************************************************************
        *********************************************************************/
        void CreateResourcesUI(StackLayout stackLayout)
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
            p_bluRay.SelectedIndex = 1;
            p_bluRay.SelectedIndexChanged += VectorChanged;
            p_bluRay.SelectedIndexChanged += VectorChanged4;

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

            sl_ijPrinter.Children.Add(p_ijPrinter);
            sl_ijPrinter.Children.Add(l_ijPrinter);
            stackLayoutRight.Children.Add(sl_ijPrinter);

            stackLayoutRight.HorizontalOptions = LayoutOptions.CenterAndExpand;
            stackLayoutRLContainer.Children.Add(stackLayoutLeft);
            stackLayoutRLContainer.Children.Add(stackLayoutRight);
            stackLayout.Children.Add(stackLayoutRLContainer);

            // resource vector
            resourceVectorE = "2231";
            l_resourceVectorE = new Label { Text = "Vector E = (" + resourceVectorE + ")", TextColor = Color.Blue };
            stackLayout.Children.Add(l_resourceVectorE);

            var l_Space0 = new Label { Text = " " };
            stackLayout.Children.Add(l_Space0);

            var l_Processes = new Label { Text = "Processes", FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) };
            stackLayout.Children.Add(l_Processes);
            var sl_total = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_total = new Label { Text = "Total: ", VerticalOptions = LayoutOptions.Center };
            p_total = new Picker();
            for (int i = 2; i < 6; i++)
            {
                p_total.Items.Add(i.ToString());
            }
            p_total.SelectedIndex = 1;
            p_total.SelectedIndexChanged += ProcessesChanged;
            sl_total.Children.Add(l_total);
            sl_total.Children.Add(p_total);
            stackLayout.Children.Add(sl_total);
        }

        /**********************************************************************
        *********************************************************************/
        void B_Default_Clicked(object sender, EventArgs e)
        {
            SetReqResPickersToZero();
            SetOccuResPickersToZero();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Preset_Clicked(object sender, EventArgs e)
        {

            
        }

        /**********************************************************************
        *********************************************************************/
        void ProcessesChanged(object sender, EventArgs e)
        {
            sl_occupiedResources.Children.Clear();
            sl_requestedResources.Children.Clear();
            var total = Int16.Parse(p_total.SelectedItem.ToString());

            if (total >= 2)
            {
                sl_occupiedResources.Children.Add(sl_OccuProcess1);
                sl_occupiedResources.Children.Add(sl_OccuProcess2);

                sl_requestedResources.Children.Add(sl_ReqProcess1);
                sl_requestedResources.Children.Add(sl_ReqProcess2);

            }
            if (total >= 3)
            {
                sl_occupiedResources.Children.Add(sl_OccuProcess3);
                sl_requestedResources.Children.Add(sl_ReqProcess3);

            }
            if (total >= 4)
            {
                sl_occupiedResources.Children.Add(sl_OccuProcess4);
                sl_requestedResources.Children.Add(sl_ReqProcess4);
            }
            if (total >= 5)
            {
                sl_occupiedResources.Children.Add(sl_OccuProcess5);
                sl_requestedResources.Children.Add(sl_ReqProcess5);

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

            resourceVectorE = "";
            if(dvd != "0"){ resourceVectorE = "" + resourceVectorE + "" + dvd; }
            if (printer != "0") { resourceVectorE = "" + resourceVectorE + "" + printer; }
            if (usb != "0") { resourceVectorE = "" + resourceVectorE + "" + usb; }
            if (bluRay != "0") { resourceVectorE = "" + resourceVectorE + "" + bluRay; }
            if (ijPrinter != "0") { resourceVectorE = "" + resourceVectorE + "" + ijPrinter; }


            l_resourceVectorE.Text = "Vector E = (" + resourceVectorE + ")";
        }

        //calculate Vector for occupied ressources (VECTOR B) 
        void SetVectorB()
        {
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();

            //Get number of processes
            var total = Int16.Parse(p_total.SelectedItem.ToString());

            //calculate vector B
            int occu_dvd = 0;
            int occu_printer = 0;
            int occu_usb = 0;
            int occu_bluRay = 0;
            int occu_ijprinter = 0;
            if (total >= 2)
            {
                occu_dvd = occu_dvd + Int32.Parse(p_p1_dvd.SelectedItem.ToString())
                    + Int32.Parse(p_p2_dvd.SelectedItem.ToString());

                occu_printer = occu_printer + Int32.Parse(p_p1_printer.SelectedItem.ToString())
                    + Int32.Parse(p_p2_printer.SelectedItem.ToString());

                occu_usb = occu_usb + Int32.Parse(p_p1_usb.SelectedItem.ToString())
                  + Int32.Parse(p_p2_usb.SelectedItem.ToString());

                occu_bluRay = occu_bluRay + Int32.Parse(p_p1_bluRay.SelectedItem.ToString())
                  + Int32.Parse(p_p2_bluRay.SelectedItem.ToString());

                occu_ijprinter = occu_ijprinter + Int32.Parse(p_p1_ijprinter.SelectedItem.ToString())
                  + Int32.Parse(p_p2_ijprinter.SelectedItem.ToString());

            }
            if (total >= 3)
            {
                occu_dvd = occu_dvd + Int32.Parse(p_p3_dvd.SelectedItem.ToString());
                occu_printer = occu_printer + Int32.Parse(p_p3_printer.SelectedItem.ToString());
                occu_usb = occu_usb + Int32.Parse(p_p3_usb.SelectedItem.ToString());
                occu_bluRay = occu_bluRay + Int32.Parse(p_p3_bluRay.SelectedItem.ToString());
                occu_ijprinter = occu_ijprinter + Int32.Parse(p_p3_ijprinter.SelectedItem.ToString());
            }
            if (total >= 4)
            {
                occu_dvd = occu_dvd + Int32.Parse(p_p4_dvd.SelectedItem.ToString());
                occu_printer = occu_printer + Int32.Parse(p_p4_printer.SelectedItem.ToString());
                occu_usb = occu_usb + Int32.Parse(p_p4_usb.SelectedItem.ToString());
                occu_bluRay = occu_bluRay + Int32.Parse(p_p4_bluRay.SelectedItem.ToString());
                occu_ijprinter = occu_ijprinter + Int32.Parse(p_p4_ijprinter.SelectedItem.ToString());

            }
            if (total >= 5)
            {
                occu_dvd = occu_dvd + Int32.Parse(p_p5_dvd.SelectedItem.ToString());
                occu_printer = occu_printer + Int32.Parse(p_p5_printer.SelectedItem.ToString());
                occu_usb = occu_usb + Int32.Parse(p_p5_usb.SelectedItem.ToString());
                occu_bluRay = occu_bluRay + Int32.Parse(p_p5_bluRay.SelectedItem.ToString());
                occu_ijprinter = occu_ijprinter + Int32.Parse(p_p5_ijprinter.SelectedItem.ToString());

            }

            occupiedResourceVectorB = "";
            if (dvd != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_dvd; }
            if (printer != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_printer; }
            if (usb != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_usb; }
            if (bluRay != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_bluRay; }
            if (ijPrinter != "0") { occupiedResourceVectorB = "" + occupiedResourceVectorB + "" + occu_ijprinter; }

            String vectorBText = "Vector B = (" + occupiedResourceVectorB + ")";

            string attention = "!!! Cannot occupy more resources than available!";
            if (occu_dvd > Int32.Parse(dvd) ||
                occu_printer > Int32.Parse(printer) ||
                occu_usb > Int32.Parse(usb) ||
                occu_bluRay > Int32.Parse(bluRay) ||
                occu_ijprinter > Int32.Parse(ijPrinter)
                )
            {
                vectorBText = vectorBText + " " + attention;
            }


            l_occupiedResourceVectorB.Text = vectorBText;

        }
        //calculate Vector for occupied ressources (VECTOR B) 
        /**********************************************************************
        *********************************************************************/
        void VectorChanged2(object sender, EventArgs e)
        {
            try {
                SetVectorB();
            }
            catch (System.NullReferenceException ex) {
              //Exception is thrown because VectorChanged2 Event is also invoked when items are removed from picker  
            }
            
        }

        //TODO: Vector changed3 für free ressources (VECTOR A) berechnet aus total ressources und occupied
        /**********************************************************************
        *********************************************************************/
        void VectorChanged3(object sender, EventArgs e)
        {
            try
            {
                SetVectorA();
            }
            catch (System.NullReferenceException ex)
            {
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

            //Get number of processes
            var total = Int16.Parse(p_total.SelectedItem.ToString());

            //calculate vector B
            int occu_dvd = 0;
            int occu_printer = 0;
            int occu_usb = 0;
            int occu_bluRay = 0;
            int occu_ijprinter = 0;

            if (total >= 2)
            {
                occu_dvd = occu_dvd + Int32.Parse(p_p1_dvd.SelectedItem.ToString())
                    + Int32.Parse(p_p2_dvd.SelectedItem.ToString());

                occu_printer = occu_printer + Int32.Parse(p_p1_printer.SelectedItem.ToString())
                    + Int32.Parse(p_p2_printer.SelectedItem.ToString());

                occu_usb = occu_usb + Int32.Parse(p_p1_usb.SelectedItem.ToString())
                  + Int32.Parse(p_p2_usb.SelectedItem.ToString());

                occu_bluRay = occu_bluRay + Int32.Parse(p_p1_bluRay.SelectedItem.ToString())
                  + Int32.Parse(p_p2_bluRay.SelectedItem.ToString());

                occu_ijprinter = occu_ijprinter + Int32.Parse(p_p1_ijprinter.SelectedItem.ToString())
                  + Int32.Parse(p_p2_ijprinter.SelectedItem.ToString());

            }
            if (total >= 3)
            {
                occu_dvd = occu_dvd + Int32.Parse(p_p3_dvd.SelectedItem.ToString());
                occu_printer = occu_printer + Int32.Parse(p_p3_printer.SelectedItem.ToString());
                occu_usb = occu_usb + Int32.Parse(p_p3_usb.SelectedItem.ToString());
                occu_bluRay = occu_bluRay + Int32.Parse(p_p3_bluRay.SelectedItem.ToString());
                occu_ijprinter = occu_ijprinter + Int32.Parse(p_p3_ijprinter.SelectedItem.ToString());
            }
            if (total >= 4)
            {
                occu_dvd = occu_dvd + Int32.Parse(p_p4_dvd.SelectedItem.ToString());
                occu_printer = occu_printer + Int32.Parse(p_p4_printer.SelectedItem.ToString());
                occu_usb = occu_usb + Int32.Parse(p_p4_usb.SelectedItem.ToString());
                occu_bluRay = occu_bluRay + Int32.Parse(p_p4_bluRay.SelectedItem.ToString());
                occu_ijprinter = occu_ijprinter + Int32.Parse(p_p4_ijprinter.SelectedItem.ToString());

            }
            if (total >= 5)
            {
                occu_dvd = occu_dvd + Int32.Parse(p_p5_dvd.SelectedItem.ToString());
                occu_printer = occu_printer + Int32.Parse(p_p5_printer.SelectedItem.ToString());
                occu_usb = occu_usb + Int32.Parse(p_p5_usb.SelectedItem.ToString());
                occu_bluRay = occu_bluRay + Int32.Parse(p_p5_bluRay.SelectedItem.ToString());
                occu_ijprinter = occu_ijprinter + Int32.Parse(p_p5_ijprinter.SelectedItem.ToString());

            }

            //calculate free ressources vector
            int free_dvd = Int32.Parse(dvd) - occu_dvd;
            int free_printer = Int32.Parse(printer) - occu_printer;
            int free_usb = Int32.Parse(usb) - occu_usb;
            int free_bluRay = Int32.Parse(bluRay) - occu_bluRay;
            int free_ijPrinter = Int32.Parse(ijPrinter) - occu_ijprinter;

            freeResourceVectorA = "";
            if (dvd != "0") { freeResourceVectorA = "" + freeResourceVectorA + "" + free_dvd; }
            if (printer != "0") { freeResourceVectorA = "" + freeResourceVectorA + "" + free_printer; }
            if (usb != "0") { freeResourceVectorA = "" + freeResourceVectorA + "" + free_usb; }
            if (bluRay != "0") { freeResourceVectorA = "" + freeResourceVectorA + "" + free_bluRay; }
            if (ijPrinter != "0") { freeResourceVectorA = "" + freeResourceVectorA + "" + free_ijPrinter; }


            l_freeResourceVectorA.Text = "Vector A = (" + freeResourceVectorA + ")";

        }

        //show Ocuupied and Requested Ressources Picker
        void VectorChanged4(object sender, EventArgs e)
        {
            AddItemsToOccuResPickers();
            AddItemsToReqResPickers();
            SetReqResPickersToZero();
            SetOccuResPickersToZero();
            SetVectorB();
            SetVectorA();
            AddPickersToOccuRes();
            AddPickersToReqRes();

        }

        void SetVectorChangedEvents()
        {
            foreach (Picker picker in occuResPickerList)
            {
                picker.SelectedIndexChanged += VectorChanged2;
                picker.SelectedIndexChanged += VectorChanged3;
            }

        }

        void SetReqResPickersToZero()
        {
            foreach (Picker picker in reqResPickerList)
            {
                picker.SelectedIndex = 0;
            }
               
        }

        void SetOccuResPickersToZero()
        {
            foreach (Picker picker in occuResPickerList)
            {
                picker.SelectedIndex = 0;
            }
        }

        void AddItemsToReqResPickers()
        {
            //get number of ressources
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();

            //first remove all items
            foreach (Picker picker in reqResPickerList)
            {
                picker.Items.Clear();
            }

            //add item for every ressource
            for (int i = 0; i <= UInt32.Parse(dvd); i++)
            {
                p_p1_req_dvd.Items.Add(i.ToString());
                p_p2_req_dvd.Items.Add(i.ToString());
                p_p3_req_dvd.Items.Add(i.ToString());
                p_p4_req_dvd.Items.Add(i.ToString());
                p_p5_req_dvd.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(printer); i++)
            {
                p_p1_req_printer.Items.Add(i.ToString());
                p_p2_req_printer.Items.Add(i.ToString());
                p_p3_req_printer.Items.Add(i.ToString());
                p_p4_req_printer.Items.Add(i.ToString());
                p_p5_req_printer.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(usb); i++)
            {
                p_p1_req_usb.Items.Add(i.ToString());
                p_p2_req_usb.Items.Add(i.ToString());
                p_p3_req_usb.Items.Add(i.ToString());
                p_p4_req_usb.Items.Add(i.ToString());
                p_p5_req_usb.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(bluRay); i++)
            {
                p_p1_req_bluRay.Items.Add(i.ToString());
                p_p2_req_bluRay.Items.Add(i.ToString());
                p_p3_req_bluRay.Items.Add(i.ToString());
                p_p4_req_bluRay.Items.Add(i.ToString());
                p_p5_req_bluRay.Items.Add(i.ToString());
            }
            for (int i = 0; i <= UInt32.Parse(ijPrinter); i++)
            {
                p_p1_req_ijprinter.Items.Add(i.ToString());
                p_p2_req_ijprinter.Items.Add(i.ToString());
                p_p3_req_ijprinter.Items.Add(i.ToString());
                p_p4_req_ijprinter.Items.Add(i.ToString());
                p_p5_req_ijprinter.Items.Add(i.ToString());
            }

        }
        void AddItemsToOccuResPickers()
        {
            //get number of ressources
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();

            //first remove all items
            foreach (Picker picker in occuResPickerList)
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
        }


        void AddPickersToOccuRes()
        {
            // get values from ressource pickers
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();

            //remove all process pickers 
            sl_OccuProcess1.Children.Clear();
            sl_OccuProcess2.Children.Clear();
            sl_OccuProcess3.Children.Clear();
            sl_OccuProcess4.Children.Clear();
            sl_OccuProcess5.Children.Clear();

            // add labels to process layouts
            sl_OccuProcess1.Children.Add(l_occuP1);
            sl_OccuProcess2.Children.Add(l_occuP2);
            sl_OccuProcess3.Children.Add(l_occuP3);
            sl_OccuProcess4.Children.Add(l_occuP4);
            sl_OccuProcess5.Children.Add(l_occuP5);
            
            if (dvd != "0")
            {
                sl_OccuProcess1.Children.Add(p_p1_dvd);
                sl_OccuProcess2.Children.Add(p_p2_dvd);
                sl_OccuProcess3.Children.Add(p_p3_dvd);
                sl_OccuProcess4.Children.Add(p_p4_dvd);
                sl_OccuProcess5.Children.Add(p_p5_dvd);
            }
            if (printer != "0")
            {
                sl_OccuProcess1.Children.Add(p_p1_printer);
                sl_OccuProcess2.Children.Add(p_p2_printer);
                sl_OccuProcess3.Children.Add(p_p3_printer);
                sl_OccuProcess4.Children.Add(p_p4_printer);
                sl_OccuProcess5.Children.Add(p_p5_printer);
            }
            if (usb != "0")
            {
                sl_OccuProcess1.Children.Add(p_p1_usb);
                sl_OccuProcess2.Children.Add(p_p2_usb);
                sl_OccuProcess3.Children.Add(p_p3_usb);
                sl_OccuProcess4.Children.Add(p_p4_usb);
                sl_OccuProcess5.Children.Add(p_p5_usb);
            }
            if (bluRay != "0")
            {
                sl_OccuProcess1.Children.Add(p_p1_bluRay);
                sl_OccuProcess2.Children.Add(p_p2_bluRay);
                sl_OccuProcess3.Children.Add(p_p3_bluRay);
                sl_OccuProcess4.Children.Add(p_p4_bluRay);
                sl_OccuProcess5.Children.Add(p_p5_bluRay);
            }
            if (ijPrinter != "0")
            {
                sl_OccuProcess1.Children.Add(p_p1_ijprinter);
                sl_OccuProcess2.Children.Add(p_p2_ijprinter);
                sl_OccuProcess3.Children.Add(p_p3_ijprinter);
                sl_OccuProcess4.Children.Add(p_p4_ijprinter);
                sl_OccuProcess5.Children.Add(p_p5_ijprinter);
            }
        }
        void AddPickersToReqRes()
        {
            // get values from ressource pickers
            dvd = p_dvd.SelectedItem.ToString();
            printer = p_printer.SelectedItem.ToString();
            usb = p_usb.SelectedItem.ToString();
            bluRay = p_bluRay.SelectedItem.ToString();
            ijPrinter = p_ijPrinter.SelectedItem.ToString();

            //remove all process pickers
            sl_ReqProcess1.Children.Clear();
            sl_ReqProcess2.Children.Clear();
            sl_ReqProcess3.Children.Clear();
            sl_ReqProcess4.Children.Clear();
            sl_ReqProcess5.Children.Clear();

            // add labels to process layouts
            sl_ReqProcess1.Children.Add(l_reqP1);
            sl_ReqProcess2.Children.Add(l_reqP2);
            sl_ReqProcess3.Children.Add(l_reqP3);
            sl_ReqProcess4.Children.Add(l_reqP4);
            sl_ReqProcess5.Children.Add(l_reqP5);

            if (dvd != "0")
            {
                sl_ReqProcess1.Children.Add(p_p1_req_dvd);
                sl_ReqProcess2.Children.Add(p_p2_req_dvd);
                sl_ReqProcess3.Children.Add(p_p3_req_dvd);
                sl_ReqProcess4.Children.Add(p_p4_req_dvd);
                sl_ReqProcess5.Children.Add(p_p5_req_dvd);

            }
            if (printer != "0")
            {
                sl_ReqProcess1.Children.Add(p_p1_req_printer);
                sl_ReqProcess2.Children.Add(p_p2_req_printer);
                sl_ReqProcess3.Children.Add(p_p3_req_printer);
                sl_ReqProcess4.Children.Add(p_p4_req_printer);
                sl_ReqProcess5.Children.Add(p_p5_req_printer);
            }
            if (usb != "0")
            {
                sl_ReqProcess1.Children.Add(p_p1_req_usb);
                sl_ReqProcess2.Children.Add(p_p2_req_usb);
                sl_ReqProcess3.Children.Add(p_p3_req_usb);
                sl_ReqProcess4.Children.Add(p_p4_req_usb);
                sl_ReqProcess5.Children.Add(p_p5_req_usb);
            }
            if (bluRay != "0")
            {
                sl_ReqProcess1.Children.Add(p_p1_req_bluRay);
                sl_ReqProcess2.Children.Add(p_p2_req_bluRay);
                sl_ReqProcess3.Children.Add(p_p3_req_bluRay);
                sl_ReqProcess4.Children.Add(p_p4_req_bluRay);
                sl_ReqProcess5.Children.Add(p_p5_req_bluRay);
            }

            if (ijPrinter != "0")
            {
                sl_ReqProcess1.Children.Add(p_p1_req_ijprinter);
                sl_ReqProcess2.Children.Add(p_p2_req_ijprinter);
                sl_ReqProcess3.Children.Add(p_p3_req_ijprinter);
                sl_ReqProcess4.Children.Add(p_p4_req_ijprinter);
                sl_ReqProcess5.Children.Add(p_p5_req_ijprinter);
            }
        }

        //TODO: Occupied changed
        /**********************************************************************
        *********************************************************************/
        void OccupiedChanged(object sender, EventArgs e)
        {

        }


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
