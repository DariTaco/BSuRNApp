using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;

//TAKE NOTE: the reference value of an action is always the same!! 
//It always refers to the values that were set when the original object was first definded.
//and not some altered value after some action that happended before the current one

namespace WertheApp.RN
{
    public class PipelineProtocolsScene : CCScene
    {
		//VARIABLES
		static CCLayer layer;

		
		String strategy;

        static CCRect window;
        static CCDrawNode cc_window;

        static CCRect expSeqnum;
        static CCDrawNode cc_expSeqnum;

        static int windowSize;
        static int baseOfWindow; //first sent but not yet acknowledged sequence number
        public static int nextSeqnum; //first not sent yet sequence number
        static int expectedSeqnum; //expected sequence number of packet at receiver
        static int lastRecentInOrderSeqnum; // last recently received in ordner sequence number at receiver

        static int activeTimers;
        static int timerKey;
        static int timerKey2;


		//CONSTRUCTOR
		public PipelineProtocolsScene(CCGameView gameView) : base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);

            windowSize = PipelineProtocols.windowSize;
            strategy = PipelineProtocols.strategy;

            DrawLabelsAndBoxes();

            baseOfWindow = 0;
            nextSeqnum = 0;
            expectedSeqnum = 0;
            lastRecentInOrderSeqnum = -1;
            activeTimers = 0;
            timerKey = 0;
            timerKey2 = 0;

