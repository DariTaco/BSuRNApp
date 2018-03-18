﻿using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Xamarin.Forms;
namespace WertheApp.RN
{
    //CLASS FOR SPRITE OBJECT
    public class PipelineProtocolsACK : CCNode
    {
        //VARIABLES
        public static bool stopEverything; //code is still running when page is not displayed anymore. Therefore there has to be a variable to stop everything
        CCSprite sprite;
        int id;
        public int seqnum;
        static int count = 0;
        int touchCount;
        public bool corrupt = false;
        public bool lost = false;
        public bool small = false; //indicates if the rectangle is smaller (for very first ACK at --)
        public bool ignore = false;
        CCEventListenerTouchOneByOne touchListener;

        //CONSTRUCTOR 
        public PipelineProtocolsACK(int seqnum) : base()
        {
            stopEverything = false;
            this.sprite = new CCSprite();
            this.id = count;
            this.seqnum = seqnum;
            count++;
            touchCount = 0;
            this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
            CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite"), new CCRect(0, 0, 40, 50));//x and y pos in the sprite image and width and heigth of the sprite
	

            sprite.Color = CCColor3B.Green; //EXtra line of code for Android.....since I didn't find out how to access the png in Android. It crashed every single time
            this.sprite.SpriteFrame = whiteFrame;
            this.AddChild(sprite);

			touchListener = new CCEventListenerTouchOneByOne();
            touchListener.OnTouchBegan = OnTouchBegan;
            AddEventListener(touchListener, this);

			Schedule(Process); // prediefined method that takes an Action and schedules it to run for every cycle
		}

        //CONSTRUCTOR 2
        public PipelineProtocolsACK(int seqnum, int small) : base()
        {
            this.small = true;
            this.sprite = new CCSprite();
            this.id = count;
            this.seqnum = seqnum;
            count++;
            touchCount = 0;
            this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
            CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite.png"), new CCRect(0, 0, 20, 25));//x and y pos in the sprite image and width and heigth of the sprite

            sprite.Color = CCColor3B.Green; //EXtra line of code for Android.....since I didn't find out how to access the png in Android. It crashed every single time
            this.sprite.SpriteFrame = whiteFrame;
            this.AddChild(sprite);

            touchListener = new CCEventListenerTouchOneByOne();
            touchListener.OnTouchBegan = OnTouchBegan;
            AddEventListener(touchListener, this);
			Schedule(Process); // prediefined method that takes an Action and schedules it to run for every cycle
		}

		/**********************************************************************
        *********************************************************************/
        /*TODO add to entsprechender List, Objekt nur zerstören wenn arrive, nicht bei timeout, timeout in der anderen Klasse, aufschrieb im Ordner*/
        private void Process(float seconds){
            if (stopEverything) { this.Dispose(); }
            //if ACK arrives (MinX + 81 = position of the rectangles on the left)
            if(this.PositionX <= VisibleBoundsWorldspace.MinX+81)
            {
                Debug.WriteLine(this.seqnum + " HIT SOMETHING");
                if(this.corrupt)
                {
                    Debug.WriteLine("corrupt");
                }
                //arrived without corruption and didn't get lost on the way
                else{
                    Debug.WriteLine("all good");
                    Debug.WriteLine("jhkjddgghjk");
                }
                this.RemoveChild(this.sprite);
            }
        }

		/**********************************************************************
        ***************************************************hghjgkh******************/
		private bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
        {
            if(this.small){
                //do nothing
                return true;
            }
            //if ACK was clicked the first time (corrupt)
            else if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
                && touchCount == 0) //50 because it's the height of the packages spriteframe
            {
                UpdateMyColor();
                this.touchCount++;
                this.corrupt = true;

                //if seqnum not already in list
                if (PipelineProtocolsScene.lostOrCorruptACK != null && !PipelineProtocolsScene.lostOrCorruptACK.Contains(this.seqnum))
                {
                    PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); //add to list 
                    Debug.WriteLine("ack corrupt: " + PipelineProtocolsScene.lostOrCorruptACK.Last());
                }

                return true;
            }
            //if ACK was clicked a second time (slowed down)
            else if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
                && touchCount == 1)
            {
                this.corrupt = false;
                this.lost = false;

                //if seqnum already in list
                if (PipelineProtocolsScene.lostOrCorruptACK != null && PipelineProtocolsScene.lostOrCorruptACK.Contains(this.seqnum))
                {
                    PipelineProtocolsScene.lostOrCorruptACK.Remove(this.seqnum); 
                }
                this.ignore = true;
                this.touchCount = 5;
                this.RemoveChild(this.sprite); //removes the visible! sprites. Actions are still running in the background
                Debug.WriteLine(touch.Location.X);
                PipelineProtocolsScene.SendSlowPackageAt(this.seqnum, ((int)touch.Location.X - 20));
                //this.Dispose();// don't know why but it makes everything stop when you click somehwere on the screen, so I commented it out
               
                return false;
            }
            //if package was clicked a third time (slowed down & corrupt)
            else if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
                && touchCount == 2)
            {
                UpdateMyColor();
                this.touchCount++;
                this.corrupt = true;

                //if seqnum not already in list
                if (PipelineProtocolsScene.lostOrCorruptACK != null && !PipelineProtocolsScene.lostOrCorruptACK.Contains(this.seqnum))
                {
                    PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); 
                    Debug.WriteLine("ACK corrupt: " + PipelineProtocolsScene.lostOrCorruptACK.Last());
                }

                return true;
            }

            //if package was clicked a fourth time (lost)
            else if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
                && touchCount == 3)
            {
                this.corrupt = false;
                this.lost = true;
                this.touchCount++;
                this.RemoveChild(this.sprite); //removes the visible! sprites. Actions are still running in the background

                //if seqnum not already in list
                if (PipelineProtocolsScene.lostOrCorruptACK != null && !PipelineProtocolsScene.lostOrCorruptACK.Contains(this.seqnum))
                {
                    PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); 
                    Debug.WriteLine("ACK lost: " + PipelineProtocolsScene.lostOrCorruptACK.Last());
                }

                return false;
            }
            else { return false; }
        }

        /**********************************************************************
        *********************************************************************/
        public void UpdateMyColor()
        {
            if (this.small)
            {
                CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite"), new CCRect(0, 0, 20, 25));
                this.sprite.SpriteFrame = whiteFrame;
                this.sprite.Color = CCColor3B.Red; //workaround for Android. but also changes the base color of the sprite
            }
            else
            {
                CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite"), new CCRect(0, 0, 40, 50));
                this.sprite.SpriteFrame = whiteFrame;
                this.sprite.Color = CCColor3B.Red; //workaround for Android. but also changes the base color of the sprite}

            }
        }

        /**********************************************************************
        *********************************************************************/
        public CCSprite GetSpriteByID(int id)
        {
            return this.sprite;
        }

        /***************************************************************
        *********************************************************************/
        public int GetID()
        {
            return this.id;
        }
    }
}