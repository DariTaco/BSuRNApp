/************************CLASS FOR SELECTIVE REPEAT****************************/
using CocosSharp;
using System.Linq;
using System.Diagnostics;
namespace WertheApp.RN
{
    //CLASS FOR SPRITE OBJECT
    public class PipelineProtocolsACK : CCNode
    {
        //VARIABLES
        public static bool stopEverything; //code is still running when page is not displayed anymore. Therefore there has to be a variable to stop everything
        static int count = 0;
        int id;
        public int seqnum;
        int touchCount;
        public bool corrupt;
        public bool lost;
        public bool small; //indicates if the rectangle is smaller (for very first ACK at --)
        public bool ignore;
        CCSprite sprite;
        CCEventListenerTouchOneByOne touchListener;

        //CONSTRUCTOR 
        public PipelineProtocolsACK(int seqnum, int tc) : base()
        {
            stopEverything = false;
            corrupt = false;
            lost = false;
            ignore = false;
            this.id = count;
            count++;
            this.seqnum = seqnum;
            touchCount = tc;

            this.sprite = new CCSprite();
            this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
            this.sprite.Color = CCColor3B.Green; //EXtra line of code for Android.....since I didn't find out how to access the png in Android. It crashed every single time
            CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite"), new CCRect(0, 0, 40, 50));//x and y pos in the sprite image and width and heigth of the sprite
            this.sprite.SpriteFrame = whiteFrame;
            this.AddChild(sprite);

			touchListener = new CCEventListenerTouchOneByOne();
            touchListener.OnTouchBegan = OnTouchBegan;
            AddEventListener(touchListener, this);

			Schedule(Process); // prediefined method that takes an Action and schedules it to run for every cycle
		}

        //CONSTRUCTOR 2
        public PipelineProtocolsACK(int seqnum, int tc, int small) : base()
        {
            this.small = true;
            corrupt = false;
            lost = false;
            ignore = true;
            stopEverything = false;
            this.id = count;
            count++;
            this.seqnum = seqnum;
            touchCount = 5; //unable to react to touch

            this.sprite = new CCSprite();
            this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
            sprite.Color = CCColor3B.Green; //EXtra line of code for Android.....since I didn't find out how to access the png in Android. It crashed every single time
            CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite.png"), new CCRect(0, 0, 20, 25));//x and y pos in the sprite image and width and heigth of the sprite
            this.sprite.SpriteFrame = whiteFrame;
            this.AddChild(sprite);

            touchListener = new CCEventListenerTouchOneByOne();
            touchListener.OnTouchBegan = OnTouchBegan;
            AddEventListener(touchListener, this);

			Schedule(Process); // prediefined method that takes an Action and schedules it to run for every cycle
		}

		/**********************************************************************
        *********************************************************************/
        private void Process(float seconds){
            if (stopEverything) { this.Dispose(); return; }

            //if ACK arrives (MinX + 81 = position of the rectangles on the left)
            if(this.PositionX <= VisibleBoundsWorldspace.MinX + 81)
            {
                if(this.ignore){
                }
                else if(this.corrupt)
                {
                    //PipelineProtocolsScene.AckCorrupt(this);
                    //PipelineProtocolsScene2.AckCorrupt(this);
                }
                else if(this.lost){
                    //PipelineProtocolsScene.AckLost(this);
                    //PipelineProtocolsScene2.AckLost(this);
                }
                //arrived without corruption and didn't get lost on the way
                else{
                    if (PipelineProtocols.strategy == "Selective Repeat")
                    {
                        PipelineProtocolsScene.AckArrived(this);
                    }else{
                        PipelineProtocolsScene2.AckArrived(this);  
                    }
                }
                this.RemoveChild(this.sprite);
            }
        }

		/**********************************************************************
        ***************************************************hghjgkh******************/
		private bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
        {
            if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
               )
            {
                switch (touchCount)
                {
                    case 0: Corrupt(); break;
                    case 1: SlowDown();
                        if (PipelineProtocols.strategy == "Selective Repeat")
                        {
                            PipelineProtocolsScene.SlowDownAck(this, ((int)touch.Location.X - 20));
                        }else{
                            PipelineProtocolsScene2.SlowDownAck(this, ((int)touch.Location.X - 20));
                        }
                            break;
                    case 2: SlowCorrupt(); break;
                    case 3: Lost(); break;
                    default: break;
                }
            }
            return true;
        }

        /**********************************************************************
        *********************************************************************/
        //if package was clicked the first time (corrupt)
        void Corrupt()
        {
            this.touchCount++;
            this.corrupt = true;
            this.lost = false;
            this.ignore = false;
            this.sprite.Color = CCColor3B.Red;
        }

        /**********************************************************************
        *********************************************************************/
        //if package was clicked a second time (slowed down)
        void SlowDown()
        {
            this.touchCount++;
            this.corrupt = false;
            this.lost = false;
            this.ignore = false;
            this.sprite.Color = CCColor3B.Green;
  }

        /**********************************************************************
        *********************************************************************/
        //if package was clicked a third time (slowed down & corrupt)
        void SlowCorrupt()
        {
            this.touchCount++;
            this.corrupt = true;
            this.lost = false;
            this.ignore = false;
            this.sprite.Color = CCColor3B.Red;
        }

        /**********************************************************************
        *********************************************************************/
        //if package was clicked a fourth time (lost)
        void Lost()
        {
            this.touchCount++;
            this.corrupt = false;
            this.lost = true;
            this.ignore = false;
            this.RemoveChild(this.sprite); //removes the visible! sprites. Actions are still running in the background
        }

        /***************************************************************
        *********************************************************************/
        public int GetID()
        {
            return this.id;
        }
    }
}