            DrawWindow(baseOfWindow);
            DrawExpectedSeqnum();

        }

		//METHODS


        private static bool TimerElapsed()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
                //resend all packets that have been previously send but not yet ack

                if (activeTimers == 1 && timerKey == timerKey2)
                {
                    for (int i = baseOfWindow; i < nextSeqnum; i++)
                    {
                        Debug.WriteLine("Packet resent with seqnum: " + i);
                        SendPackageAt(i);
                    }
                    //start timer // time = roundtrip time
                    timerKey2 = timerKey;
                    Device.StartTimer(new TimeSpan(0, 0, 0, 10, 0), TimerElapsed); //days, hours , minutes, seconds, milioseconds
                }
                else { activeTimers--; }

               
			});
            //return true to keep timer reccuring
            //return false to stop timer
            return false;
		}


        public static void InvokeSender()
        {
            //if window is not full, that means if there are still packets left inside the window which are not sent yet
            if(nextSeqnum < (baseOfWindow+windowSize))
            {
                SendPackageAt(nextSeqnum);

/*TODO*/                
                if(baseOfWindow == nextSeqnum)
                {
                    //start timer // time = roundtrip time
                    timerKey2 = timerKey;
                    Device.StartTimer(new TimeSpan( 0, 0, 0, 10, 0 ), TimerElapsed); //days, hours , minutes, seconds, milioseconds
                    activeTimers++;

				}
                nextSeqnum++;
            }
            else
            {
                //refuse //either disable button before or pop up window: try again later
            }
        }

        //Receiver sends ACK
        public static async void SendACKFor(int seqnum)
        {
			//seqnum
			//define object
			float yPos = 15 + (65 * (28 - seqnum)); //where the box !starts!
			var pp = new PipelineProtocolsPackage(seqnum, true);
			pp.Position = new CCPoint(280, yPos);
			layer.AddChild(pp);

			//define action
			float timeToTake = 5f;
			var distance = new CCPoint(80, yPos); //82 to 278 = 278-82 = 196
			var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
			var removeAction = new CCRemoveSelf(); //this action removes the object*/

			//define sequence of actions 
			var cc_seq1 = new CCSequence(sendPackageAction, removeAction);
            Debug.WriteLine("ACK sent for seqnum: " + pp.seqnum);

			//apply sequence of actions to object
			await pp.RunActionAsync(cc_seq1); //await async: only after this is done. The following code will be visited!!!

			//Debug.WriteLine("ASYnc Action done");

			//Code for sending ACK
			//if ACK was lost
			if (pp.lost)
			{
				Debug.WriteLine("ACK WAS LOST");
				pp.Dispose();
				//do nothing
			}
			//if ACK was not lost or corrupted //in order is not necessary
			else if (!pp.corrupt)
			{
                DrawFillLeft2(pp.seqnum);
                if(pp.seqnum != -1){PipelineProtocols.l_LastRecentAcknowlegement.Text = "Last recent acknowlegment: " + pp.seqnum; }
                Debug.WriteLine("Ack for seqn"+pp.seqnum);
                baseOfWindow = pp.seqnum + 1;
                DrawWindow(baseOfWindow);
				pp.Dispose();

/*TODO*/                if(baseOfWindow == nextSeqnum)
                {
                    //stop timer
                    timerKey++;

                }
                else
                {
                    //start_timer
                    //start timer // time = roundtrip time
                    timerKey2 = timerKey;
					Device.StartTimer(new TimeSpan(0, 0, 0, 10, 0), TimerElapsed); //days, hours , minutes, seconds, milioseconds
                    activeTimers++;
				}
			}//send ACK for last recently received in order sequence number
			else
			{
                //do nothing
				Debug.WriteLine("ACK CORRUPT");
				pp.Dispose();
			}
        }

        //this method imitates both sender and receiver of a packet. It is called by the method invoke 
        public static async  void SendPackageAt(int seqnum)
        {

            /*//define object
            float yPos = 15 + (65 * (28-a)); //where the box !starts!
			var startBox = new CCRect(82, yPos, 40, 50); //x,y,length, width 82
			var cc_startBox = new CCDrawNode();
			cc_startBox.DrawRect(
			startBox,
              fillColor: CCColor4B.White,
			borderWidth: 1,
				borderColor: CCColor4B.Red);
			//layer.AddChild(cc_startBox); */

            /*//add touch listener
			var touchListener = new CCEventListenerTouchAllAtOnce();
			touchListener.OnTouchesBegan = HandleInput; */

            /*//define action for DrawRect
			var distance = new CCPoint(196, 0); //82 to 278 = 278-82 = 196
			var distance2 = new CCPoint(0, 0); //0 as an x-value migth seem strange, but the reference value is the x-value of the object when it was first defined!
			float timeToTake = 5f;
			var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
			var sendAckAction = new CCMoveTo(timeToTake, distance2); //this action moves the object back to where it originally was
			var removeAction = new CCRemoveSelf(); //this action removes the object*/

            /*//apply action to object
			cc_startBox.AddAction(sendingAction);*/

            //apply sequence of actions to object
            //cc_startBox.RunAction(cc_seq1);

            DrawFillLeft(seqnum);

			//define object
			float yPos = 15 + (65 * (28 - seqnum)); //calculate where the box !starts! in the coordinate system
            var pp = new PipelineProtocolsPackage(seqnum, false);
            pp.Position = new CCPoint(80,yPos);
            layer.AddChild(pp);

			//define action
            float timeToTake = 5f;
            var distance = new CCPoint(280, yPos); //82 to 278 = 278-82 = 196
			var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
			var removeAction = new CCRemoveSelf(); //this action removes the object*/

			//define sequence of actions 
			var cc_seq1 = new CCSequence(sendPackageAction, removeAction);

            //apply sequence of actions to object
            await pp.RunActionAsync(cc_seq1); //await async: only after this is done. The following code will be visited!!!
            //Debug.WriteLine("ASYnc Action done");

            //Code for receiving packte and maybe sending ACK
            //if packet was lost
            if(pp.lost)
            {
                Debug.WriteLine("PACKAGE WAS LOST");
                pp.Dispose();
                //do nothing
            }
			//if packet was not lost or corrupted and packet was received in order(expected sequence number) 
            else if(!pp.corrupt && pp.seqnum == expectedSeqnum)
			{ 
                Debug.WriteLine("PACKAGE"+ pp.seqnum+" RECEIVED IN ORDER, NOT CORRUPT, NOT LOST");
                expectedSeqnum++; //increase expectedSeqnum
                DrawExpectedSeqnum();
                DrawFillRight(pp.seqnum);
                lastRecentInOrderSeqnum = pp.seqnum;
                PipelineProtocols.l_LastRecentInOrderAtReceiver.Text = "Last recent in-order received packet: " + pp.seqnum;
                SendACKFor(pp.seqnum); //send ACK 
                pp.Dispose();
            }//send ACK for last recently received in order sequence number
            else
            {
                Debug.WriteLine("PACKAGE CORRUPT");
                SendACKFor(lastRecentInOrderSeqnum);
                pp.Dispose();
            }
        }

        private static void HandleInput(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
            touchEvent.CurrentTarget.Color = CCColor3B.Blue;
            touchEvent.CurrentTarget.UpdateColor();
            touchEvent.CurrentTarget.Update(0);
           
            //touchEvent.CurrentTarget.Dispose();
			if (touches.Count > 0)
			{
                
                CCTouch firstTouch = touches[0];
                Debug.WriteLine("first touch" + touches.Count());
                //cc_startBox.PositionX = firstTouch.Location.X;
				//cc_startBox.PositionY = firstTouch.Location.Y;
			}
            else{Debug.WriteLine("clicked " + touches.Count()); }

            //cc_startBox.Color = CCColor3B.Magenta;

            //col1 = CCColor4B.Magenta;
		}

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

		static void DrawFillLeft2(int seqnum)
		{

			float a = 28 - seqnum;
            if(a != 29) // 29 would be th very firs line which is marked by "--"
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

        static void DrawExpectedSeqnum()
        {
            if(cc_expSeqnum != null)
            {
                cc_expSeqnum.Clear();
            }

            float a = 29 - expectedSeqnum;
            float yMin = 7 + (a - 1) * 65;
            expSeqnum = new CCRect(315, yMin, 50, 65);
            cc_expSeqnum = new CCDrawNode();
            cc_expSeqnum.DrawRect(
                expSeqnum,
				fillColor: CCColor4B.Transparent,
				borderWidth: 1,
                borderColor: CCColor4B.Blue);
            layer.AddChild(cc_expSeqnum);
        }

       static void DrawWindow(float pos)
        {
            //delete existing window
            if(cc_window != null)
            {
                cc_window.Clear();
            }
            float a = 29 - pos;
            float yMin = 7 + (a - windowSize)*65; // start of the coordinate system is at the lower left. But We start at the upper left
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

                /*var label4 = new CCLabel(i.ToString(), "Arial", 20)
				{
					Position = new CCPoint(360, yPos),//layer.VisibleBoundsWorldspace.Center,
					Color = CCColor3B.Black,
					IsAntialiased = true,
					HorizontalAlignment = CCTextAlignment.Center,
					VerticalAlignment = CCVerticalTextAlignment.Center,
					IgnoreAnchorPointForPosition = true
				};
				layer.AddChild(label4);*/

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
