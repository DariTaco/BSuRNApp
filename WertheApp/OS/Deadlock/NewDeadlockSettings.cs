using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

/*
 Picker abhängigkeiten:
    -Anzahl der zu wählenden Elemente (0-9) abhängig von der Anzahl der jeweiligen Resource
    -Anzeige einer vertikalen Reihe abhängig davon, ob die jeweilige Resource existiert
    -Anzahl der horzontalen Reihen davon abhängig wieviele Prozesse es gibt

    Werte der Picker müssen wahrscheinlich woanders zwischengespeichert werden. Damit bei Veränderung der Anzahl der jeweilgen Resource, die Picker neu angelegt werden können mit entsprechender Auswahl an Elementen (wenn von 9 auf 2 Resourcen verringert wird, soll der Picker auch nur noch 2 ermöglichen und nicht mehr 9. Problem: wenn 9 eingegeben wurde steht es ja noch drin....sollte dann automatisch auf 2 geändert werden....
 
 */
namespace WertheApp.OS
{
    public class NewDeadlockSettings : ContentPage
    {
        private Xamarin.Forms.ScrollView scrollView;
        private Dictionary<String, Xamarin.Forms.Picker> resourcesDict = new Dictionary<String, Xamarin.Forms.Picker>();
        private Dictionary<String, Xamarin.Forms.Picker> busyDict = new Dictionary<String, Xamarin.Forms.Picker>();
        private Dictionary<String, Xamarin.Forms.Picker> upcomingDict = new Dictionary<String, Xamarin.Forms.Picker>();
        private Dictionary<String, Xamarin.Forms.StackLayout> busyLayoutDict = new Dictionary<String, Xamarin.Forms.StackLayout>();
        private Dictionary<String, Xamarin.Forms.StackLayout> upcomingLayoutDict = new Dictionary<String, Xamarin.Forms.StackLayout>();

        List<string> resourceNameList = new List<string>{
                        "DVD Drive(s)",
                        "Laser Printer(s)",
                        "USB Disk Drive(s)",
                        "BluRay Drive(s)",
                        "Inkjet Printer(s)",
                        "3D Printer(s)"
                    };

        public NewDeadlockSettings()
        {
            Title = "Deadlock";

            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            // content starts only after notch
            On<iOS>().SetUseSafeArea(true);

            resourcesDict.Clear();
            busyDict.Clear();
            upcomingDict.Clear();
            busyLayoutDict.Clear();
            upcomingLayoutDict.Clear();
            Vector.DeleteAll();

            CreateContent();
            ModifyPickers(busyDict);
            ModifyPickers(upcomingDict);


        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            StackLayout layout = CreateLayout();

            CreatePresetButtons(layout);

            CreateExistingResourcesContent(layout);
            CreateRunningProcessesContent(layout);
            CreateBusyResourcesContent(layout);
            CreateAvailableResourcesContent(layout);
            CreateUpcomingRequestsContent(layout);

            CreateStartButton(layout);
        }



        /**********************************************************************
        *********************************************************************/
        void CreateExistingResourcesContent(StackLayout layout)
        {
            //caption
            CreateCaption(layout, "", "E", "xisting Resources", Color.Blue);

            //pickers
            CreateResources(layout);
            SetResources(2, 2, 3, 0, 0, 0);

            //vector
            CreateVector(layout, "E", Color.Blue, 2, 2, 3, -1, -1, -1);

            //spacing
            AddSpace(layout);


        }

        /**********************************************************************
        *********************************************************************/
        void CreateBusyResourcesContent(StackLayout layout)
        {
            //caption
            CreateCaption(layout, "", "B", "usy Resources", Color.Red);

            //pickers
            CreatePickers(layout, busyDict, busyLayoutDict);

            //vector
            CreateVector(layout, "B", Color.Red, 0, 0, 0, -1, -1, -1);

            //clear button
            CreateClearButton(layout, "Clear Busy");

            //spacing
            AddSpace(layout);


        }

