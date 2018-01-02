﻿/************************CLASS FOR SELECTIVE REPEAT****************************/
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

        static List<int> pufferP; // list of packages that where received accurate after as lost or corrupt one
        static List<int> pufferACK; // list of ACK that where received accurate after a lost or corrupt one
        public static List<int> lostOrCorruptP; //list of currently lost or corrupt seqnum of a package
        public static List<int> lostOrCorruptACK; // list of currently lost or corrupt seqnum of an ACK

        static CCRect window;
        static CCRect window2;
        static CCDrawNode cc_window;
        static CCDrawNode cc_window2;

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
            baseOfWindow2 = 0;
            nextSeqnum = 0;

            pufferP = new List<int>();
            pufferACK = new List<int>();
            lostOrCorruptP = new List<int>();
            lostOrCorruptACK = new List<int>();

            DrawWindow(baseOfWindow);
            DrawWindow2(0);
        }

        //METHODS
        public static async void MyTimer(int seqnum, int c)
        {
            int counter = c;
            await Task.Delay(1000); //wait a second
			counter++;
            if(counter == 11){ //11seconds. otherwise resending the package looks unnatural
                if (lostOrCorruptP.Contains(seqnum) || lostOrCorruptACK.Contains(seqnum))//seqnumber lost or corrupt
				{
                    SendPackageAt(seqnum);//resend seqnum
                    MyTimer(seqnum, 0); 
                }
			}
            else{
                MyTimer(seqnum, counter);
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
                MyTimer(nextSeqnum, 0); 
										
                nextSeqnum++;
            }
            else
            {
                //refuse //do nothing or either disable button before or pop up window: try again later
            }
        }

        /**********************************************************************
        *********************************************************************/
        //this method imitates both sender and receiver of a packet. It is called by the method invoke 
        public static async void SendPackageAt(int seqnum)
        {
            DrawFillLeft(seqnum);

			//define object
			float yPos = 15 + (65 * (28 - seqnum)); //calculate where the box !starts! in the coordinate system
            var pp = new PipelineProtocolsPackage(seqnum);
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
            PipelineProtocols.l_Timeout.Text = "Timeout: restart";//everytime a new package is sent, the timer will be restarted
			await pp.RunActionAsync(cc_seq1); //await async: only after this is done. The following code will be visited!!!

            /******************************************************************/
            if(!pp.lost && !pp.corrupt)
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
            }
            pp.Dispose();
        }

        /**********************************************************************
        *********************************************************************/
        //Receiver sends ACK
        public static async void SendACKFor(int seqnum)
        {
            //define object
            float yPos = 15 + (65 * (28 - seqnum)); //where the box !starts!
            var pp = new PipelineProtocolsACK(seqnum);
            pp.Position = new CCPoint(280, yPos);
            layer.AddChild(pp);

            //define action
            float timeToTake = 5f;
            var distance = new CCPoint(80, yPos); //82 to 278 = 278-82 = 196
            var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
            var removeAction = new CCRemoveSelf(); //this action removes the object*/

            //define sequence of actions 
            var cc_seq1 = new CCSequence(sendPackageAction, removeAction);

            //apply sequence of actions to object
            await pp.RunActionAsync(cc_seq1); //await async: only after this is done. The following code will be visited!!!

            /******************************************************************/

            //ack arrived. in order is not necessary (but no cummulaive ackn)
            if(!pp.lost && !pp.corrupt)
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
            }
            pp.Dispose();
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