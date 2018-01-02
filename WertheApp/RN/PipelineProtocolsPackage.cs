using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Xamarin.Forms;
namespace WertheApp.RN
{
	//CLASS FOR SPRITE OBJECT
	public class PipelineProtocolsPackage : CCNode
	{
        //VARIABLES
		//CCSpriteFrame greenFrame;
		//CCSpriteFrame redFrame;

		CCSprite sprite;
		int id;
        public int seqnum;
		static int count = 0;
        int touchCount;
		public bool corrupt = false;
        public bool lost = false;
		CCEventListenerTouchOneByOne touchListener;

        //CONSTRUCTOR
		public PipelineProtocolsPackage(int seqnum) : base()
		{
			this.sprite = new CCSprite();
			this.id = count;
            this.seqnum = seqnum;
			count++;
            touchCount = 0;
			this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
			//CCSpriteFrame greenFrame = new CCSpriteFrame(new CCTexture2D("myGreen.png"), new CCRect(0, 0, 40, 50));//x and y pos in the sprite image and width and heigth of the sprite
            CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite.png"), new CCRect(0, 0, 40, 50));
      
            sprite.Color = CCColor3B.Gray; //workaround for Android. but also changes the base color of the sprite. Which is why I use a white image instead of a gray one
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
            //Debug.WriteLine("touch.Location.X = "+touch.Location.X);
            //Debug.WriteLine("touchEvent.CurrentTarget.PositionX = " + touchEvent.CurrentTarget.PositionX);
            //Debug.WriteLine("touchEvent.CurrentTarget.PositionX+40 = " + (touchEvent.CurrentTarget.Position.X + 40.0f));
            //if package was clicked the first time (corrupt)
            if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
                && touchCount == 0) //50 because it's the height of the packages spriteframe
            {
                UpdateMyColor();
                this.touchCount++;
                this.corrupt = true;

				//if seqnum not already in list
                if (PipelineProtocolsScene.lostOrCorruptP != null && !PipelineProtocolsScene.lostOrCorruptP.Contains(this.seqnum))
				{
                    PipelineProtocolsScene.lostOrCorruptP.Add(this.seqnum); //add to list 
                    PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); //the ACK will also never arrive 
					Debug.WriteLine("package corrupt: " + PipelineProtocolsScene.lostOrCorruptP.Last());
				}

                return true;
            }
            //if package was clicked a second time (lost)
			else if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
				&& touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
				&& touchCount > 0)
            {
                this.lost = true;
                this.touchCount++;
                this.RemoveChild(this.sprite); //removes the visible! sprites. Actions are still running in the background

				//if seqnum not already in list
                if (PipelineProtocolsScene.lostOrCorruptP != null && !PipelineProtocolsScene.lostOrCorruptP.Contains(this.seqnum))
				{

                    PipelineProtocolsScene.lostOrCorruptP.Add(this.seqnum); //add to list 
                    PipelineProtocolsScene.lostOrCorruptACK.Add(this.seqnum); //the ACK will also never arrive 
					Debug.WriteLine("package lost: " + PipelineProtocolsScene.lostOrCorruptP.Last());
				}

                return false;
            }
            else { return false; }
		}

		/**********************************************************************
        *********************************************************************/
		public void UpdateMyColor()
		{
			CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite"), new CCRect(0, 0, 40, 50));
			this.sprite.SpriteFrame = whiteFrame;
			this.sprite.Color = CCColor3B.Red; //workaround for Android. but also changes the base color of the sprite
		}

		/**********************************************************************
        *********************************************************************/
		public CCSprite GetSpriteByID(int id)
		{
			return this.sprite;
		}

		/**********************************************************************
        *********************************************************************/
		public int GetID()
		{
			return this.id;
		}
	}


	/*CCSprite sprite;

		CCEventListenerTouchOneByOne touchListener;

		public PPackage() : base()
		{
			sprite = new CCSprite("ship.png");
			// Center the Sprite in this entity to simplify
			// centering the Ship on screen
			//sprite.AnchorPoint = CCPoint.AnchorMiddle;
			this.AddChild(sprite);

			touchListener = new CCEventListenerTouchOneByOne();
			touchListener.OnTouchBegan = OnTouchBegan;
			AddEventListener(touchListener, this);

		}

		private bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
		{
			if (touchEvent.CurrentTarget.BoundingBoxTransformedToParent.ContainsPoint(touch.Location))
			{
				var location = touch.Location;
				this.Color = CCColor3B.Magenta;
				Debug.WriteLine("#############CLICKED: " + location + "#############");
				return true;
			}
			else { Debug.WriteLine("##" + sprite.BoundingBox.UpperRight); return false; 

				}
		}*/

}