        /**********************************************************************
        *********************************************************************/
        void CreateAvailableResourcesContent(StackLayout layout)
        {
            //caption
            CreateCaption(layout, "", "A", "vailable Resources", Color.Green);

            //vector
            CreateVector(layout, "A", Color.Green, 2, 2, 3, -1, -1, -1);

            //spacing
            AddSpace(layout);
        }


        /**********************************************************************
        *********************************************************************/
        void CreateUpcomingRequestsContent(StackLayout layout)
        {

            //caption
            CreateCaption(layout, "Up", "c", "oming Requests", Color.Orange);

            //pickers
            CreatePickers(layout, upcomingDict, upcomingLayoutDict);

            //vector
            CreateVector(layout, "C", Color.Orange, 0, 0, 0, -1, -1, -1);

            //clear button
            CreateClearButton(layout, "Clear Upcoming");

            //spacing
            AddSpace(layout);
        }


        /**********************************************************************
        *********************************************************************/
        void CreateVector(StackLayout layout, String p_name, Color p_color,
            int p_dvd, int p_laser, int p_usb, int p_bluray, int p_inkjet, int p_printer3d)
        {
            Vector vector = new Vector(p_name, p_dvd, p_laser, p_usb, p_bluray, p_inkjet, p_printer3d);
            Label l_vector = new Label
            {
                Text = p_name + " = (    " + vector.GetVectorString() + " )",
                TextColor = p_color,
                FontSize = App._h4FontSize,
                VerticalOptions = LayoutOptions.End
            };
            layout.Children.Add(l_vector);
        }


        /**********************************************************************
        *********************************************************************/
        void RunningProcessesChanged(object sender, EventArgs e)
        {
            //TODO:
        }

        /**********************************************************************
        *********************************************************************/
        void CreateResources(StackLayout layout)
        {

            var sl_right = new StackLayout() { HorizontalOptions = LayoutOptions.CenterAndExpand };
            var sl_left = new StackLayout();
            var sl_container = new StackLayout() { Orientation = StackOrientation.Horizontal };
            int count = 0;

            foreach (string resourceName in resourceNameList)
            {
                //layout
                var sl_picker = new StackLayout() { Orientation = StackOrientation.Horizontal };

                //picker
                Xamarin.Forms.Picker p_picker = new Xamarin.Forms.Picker()
                {
                    FontSize = App._textFontSize
                };
                for (int i = 0; i < 10; i++)
                {
                    p_picker.Items.Add(i.ToString());
                }
                p_picker.AutomationId = resourceName;
                p_picker.SelectedIndexChanged += ResourceChanged;
                p_picker.SelectedIndex = 0;
             
                sl_picker.Children.Add(p_picker);
                resourcesDict.Add(resourceName, p_picker);

                //label
                var l_picker = new Label
                {
                    Text = resourceName,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = App._textFontSize
                };
                sl_picker.Children.Add(l_picker);

                // add to left or right layout
                if (count < 3)
                {
                    sl_left.Children.Add(sl_picker);
                }
                else
                {
                    sl_right.Children.Add(sl_picker);
                }
                count++;
            }

            sl_container.Children.Add(sl_left);
            sl_container.Children.Add(sl_right);
            layout.Children.Add(sl_container);

        }

        /**********************************************************************
        *********************************************************************/
        void SetResources(int p_dvd, int p_laser, int p_usb, int p_bluray, int p_inkjet, int p_printer3d)
        {
            SetResource("DVD Drive(s)", p_dvd);
            SetResource("Laser Printer(s)", p_laser);
            SetResource("USB Disk Drive(s)", p_usb);
            SetResource("BluRay Drive(s)", p_bluray);
            SetResource("Inkjet Printer(s)", p_inkjet);
            SetResource("3D Printer(s)", p_printer3d);
        }

