using System;
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
        CCSprite sprite;
        int id;
        public int seqnum;
        static int count = 0;
        int touchCount;
        public bool corrupt = false;
        public bool lost = false;
        public bool small = false; //indicates if the rectangle is smaller (for very first ACK at --)
        CCEventListenerTouchOneByOne touchListener;

        //CONSTRUCTOR 
        public PipelineProtocolsACK(int seqnum) : base()
        {
            this.sprite = new CCSprite();
            this.id = count;
            this.seqnum = seqnum;
            count++;
            touchCount = 0;
            this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
            CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite.png"), new CCRect(0, 0, 40, 50));//x and y pos in the sprite image and width and heigth of the sprite

            sprite.Color = CCColor3B.Green; //EXtra line of code for Android.....since I didn't find out how to access the png in Android. It crashed every single time
            this.sprite.SpriteFrame = whiteFrame;
            this.AddChild(sprite);

            touchListener = new CCEventListenerTouchOneByOne();
            touchListener.OnTouchBegan = OnTouchBegan;
            AddEventListener(touchListener, this);
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
        }


        /**********************************************************************
        *********************************************************************/
        private bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
        {
            //if ACK was clicked the first time (corrupt)
            if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
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
            //if ACK was clicked a second time (lost)
            else if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
                && touchCount > 0)
            {
                this.corrupt = false;
                this.lost = true;
                this.touchCount++;
                this.RemoveChild(this.sprite); //removes the visible! sprites. Actions are still running in the background

                //if seqnum not already in list
                if (PipelineProtocolsScene.lostOrCorruptACK != null && !PipelineProtocolsScene.lostOrCorruptACK.Contains(this.seqnum))
                {
                    PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); //add to list 
                    Debug.WriteLine("ack lost: " + PipelineProtocolsScene.lostOrCorruptACK.Last());
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
