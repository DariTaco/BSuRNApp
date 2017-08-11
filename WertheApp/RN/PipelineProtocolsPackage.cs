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
		static int count = 0;
		bool corrupt = false;

		CCEventListenerTouchOneByOne touchListener;

		public PipelineProtocolsPackage() : base()
		{
            Debug.WriteLine("create ccSprite");
			this.sprite = new CCSprite();
            Debug.WriteLine("add count");
			this.id = count;
            Debug.WriteLine("increase count");
			count++;
			// Center the Sprite in this entity to simplify
			// centering the Ship on screen
            Debug.WriteLine("set AnchorPoint");
			this.sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
            Debug.WriteLine("access myGreen");
			CCSpriteFrame greenFrame = new CCSpriteFrame(new CCTexture2D("myGreen.png"), new CCRect(0, 0, 40, 50));//x and y pos in the sprite image and size and heigth of the sprite
            sprite.Color = CCColor3B.Green; //EXtra line of code for Android.....since I didn't find out how to access the png in Android. It crashed every single time
            Debug.WriteLine("add spriteframe");
			this.sprite.SpriteFrame = greenFrame;

			this.AddChild(sprite);

			touchListener = new CCEventListenerTouchOneByOne();
			touchListener.OnTouchBegan = OnTouchBegan;
			AddEventListener(touchListener, this);

		}

		public void UpdateMyColor()
		{
			CCSpriteFrame redFrame = new CCSpriteFrame(new CCTexture2D("myRed"), new CCRect(0, 0, 40, 50));
            this.sprite.SpriteFrame = redFrame;
            this.sprite.Color = CCColor3B.Red; //workaround for Android
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
			if (BoundingBoxTransformedToParent.ContainsPoint(touch.Location))
			{
				var location = touch.Location;
				this.Color = CCColor3B.Magenta;
				Debug.WriteLine("#############CLICKED: " + location + "#############");
				return true;
			}
			else
			{
				UpdateMyColor();
				//GetSpriteByID(1).SpriteFrame.Texture = new CCTexture2D("red");
				//CCTexture2D redX = new CCTexture2D("red");
				//sprite.SpriteFrame.Texture = new CCTexture2D("red"); 
				Debug.WriteLine("ID:" + GetID() + " x:" + touchEvent.CurrentTarget.PositionX + " y:" + touchEvent.CurrentTarget.PositionY);
				return false;

			}
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
