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

namespace WertheApp.RN
{
    public class PipelineProtocolsScene : CCScene
    {
		//VARIABLES
		static CCLayer layer;

		static String strategy;
        static int windowSize;

        static int baseOfWindow; //first sent but not yet acknowledged sequence number
        static int baseOfWindow2;
        public static int nextSeqnum; //first not sent yet sequence number

        static int tmr;
        static bool stopTmr;

        static List<int> pufferP; // list of packages that where received accurate after as lost or corrupt one
        static List<int> pufferACK; // list of ACK that where received accurate after a lost or corrupt one
        static List<int> lostOrCorruptP; //list of currently lost or corrupt seqnum of a package
        static List<int> lostOrCorruptACK; // list of currently lost or corrupt seqnum of an ACK

        static CCRect window;
        static CCRect window2;
        static CCDrawNode cc_window;
        static CCDrawNode cc_window2;

        //CONSTRUCTOR
        public PipelineProtocolsScene(CCGameView gameView) : base(gameView)
        {
            Debug.WriteLine("SELECTIVE REPEAT");
            //add a layer to draw on
            layer = new CCLayer();
            this.AddLayer(layer);

            windowSize = PipelineProtocols.windowSize;
            strategy = PipelineProtocols.strategy;

            DrawLabelsAndBoxes();

            baseOfWindow = 0;
            baseOfWindow2 = 0;
            nextSeqnum = 0;

            tmr = 0;
            stopTmr = true;

            pufferP = new List<int>();
            pufferACK = new List<int>();
            lostOrCorruptP = new List<int>();
            lostOrCorruptACK = new List<int>();

            DrawWindow(baseOfWindow);
            DrawWindow2(0);

        }

		//METHODS

        /**********************************************************************
         *********************************************************************/
        //!!!!if you a better solution for a timer. please rewrite this section of code. Or maybe Xamarin will have an implementation of a useful time at some point
        //I did this to simulate a timer. Didn't know how to do it. Since the built in timer of Xamarin can't be stopped, I had to programm my own.
        //created an invisible object and applied an action of 1 second to it.
        //As long as the tmr variable is <10 the action is repeated.
        //as soon as the tmr variable equals 10, the timer is stopped and packages will be resent and the timer restarted
        public static async void MyTimer()
        {
			if(stopTmr)
            {
                //stop timer
                PipelineProtocols.l_Timeout.Text = "Timeout: --";
            }
            else if(tmr == 11) 
            {
                stopTmr = true;
                int i = baseOfWindow; //packet to resend TODO RESEND
				//timer elapsed -> resend only the packet itself
                Debug.WriteLine("RESENT P: " + i);
                await InvokeSender2(i);
     
                //restart timer
                tmr = 0;
                stopTmr = false;
                MyTimer();
            }
            else if (tmr < 11)
            {
                PipelineProtocols.l_Timeout.Text = "Timeout: " + tmr;

                var pointlessPoint = new CCRect(10, 10, 10, 10);//arbitrary
                var cc_pointlessPoint = new CCDrawNode();
                cc_pointlessPoint.DrawRect(
                pointlessPoint,
                fillColor: CCColor4B.Transparent,
                borderWidth: 1,
                borderColor: CCColor4B.Transparent);
                layer.AddChild(cc_pointlessPoint); //DO NOT ADD the defined object to the layer. We only need it for simulating the timer

                //define action
                float timeToTake = 1f; //1 second!!
                var distance = new CCPoint(100, 100); //arbitrary
                var wasteASecondAction = new CCMoveTo(timeToTake, distance);
                var removeAction = new CCRemoveSelf(); //this action removes the object*/

                //define sequence of actions 
                var cc_seq1 = new CCSequence(wasteASecondAction, removeAction);

                //apply sequence of actions to object
                await cc_pointlessPoint.RunActionAsync(cc_seq1); //await async: only after this is done. The following code will be visited!!

                tmr++;
                MyTimer();
            }
            
        }

