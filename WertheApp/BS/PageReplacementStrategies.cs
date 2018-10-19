using System;
using CocosSharp;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq; //fragmentList.ElementAt(i);
using SkiaSharp.Views.Forms;
using SkiaSharp;
using System.Diagnostics;
namespace WertheApp.BS
{

    //TODO: Pagefails in ram array
    public class PageReplacementStrategies : ContentPage
    {
        //VARIABLES
        double StackChildSize;
        public static List<int> SequenceList { get; set; }
        public static String strategy;
        public static int ramSize;
        public static int discSize;

        public static int[,,] ram; //3d array to save the page numbers in ram (1d,2d) and if r-bits and m-bits are set(3d)
        public static int[,] disc; //2d array to save the page numbers in disc


        bool landscape = false; //indicates device orientation

		private double width = 0;
		private double height = 0;
        public int gameviewWidth;
        public int gameviewHeight;

        public static ScrollView scrollview;
        public static Button b_Reset_Rbits;
        public static Button b_Set_Mbit;
        public static Button b_Next;

        public static int currentStep;
        public static int currentPage;
        public static int sequenceLength;
        public static int indexCurrentPage;
        public static List<int> pagesInRam;
        public static List<int> pagesInDisc;

        private SKCanvasView skiaview;
        private PageReplacementStrategiesDraw draw;


