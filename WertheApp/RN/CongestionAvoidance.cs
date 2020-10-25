using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;

namespace WertheApp.RN
{
    public class CongestionAvoidance : ContentPage
    {
        //VARIABLES
        public static bool renoOn;
        public static bool tahoeOn;
        private static bool flag1; //indicates that state was changed from 1 to 0 because of 3 dupACK, so that 3 dupACK doesn't trigger an action twice
        private static bool flagDupAckAndNoNewRound; // indicates if currently stuck waiting and no new round has begun yet. Maybe solve this problem in future by creating a new state instead
        bool landscape = false; //indicates device orientation
        double StackChildSize;


        private double width = 0;
		private double height = 0;

        private SKCanvasView skiaview;
        private CongestionAvoidanceDraw draw;

        Button b_DupAck, b_Timeout, b_NewAck, b_Restart;
        private Color orange1 = new Color(242, 115, 0);

        public static int stateT, stateR; //0 -> slow start, 1 -> congestion avoidance, 2 -> fast recovery
        public static int dupAckCountR, dupAckCountT;
        public static int cwndR, cwndT, cwndTBeforeDupAck, cwndRBeforeDupAck;
        public static int currentRoundR, currentRoundT, currentIndex;
        public const int numberOfRounds = 32;
        public static int maxCwnd;
        public static int tresholdR,tresholdT;
        private static int initial_tresh;
        public static int[,] reno; //contains y values for reno [value, round]
        public static int[,] tahoe; //contains y values for tahoe
        public static int[,] sstreshR; //contains y values for treshold Reno
        public static int[,] sstreshT; //contains y values for treshold Tahoe
        public static String strategy;