        /**********************************************************************
        *********************************************************************/
        public static void InvokeSender()
        {
            //if window is not full, that means if there are still packets left inside the window which are not sent yet
            if(nextSeqnum < (baseOfWindow+windowSize))
            {
                SendPackageAt(nextSeqnum);
                //await InvokeSender2(nextSeqnum);

                if(baseOfWindow == nextSeqnum)
                {
                    /*TODO TIMER*/
                    //start timer 
                    if (stopTmr)
                    {
                        stopTmr = false;
                        tmr = 0;
                        MyTimer();
                    }//reset timer
                    else { tmr = 0; }
				}
                nextSeqnum++;
            }
            else
            {
                //refuse //do nothing or either disable button before or pop up window: try again later
            }
        }

        /**********************************************************************
        *********************************************************************/
        //is needed because turning sendPAckageAt into async is unpractical. The return had to be before actually sending, which is not possible
        public static async Task<int> InvokeSender2(int a)
        {
            await Task.Delay(10); //this delay (10 milliseconds) makes sure one package arrives after another
            SendPackageAt(a);
            return 0;
        }


        /**********************************************************************
        *********************************************************************/
        //this method imitates both sender and receiver of a packet. It is called by the method invoke 
        public static async void SendPackageAt(int seqnum)
        {
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
            tmr = 0;
            PipelineProtocols.l_Timeout.Text = "Timeout: restart";//everytime a new package is sent, the timer will be restarted
            Debug.WriteLine("SEND PACKAGE: " + pp.seqnum);
            await pp.RunActionAsync(cc_seq1); //await async: only after this is done. The following code will be visited!!!

            /******************************************************************/

            //packet was lost
            if (pp.lost)
            {
                //if seqnum not already in list
                if (!lostOrCorruptP.Contains(pp.seqnum))
                {

                    lostOrCorruptP.Add(pp.seqnum); //add to list 
                    lostOrCorruptACK.Add(pp.seqnum); //the ACK will also never arrive 
                    Debug.WriteLine("package lost: " + lostOrCorruptP.Last());
                }

                pp.Dispose();
            }
            //package corrupt
            else if (pp.corrupt)
            {
                //if seqnum not already in list
                if (!lostOrCorruptP.Contains(pp.seqnum))
                {
                    lostOrCorruptP.Add(pp.seqnum); //add to list 
                    lostOrCorruptACK.Add(pp.seqnum); //the ACK will also never arrive 
                    Debug.WriteLine("package corrupt: " + lostOrCorruptP.Last());
                }

                pp.Dispose();
            }
            //package arrived
            else
            {
                //has been lost/corrupt and is therfore in the list
                if (lostOrCorruptP.Contains(pp.seqnum))
                {

                    lostOrCorruptP.Remove(pp.seqnum); //remove from list

                    //still other ack lost/corrupt
                    if (lostOrCorruptP.Any())
                    {
                        //seqnum is bigger than the first item in list. note the item itself was removed
                        if (pp.seqnum > lostOrCorruptP.First())
                        {
                            pufferP.Add(pp.seqnum);
                            baseOfWindow2 = baseOfWindow2; //just to make it clear
                        }
                        else
                        {
                            baseOfWindow2 = lostOrCorruptP.First();
                        }
                    }
                    else
                    {
                        baseOfWindow2 = pufferP.Last() + 1;
                    }
                }
                //has never been lost/corrupt before . this implies that there are no lost/corrupt packages after this one and also no other packages have arrived after this one
                else
                {
                    //lost/corrupt packages before this one. since the implication above it is sufficient to only ask if there are any lost/corrupt packages existant
                    if (lostOrCorruptP.Any())
                    {
                        pufferP.Add(pp.seqnum);
                        baseOfWindow2 = baseOfWindow2; // just to make it clear
                    }
                    else
                    {
                        baseOfWindow2 = pp.seqnum + 1;
                    }

                }

                DrawWindow2(baseOfWindow2);
                DrawFillRight(pp.seqnum);
                SendACKFor(pp.seqnum); //send ACK 
                pp.Dispose();
            }

        }

