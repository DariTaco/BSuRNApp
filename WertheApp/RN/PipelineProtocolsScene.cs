/************************CLASS FOR SELECTIVE REPEAT****************************/
using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Xamarin.Forms;
using System.Threading.Tasks;

//TAKE NOTE: the reference value of an action is always the same!! 
//It always refers to the values that were set when the original object was first definded.
//and not some altered value after some action that happended before the current one

/*TODO
slow down anders implementieren??(wird teilweise zu langsam/ zu schnell -> beachten wie früh geklickt wird/ bzw wie weit das Packet ist)
*/
namespace WertheApp.RN
{
    public class PipelineProtocolsScene : CCScene
    {
		//VARIABLES
        static CCLayer layer;

        public static bool stopEverything; //code is still running when page is not displayed anymore. Therefore there has to be a variable to stop everything
        public static bool animationIsPaused; //indicates i the pause button(PipelineProtocols.cs) was clicked

        static String strategy;
        static int windowSize;
        static int timeouttime;

        static int baseOfWindowLeft; //first sent but not yet acknowledged sequence number
        static int baseOfWindowRight; //first not yet received package sequencenumber
        public static int nextSeqnum; //first not sent yet sequence number

        static List<int> pendingAck;
        static List<int> arrivedPack;
        static List<int> arrivedAck;

        static CCRect window;
        static CCRect window2;
        static CCDrawNode cc_window;
        static CCDrawNode cc_window2;

          
        //CONSTRUCTOR
        public PipelineProtocolsScene(CCGameView gameView) : base(gameView)
        {
            stopEverything = false;
            animationIsPaused = false;

            //add a layer to draw on
            layer = new CCLayer();
            this.AddLayer(layer);

            windowSize = PipelineProtocols.windowSize;
            strategy = PipelineProtocols.strategy;
            timeouttime = PipelineProtocols.timeoutTime;

            pendingAck = new List<int>();
            arrivedPack = new List<int>();
            arrivedAck = new List<int>();

			//Android bug: Background in Android is always black. Workaround: draw a white rect with the size of the layer
			if (Device.RuntimePlatform == Device.Android)
			{
				var cc_background = new CCDrawNode();
				var backgroundWorkAround = new CCRect(
					0, 0, layer.VisibleBoundsWorldspace.MaxX, layer.VisibleBoundsWorldspace.MaxY);
				cc_background.DrawRect(backgroundWorkAround,
					fillColor: CCColor4B.White);
				layer.AddChild(cc_background);
			}

            DrawLabelsAndBoxes();

            baseOfWindowLeft = 0;
            baseOfWindowRight = 0;
            nextSeqnum = 0;

            DrawWindowLeft(baseOfWindowLeft);
            DrawWindowRight(baseOfWindowRight);
        }

        //METHODS

