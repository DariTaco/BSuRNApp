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
        private static Button b_Next;
        public static ListView listView;
        public static ObservableCollection<DeadlockViewCell> deadlockCells; // deadlock canvas
        public static int cellNumber;


        // dictionary of individual amount of existing resources
        //keys: dvd, printer, usb, bluRay, ijPrinter, printer3D
        public static Dictionary<string, int> exResDict;
        private static String vectorE, vectorB, vectorC, vectorA;
        private static Dictionary<int, String> vectorBProcesses, vectorCProcesses;
        private static int totalProcesses;

   
        public Deadlock(Dictionary<string, int> d,
            String VE, String VB, String VC, String VA,
            int tProcesses,
            Dictionary<int, String> VBProcesses, Dictionary<int,String> VCProcesses )
        {

            Title = "Deadlock";

            exResDict = d; //exResDict["dvd"]
            vectorE = VE;
            vectorB = VB;
            vectorC = VC;
            vectorA = VA;
            vectorBProcesses = VBProcesses;
            vectorCProcesses = VCProcesses;
            totalProcesses = tProcesses;

            cellNumber = -1;

            Debug.WriteLine(VE + " " + VB + " " + VC + " " + VA + " " + totalProcesses);
            CreateContent();
        }

        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            // This is the top-level grid, which will split our page in half
            var grid = new Grid();
            this.Content = grid;
            grid.RowDefinitions = new RowDefinitionCollection {
                    new RowDefinition{ Height = new GridLength(7, GridUnitType.Star)},
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

            b_Next = new Button
            {
                Text = "Next",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            b_Next.Clicked += B_Next_Clicked;

            stackLayout.Children.Add(b_Next);
            grid.Children.Add(stackLayout, 0, 1);
        }

        /**********************************************************************
        *********************************************************************/
        static void B_Next_Clicked(object sender, EventArgs e)
        {
            cellNumber++;
            AddDeadlockCell();
        }

        /**********************************************************************
        *********************************************************************/
        public static void AddDeadlockCell()
        {
            deadlockCells.Add(new DeadlockViewCell()); //actually creates a new deadlockviewcell

            //listView.ScrollTo(deadlockCells[deadlockCells.Count - 1], ScrollToPosition.End, false);
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
    }
}