        void SetResource(String p_name, int index)
        {
            Xamarin.Forms.Picker p;
            if (resourcesDict.TryGetValue(p_name, out p))
            {
                p.SelectedIndex = index;
                DisplayOrHideLayout(p_name, busyLayoutDict, Convert.ToBoolean(index));
                DisplayOrHideLayout(p_name, upcomingLayoutDict, Convert.ToBoolean(index));

            }

        }

        /**********************************************************************
        *********************************************************************/
        void ResourceChanged(object sender, EventArgs e)
        {
  
            var picker = (sender as Xamarin.Forms.Picker);

            ModifyPickers(busyDict);
            ModifyPickers(upcomingDict);

            DisplayOrHideLayout(picker, busyLayoutDict);
            DisplayOrHideLayout(picker, upcomingLayoutDict);


            /*
            switch (pickerName)
            {
                case "DVD Drive(s)":
                    //TODO: "DVD Drive(s)"
                    Debug.WriteLine("DVD Drive(s)");
                    break;
                case "Laser Printer(s)":
                    //TODO:  "Laser Printer(s)"
                    break;
                case "USB Disk Drive(s)":
                    //TODO: "USB Disk Drive(s)"
                    break;
                case "BluRay Drive(s)":
                    //TODO: "BluRay Drive(s)"
                    break;
                case "Inkjet Printer(s)":
                    //TODO: "Inkjet Printer(s)"
                    break;
                case "3D Printer(s)":
                    //TODO: "3D Printer(s)"
                    break;
            }*/
        }

        void DisplayOrHideLayout(Xamarin.Forms.Picker picker, Dictionary<String, StackLayout> layoutDict)
        {
            string pickerName = picker.AutomationId;

            StackLayout l;
            if (layoutDict.TryGetValue(pickerName, out l))
            {
                if (picker.SelectedIndex == 0)
                {
                    l.IsVisible = false;
                }
                else { l.IsVisible = true; }
            }
        }

        void DisplayOrHideLayout(String name, Dictionary<String, StackLayout> layoutDict, bool visible)
        {
            Debug.WriteLine(visible);
            StackLayout l;
            if (layoutDict.TryGetValue(name, out l))
            {
                if (!visible)
                {
                    l.IsVisible = false;
                }
                else { l.IsVisible = true; }
            }
        }

        /**********************************************************************
        *********************************************************************/
        Dictionary<String, Xamarin.Forms.Picker> CreatePickers(StackLayout layout, Dictionary<String, Xamarin.Forms.Picker> dict, Dictionary<String, Xamarin.Forms.StackLayout> layoutDict)
        {
            // create container stacklayout which will hold all upcoming resource layouts. First add a layout of 5 labels
            StackLayout containerStackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

            StackLayout labelStackLayout = new StackLayout {/*BackgroundColor = Color.Aqua*/};
            for (int i = 0; i < 5; i++)
            {
                Label label = new Label
                {
                    Text = "P" + i + ": ",
                    FontSize = App._textFontSize,
                    VerticalOptions = LayoutOptions.EndAndExpand
                };
                labelStackLayout.Children.Add(label);
            }
            containerStackLayout.Children.Add(labelStackLayout);

            /*________________________________________________________________*/
            // create a vertical stacklayout for every resource and add pickers
            foreach (string resourceName in resourceNameList)
            {
                StackLayout resourceStackLayout = new StackLayout { Orientation = StackOrientation.Vertical };
                resourceStackLayout.AutomationId = resourceName;
                layoutDict.Add(resourceName, resourceStackLayout); 

                // add 5 new pickers to the stacklayout
                for (int i = 0; i < 5; i++)
                {
                    String key = resourceName + i;

                    Xamarin.Forms.Picker picker = new Xamarin.Forms.Picker()
                    {
                        FontSize = App._textFontSize
                    };
                    picker.Items.Add("0");
                    picker.SelectedIndex = 0;
                    picker.SelectedIndexChanged += PickerChanged;
                    picker.AutomationId = key;

                    resourceStackLayout.Children.Add(picker);
                    dict.Add(key, picker);
                }

                //add vertical stacklayouts to horizontal container layout
                containerStackLayout.Children.Add(resourceStackLayout);
            }
            layout.Children.Add(containerStackLayout);
            return dict;
        }

