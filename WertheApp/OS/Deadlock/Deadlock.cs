using System;
using Xamarin.Forms;
using System.Collections.Generic;
using SkiaSharp.Views.Forms;
using System.Linq; //list.Any()
using System.Collections.ObjectModel;
using System.Diagnostics;


using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace WertheApp.OS
{

    public class Deadlock: ContentPage
    {
        //VARIABLES
        private static Button b_undo, b_restart;
        private static Label l_info;
        public static Xamarin.Forms.ListView listView;
        public static ObservableCollection<DeadlockViewCell> deadlockCells; // deadlock canvas
        public static int cellNumber;


        // dictionary of individual amount of existing resources
        //keys: dvd, printer, usb, bluRay, ijPrinter, printer3D
        public static Dictionary<string, int> exResDict;
        private static String vectorE, vectorB, vectorC, vectorA;
        private static Dictionary<int, String> vectorBProcesses, vectorCProcesses;
        private static int totalProcesses;

        public static List<int> todoProcesses, doneProcesses;
        public static bool P1done, P2done, P3done, P4done, P5done;
        public static String[,] history;
        public static int currentStep;
        public static String currentAnew;

        public static List<bool> deadLockViewCellTouchableList;
        public static List<SKCanvasView> deadLockViewCellCansvasList;
        public static List<DeadlockViewCell> deadLockViewCellList;

        private double width = 0;
        private double height = 0;

        public Deadlock(Dictionary<string, int> d,
            String VE, String VB, String VC, String VA,
            int tProcesses,
            Dictionary<int, String> VBProcesses, Dictionary<int,String> VCProcesses )
        {
            ToolbarItem info = new ToolbarItem();
            info.Text = App._sHelpInfoHint;
            this.ToolbarItems.Add(info);
            info.Clicked += B_Info_Clicked;

            Title = "Deadlock";

            // content starts only after notch
            On<iOS>().SetUseSafeArea(true);



            currentStep = -1;
            exResDict = d; //exResDict["dvd"]
            vectorE = VE;
            vectorB = VB;
            vectorC = VC;
            vectorA = VA;
            vectorBProcesses = VBProcesses;
            vectorCProcesses = VCProcesses;
            totalProcesses = tProcesses;
            //5 processes, 4: processNumber,  Anew, C(Px), B(Px),
            history = new String[,]{
                { "0", "0", "0", "0" },
                { "0", "0", "0", "0" },
                { "0", "0", "0", "0" },
                { "0", "0", "0", "0" },
                { "0", "0", "0", "0" }};

            todoProcesses = new List<int>();
            for(int i = 1; i <= totalProcesses; i++)
            {
                todoProcesses.Add(i);
            }
            doneProcesses = new List<int>();

            cellNumber = -1;
            P1done = false;
            P2done = false;
            P3done = false;
            P4done = false;
            P5done = false;
            //Debug.WriteLine("VE: "+ VE + ", VB: " + VB + ", VC: " + VC + ", VA: " + VA + ", total Processes: " + totalProcesses);

            currentAnew = vectorA;
            CreateContent();

            //ShowMyHint();
            bool deadlock = CheckIfDeadlock();
            if (deadlock)
            {
                l_info.TextColor = Color.Red;
                l_info.Text = "Some processes cannot terminate => deadlock.";
            }

            deadLockViewCellTouchableList = new List<bool>();
            deadLockViewCellCansvasList = new List<SKCanvasView>();
            deadLockViewCellList = new List<DeadlockViewCell>();
            CreateContent();
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        public async void ShowMyHint()
        {
            await DisplayAlert("Hint", "Pick and click on a process in C until all processes are finished or a Deadlock happens", "OK");
        }

        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            // This is the top-level grid, which will split our page in half
            var grid = new Grid();
            this.Content = grid;
            grid.RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition{ Height = new GridLength(4, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)}
                };
            CreateTopHalf(grid);
            CreateBottomHalf(grid);
        }

        /**********************************************************************
        *********************************************************************/
        void CreateTopHalf(Grid grid)
        {
            listView = new Xamarin.Forms.ListView
            {
                ItemTemplate = new DataTemplate(typeof(DeadlockViewCell)),
                RowHeight = 200,
                BackgroundColor = Color.Transparent,
            };
            deadlockCells = new ObservableCollection<DeadlockViewCell>();
            listView.ItemsSource = deadlockCells;
            grid.Children.Add(listView, 0, 0);
            AddDeadlockCell(); //add very first cell

        }

        /**********************************************************************
        ********************************************************************/
        void CreateBottomHalf(Grid grid)
        {
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(5),

            };

            l_info = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "Pick a pending request C(Pi).",
                TextColor = Color.Blue,
                FontSize = App._textFontSize
            };
            stackLayout.Children.Add(l_info);

            b_undo = new Button
            {
                Text = "Undo",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize


            };
            b_undo.Clicked += B_Undo_Clicked;
            b_undo.IsEnabled = false;

            //stackLayout.Children.Add(b_undo);

            b_restart = new Button
            {
                Text = "Restart",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = 0,
                Padding = 0,
                BackgroundColor = App._buttonBackground,
                TextColor = App._buttonText,
                CornerRadius = App._buttonCornerRadius,
                FontSize = App._buttonFontSize

            };
            b_restart.Clicked += B_Restart_Clicked;
            b_restart.IsEnabled = false;
            stackLayout.Children.Add(b_restart);

            grid.Children.Add(stackLayout, 0, 1);
        }

        /**********************************************************************
        *********************************************************************/
        static void B_Restart_Clicked(object sender, EventArgs e)
        {
            //clear scrollbar
            deadlockCells.Clear();

            //reset everything
            currentStep = -1;
            cellNumber = -1;
            currentAnew = vectorA;

            history = new String[,]{
                { "0", "0", "0", "0" },
                { "0", "0", "0", "0" },
                { "0", "0", "0", "0" },
                { "0", "0", "0", "0" },
                { "0", "0", "0", "0" }};
            P1done = false;
            P2done = false;
            P3done = false;
            P4done = false;
            P5done = false;
            todoProcesses.Clear();
            for (int i = 1; i <= totalProcesses; i++)
            {
                todoProcesses.Add(i);
            }
            doneProcesses.Clear();

            //disbale button
            b_restart.IsEnabled = false;
            l_info.Text = "Pick a pending request C(Pi).";
            l_info.TextColor = Color.Blue;

            AddDeadlockCell(); //add very first cell
        }

        /**********************************************************************
        *********************************************************************/
        static void B_Undo_Clicked(object sender, EventArgs e)
        {
            deadlockCells.Remove(deadlockCells.Last());

            //Variablen und Listen updaten
            int processNumber = doneProcesses.Last();
            //Debug.WriteLine("done processes Last:" + doneProcesses.Last());
            switch (processNumber)
            {
                case 1:
                    P1done = false;
                    break;
                case 2:
                    P2done = false;
                    break;
                case 3:
                    P3done = false;
                    break;
                case 4:
                    P4done = false;
                    break;
                case 5:
                    P5done = false;
                    break;
            }
            history[processNumber-1, 0] = "0";
            history[processNumber-1, 1] = "0";
            todoProcesses.Add(processNumber);
            doneProcesses.Remove(processNumber);
            currentStep--;
            cellNumber--;

            //check if undo is still possible
            if (!doneProcesses.Any())
            {
                b_undo.IsEnabled = false;
            }

            //Debug.WriteLine("count in observable collection" + deadlockCells.Count());
            deadlockCells.Last().Paint();

            //TODO: enable touch sensitiveness 
            //Debug.WriteLine("current step" + currentStep);
            deadLockViewCellTouchableList[currentStep+1] = true;
            deadLockViewCellCansvasList[currentStep+1].EnableTouchEvents = true;

            //Debug.WriteLine("dvtl count = " + deadLockViewCellTouchableList.Count);

        }

        /**********************************************************************
        *********************************************************************/
        public static void AddDeadlockCell()
        {
            //DeadlockViewCell newCell = new DeadlockViewCell();
            //newCell.SetVariables(cellNumber, vectorE, vectorB, vectorC, vectorA,
            //vectorBProcesses, vectorCProcesses,
            //totalProcesses, doneProcesses);
            DeadlockItem item = new DeadlockItem(cellNumber, doneProcesses);
        
            deadlockCells.Add(new DeadlockViewCell()); //actually creates a new deadlockviewcell and doesn't pass the reference
            //can't even pass reference because Add method doesn't implement it

   
            listView.ScrollTo(deadlockCells[deadlockCells.Count-1], ScrollToPosition.End, false);
        }



        /**********************************************************************
        *********************************************************************/
        public static int GetCellNumber()
        {
            return cellNumber;
        }

        public static String GetVectorA()
        {
            return vectorA;
        }

        public static String GetVectorB()
        {
            return vectorB;
        }

        public static String GetVectorC()
        {
            return vectorC;
        }

        public static String GetVectorE()
        {
            return vectorE;
        }

        public static Dictionary<int, String> GetVectorBProcesses()
        {
            return vectorBProcesses;
        }
        public static Dictionary<int, String> GetVectorCProcesses()
        {
            return vectorCProcesses;
        }

        public static int GetTotalProcesses()
        {
            return totalProcesses;
        }

        public static List<int> GetDoneProcesses()
        {
            return doneProcesses;
        }
        public static ObservableCollection<DeadlockViewCell> GetDeadlockCellsCollection()
        {
            return deadlockCells;
        }

        /* LOGIC
         * *********************************************************************
        *********************************************************************/

        public static void CPx_Clicked(ref SKCanvasView sender, ref bool touchable,
            int processNumber, String oldVA)
        {
            bool found = todoProcesses.Contains(processNumber);
            if (found)
            {
                bool fits = CheckIfUpcomingProcessFitsInVectorA(vectorCProcesses[processNumber], oldVA);
                bool deadlock = CheckIfDeadlock();

                if (!fits)
                {
                    if (deadlock)
                    {
                        l_info.TextColor = Color.Red;
                        l_info.Text = "Some processes cannot terminate => deadlock.";
                    }
                    else
                    {
                        l_info.TextColor = Color.Red;
                        l_info.Text = "Not enough free resources for upcoming requests C(P" + processNumber + ") !";
                    }
   
                }
            
                else if (found && fits)
                {
     
                todoProcesses.Remove(processNumber);
                doneProcesses.Add(processNumber);
                sender.EnableTouchEvents = false;
                touchable = false;

                // ad to List so that reference is later accessible
                deadLockViewCellCansvasList.Add(sender);
                deadLockViewCellTouchableList.Add(touchable);
                switch (processNumber)
                {
                    case 1:
                        P1done = true;
                        currentStep++;
                        History(processNumber, oldVA, vectorBProcesses[1]);
                        DisplayInfo(processNumber);
                        break;
                    case 2:
                        P2done = true;
                        currentStep++;
                        History(processNumber, oldVA, vectorBProcesses[2]);
                        DisplayInfo(processNumber);
                        break;
                    case 3:
                        P3done = true;
                        currentStep++;
                        History(processNumber, oldVA, vectorBProcesses[3]);
                        DisplayInfo(processNumber);
                        break;
                    case 4:
                        P4done = true;
                        currentStep++;
                        History(processNumber, oldVA, vectorBProcesses[4]);
                        DisplayInfo(processNumber);
                        break;
                    case 5:
                        P5done = true;
                        currentStep++;
                        History(processNumber, oldVA, vectorBProcesses[5]);
                        DisplayInfo(processNumber);
                        break;
                }
                cellNumber++;
                AddDeadlockCell();
                b_undo.IsEnabled = true;
                b_restart.IsEnabled = true;
            }
            }
        }


        /**********************************************************************
        *********************************************************************/
        public static void History(int processNumber, String oldVA, String currVB)
        {
            history[currentStep, 0] = processNumber.ToString();
            history[currentStep, 1] = CalculateANew(oldVA, currVB);
            currentAnew = history[currentStep, 1];
            //history[currentStep, 2] = vectorBProcesses[processNumber];
            //history[currentStep, 3] = vectorCProcesses[processNumber];
        }


        /**********************************************************************
        *********************************************************************/
        public static String CalculateANew(String oldVA, String currVB)
        {

            String newA = "";
            for (int i = 0; i < vectorA.Length; i++)
            {
                //Anew = A + B
                int aplusb = Int16.Parse(oldVA[i].ToString()) + Int16.Parse(currVB[i].ToString());
                newA = newA + "" + aplusb;
            }
            return newA;
        }


        /**********************************************************************
        *********************************************************************/
        public static String GetHistory(int step, int index)
        {
            switch (index)
            {
                case 0: return history[step, 0]; 
                case 1: return history[step, 1]; 
                default: return ""; 
            }
            
        }


        /**********************************************************************
        *********************************************************************/
        public static bool CheckIfUpcomingProcessFitsInVectorA(String C, String A)
        {
            for (int i = 0; i < A.Length; i++)
            {
                //Anew = A - C
                int aminusc = Int16.Parse(A[i].ToString()) - Int16.Parse(C[i].ToString());
                if(aminusc < 0)
                {
                    return false;
                }
            }
            return true;
        }
        /**********************************************************************
        *********************************************************************/
        public static bool CheckIfDeadlock()
        {
            String vectorP = "";
            int counter = 0;
            foreach (int processNumber in todoProcesses)
            {
                vectorP = vectorCProcesses[processNumber];
                bool fit  = CheckIfUpcomingProcessFitsInVectorA(vectorP, currentAnew);
                if (!fit)
                {
                    counter++;
                }

            }
            if (counter == todoProcesses.Count)
            {
                return true; //deadlock
            }
            return false;

        }

        public static bool checkIfDone()
        {
            return (doneProcesses.Count == GetTotalProcesses());
        }

        private static void DisplayInfo(int processNumber)
        {
            l_info.TextColor = Color.Blue;
            l_info.Text = "Process " + processNumber + " done! Pick another pending request.";

            bool deadlock = CheckIfDeadlock();

            bool done = checkIfDone();
            if (done) {
                l_info.TextColor = Color.Blue;
                l_info.Text = "All processes can terminate => no deadlock.";
            }else if (deadlock)
            {
                l_info.TextColor = Color.Red;
                l_info.Text = "Some processes cannot terminate => deadlock.";
            }
        }

        /**********************************************************************
        *********************************************************************/
        //this method is called everytime the device is rotated
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height); //must be called
            if (this.width != width || this.height != height)
            {
                MessagingCenter.Send<object>(this, "Landscape"); // enforce landscape mode
            }
        }

        AppLinkEntry _appLink; // App Linking
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Debug.WriteLine("   ");
            Debug.WriteLine(Device.Idiom.ToString());
            Debug.WriteLine("   ");

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                MessagingCenter.Send<object>(this, "Portrait"); 

            }
            else
            {
                MessagingCenter.Send<object>(this, "Landscape"); 

            }



            // App Linking
            Uri appLinkUri = new Uri(string.Format(App.AppLinkUri, Title).Replace(" ", "_"));
            _appLink = new AppLinkEntry
            {
                AppLinkUri = appLinkUri,
                Description = string.Format($"This App visualizes {Title}"),
                Title = string.Format($"WertheApp {Title}"),
                IsLinkActive = true,
                Thumbnail = ImageSource.FromResource("WertheApp.png")

            };
            Xamarin.Forms.Application.Current.AppLinks.RegisterLink(_appLink);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Send<object>(this, "Unspecified");  // undo enforcing landscape mode
            Debug.WriteLine("on disappearing");

            // App Linking
            _appLink.IsLinkActive = false;
            Xamarin.Forms.Application.Current.AppLinks.RegisterLink(_appLink);
        }
        /**********************************************************************
        *********************************************************************/
        async void B_Info_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeadlockHelp());
        }
    }


    /**********************************************************************
    *********************************************************************/
    public class DeadlockItem
    {
        List<int> doneProcesses;
        public DeadlockItem(int cellNumber, List<int> dProcesses)
        {
            this.doneProcesses = dProcesses;
            /*
            this.cellNumber = cellNumber;
            this.vectorE = Deadlock.GetVectorE();
            this.vectorB = Deadlock.GetVectorB();
            this.vectorC = Deadlock.GetVectorC();
            this.vectorA = Deadlock.GetVectorA();
            this.vectorCProcesses = Deadlock.GetVectorCProcesses();
            this.vectorBProcesses = Deadlock.GetVectorBProcesses();
            this.totalProcesses = Deadlock.GetTotalProcesses();
            this.doneProcesses = Deadlock.GetDoneProcesses();
            */
        }
        public List<int> GetDoneProcesses()
        {
            return doneProcesses;
        }
    }


}