        //CONSTRUCTOR
        public CongestionAvoidance(int th, bool r, bool t)
        {
            initial_tresh = th;
            tresholdR = th;
            tresholdT = th;
            renoOn = r;
            tahoeOn = t;
            flag1 = false;
            flagDupAckAndNoNewRound = false;
            stateT = 0;
            stateR = 0;
            dupAckCountR = 0;
            dupAckCountT = 0;
            cwndR = 1;
            cwndT = 1;
            currentRoundR = 0;
            currentRoundT = 0;
            currentIndex = 0;
            maxCwnd = 14;

            //note: numberOfRounds*4 is kinda arbitrary. Since there can be no arrays of unknown length, but it has to be long enough
            reno = new int[2,numberOfRounds*4]; 
            tahoe = new int[2,numberOfRounds*4];
            reno[1, 0] = 0;
            reno[0,0] = cwndR;
            tahoe[1, 0] = 0;
            tahoe[0,0] = cwndT;

            sstreshR = new int[2,numberOfRounds*4];
            sstreshT = new int[2,numberOfRounds*4];
            sstreshR[1, 0] = 0;
            sstreshR[0,0] = tresholdR;
            sstreshT[1, 0] = 0;
            sstreshT[0,0] = tresholdT;

            if (tahoeOn && !renoOn)
            {
                strategy = "Tahoe";
            }
            else if (renoOn && !tahoeOn)
            {
                strategy = "Reno";
            }
            else { strategy = "Reno & Tahoe"; }
            Title = "Congestion Control: " + strategy;

            draw = new CongestionAvoidanceDraw();

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


        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        void B_NewAck_Clicked(object sender, EventArgs e)
        {
            dupAckCountR = 0;
            dupAckCountT = 0;
            currentIndex++;
            flagDupAckAndNoNewRound = false;

            //RENO:
            switch (stateR){
                case 0:
                    currentRoundR++;
                    int cwndROld = cwndR;
                    cwndR = cwndR + cwndR; //exponential growth
                    if (cwndR >= tresholdR) 
                    { 
                        stateR = 1; //switch to congestion avoidance
                        if (cwndROld < tresholdR) { cwndR = tresholdR; } 
                    } 
                    break;
                case 1:
                    currentRoundR++;
                    cwndR++; //linear growth
                    break;
                case 2:
                    currentRoundR++; 
                    cwndR = tresholdR;
                    stateR = 1; //switch to congestion avoidance
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                    currentRoundT++;
                    int cwndTOld = cwndT;
                    cwndT = cwndT + cwndT; //exponential growth
                    if (cwndT >= tresholdT) 
                    { 
                        stateT = 1; //switch to congestion avoidance
                        if(cwndTOld < tresholdT){ cwndT = tresholdT;}
                    } 
                    break;
                case 1:
                    currentRoundT++;
                    cwndT++;
                    break;
            }

            UpdateButtons();
            SaveState();
            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void B_DupAck_Clicked(object sender, EventArgs e)
        {
            dupAckCountR++;
            dupAckCountT++;
            currentIndex++;

            if (!flagDupAckAndNoNewRound)
            {
                cwndTBeforeDupAck = cwndT; //save cwnd before the first dupAck was sent
                cwndRBeforeDupAck = cwndR;
            }
            flagDupAckAndNoNewRound = true;


            //RENO:
            switch (stateR)
            {
                case 0:
                    if (dupAckCountR == 3) 
                    {
                        //*geändert*/currentRoundR++;
                        tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                        cwndR = tresholdR + 3; 
                        stateR = 2; //switch to fast recovery
                    }
                    break;
                case 1:
                    if (dupAckCountR == 3)
                    {
                        tresholdR = ((cwndR / 2) >= 1 ? (cwndR / 2) : 1); //cannot be smaller than 1
                        cwndR = tresholdR + 3;
                        stateR = 2; //switch to fast recovery
                    }
                    break;
                case 2:
                    cwndR++;
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                    if(dupAckCountT == 3 && !flag1)
                    {
                        tresholdT = (cwndT / 2 >= 1 ? cwndT / 2 : 1); //cannot be smaller than 1
                        cwndT = 1;
                    }
                    flag1 = false;
                    break;
                case 1:
                    if (dupAckCountT == 3)
                    {
                        tresholdT = (cwndT / 2 >= 1 ? cwndT / 2 : 1); //cannot be smaller than 1
                        cwndT = 1;
                        stateT = 0; //Switch to Slow Start
                        flag1 = true;
                    }
                    break;
            }

            UpdateButtons();
            SaveState();
            UpdateDrawing();
        }

        /**********************************************************************
        *********************************************************************/
        void B_Timeout_Clicked(object sender, EventArgs e)
        {
            currentRoundR++;
            currentRoundT++;
            currentIndex++;
            int dupAckCountTOld = dupAckCountT;
            dupAckCountR = 0;
            dupAckCountT = 0;
            flagDupAckAndNoNewRound = false;

            //RENO:
            switch (stateR)
            {
                case 0:
                    tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                    cwndR = 1;
                    break;
                case 1:
                    tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                    cwndR = 1;
                    stateR = 0;
                    break;
                case 2:
                    tresholdR = (cwndR / 2 >= 1 ? cwndR / 2 : 1); //cannot be smaller than 1
                    cwndR = 1;
                    stateR = 0;
                    break;
            }

            //TAHOE:
            switch (stateT)
            {
                case 0:
                  
                    if(dupAckCountTOld >= 3)
                    {
                        //use old trshold since no new round has begun
                        int dummy = tahoe[0, currentIndex - dupAckCountTOld];
                        tresholdT = (dummy / 2 >= 1 ? dummy / 2 : 1); //cannot be smaller than 1
                    }
                    else
                    {
                        tresholdT = (cwndT / 2 >= 1 ? cwndT / 2 : 1); //cannot be smaller than 1
                    }
                    
                    cwndT = 1;
                    break;
                case 1:
                    
                    if (dupAckCountTOld >= 3)
                    {
                        //use old trshold since no new round has begun
                        int dummy = tahoe[0, currentIndex - dupAckCountTOld];
                        tresholdT = (dummy / 2 >= 1 ? dummy / 2 : 1); //cannot be smaller than 1
                    }
                    else
                    {
                        tresholdT = (cwndT / 2 >= 1 ? cwndT / 2 : 1); //cannot be smaller than 1
                    }
                    cwndT = 1;
                    stateT = 0;
                    break;
            }

            UpdateButtons();
            SaveState();
            UpdateDrawing();
        }





        /**********************************************************************
        *********************************************************************/
        void UpdateDrawing(){
            //update background
            CongestionAvoidanceDraw.stateR = stateR;
            CongestionAvoidanceDraw.stateT = stateT;
            CongestionAvoidanceDraw.Paint();

        }

        /***************************************************************
        *********************************************************************/
        void SaveState()
        {
            b_Restart.IsEnabled = true;

            if (renoOn)
            {
                sstreshR[0, currentIndex] = tresholdR;
                sstreshR[1, currentIndex] = currentRoundR;
                reno[0, currentIndex] = cwndR;
                reno[1, currentIndex] = currentRoundR;
            }
            if (tahoeOn)
            {
                sstreshT[0, currentIndex] = tresholdT;
                sstreshT[1, currentIndex] = currentRoundT;
                tahoe[0, currentIndex] = cwndT;
                tahoe[1, currentIndex] = currentRoundT;
            }
        }

        /***************************************************************
        *********************************************************************/

        void UpdateButtonColor()
        {
            switch (dupAckCountR)
            {
                case 0:
                    b_DupAck.TextColor = Color.Green;
                    break;
                case 1:
                    b_DupAck.TextColor = Color.DarkOrange;
                    break;
                case 2:
                    b_DupAck.TextColor = Color.Crimson;
                    break;
                case 3:
                    b_DupAck.TextColor = Color.Purple;
                    break;
                default:
                    b_DupAck.TextColor = Color.Purple;
                    break;
            }
        }

        void UpdateButtonText()
        {
            b_DupAck.Text = "Dup ACK (" + dupAckCountR + ")";
            switch (stateR)
            {
                case 0:
                    b_NewAck.Text = "New Acks";
                    break;
                case 1:
                    b_NewAck.Text = "New Acks";
                    break;
                case 2:
                    if (!tahoeOn && renoOn) { b_NewAck.Text = "New Ack"; }
                    else if (!renoOn && tahoeOn) { b_NewAck.Text = "New Acks"; }
                    else if (renoOn && tahoeOn) { b_NewAck.Text = "New Ack(s)"; }
                    break;
            }
        }

        void EnableDisableButtons()
        {
            b_NewAck.IsEnabled = true;
            b_DupAck.IsEnabled = true;

            //diable buttons according to the follwowing situations
            if (renoOn && !tahoeOn)
            {
                //if window is too big for screen
                if (cwndR >= maxCwnd)
                {
                    b_DupAck.IsEnabled = false;
                    //new ack only disabled when reno not in fast recovery
                    if (stateR != 2) { b_NewAck.IsEnabled = false; }
                }

                int window = cwndR;
                if (flagDupAckAndNoNewRound)
                {
                    window = cwndRBeforeDupAck;//if dupacks have already been sent, the cwnd might have changed during the current round, but we need to compare to the value at the beginning
                }
                if (dupAckCountR >= window - 1)
                {
                    b_DupAck.IsEnabled = false;
                }
            }

            else if (tahoeOn && !renoOn)
            {
                //if window is too big for screen
                if (cwndT >= maxCwnd)
                {
                    b_DupAck.IsEnabled = false;
                    b_NewAck.IsEnabled = false;
                }
                int window = cwndT;
                //check if more dupAcks can be sent
                if (flagDupAckAndNoNewRound)
                {
                    window = cwndTBeforeDupAck;//if dupacks have already been sent, the cwnd might have changed during the current round, but we need to compare to the value at the beginning
                }
                if (dupAckCountT >= window - 1)
                {
                    b_DupAck.IsEnabled = false;
                }
            }

            else if(renoOn && tahoeOn)
            {
                //if window is too big for screen
                if (cwndR >= maxCwnd)
                {
                    b_DupAck.IsEnabled = false;
                    //new ack only disabled when reno not in fast recovery
                    if (stateR != 2) { b_NewAck.IsEnabled = false; }
                }
                if(cwndT >= maxCwnd)
                {
                    b_DupAck.IsEnabled = false;
                    b_NewAck.IsEnabled = false;
                }
                int windowR = cwndR;
                int windowT = cwndT;
                if (flagDupAckAndNoNewRound)
                {
                    //if dupacks have already been sent, the cwnd might have changed during the current round, but we need to compare to the value at the beginning
                    windowR = cwndRBeforeDupAck;
                    windowT = cwndTBeforeDupAck;
                }
                //if no dup Acks can be send anymore
                if (dupAckCountT >= windowT - 1 || dupAckCountR >= windowR - 1)
                {
                    b_DupAck.IsEnabled = false;
                }
            }
        }

        void CheckIfMaxRoundsReached()
        {
            if (currentRoundR >= numberOfRounds && renoOn || currentRoundT >= numberOfRounds && tahoeOn)
            {
                b_DupAck.IsEnabled = false;
                b_NewAck.IsEnabled = false;
                b_Timeout.IsEnabled = false;
            }
        }

        void UpdateButtons(){

            UpdateButtonColor();
            UpdateButtonText();
            EnableDisableButtons();
            CheckIfMaxRoundsReached();

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
            skiaview = CongestionAvoidanceDraw.ReturnCanvas();
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
                StackChildSize = (Application.Current.MainPage.Height - 20) / 4;
            }
            else
            {
                StackChildSize = (Application.Current.MainPage.Width - 20) / 4;
            }

            //Using a Stacklayout to organize elements
            //with corresponding labels and String variables. 
            //For example l_Size, size
            var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Margin = new Thickness(10),

			};

            b_Restart = new Button
            {
                Text = "Restart",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Restart.Clicked += B_Restart_Clicked;
            b_Restart.IsEnabled = false;
            stackLayout.Children.Add(b_Restart);

            b_DupAck = new Button
            {
                Text = "Dup ACK (" + dupAckCountR +")",
                TextColor = Color.Green,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = StackChildSize
            };
            b_DupAck.IsEnabled = false;
            b_DupAck.Clicked += B_DupAck_Clicked;
            stackLayout.Children.Add(b_DupAck);

            b_Timeout = new Button
            {
                Text = "Timeout",
                WidthRequest = StackChildSize,
                VerticalOptions = LayoutOptions.Center
            };
            b_Timeout.Clicked += B_Timeout_Clicked;
            stackLayout.Children.Add(b_Timeout);

            b_NewAck = new Button
            {
                Text = "New Acks",
				WidthRequest = StackChildSize,
				VerticalOptions = LayoutOptions.Center
            };
            b_NewAck.Clicked += B_NewAck_Clicked;;
            stackLayout.Children.Add(b_NewAck);

            grid.Children.Add(stackLayout, 0, 1);
		}

        /**********************************************************************
        *********************************************************************/
        async void B_Restart_Clicked(object sender, EventArgs e)
        {
            tresholdR = initial_tresh;
            tresholdT = initial_tresh;
            flag1 = false;
            flagDupAckAndNoNewRound = false;
            stateT = 0;
            stateR = 0;
            dupAckCountR = 0;
            dupAckCountT = 0;
            cwndR = 1;
            cwndT = 1;
            currentRoundR = 0;
            currentRoundT = 0;
            currentIndex = 0;
            maxCwnd = 14;

            //note: numberOfRounds*4 is kinda arbitrary. Since there can be no arrays of unknown length, but it has to be long enough
            reno = new int[2, numberOfRounds * 4];
            tahoe = new int[2, numberOfRounds * 4];
            reno[1, 0] = 0;
            reno[0, 0] = cwndR;
            tahoe[1, 0] = 0;
            tahoe[0, 0] = cwndT;

            sstreshR = new int[2, numberOfRounds * 4];
            sstreshT = new int[2, numberOfRounds * 4];
            sstreshR[1, 0] = 0;
            sstreshR[0, 0] = tresholdR;
            sstreshT[1, 0] = 0;
            sstreshT[0, 0] = tresholdT;

            b_Restart.IsEnabled = false;
            UpdateButtons();
            UpdateDrawing();

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

        public static void PrintArray(int[,] array)
        {
            //Debug.WriteLine("");
            // Loop over 2D int array and display it.
            for (int i = 0; i <= array.GetUpperBound(0); i++)
            {
                String s = "";
                for (int j = 0; j <= array.GetUpperBound(1); j++)
                {
                    s += array[i, j];
                    s += " ";
                }
                //Debug.WriteLine(s);
            }
        }
    }
}