        /**********************************************************************
        *********************************************************************/
        void ModifyPickers(Dictionary<String, Xamarin.Forms.Picker> dict)
        {
            foreach (string resourceName in resourceNameList)
            {
                // find out how many instances of each resource exist
                Xamarin.Forms.Picker p;
                int selectableItems = -1;
                if (resourcesDict.TryGetValue(resourceName, out p))
                {
                    selectableItems = p.SelectedIndex;
                }

                for (int i = 0; i < 5; i++)
                {
                    String key = resourceName + i;
                    int selectedIndex = 0;
                    Xamarin.Forms.Picker p2;
                    if (dict.TryGetValue(key, out p2))
                    {
                        // delete possible items but remember selected index
                        if (p2.Items.Count > 0)
                        {
                            // if selected index is bigger than selectable items, change it to the max possible
                            if (selectedIndex < selectableItems) { selectedIndex = p2.SelectedIndex; }
                            else { selectedIndex = selectableItems; }
                            p2.Items.Clear();
                        }

                        // modify maximum number of selectable items for each picker of each resource
                        for (int j = 0; j <= selectableItems; j++)
                        {
                            p2.Items.Add(j.ToString());
                        }
                        p2.SelectedIndex = selectedIndex;
                    }
                }

                // depending on which resources exist
                DisplayOrHideLayout(resourceName, busyLayoutDict, Convert.ToBoolean(selectableItems));
                DisplayOrHideLayout(resourceName, upcomingLayoutDict, Convert.ToBoolean(selectableItems));
            }
        }

        /**********************************************************************
        *********************************************************************/
        void PickerChanged(object sender, EventArgs e)
        {
            //TODO: Vektoren anpassen
            //Schritt 1: alle Picker zusammenzählen und Vektor neu berechnen
            //Schritt 2: entsprechende Vekto Objekte finden mit Vector.GetVector(name) und v.ChangeResources(...)
            //Schritt 3: Vektor Objekte updaten mit neuen Werten

            var picker = (sender as Xamarin.Forms.Picker);
            string pickerName = picker.AutomationId;

        }

        /**********************************************************************
        *********************************************************************/
        void B_Preset_Clicked(object sender, EventArgs e)
        {
            var button = (sender as Button);
            string buttonName = button.Text;

            switch (buttonName)
            {
                case "Preset 1":
                    //TODO: Preset 1
                    break;
                case "Preset 2":
                    //TODO: Preset 2
                    break;
                case "Preset":
                    //TODO: Preset 3
                    break;
            }
        }


        /**********************************************************************
        *********************************************************************/
        void B_Clear_Clicked(object sender, EventArgs e)
        {
            var button = (sender as Button);
            string buttonName = button.Text;

            switch (buttonName)
            {
                case "Clear Busy":
                    foreach (var item in busyDict)
                    {
                        item.Value.SelectedIndex = 0;
                    }
                    break;
                case "Clear Upcoming":
                    foreach (var item in upcomingDict)
                    {
                        item.Value.SelectedIndex = 0;
                    }
                    break;
            }
        }

        /**********************************************************************
        *********************************************************************/
        void B_Start_Clicked(object sender, EventArgs e)
        {
            //TODO:

            /*
             * await Navigation.PushAsync(new Deadlock());

            // dictionary of individual amount of existing resources
            //keys: dvd, printer, usb, bluRay, ijPrinter, printer3D
            Dictionary<string, int> d,


            String VE,
            String VB,
            String VC,
            String VA,

            int tProcesses,

            Dictionary<int, String> VBProcesses,
            Dictionary<int,String> VCProcesses*/
        }









        /**********************************************************************
         * ****************************************************************** *
         * *                   easy UI                                      * *
         * ****************************************************************** *
         *********************************************************************/