        /**********************************************************************
        *********************************************************************/
        //Receiver sends ACK
        public static async void SendACKFor(int seqnum)
        {
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
            Debug.WriteLine("SEND ACK: " + pp.seqnum);

            //apply sequence of actions to object
            await pp.RunActionAsync(cc_seq1); //await async: only after this is done. The following code will be visited!!!

            /******************************************************************/

            //ack was lost
            if (pp.lost)
            {
                //if seqnum not already in list
                if (!lostOrCorruptACK.Contains(pp.seqnum))
                {
                    lostOrCorruptACK.Add(pp.seqnum); //add to list 
                    Debug.WriteLine("ack lost: " + lostOrCorruptACK.Last());
                }

                pp.Dispose();
            }
            //ack corrupt
            else if (pp.corrupt)
            {
                //do nothing
                //if seqnum not already in list
                if (!lostOrCorruptACK.Contains(pp.seqnum))
                {
                    lostOrCorruptACK.Add(pp.seqnum); //add to list 
                    Debug.WriteLine("ack corrupt: " + lostOrCorruptACK.Last());
                }

                pp.Dispose();
            }
            //ack arrived. in order is not necessary (but no cummulaive ackn)
            else
            {
                //has been lost/corrupt and is therfore in the list
                if (lostOrCorruptACK.Contains(pp.seqnum))
                {

                    lostOrCorruptACK.Remove(pp.seqnum); //remove from list

                    //still other ack lost/corrupt
                    if (lostOrCorruptACK.Any())
                    {
                        //seqnum is bigger than the first item in list. note the item itself was removed
                        if (pp.seqnum > lostOrCorruptACK.First())
                        {
                            pufferACK.Add(pp.seqnum);
                            baseOfWindow = baseOfWindow; //just to make it clear
                        }
                        else
                        {
                            baseOfWindow = lostOrCorruptACK.First();
                        }
                    }
                    else
                    {
                        baseOfWindow = pufferACK.Last() + 1;
                    }
                }
                //has never been lost/corrupt before . this implies that there are no lost/corrupt ack after this one and also no other ack have arrived after this one
                else
                {
                    //lost/corrupt ack before this one. since the implication above it is sufficient to only ask if there are any lost/corrupt ack existant
                    if (lostOrCorruptACK.Any())
                    {
                        pufferACK.Add(pp.seqnum);
                        baseOfWindow = baseOfWindow; // just to make it clear
                    }
                    else
                    {
                        baseOfWindow = pp.seqnum + 1;
                    }

                }
                DrawWindow(baseOfWindow);
                DrawFillLeft2(pp.seqnum);  //only draw current fill
                pp.Dispose();

                /**************************************************************/
                //*TODO TIMER*/
                if (baseOfWindow == nextSeqnum)
                {
                    //stop timer
                    stopTmr = true;

                }
                else
                {
                    //start timer 
                    if (stopTmr)
                    {
                        stopTmr = false;
                        tmr = 0;
                        MyTimer();
                    }//reset timer
                    else { tmr = 0; }
                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        //what happens when a package is clicked ? first touch -> corrupt, second touch -> lost
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
            if (a != 29) // 29 would be th very firs line which is marked by "--"
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
        static void DrawWindow(float pos)
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
        static void DrawWindow2(float pos)
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

/*//if it's the last ack in the lost/corrupt list and therfore there are no further lost/corrupt ack after this one
if(lostOrCorruptACK.Find(item => item.ToString() == pp.seqnum.ToString()) == lostOrCorruptACK.Last()){
    Debug.WriteLine("there are NO futher lost or corrupt ack after: " + lostOrCorruptACK.Find(item => item.ToString() == pp.seqnum.ToString()));
    pufferACK.Add(                                                                                                                                                                                                            );
    baseOfWindow = pufferACK.Last()+1;
    pufferACK.Remove(pufferACK.Last()); //remove the arrived package from list

}
//if it's not the last one in the list
else{
    Debug.WriteLine("there ARE further lost or corrupt ack after: " + lostOrCorruptACK.Find(item => item.ToString() == pp.seqnum.ToString()));
    lostOrCorruptACK.Remove(lostOrCorruptACK.Find(item => item.ToString() == pp.seqnum.ToString())); // remove the arrived package from list
    baseOfWindow = lostOrCorruptACK.First();

}*/