        /**********************************************************************
        *********************************************************************/
        /*TODO*/
        public static void MyTimer(int seqnum, int c)
        {
            int counter = c;

			//draw counter label in respective rectangle
			float a = 28 - seqnum;
			float yPos = 15 + (a * 65);
            String counterText = "" + counter;
            var ccl_LNumber = new CCLabel(counterText, "Arial", 20);
			ccl_LNumber.Position = new CCPoint(60, yPos + 25);
            ccl_LNumber.Color = CCColor3B.Red;
			layer.AddChild(ccl_LNumber);

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                // update counter label- but only if the user didn't click pause 
                if(!animationIsPaused){
                    counter++;
                }
                counterText = "" + counter;
                ccl_LNumber.Text = counterText;

                if (stopEverything || counter == timeouttime)
                {
                    layer.RemoveChild(ccl_LNumber);

                    //resend pending ACK after timeout if seqnum is in list
                    if (pendingAck.Any() && pendingAck.Contains(seqnum) && !stopEverything)
                    {
                        SendPackageAt(seqnum);
                        MyTimer(seqnum, 0);
                    }
                    return false; //False = Stop the timer
                }else { return true; } // True = Repeat again
             });
        }

        /**********************************************************************
        *********************************************************************/
        public static void InvokeSender()
        {
            //if window is not full, that means if there are still packets left inside the window which are not sent yet
            if(nextSeqnum < (baseOfWindowLeft+windowSize))
            {
                SendPackageAt(nextSeqnum);
                MyTimer(nextSeqnum, 0); 
                nextSeqnum++;
            }
        }

        /**********************************************************************
        *********************************************************************/
        //this method imitates the sender of a packet. It is called by the method invoke 
        public static void SendPackageAt(int seqnum)
        {
            if(seqnum == nextSeqnum){
                DrawFillLeft(seqnum);  
            }
         
            //pending if not already acknowledged . Add to list only once
            if(!arrivedAck.Any() && !pendingAck.Contains(seqnum)
               || arrivedAck.Any() && !arrivedAck.Contains(seqnum) && !pendingAck.Contains(seqnum)){
                pendingAck.Add(seqnum);
            }

            //define object
            float yPos = 15 + (65 * (28 - seqnum)); //calculate where the box !starts! in the coordinate system
            var pp = new PipelineProtocolsPack(seqnum,0);
            pp.Position = new CCPoint(80, yPos);
            layer.AddChild(pp);

            //define actions
            float timeToTake = 5f;
            var distance = new CCPoint(280, yPos); //82 to 278 = 278-82 = 196
            var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
            var removeAction = new CCRemoveSelf(); //this action removes the object

            //define sequence of actions and apply to object
            var cc_seq1 = new CCSequence(sendPackageAction, removeAction);
	        pp.RunAction(cc_seq1); 
        }

        /**********************************************************************
        *********************************************************************/
        //Receiver sends ACK
        public static void SendACKFor(int seqnum)
        {
            //define object
            float yPos = 15 + (65 * (28 - seqnum)); //where the box !starts!
            PipelineProtocolsACK pp;										

            //smaller rectangle at -- 
			switch (seqnum)
			{
				case -1:
					pp = new PipelineProtocolsACK(seqnum, 5, 1);
					yPos = yPos + 12; //since it's smaller, it has to be a little further up, in order to look pretty
					break;
				default:
					pp = new PipelineProtocolsACK(seqnum, 0);
					break;
			}
            pp.Position = new CCPoint(280, yPos);
            layer.AddChild(pp);

            //define action
            float timeToTake = 5f;
            var distance = new CCPoint(80, yPos); //82 to 278 = 278-82 = 196
            var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
            var removeAction = new CCRemoveSelf(); //this action removes the object*/

            //define sequence of actions and apply to object
            var cc_seq1 = new CCSequence(sendPackageAction, removeAction);
            pp.RunAction(cc_seq1);
        }

        /**********************************************************************
        *********************************************************************/
        public static void SlowDownPack(PipelineProtocolsPack pp, int xPos)
        {
            //stop running actions
            pp.StopAllActions();

            //define actions
            float yPos = 15 + (65 * (28 - pp.seqnum));
            float timeToTake = 8f;
            var distance = new CCPoint(280, yPos);
            var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the Object to the CCPoint
            var removeAction = new CCRemoveSelf(); //this action removes the object*/

            //define sequence of actions and apply to object 
            var cc_seq1 = new CCSequence(sendPackageAction, removeAction);
            pp.RunAction(cc_seq1);
        }

        /**********************************************************************
        *********************************************************************/
        /*TODO */
        public static void SlowDownAck(PipelineProtocolsACK aa, int xPos)
        {
            //stop running actions
            aa.StopAllActions();

            //define actions
            float timeToTake = 8f; //SET TIME TO TAKE DEPENDING ON xPos?

            float yPos = 15 + (65 * (28 - aa.seqnum));
            var distance = new CCPoint(80, yPos);
            var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the Object to the CCPoint
            var removeAction = new CCRemoveSelf(); //this action removes the object*/

            //define sequence of actions and apply to object 
            var cc_seq1 = new CCSequence(sendPackageAction, removeAction);
            aa.RunAction(cc_seq1);
        }

        /**********************************************************************
        *********************************************************************/
        public static void PackCorrupt(PipelineProtocolsPack pp){}
        public static void PackLost(PipelineProtocolsPack pp){}

        public static void PackArrived(PipelineProtocolsPack pp){
            //if there has no other pack with the same seqnum has arrived before
            if(!arrivedPack.Any() || arrivedPack.Any() && !arrivedPack.Contains(pp.seqnum)){
                arrivedPack.Add(pp.seqnum);
                DrawFillRight(pp.seqnum);
                baseOfWindowRight = findFirstNotYetArrivedPack();
                DrawWindowRight(baseOfWindowRight);
            }
            SendACKFor(pp.seqnum);
            layer.RemoveChild(pp);
        }

        /**********************************************************************
        *********************************************************************/
        public static void AckCorrupt(PipelineProtocolsACK aa) { }
        public static void AckLost(PipelineProtocolsACK aa) { }

        public static void AckArrived(PipelineProtocolsACK aa) {
            Debug.WriteLine("arrived");
            //if there has no other Ack with the same seqnum has arrived before
            if(!arrivedAck.Any() || arrivedAck.Any() && !arrivedAck.Contains(aa.seqnum)){
                arrivedAck.Add(aa.seqnum);
                pendingAck.Remove(aa.seqnum);
                DrawFillLeft2(aa.seqnum);
                baseOfWindowLeft = findFirstNotYetArrivedAck();
                DrawWindowLeft(baseOfWindowLeft);
                Debug.WriteLine("ACK arrived the first time " + aa.GetID());
            }
            layer.RemoveChild(aa);
        }

        /**********************************************************************
        *********************************************************************/
        public static int findFirstNotYetArrivedPack()
        {
            int x = baseOfWindowRight;
            while (arrivedPack.Any() && arrivedPack.Contains(x))
            {
                x++;
            }
            return x;
        }

        /**********************************************************************
        *********************************************************************/
        public static int findFirstNotYetArrivedAck()
        {
            int x = baseOfWindowLeft;
            while (arrivedAck.Any() && arrivedAck.Contains(x))
            {
                x++;
            } 
            return x;
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawFillRight(int seqnum)
        {
            float a = 28 - seqnum;
            float yPos = 15 + (a * 65);

			//draw the box on the right side
			var rightBox = new CCRect(320, yPos, 40, 50);
			CCDrawNode cc_rightBox = new CCDrawNode();
			cc_rightBox.DrawRect(
			rightBox,
                fillColor: CCColor4B.LightGray,
			borderWidth: 1,
			borderColor: CCColor4B.Gray);
			layer.AddChild(cc_rightBox);
        }

        /**********************************************************************
        *********************************************************************/
        static void DrawFillLeft(int seqnum)
        {
            
            float a = 28 - seqnum;
            float yPos = 15 + (a* 65);

			//draw the box on the left side
			var leftBox = new CCRect(40, yPos, 40, 50); //x,y,length, width
			CCDrawNode cc_leftBox = new CCDrawNode();
			cc_leftBox.DrawRect(
			leftBox,
                fillColor: CCColor4B.LightGray,
			borderWidth: 1,
			borderColor: CCColor4B.Gray);
			layer.AddChild(cc_leftBox);
        }

        /**********************************************************************
        *********************************************************************/
		static void DrawFillLeft2(int seqnum)
		{

			float a = 28 - seqnum;
            if ((int)a != 29) // 29 would be th very firs line which is marked by "--"
            {
				float yPos = 15 + (a * 65);

				//draw the box on the left side
				var leftBox = new CCRect(40, yPos, 40, 50); //x,y,length, width
				CCDrawNode cc_leftBox = new CCDrawNode();
				cc_leftBox.DrawRect(
				leftBox,
					fillColor: CCColor4B.Green,
				borderWidth: 1,
				borderColor: CCColor4B.Gray);
				layer.AddChild(cc_leftBox);
            }
	
		}

        /**********************************************************************
        *********************************************************************/
        //window on the left
        static void DrawWindowLeft(float pos)
        {
            //delete existing window
            if (cc_window != null)
            {
                cc_window.Clear();
            }
            float a = 29 - pos;
            float yMin = 7 + (a - windowSize) * 65; // start of the coordinate system is at the lower left. But We start at the upper left
            float height = windowSize * 65;
            window = new CCRect(35, yMin, 50, height);//CCRect(x,y,legth,heigth)
            cc_window = new CCDrawNode();
            cc_window.DrawRect(
                window,
                fillColor: CCColor4B.Transparent,
                borderWidth: 1,
                borderColor: CCColor4B.Blue);
            //add box to layer
            layer.AddChild(cc_window);
        }

        /**********************************************************************
        *********************************************************************/
        //window on the right 
        static void DrawWindowRight(float pos)
        {
            if(cc_window2 != null)
            {
                cc_window2.Clear();
            }

       
            float a = 29 - pos ;
            float yMin = 7 + (a - windowSize) * 65;
            float height = windowSize * 65;
            window2 = new CCRect(315, yMin, 50, height);
            cc_window2 = new CCDrawNode();
            cc_window2.DrawRect(
                window2,
				fillColor: CCColor4B.Transparent,
				borderWidth: 1,
                borderColor: CCColor4B.Blue);
            layer.AddChild(cc_window2);
        }

        /**********************************************************************
        *********************************************************************/
        //draw everything. Begginging from the Bottom
		void DrawLabelsAndBoxes()
		{

            ///////draw 29 lines of boxes and labels
            float yPos = 15;
            for (int i = 0; i < 29; i++)
            {
				//draw label on the left side
                var ccl_LeftNumber = new CCLabel((28-i).ToString(), "Arial", 20);
                ccl_LeftNumber.Position = new CCPoint(20, yPos+25);
                ccl_LeftNumber.Color = CCColor3B.Gray;
                layer.AddChild(ccl_LeftNumber);

                //draw the box on the left side
                var leftBox = new CCRect(40, yPos,40,50); //x,y,length, width
                CCDrawNode cc_leftBox = new CCDrawNode();
				cc_leftBox.DrawRect(
                leftBox,
				fillColor: CCColor4B.White,
				borderWidth: 1,
				borderColor: CCColor4B.Gray);
                layer.AddChild(cc_leftBox);

				//draw label on the right side
                var ccl_RightNumber = new CCLabel((28-i).ToString(), "Arial", 20);
				ccl_RightNumber.Position = new CCPoint(380, yPos + 25);
                ccl_RightNumber.Color = CCColor3B.Gray;
				layer.AddChild(ccl_RightNumber);

                //draw the box on the right side
                var rightBox = new CCRect(320, yPos,40,50);
				CCDrawNode cc_rightBox = new CCDrawNode();
				cc_rightBox.DrawRect(
				rightBox,
				fillColor: CCColor4B.White,
				borderWidth: 1,
				borderColor: CCColor4B.Gray);
				layer.AddChild(cc_rightBox);
                //set yPos 
                yPos += 65; //50 heigth + 15 distance

			}

			///////////draw labels on the top
			//draw label on the left side
			var ccl_LNumber = new CCLabel("--", "Arial", 20);
			ccl_LNumber.Position = new CCPoint(20, yPos + 25);
			ccl_LNumber.Color = CCColor3B.Gray;
			layer.AddChild(ccl_LNumber);

			//draw label on the right side
			var ccl_RNumber = new CCLabel("--", "Arial", 20);
			ccl_RNumber.Position = new CCPoint(380, yPos + 25);
			ccl_RNumber.Color = CCColor3B.Gray;
			layer.AddChild(ccl_RNumber);

		}
    }
}