        StackLayout CreateLayout()
        {
            // Wrap ScrollView around StackLayout to be able to scroll the content
            scrollView = new Xamarin.Forms.ScrollView
            {
                Margin = new Thickness(10)
            };
            var stackLayout = new StackLayout();
            scrollView.Content = stackLayout;
            this.Content = scrollView;

            return stackLayout;
        }

        /**********************************************************************
         *********************************************************************/
        void CreateCaption(StackLayout layout, String a, String b, String c, Color color)
        {
            var formattedStringUpComing = new FormattedString();
            formattedStringUpComing.Spans.Add(new Span
            {
                Text = a,
                TextDecorations = TextDecorations.Underline
            });
            formattedStringUpComing.Spans.Add(new Span
            {
                Text = b,
                ForegroundColor = color,
                FontAttributes = FontAttributes.Bold,
                FontSize = App._h3FontSize,

            });
            formattedStringUpComing.Spans.Add(new Span
            {
                Text = c,
                TextDecorations = TextDecorations.Underline
            });
            var l_caption = new Label
            {
                FormattedText = formattedStringUpComing,
                FontSize = App._h3FontSize,
                VerticalOptions = LayoutOptions.Center,
                TextDecorations = TextDecorations.Underline
            };
            layout.Children.Add(l_caption);

        }

        /**********************************************************************
         *********************************************************************/

        void AddSpace(StackLayout layout)
        {
            Label l_space = new Label { Text = " " };
            layout.Children.Add(l_space);
        }


        /**********************************************************************
        *********************************************************************/
        void CreateClearButton(StackLayout layout, String buttonName)
        {
            var b_Clear = new Button
            {
                Text = buttonName,
                FontSize = App._smallButtonFontSize,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                HorizontalOptions = LayoutOptions.Start

            };
            b_Clear.Clicked += B_Clear_Clicked;
            layout.Children.Add(b_Clear);
        }

        /**********************************************************************
         *********************************************************************/
        void CreateStartButton(StackLayout layout)
        {
            var b_Start = new Button
            {
                Text = "Start",
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
            b_Start.Clicked += B_Start_Clicked;
            layout.Children.Add(b_Start);
        }

        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeadlockHelp());
        }

        /**********************************************************************
        *********************************************************************/
        void CreatePresetButtons(StackLayout layout)
        {
            StackLayout bLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            List<string> buttonNameList = new List<string>()    {
                        "Preset 1",
                        "Preset 2",
                        "Preset 3"
                    };
            foreach (string buttonName in buttonNameList)
            {
                var b_button = new Button
                {
                    Text = buttonName,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = App._buttonBackground,
                    TextColor = App._buttonText,
                    CornerRadius = App._buttonCornerRadius,
                    FontSize = App._smallButtonFontSize
                };
                b_button.Clicked += B_Preset_Clicked;
                bLayout.Children.Add(b_button);
            }
            layout.Children.Add(bLayout);


            //spacing
            AddSpace(layout);
        }

        /**********************************************************************
         *********************************************************************/
        void CreateRunningProcessesContent(StackLayout layout)
        {
            //caption
            var sl_runningProcesses = new StackLayout() { Orientation = StackOrientation.Horizontal };
            var l_Processes = new Label
            {
                Text = "Running Processes",
                FontSize = App._h3FontSize,
                VerticalOptions = LayoutOptions.Center,
                TextDecorations = TextDecorations.Underline
            };
            sl_runningProcesses.Children.Add(l_Processes);

            //picker
            Xamarin.Forms.Picker p_runningprocesses = new Xamarin.Forms.Picker()
            { FontSize = App._textFontSize };
            for (int i = 2; i < 6; i++)
            {
                p_runningprocesses.Items.Add(i.ToString());
            }
            p_runningprocesses.SelectedIndex = 0;
            p_runningprocesses.SelectedIndexChanged += RunningProcessesChanged;

            sl_runningProcesses.Children.Add(p_runningprocesses);
            layout.Children.Add(sl_runningProcesses);

            //spacing
            AddSpace(layout);

        }

    }
}
