/************************CLASS FOR SELECTIVE REPEAT****************************/
using CocosSharp;
using System.Linq;
using System.Diagnostics;
namespace WertheApp.RN
{
	//CLASS FOR SPRITE OBJECT
	public class PipelineProtocolsPack : CCNode
	{
        //VARIABLES
        public static bool stopEverything; //code is still running when page is not displayed anymore. Therefore there has to be a variable to stop everything
        static int count = 0;
        int id;
        public int seqnum;
		int touchCount;
		public bool corrupt;
		public bool lost;
        public bool ignore;
        CCSprite sprite;
		CCEventListenerTouchOneByOne touchListener;

		//CONSTRUCTOR
		public PipelineProtocolsPack(int seqnum, int tc) : base()
		{
            stopEverything = false;
            corrupt = false;
            lost = false;
            ignore = false;
			this.id = count;
			count++;
            this.seqnum = seqnum;
            this.touchCount = tc;
		
            this.sprite = new CCSprite();
            this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
            this.sprite.Color = CCColor3B.Gray;//Extra line of code for Android.....since I didn't find out how to access the png in Android. It crashed every single time
			CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite"), new CCRect(0, 0, 40, 50));//x and y pos in the sprite image and width and heigth of the sprite
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
        private void Process(float seconds)
        {
            //stop code from running in the background
            if (stopEverything) { this.Dispose(); return; }

            //if Pack arrives (MinX + 319 = position of the rectangles on the right)
            if (this.PositionX+40 >= VisibleBoundsWorldspace.MinX + 319)
            {
                

                if(this.ignore){
                    Debug.WriteLine("ignore");
                }
                else if (this.corrupt)
                {
                    Debug.WriteLine("corrupt");
                    PipelineProtocolsScene.PackCorrupt(this);
                }
                else if (this.lost)
                {
                    Debug.WriteLine("lost");
                    PipelineProtocolsScene.PackLost(this);
                }
                //arrived without corruption and didn't get lost on the way
                else
                {
                    Debug.WriteLine("all good");
                    PipelineProtocolsScene.PackArrived(this);

                }
                this.RemoveChild(this.sprite);
            }
        }

		/**********************************************************************
        *********************************************************************/
		private bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
		{
            if(touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)//50 because it's the height of the packages spriteframe
              )
            {
                switch(touchCount){
                    case 0: Corrupt(); break;
                    case 1: SlowDown(); 
                            PipelineProtocolsScene.SendSlowPackageAt(this.seqnum, ((int)touch.Location.X - 20));
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
        void Corrupt(){
            this.touchCount++;
            this.corrupt = true;
            this.lost = false;
            this.ignore = false;
            UpdateMyColor();

            //if seqnum not already in list
            /*if (PipelineProtocolsScene.lostOrCorruptP != null && !PipelineProtocolsScene.lostOrCorruptP.Contains(this.seqnum))
            {
                PipelineProtocolsScene.lostOrCorruptP.Add(this.seqnum); //add to list 
                PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); //the ACK will also never arrive 
                Debug.WriteLine("package corrupt: " + PipelineProtocolsScene.lostOrCorruptP.Last());
            }*/
        }

        /**********************************************************************
        *********************************************************************/
        //if package was clicked a second time (slowed down)
        void SlowDown()
        {
            this.touchCount = 5; // unable to react to another touch
            this.corrupt = false;
            this.lost = false;
            this.ignore = true;

            //seqnum is not corrupt anymore . just slowed down
            /*if (PipelineProtocolsScene.lostOrCorruptP != null && PipelineProtocolsScene.lostOrCorruptP.Contains(this.seqnum))
            {
                PipelineProtocolsScene.lostOrCorruptP.Remove(this.seqnum);
                PipelineProtocolsScene.lostOrCorruptACK.Remove(this.seqnum);
            }
            */
            this.RemoveChild(this.sprite); //removes the visible! sprites. Actions are still running in the background
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
            UpdateMyColor();

            //if seqnum not already in list
            /*if (PipelineProtocolsScene.lostOrCorruptP != null && !PipelineProtocolsScene.lostOrCorruptP.Contains(this.seqnum))
            {
                PipelineProtocolsScene.lostOrCorruptP.Add(this.seqnum); //add to list 
                PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); //the ACK will also never arrive 
                Debug.WriteLine("package corrupt: " + PipelineProtocolsScene.lostOrCorruptP.Last());
            }
            */
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

            //if seqnum not already in list
            /*if (PipelineProtocolsScene.lostOrCorruptP != null && !PipelineProtocolsScene.lostOrCorruptP.Contains(this.seqnum))
            {

                PipelineProtocolsScene.lostOrCorruptP.Add(this.seqnum); //add to list 
                PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); //the ACK will also never arrive 
                Debug.WriteLine("package lost: " + PipelineProtocolsScene.lostOrCorruptP.Last());
            }*/
        }

		/**********************************************************************
        *********************************************************************/
		public void UpdateMyColor()
		{
			this.sprite.Color = CCColor3B.Red; //workaround for Android. but also changes the base color of the sprite
		}

		/**********************************************************************
        *********************************************************************/
		public int GetID()
		{
			return this.id;
		}
	}
}