﻿/**************************CLASS FOR GO BACK N*********************************/
using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Threading.Tasks;

//TAKE NOTE: the reference value of an action is always the same!! 
//It always refers to the values that were set when the original object was first definded.
//and not some altered value after some action that happended before the current one

namespace WertheApp.CN
{
    public class PipelineProtocolsScene2 : CCScene
    {
        //VARIABLES
        static CCLayer layer;

        public static bool stopEverything; //code is still running when page is not displayed anymore. Therefore there has to be a variable to stop everything
        public static bool animationIsPaused; //indicates i the pause button(PipelineProtocols.cs) was clicked

        static String strategy;
        static int windowSize;
        static int timeouttime;
        static bool timerRunning;
        static int counter;


        static int baseOfWindowLeft; //first sent but not yet acknowledged sequence number
        static int expectedSeqnum; //expected sequence number of packet at receiver
        public static int nextSeqnum; //first not sent yet sequence number

        static List<int> pendingAck;
        static List<int> arrivedPack; //static int lastRecentInOrderSeqnum; 
        static List<int> arrivedAck;

        static CCRect window; 
        static CCRect expSeqnum;
        static CCDrawNode cc_window; 
        static CCDrawNode cc_expSeqnum;

        //CONSTRUCTOR
        public PipelineProtocolsScene2(CCGameView gameView) : base(gameView)
        {
            stopEverything = false;
            animationIsPaused = false;

            //add a layer to draw on
            layer = new CCLayer();
            this.AddLayer(layer);

            windowSize = PipelineProtocols.windowSize;
            strategy = PipelineProtocols.strategy;
			timeouttime = PipelineProtocols.timeoutTime;
            timerRunning = false;


            pendingAck = new List<int>();
            arrivedPack = new List<int>();
            arrivedPack.Add(-1); //important for knowing which seqnum was the las recent one to be received in order
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
            expectedSeqnum = 0;
            nextSeqnum = 0;

            DrawWindow(baseOfWindowLeft);
            DrawExpectedSeqnum();
        }

		//METHODS

