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

		public PipelineProtocolsPackage(int seqnum, bool ack) : base()
		{
			this.sprite = new CCSprite();
			this.id = count;
            this.seqnum = seqnum;
			count++;
            touchCount = 0;
			this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
			//CCSpriteFrame greenFrame = new CCSpriteFrame(new CCTexture2D("myGreen.png"), new CCRect(0, 0, 40, 50));//x and y pos in the sprite image and width and heigth of the sprite
            CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite.png"), new CCRect(0, 0, 40, 50));
            //if its an ACK from the receiver
            if (ack)
            {
                sprite.Color = CCColor3B.Green; //EXtra line of code for Android.....since I didn't find out how to access the png in Android. It crashed every single time
                this.sprite.SpriteFrame = whiteFrame;
            }//if its a packet
            else
            {
                sprite.Color = CCColor3B.Gray; //workaround for Android. but also changes the base color of the sprite. Which is why I use a white image instead of a gray one
				this.sprite.SpriteFrame = whiteFrame;
            }


			this.AddChild(sprite);

			touchListener = new CCEventListenerTouchOneByOne();
			touchListener.OnTouchBegan = OnTouchBegan;
			AddEventListener(touchListener, this);

		}

		public void UpdateMyColor()
		{
			CCSpriteFrame whiteFrame = new CCSpriteFrame(new CCTexture2D("myWhite"), new CCRect(0, 0, 40, 50));
            this.sprite.SpriteFrame = whiteFrame;
            this.sprite.Color = CCColor3B.Red; //workaround for Android. but also changes the base color of the sprite
		}
		public CCSprite GetSpriteByID(int id)
		{
			return this.sprite;
		}

		public int GetID()
		{
			return this.id;
		}

		private bool OnTouchBegan(CCTouch touch, CCEvent touchEvent)
		{
            Debug.WriteLine("touch.Location.X = "+touch.Location.X);
            Debug.WriteLine("touchEvent.CurrentTarget.PositionX = " + touchEvent.CurrentTarget.PositionX);
            Debug.WriteLine("touchEvent.CurrentTarget.PositionX+40 = " + (touchEvent.CurrentTarget.Position.X + 40.0f));
            //if package was clicked the first time
            if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
                && touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
                && touchCount == 0) //50 because it's the height of the packages spriteframe
            {
                UpdateMyColor();
                this.touchCount++;
                this.corrupt = true;
                return true;
            }
            //if package was clicked a second time
			else if (touch.Location.X > touchEvent.CurrentTarget.PositionX && touch.Location.X < (touchEvent.CurrentTarget.Position.X + 40.0f) //40 because it's the width of the packages spriteframe
				&& touch.Location.Y > touchEvent.CurrentTarget.PositionY && touch.Location.Y < (touchEvent.CurrentTarget.PositionY + 50.0f)
				&& touchCount > 0)
            {
                this.lost = true;
                this.touchCount++;
                this.RemoveChild(this.sprite); //removes the visible! sprites. Actions are still running in the background
                return false;

            }
            else { return false; }
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