        //CONSTRUCTOR
        public PageReplacementStrategies(List<int> l, String s, int r, int d)
        {
            
            SequenceList = l;
            strategy = s;
            ramSize = r;
            discSize = d;

            //Ram array explanation
            //[step, ram , 0] = pagenumber (range 0-9, -1 -> no page)
            //[step, ram , 1] = r-bit (0 -> not set, 1 -> set)
            //[step, ram , 2] = m-bit (0 -> not set, 1 -> set)
            //[step, ram , 3] = pagefail (0 -> no pagefail, 1 -> pagefail without replacement, 2 -> pagefail with replacement)
            //[step, ram , 4] = r bit were reset in this step -> 1, else -> 0
            //[step, ram , 5] = m bit was set in this step -> 1, else -> 0
            ram = new int[SequenceList.Count, ramSize, 6]; 
            disc = new int[SequenceList.Count, discSize];

            currentStep = -1; //no page in ram or disc yet
            currentPage = -1;
            indexCurrentPage = -1;
            sequenceLength = SequenceList.Count();
            pagesInRam = new List<int>();
            pagesInDisc = new List<int>();

            draw = new PageReplacementStrategiesDraw(ramSize, discSize, sequenceLength, SequenceList);

			Title = "Page Replacement Strategies"; //since the name is longer than average, 
            //the button ahead will automatically be named "back" instead of "Betriebssysteme"

            //if orientation Horizontal
            if (Application.Current.MainPage.Width < Application.Current.MainPage.Height)
            {
                landscape = false;
                CreateContent();
                this.Content.IsVisible = false;
            }
            //if orientation Landscape
            else
            {
                landscape = true;
                CreateContent();
            }

            //enable/disable buttons depending on strategy
            if(strategy == "RNU FIFO" || strategy == "RNU FIFO Second Chance")
            {
                b_Set_Mbit.IsEnabled = true;
                b_Reset_Rbits.IsEnabled = true;
            }
            else
            {
                b_Set_Mbit.IsEnabled = false;
                b_Reset_Rbits.IsEnabled = false;
            }
            b_Next.IsEnabled = true;
         
            InitializeDisc(disc);
            InitializeRam(ram);
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        void B_Reset_Rbits_Clicked(object sender, EventArgs e)
        {
            if(currentStep > -1){
                //reset all r-bits of current step in ram
                for (int j = 0; j <= ram.GetUpperBound(1); j++)
                {
                    if (ram[currentStep, j, 1] == 1)
                    {
                        ram[currentStep, j, 1] = 0; //r-bit reset
                        ram[currentStep, j, 4] = 1; //mark r-bit as reset
                    }

                }
                PrintRam(ram);
                PageReplacementStrategiesDraw.Paint();//Update Canvas
            }
        }

		/**********************************************************************
        *********************************************************************/
		void B_Set_Mbit_Clicked(object sender, EventArgs e)
        {
            if(currentStep > -1){
                //set m-bit of current page
                ram[currentStep, indexCurrentPage, 2] = 1;
                ram[currentStep, 0, 5] = 1;
                PrintRam(ram);
            }
            PageReplacementStrategiesDraw.Paint();//Update Canvas
        }

		/**********************************************************************
        *********************************************************************/
		void B_Next_Clicked(object sender, EventArgs e)
        {
            currentStep++;
            currentPage = SequenceList.First();
            SequenceList.Remove(SequenceList.First());
            switch (strategy)
            {
                case "Optimal Strategy":
                    Optimal();
                    break;
                case "FIFO":
                    Fifo();
                    break;
                case "FIFO Second Chance":
                    FifoSecond();
                    break;
                case "RNU FIFO":
                    Rnu();
                    break;
                case "RNU FIFO Second Chance":
                    RnuSecond();
                    break;
            }
            PrintRam(ram);
            PageReplacementStrategiesDraw.Paint();//Update Canvas
            //PrintDisc(disc);
            //disable next button if the sequence was processed entirely
            if (currentStep == sequenceLength-1){
                b_Next.IsEnabled = false;
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void Optimal()
        {
            int prevStep = currentStep - 1;

            //if ram is full 
            if (pagesInRam.Count == ramSize)
            {
                //if Pagefail, push optimal page in disc and push previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    PrintPagesInRam();
                    //find optimal page, which will be accessed the furthest in the future
                    int optimalPage = -1;
                    for (var i = 0; i < pagesInRam.Count; i++)
                    {
                        bool inSequenceList = false;
                        int optimalIndex = -1;
                        //under the pages in ram, find the page in remaining sequence which is the furthest away
                        for (var j = 0; j < SequenceList.Count; j++){
                            if(pagesInRam.ElementAt(i) == SequenceList.ElementAt(j) && j > optimalIndex){
                                inSequenceList = true;
                                optimalIndex = j;
                                optimalPage = pagesInRam.ElementAt(i);
                                j = SequenceList.Count; //exit for loop
                            }
                        }
                        if(!inSequenceList){
                            optimalPage = pagesInRam.ElementAt(i);
                            break;
                        }
                    }

                    pagesInRam.Remove(optimalPage);
                    pagesInDisc.Add(optimalPage);
                    if(pagesInDisc.Contains(currentPage)){
                        pagesInDisc.Remove(currentPage);
                    }
                    PrintPagesInDisc();

                    disc[currentStep, 0] = optimalPage;
                    //if there where pages in disc bevore, push previous pages forward
                    if (disc[currentStep - 1, 0] != -1)
                    {
                        for (int j = 1; j <= disc.GetUpperBound(1); j++)
                        {
                            if(pagesInDisc.Count -j -1 >= 0){
                                disc[currentStep, j] = pagesInDisc.ElementAt(pagesInDisc.Count - j - 1);
                            }else{
                                disc[currentStep, j] = -1;
                            }

                        }
                    }

                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Add(currentPage);
                    for (int j = 1; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = pagesInRam.ElementAt(pagesInRam.Count -1 - j);
                    }
                    ram[currentStep, 0, 3] = 2;//mark pagefail with replacment

                }
                //if page is already in ram, put current page at first place
                else
                {
                    //in ram
                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Remove(currentPage); // remove current page from it's position
                    pagesInRam.Add(currentPage); // and put it at the end of the list
                    for (int j = 0; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = pagesInRam.ElementAt(pagesInRam.Count - 1 - j);
                    }
                    //in disc leave everything as it is
                    for (int j = 0; j <= disc.GetUpperBound(1); j++)
                    {
                        disc[currentStep, j] = disc[prevStep, j];
                    }
                }
            }
            //if there's still space in ram
            else
            {
                //if page is not already in ram, push previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Add(currentPage);

                    //push previous pages forward in ram
                    if (currentStep > 0)
                    {
                        for (int j = 1; j <= ram.GetUpperBound(1); j++)
                        {
                            ram[currentStep, j, 0] = ram[prevStep, j - 1, 0];
                        }
                    }
                    ram[currentStep, 0, 3] = 1;//mark pagefail without replacment
                }
                //if current page is already in ram, put current page at first place
                else
                {
                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Remove(currentPage); // remove current page from it's position
                    pagesInRam.Add(currentPage); // and put it at the end of the list
                    for (int j = 0; j <= pagesInRam.Count - 1; j++)
                    {
                        ram[currentStep, j, 0] = pagesInRam.ElementAt(pagesInRam.Count - 1 - j);
                    }
                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void Fifo()
        {
            int prevStep = currentStep - 1;

            //if ram is full 
            if (pagesInRam.Count == ramSize)
            {
                //if Pagefail, push oldest page in disc and push previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    int oldestPageInRam = pagesInRam.First();
                    pagesInRam.Remove(pagesInRam.First());

                    pagesInDisc.Add(oldestPageInRam);
                    if (pagesInDisc.Contains(currentPage))
                    {
                        pagesInDisc.Remove(currentPage);
                    }
                    disc[currentStep, 0] = oldestPageInRam;
                    //if there where pages in disc bevore, push previous pages forward
                    if(disc[currentStep-1, 0] != -1){
                        for (int j = 1; j <= disc.GetUpperBound(1); j++)
                        {
                            if (pagesInDisc.Count - j - 1 >= 0)
                            {
                                disc[currentStep, j] = pagesInDisc.ElementAt(pagesInDisc.Count - j - 1);
                            }
                            else
                            {
                                disc[currentStep, j] = -1;
                            }
                        }
                    }

                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Add(currentPage);
                    for (int j = 1; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = ram[prevStep, j - 1, 0];
                    }
                    ram[currentStep, 0, 3] = 2;//mark pagefail with replacment

                }
                //if page is already in ram, leave everything as it is
                else
                {
                    //in ram
                    for (int j = 0; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = ram[prevStep, j, 0];
                    }
                    //in disc
                    for (int j = 0; j <= disc.GetUpperBound(1); j++)
                    {
                        disc[currentStep, j] = disc[prevStep, j];
                    }
                }
            }
            //if there's still space in ram
            else{
                //if page is not already in ram, push previous pages forward in ram
                if (!pagesInRam.Contains(currentPage)){
                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Add(currentPage);

                    //push previous pages forward in ram
                    if (currentStep > 0)
                    {
                        for (int j = 1; j <= ram.GetUpperBound(1); j++)
                        {
                            ram[currentStep, j, 0] = ram[prevStep, j - 1, 0];
                        }
                    }
                    ram[currentStep, 0, 3] = 1;//mark pagefail without replacment
                }
                //if page is already in ram, leave everything as it is
                else{
                    for (int j = 0; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = ram[prevStep, j, 0];
                    }
                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void FifoSecond()
        {
            int prevStep = currentStep - 1;

            //if ram is full 
            if (pagesInRam.Count == ramSize)
            {
                //if Pagefail, push oldest page in disc and push previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    int oldestPageInRam = pagesInRam.First();
                    pagesInRam.Remove(pagesInRam.First());

                    pagesInDisc.Add(oldestPageInRam);
                    disc[currentStep, 0] = oldestPageInRam;
                    if (pagesInDisc.Contains(currentPage))
                    {
                        pagesInDisc.Remove(currentPage);
                    }

                    //if there where pages in disc bevore, push previous pages forward
                    if (disc[currentStep - 1, 0] != -1)
                    {
                        for (int j = 1; j <= disc.GetUpperBound(1); j++)
                        {
                            if (pagesInDisc.Count - j - 1 >= 0)
                            {
                                disc[currentStep, j] = pagesInDisc.ElementAt(pagesInDisc.Count - j - 1);
                            }
                            else
                            {
                                disc[currentStep, j] = -1;
                            }
                        }
                    }

                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Add(currentPage);
                    for (int j = 1; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = ram[prevStep, j - 1, 0];
                    }
                    ram[currentStep, 0, 3] = 2;//mark pagefail with replacment
                }
                //if page is already in ram, put current page at first place
                else
                {
                    //in ram
                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Remove(currentPage); // remove current page from it's position
                    pagesInRam.Add(currentPage); // and put it at the end of the list
                    for (int j = 0; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = pagesInRam.ElementAt(pagesInRam.Count - 1 - j);
                    }
                    //in disc
                    for (int j = 0; j <= disc.GetUpperBound(1); j++)
                    {
                        disc[currentStep, j] = disc[prevStep, j];
                    }
                }
            }
            //if there's still space in ram
            else
            {
                //if page is not already in ram, push previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Add(currentPage);

                    //push previous pages forward in ram
                    if (currentStep > 0)
                    {
                        for (int j = 1; j <= ram.GetUpperBound(1); j++)
                        {
                            ram[currentStep, j, 0] = ram[prevStep, j - 1, 0];
                        }
                    }
                    ram[currentStep, 0, 3] = 1;//mark pagefail without replacment
                }
                //if current page is already in ram, put current page at first place
                else{
                    ram[currentStep, 0, 0] = currentPage;
                    pagesInRam.Remove(currentPage); // remove current page from it's position
                    pagesInRam.Add(currentPage); // and put it at the end of the list
                    for (int j = 0; j <= pagesInRam.Count-1; j++)
                    {
                        ram[currentStep, j, 0] = pagesInRam.ElementAt(pagesInRam.Count - 1 - j);
                    }
                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        //
        // 1. class  r0 m0
        // 2. class  r0 m1
        // 3. class  r1 m0
        // 4. class  r1 m1
        //
        //replace page with lowest class
        //
        //[step, ram , 0] = pagenumber (range 0-9, -1 -> no page)
        //[step, ram , 1] = r-bit (0 -> not set, 1 -> set)
        //[step, ram , 2] = m-bit (0 -> not set, 1 -> set)
        //[step, ram , 3] = pagefail (0 -> no pagefail, 1 -> pagefail without replacement, 2 -> pagefail with replacement)
        public static void Rnu()
        {
            int prevStep = currentStep - 1;

            //----------------------------------------------------------//
            //----------------------------------------------------------//
            //if ram is full 
            if (pagesInRam.Count == ramSize)
            {
                //----------------------------------------------------------//
                //----------------------------------------------------------//
                //if Pagefail, push rnu page in disc. in case two or more pages have the lowest class, push oldest page in disc 
                //alsopush previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    Debug.WriteLine("ram is full and page is not in ram");
                    PrintPagesInRam();
                    //find rnu page
                    int rnuPage = -1;
                    int rnuIndex = -1;
                    int lowestRnuClass = 5;

                    for (var i = 0; i < pagesInRam.Count; i++)
                    {
                        //find page with lowest class
                        bool r = false;
                        bool m = false;
                        int rnuClass = 6;
                        //determine if bits are set
                        if(ram[prevStep, i, 1] == 1){
                            r = true;
                        }
                        if(ram[prevStep, i, 2] == 1){
                            m = true;
                        }
                        //determine class
                        if (!r && !m) { rnuClass = 1; }
                        else if (!r && m) { rnuClass = 2; }
                        else if (r && !m) { rnuClass = 3; }
                        else if (r && m) { rnuClass = 4; }
                        //determine if lowest class so far
                        if(rnuClass <= lowestRnuClass){
                            rnuIndex = i; // oldest in list is left. oldest in ram is right
                            rnuPage = ram[prevStep, rnuIndex, 0];
                            lowestRnuClass = rnuClass;
                        }
                        Debug.WriteLine("Class of " + ram[prevStep, i, 0] + " is: " + rnuClass);
                    }
                    Debug.WriteLine("rnu page: " + rnuPage);
                    pagesInRam.Remove(rnuPage);
                    pagesInDisc.Add(rnuPage);
                    PrintPagesInRam();

                    //remove current page from disc if it is in disc
                    if (pagesInDisc.Contains(currentPage))
                    {
                        pagesInDisc.Remove(currentPage);
                    }
                    disc[currentStep, 0] =rnuPage;
                    //if there where pages in disc bevore, push previous pages forward
                    if (disc[currentStep - 1, 0] != -1)
                    {
                        for (int j = 1; j <= disc.GetUpperBound(1); j++)
                        {
                            if (pagesInDisc.Count - j - 1 >= 0)
                            {
                                disc[currentStep, j] = pagesInDisc.ElementAt(pagesInDisc.Count - j - 1);
                            }
                            else
                            {
                                disc[currentStep, j] = -1;
                            }
                        }
                    }

                    //rewrite ram
                    ram[currentStep, 0, 0] = currentPage;
                    ram[currentStep, 0, 1] = 1; //set r-bit by access
                    ram[currentStep, 0, 2] = 0; //no m bit set
                    indexCurrentPage = 0;
                    pagesInRam.Add(currentPage);
                    for (int i = 1; i <= ram.GetUpperBound(1); i++)
                    {
                        ram[currentStep, i, 0] = pagesInRam.ElementAt(pagesInRam.Count - 1 - i);
                        for (int j = 0; j <= ram.GetUpperBound(1); j++){
                            if(ram[prevStep, j, 0] == ram[currentStep, i, 0]){
                                ram[currentStep, i, 1] = ram[prevStep, j, 1];
                                ram[currentStep, i, 2] = ram[prevStep, j, 2];
                            }
                        }
                    }
                    ram[currentStep, 0, 3] = 2;//mark pagefail with replacment
                }
                //----------------------------------------------------------//
                //----------------------------------------------------------//
                //if page is already in ram, leave everything as it is
                else
                {
                    Debug.WriteLine("ram is full and page is already in ram");
                    PrintPagesInRam();
                    //in ram
                    for (int j = 0; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = ram[prevStep, j, 0];
                        ram[currentStep, j, 1] = ram[prevStep, j, 1];
                        ram[currentStep, j, 2] = ram[prevStep, j, 2];
                        if (ram[currentStep, j, 0] == currentPage)
                        {
                            ram[currentStep, j, 1] = 1; //set r-bit by access
                            indexCurrentPage = j; 
                        }
                    }

                    //in disc
                    for (int j = 0; j <= disc.GetUpperBound(1); j++)
                    {
                        disc[currentStep, j] = disc[prevStep, j];
                    }
                }
            }
            //----------------------------------------------------------//
            //----------------------------------------------------------//
            //if there's still space in ram
            else
            {
                //----------------------------------------------------------//
                //----------------------------------------------------------//
                //if page is not already in ram, push previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    Debug.WriteLine("ram is not full and page is not in ram");
                    PrintPagesInRam();
                    ram[currentStep, 0, 0] = currentPage;
                    ram[currentStep, 0, 1] = 1; //set r-bit by access 
                    ram[currentStep, 0, 2] = 0; //no m bit set
                    indexCurrentPage = 0; 
                    pagesInRam.Add(currentPage);

                    //push previous pages forward in ram
                    if (currentStep > 0)
                    {
                        for (int j = 1; j <= ram.GetUpperBound(1); j++)
                        {
                            ram[currentStep, j, 0] = ram[prevStep, j - 1, 0];
                            ram[currentStep, j, 1] = ram[prevStep, j - 1, 1];
                            ram[currentStep, j, 2] = ram[prevStep, j - 1, 2];
                        }
                    }
                    ram[currentStep, 0, 3] = 1;//mark pagefail without replacment
                }
                //----------------------------------------------------------//
                //----------------------------------------------------------//
                //if page is already in ram, leave everything as it is
                else
                {
                    Debug.WriteLine("ram is not full and page is already in ram");
                    PrintPagesInRam();
                    for (int j = 0; j <= ram.GetUpperBound(1); j++)
                    {
                        ram[currentStep, j, 0] = ram[prevStep, j, 0];
                        ram[currentStep, j, 1] = ram[prevStep, j, 1];
                        ram[currentStep, j, 2] = ram[prevStep, j, 2];
                        if (ram[currentStep, j, 0] == currentPage)
                        {
                            //set r-bit bei Zugriff
                            ram[currentStep, j, 1] = 1;
                            indexCurrentPage = j; 
                        }
                    }
                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void RnuSecond()
        {
            int prevStep = currentStep - 1;

            //if ram is full 
            if (pagesInRam.Count == ramSize)
            {
                //if Pagefail, push rnu page in disc. in case two or more pages have the lowest class, push oldest page in disc 
                //alsopush previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    Debug.WriteLine("ram is full and page is not in ram");
                    PrintPagesInRam();
                    //find rnu page
                    int rnuPage = -1;
                    int rnuIndex = -1;
                    int lowestRnuClass = 5;
                   
                    for (var i = 0; i < pagesInRam.Count; i++)
                    {
                        //find page with lowest class
                        bool r = false;
                        bool m = false;
                        int rnuClass = 6;
                        //determine if bits are set
                        if (ram[prevStep, i, 1] == 1)
                        {
                            r = true;
                        }
                        if (ram[prevStep, i, 2] == 1)
                        {
                            m = true;
                        }
                        //determine class
                        if (!r && !m) { rnuClass = 1; }
                        else if (!r && m) { rnuClass = 2; }
                        else if (r && !m) { rnuClass = 3; }
                        else if (r && m) { rnuClass = 4; }
                        //determine if lowest class so far
                        if (rnuClass <= lowestRnuClass)
                        {
                            rnuIndex = i; // oldest in list is left. oldest in ram is right
                            rnuPage = ram[prevStep, rnuIndex, 0];
                            lowestRnuClass = rnuClass;
                        }
                        Debug.WriteLine("Class of " + ram[prevStep, i, 0] + " is: " + rnuClass);
                    }
                    Debug.WriteLine("rnu page: " + rnuPage);
                    pagesInRam.Remove(rnuPage);
                    pagesInDisc.Add(rnuPage);
                    PrintPagesInRam();

                    //remove current page from disc if it is in disc
                    if (pagesInDisc.Contains(currentPage))
                    {
                        pagesInDisc.Remove(currentPage);
                    }
                    disc[currentStep, 0] = rnuPage;
                    //if there where pages in disc bevore, push previous pages forward
                    if (disc[currentStep - 1, 0] != -1)
                    {
                        for (int j = 1; j <= disc.GetUpperBound(1); j++)
                        {
                            if (pagesInDisc.Count - j - 1 >= 0)
                            {
                                disc[currentStep, j] = pagesInDisc.ElementAt(pagesInDisc.Count - j - 1);
                            }
                            else
                            {
                                disc[currentStep, j] = -1;
                            }
                        }
                    }

                    //rewrite ram
                    ram[currentStep, 0, 0] = currentPage;
                    ram[currentStep, 0, 1] = 1; //set r-bit by access
                    ram[currentStep, 0, 2] = 0; //no m bit set
                    indexCurrentPage = 0;
                    pagesInRam.Add(currentPage);
                    for (int i = 1; i <= ram.GetUpperBound(1); i++)
                    {
                        ram[currentStep, i, 0] = pagesInRam.ElementAt(pagesInRam.Count - 1 - i);
                        for (int j = 0; j <= ram.GetUpperBound(1); j++)
                        {
                            if (ram[prevStep, j, 0] == ram[currentStep, i, 0])
                            {
                                ram[currentStep, i, 1] = ram[prevStep, j, 1];
                                ram[currentStep, i, 2] = ram[prevStep, j, 2];
                            }
                        }
                    }
                    ram[currentStep, 0, 3] = 2;//mark pagefail with replacment

                }
                //if page is already in ram, put current page at first place
                else
                {
                    //rewrite ram
                    ram[currentStep, 0, 0] = currentPage;
                    ram[currentStep, 0, 1] = 1; //set r-bit by access
                    ram[currentStep, 0, 2] = 0; //no m bit set
                    indexCurrentPage = 0;
                    pagesInRam.Remove(currentPage); // remove current page from it's position
                    pagesInRam.Add(currentPage); // and put it at the end of the list
                    for (int i = 0; i <= ram.GetUpperBound(1); i++)
                    {
                        ram[currentStep, i, 0] = pagesInRam.ElementAt(pagesInRam.Count - 1 - i);
                        ram[currentStep, 0, 1] = 1; //set r-bit
                        indexCurrentPage = 0;
                        for (int j = 0; j <= ram.GetUpperBound(1); j++)
                        {
                            if (ram[prevStep, j, 0] == ram[currentStep, i, 0])
                            {
                                ram[currentStep, i, 1] = ram[prevStep, j, 1];
                                ram[currentStep, i, 2] = ram[prevStep, j, 2];
                            }
                        }

                    }
                    //in disc
                    for (int j = 0; j <= disc.GetUpperBound(1); j++)
                    {
                        disc[currentStep, j] = disc[prevStep, j];
                    }
                }

                //################rearange ram ############//
            }
            //if there's still space in ram
            else
            {
                //if page is not already in ram, push previous pages forward in ram
                if (!pagesInRam.Contains(currentPage))
                {
                    Debug.WriteLine("ram is not full and page is not in ram");
                    PrintPagesInRam();
                    ram[currentStep, 0, 0] = currentPage;
                    ram[currentStep, 0, 1] = 1; //set r-bit by access 
                    ram[currentStep, 0, 2] = 0; //no m bit set
                    indexCurrentPage = 0; 
                    pagesInRam.Add(currentPage);

                    //push previous pages forward in ram
                    if (currentStep > 0)
                    {
                        for (int j = 1; j <= ram.GetUpperBound(1); j++)
                        {
                            ram[currentStep, j, 0] = ram[prevStep, j - 1, 0];
                            ram[currentStep, j, 1] = ram[prevStep, j - 1, 1];
                            ram[currentStep, j, 2] = ram[prevStep, j - 1, 2];
                        }
                    }
                    ram[currentStep, 0, 3] = 1;//mark pagefail without replacment
                }
///////////////
                //if current page is already in ram, put current page at first place
                else
                {
                    ram[currentStep, 0, 0] = currentPage;
                    ram[currentStep, 0, 1] = 1; //set r-bit by access
                    ram[currentStep, 0, 2] = 0; //no m bit set
                    indexCurrentPage = 0;
                    pagesInRam.Remove(currentPage); // remove current page from it's position
                    pagesInRam.Add(currentPage); // and put it at the end of the list
                    for (int i = 0; i <= pagesInRam.Count - 1; i++)
                    {
                        PrintPagesInRam();
                        Debug.WriteLine("current step: " + currentStep);
                        Debug.WriteLine("Pagesinram.count" + pagesInRam.Count);
                        Debug.WriteLine("i:" + i);

                        //TODO: Sonderfall 0,0 Sequenz am Anfang (tritt sonst nirgends auf) oder 011. immer wenn ram nicht voll
                        ram[currentStep, i, 0] = pagesInRam.ElementAt(pagesInRam.Count - 1 - i);
                        ram[currentStep, 0, 1] = 1; //set r-bit
                        indexCurrentPage = 0;
                        for (int j = 0; j <= ram.GetUpperBound(1); j++)
                        {
                            if (ram[prevStep, j, 0] == ram[currentStep, i, 0])
                            {
                                ram[currentStep, i, 1] = ram[prevStep, j, 1];
                                ram[currentStep, i, 2] = ram[prevStep, j, 2];
                            }
                        }
                    }
                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        //Gets called everytime the Page is not shown anymore. For example when clicking the back navigation
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            // This is the top-level grid, which will split our page in half
            var grid = new Grid();
            this.Content = grid;
            grid.RowDefinitions = new RowDefinitionCollection {
                    // Each half will be the same size:
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
            skiaview = new SKCanvasView();
            skiaview = PageReplacementStrategiesDraw.ReturnCanvas();
            grid.Children.Add(skiaview, 0, 0);
        }

        /**********************************************************************
        *********************************************************************/
        void CreateBottomHalf(Grid grid)
        {
            //set the size of the elements in such a way, that they all fit on the screen
            //Screen Width is divided by the amount of elements (3)
            //Screen Width -20 because Margin is 10
            if (!landscape)
            {
                StackChildSize = (Application.Current.MainPage.Height - 20) / 3;
            }
            else
            {
                StackChildSize = (Application.Current.MainPage.Width - 20) / 3;
            }

            //Using a Stacklayout to organize elements
            //with corresponding labels and String variables. 
            //For example l_Size, size
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Margin = new Thickness(10),

            };

            b_Reset_Rbits = new Button
            {
                Text = "Reset R-Bits",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Reset_Rbits.Clicked += B_Reset_Rbits_Clicked;
            stackLayout.Children.Add(b_Reset_Rbits);

            b_Set_Mbit = new Button
            {
                Text = "Set M-Bit",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Set_Mbit.Clicked += B_Set_Mbit_Clicked;
            stackLayout.Children.Add(b_Set_Mbit);

            b_Next = new Button
            {
                Text = "Next",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Next.Clicked += B_Next_Clicked;
            stackLayout.Children.Add(b_Next);

            grid.Children.Add(stackLayout, 0, 1);
        }

		/**********************************************************************
        *********************************************************************/
		//this method is called everytime the device is rotated
		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height); //must be called
			if (this.width != width || this.height != height)
			{
				this.width = width;
				this.height = height;
			}

            //reconfigure layout
            if (width > height)
            {
                //isContentCreated = true;
                this.Content.IsVisible = true;
            }
            else if (height > width)
            {
                this.Content.IsVisible = false;
            }
		}


        /**********************************************************************
        *********************************************************************/
        static void InitializeRam(int[,,] array)
        {

            // Loop over 2D int array and fill it with default values
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                {
                    array[i, j, 0] = -1; //-1 means no page (pages range from 0 to 9)
                    array[i, j, 1] = 0; //r-bit is not set
                    array[i, j, 2] = 0; //m-bit is not set
                    array[i, j, 3] = 0; //no pagefail
                    array[i, j, 4] = 0; //no r-bits were reset
                    array[i, j, 5] = 0; //no m-bit was set
                }

            }
        }

        /**********************************************************************
        *********************************************************************/
        static void InitializeDisc(int[,] array)
        {
            // Loop over 2D int array and fill it with default values
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                {
                    array[i, j] = -1; //-1 means no page (pages range from 0 to 9)
                }
            }
        }


        /**********************************************************************
        *********************************************************************/
        //print 2d array
        static void PrintDisc(int[,] array)
        {
            Debug.WriteLine("");
            Debug.WriteLine("DISC " + SequenceList.Count + "x" + discSize);
            // Loop over 2D int array and display it.
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                String s = "";
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                {
                    s += array[i, j];
                    s += " ";
                }
                Debug.WriteLine(s);
            }
        }

        /**********************************************************************
        *********************************************************************/
        //print 3d array
        static void PrintRam(int[,,] array)
        {
            Debug.WriteLine("");
            Debug.WriteLine("RAM" + SequenceList.Count + "x" + ramSize);
            // Loop over 3D int array and display it.
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                String s = "";
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                {
                    String rbit = "x";
                    String mbit = "x";
                    //if rbit is set
                    if(array[i, j, 1] == 1){
                        rbit = "r";
                    }
                    //if mbit is set
                    if (array[i, j, 2] == 1)
                    {
                        mbit = "m";
                    }
                    //if page is not -1
                    s += array[i, j, 0] + rbit + mbit;
                    s += " ";
                }
                Debug.WriteLine(s);
            }
        }

        static void PrintPagesInRam(){
            String s = "";
            foreach(var p in pagesInRam){
                s += ", " + p;
            }
            Debug.WriteLine(s);
        }

        static void PrintPagesInDisc(){
            String s = "";
            foreach (var p in pagesInDisc)
            {
                s += ", " + p;
            }
            Debug.WriteLine(s);
        }
        /**********************************************************************
        *********************************************************************/
    }
}