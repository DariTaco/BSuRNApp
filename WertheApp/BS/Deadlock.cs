using System;
using Xamarin.Forms;
using System.Collections.Generic;
using SkiaSharp.Views.Forms;
using System.Linq; //list.Any()
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace WertheApp.BS
{

    public class Deadlock: ContentPage
    {
        //VARIABLES
        private static Button b_undo;
        private static Label l_info;
        public static ListView listView;
        public static ObservableCollection<DeadlockViewCell> deadlockCells; // deadlock canvas
        public static int cellNumber;


        // dictionary of individual amount of existing resources
        //keys: dvd, printer, usb, bluRay, ijPrinter, printer3D
        public static Dictionary<string, int> exResDict;
        private static String vectorE, vectorB, vectorC, vectorA;
        private static Dictionary<int, String> vectorBProcesses, vectorCProcesses;
        private static int totalProcesses;
        private static int[] doneArr;

        public static List<int> todoProcesses, doneProcesses;
        public static bool P1done, P2done, P3done, P4done, P5done;
        public static String[,] history;
        public static int currentStep;
        public static String currentAnew;


        public Deadlock(Dictionary<string, int> d,
            String VE, String VB, String VC, String VA,
            int tProcesses,
            Dictionary<int, String> VBProcesses, Dictionary<int,String> VCProcesses )
        {

            Title = "Deadlock";

            currentStep = -1;
            exResDict = d; //exResDict["dvd"]
            vectorE = VE;
            vectorB = VB;
            vectorC = VC;
            vectorA = VA;
            vectorBProcesses = VBProcesses;
            vectorCProcesses = VCProcesses;
            totalProcesses = tProcesses;
            doneArr = new int[5] { 0,0,0,0,0};
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

            ShowMyHint();
            bool deadlock = CheckIfDeadlock();
            if (deadlock)
            {
                l_info.TextColor = Color.Red;
                l_info.Text = "Some processes cannot terminate => deadlock.";
            }

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
                    new RowDefinition{ Height = new GridLength(5, GridUnitType.Star)},
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star)}
                };
            CreateTopHalf(grid);
            CreateBottomHalf(grid);
        }

        /**********************************************************************
        *********************************************************************/
        void CreateTopHalf(Grid grid)
        {
            listView = new ListView
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
                Margin = new Thickness(10),

            };

            l_info = new Label
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Text = "pick a process",
                TextColor = Color.Blue,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            stackLayout.Children.Add(l_info);

            b_undo = new Button
            {
                Text = "Undo"
            };
            b_undo.Clicked += B_Undo_Clicked;
            b_undo.IsEnabled = false;

            stackLayout.Children.Add(b_undo);

            grid.Children.Add(stackLayout, 0, 1);
        }

        /**********************************************************************
        *********************************************************************/
        static void B_Undo_Clicked(object sender, EventArgs e)
        {
            deadlockCells.Remove(deadlockCells.Last());

            //Variablen und Listen updaten
            int processNumber = doneProcesses.Last();
            Debug.WriteLine("done processes Last:" + doneProcesses.Last());
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

            //check if undo is still possible
            if (!doneProcesses.Any())
            {
                b_undo.IsEnabled = false;
                Debug.WriteLine("Empty");
            }

            //TODO: enable touch sensitiveness
            deadlockCells[0].SetTouchSensitive(true);
            deadlockCells[0].skiaview.EnableTouchEvents = true;
            deadlockCells[0].touchable = true;
            deadlockCells.Last().SetTouchSensitive(true);
            deadlockCells.Last().skiaview.EnableTouchEvents = true;
            deadlockCells.Last().touchable = true;
            deadlockCells.Last().Paint();
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
            //TODO: hier Referenz übergeben!!!
            deadlockCells.Add(new DeadlockViewCell()); //actually creates a new deadlockviewcell and doesn't pass the reference

   
            //listView.ScrollTo(deadlockCells[deadlockCells.Count-1], ScrollToPosition.End, false);
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
        public static int[] GetDoneArr()
        {
            return doneArr;
        }
        /* LOGIC
         * *********************************************************************
        *********************************************************************/

        public static void CPx_Clicked(ref SKCanvasView sender, ref bool touchable,
            int processNumber, String oldVA)
        {
            bool found = todoProcesses.Contains(processNumber);
            bool fits = CheckIfUpcomingProcessFitsInVectorA(vectorCProcesses[processNumber], oldVA);
            //l_info.Text = "";
            //l_info.TextColor = Color.Black;
            if (!fits && found)
            {
                l_info.TextColor = Color.Red;
                l_info.Text = "Not enough free resources for upcoming requests C(P" + processNumber + ") !";
            }
            if (found && fits)
            {
     
                todoProcesses.Remove(processNumber);
                doneArr[processNumber - 1] = processNumber;
                doneProcesses.Add(processNumber);
                sender.EnableTouchEvents = false;
                touchable = false;
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
            l_info.Text = "Process " + processNumber + " done! Pick another process.";

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