        /**********************************************************************
        *********************************************************************/
        public static void MyTimer(int seqnum, int c)
        {
            timerRunning = true;
            counter = c;

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
                if (!animationIsPaused){
                    counter++;
                }
                counterText = "" + counter;
                ccl_LNumber.Text = counterText;

                ///when timer runs out
                if (stopEverything || counter == timeouttime )
                {
                    layer.RemoveChild(ccl_LNumber);
                    timerRunning = false;

                    //resend packages for all pending ACK after timeout 
                    if (pendingAck.Any() && !stopEverything)
                    {
                        ResendPending();
                   }
                    return false; //False = Stop the timer
                }
                else { 
                    if(arrivedAck.Any() && arrivedAck.Contains(seqnum) && pendingAck.Any()){
                        timerRunning = false;
                        MyTimer(pendingAck.First(), 0);
                        return false;
                    }
                    return true; } // True = Repeat again
            });
        }
        /**********************************************************************
        *********************************************************************/
        public static void InvokeSender()
        {
            //if window is not full, that means if there are still packets left inside the window which are not sent yet
            if (nextSeqnum < (baseOfWindowLeft + windowSize))
            {
                SendPackageAt(nextSeqnum);
                if(timerRunning == false){
                    MyTimer(nextSeqnum, 0);
                }

                nextSeqnum++;
            }
        }

        public static async void ResendPending(){
            int i = 0;
            MyTimer(pendingAck.ElementAt(0), 0);
            while (i < pendingAck.Count){
                SendPackageAt(pendingAck.ElementAt(i));
                await Task.Delay(100); // necessary! Packages that arrive all at once are hard to process for the receiver
                i++;
            }
        }

        /**********************************************************************
        *********************************************************************/
        //this method imitates both sender and receiver of a packet. It is called by the method invoke 
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
        public static void SlowDownAck(PipelineProtocolsACK aa, int xPos)
        {
            //stop running actions
            aa.StopAllActions();

            //define actions
            float yPos = 15 + (65 * (28 - aa.seqnum));
            float timeToTake = 8f;
            var distance = new CCPoint(80, yPos);
            var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the Object to the CCPoint
            var removeAction = new CCRemoveSelf(); //this action removes the object*/

            //define sequence of actions and apply to object 
            var cc_seq1 = new CCSequence(sendPackageAction, removeAction);
            aa.RunAction(cc_seq1);
        }

        /**********************************************************************
        *********************************************************************/
        public static void PackCorrupt(PipelineProtocolsPack pp) {
            SendACKFor(arrivedPack.Last()); // send ACK for las recent in Order received Pack
        }
        public static void PackLost(PipelineProtocolsPack pp) { }

        public static void PackArrived(PipelineProtocolsPack pp)
        {
            //if it is the exoected seqnum and there has no other pack with the same seqnum has arrived before
            if (pp.seqnum == expectedSeqnum && (!arrivedPack.Any() || arrivedPack.Any() && !arrivedPack.Contains(pp.seqnum)))
            {
                arrivedPack.Add(pp.seqnum);
                DrawFillRight(pp.seqnum);
                expectedSeqnum = FindFirstNotYetArrivedPack();
                DrawExpectedSeqnum();
                SendACKFor(pp.seqnum);
            }else if(pp.seqnum <= expectedSeqnum){
                SendACKFor(pp.seqnum);
            }else if(pp.seqnum > expectedSeqnum){
                SendACKFor(arrivedPack.Last()); // send ACK for las recent in Order received Pack
            }
            layer.RemoveChild(pp);
        }

        /**********************************************************************
        *********************************************************************/
        public static void AckCorrupt(PipelineProtocolsACK aa) { }
        public static void AckLost(PipelineProtocolsACK aa) { }

        public static void AckArrived(PipelineProtocolsACK aa)
        {
            //if there has no other Ack with the same seqnum has arrived before
            if (!arrivedAck.Any() || arrivedAck.Any() && !arrivedAck.Contains(aa.seqnum))
            {
                //kummulatives ACK
                for (int i = 0 ; i <= aa.seqnum; i++){
                    if(pendingAck.Any() && pendingAck.Contains(i)){
                        arrivedAck.Add(i);
                        DrawFillLeft2(i);
                        pendingAck.Remove(i); 
                    }
                }

                baseOfWindowLeft = FindFirstNotYetArrivedAck();
                DrawWindow(baseOfWindowLeft);
            }
            layer.RemoveChild(aa);
        }

        /**********************************************************************
        *********************************************************************/
        public static int FindFirstNotYetArrivedPack()
        {
            int x = expectedSeqnum;
            while (arrivedPack.Any() && arrivedPack.Contains(x))
            {
                x++;
            }
            return x;
        }

        /**********************************************************************
        *********************************************************************/
        public static int FindFirstNotYetArrivedAck()
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
            float yPos = 15 + (a * 65);

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
        static void DrawExpectedSeqnum()
        {
            if (cc_expSeqnum != null)
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
        //draw everything. Begginging from the Bottom
        void DrawLabelsAndBoxes()
        {

            ///////draw 29 lines of boxes and labels
            float yPos = 15;
            for (int i = 0; i < 29; i++)
            {
                //draw label on the left side
                var ccl_LeftNumber = new CCLabel((28 - i).ToString(), "Arial", 20);
                ccl_LeftNumber.Position = new CCPoint(20, yPos + 25);
                ccl_LeftNumber.Color = CCColor3B.Gray;
                layer.AddChild(ccl_LeftNumber);

                //draw the box on the left side
                var leftBox = new CCRect(40, yPos, 40, 50); //x,y,length, width
                CCDrawNode cc_leftBox = new CCDrawNode();
                cc_leftBox.DrawRect(
                leftBox,
                fillColor: CCColor4B.White,
                borderWidth: 1,
                borderColor: CCColor4B.Gray);
                layer.AddChild(cc_leftBox);

                //draw label on the right side
                var ccl_RightNumber = new CCLabel((28 - i).ToString(), "Arial", 20);
                ccl_RightNumber.Position = new CCPoint(380, yPos + 25);
                ccl_RightNumber.Color = CCColor3B.Gray;
                layer.AddChild(ccl_RightNumber);

                //draw the box on the right side
                var rightBox = new CCRect(320, yPos, 40, 50);
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