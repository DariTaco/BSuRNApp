using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;

//TAKE NOTE: the reference value of an action is always the same!! 
//It always refers to the values that were set when the original object was first definded.
//and not some altered value after some action that happended before the current one

namespace WertheApp.RN
{
    public class PipelineProtocolsScene : CCScene
    {
		//VARIABLES
		static CCLayer layer;

		int windowSize;
		String strategy;



        //CONSTRUCTOR
        public PipelineProtocolsScene(CCGameView gameView) : base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);

            windowSize = PipelineProtocols.windowSize;
            strategy = PipelineProtocols.strategy;

            DrawLabelsAndBoxes();
        }

		//METHODS
        //boxes range from 0 to 28
        public static void SendPackageAt(int a)
        {

			/*//define object
            float yPos = 15 + (65 * (28-a)); //where the box !starts!
			var startBox = new CCRect(82, yPos, 40, 50); //x,y,length, width 82
			var cc_startBox = new CCDrawNode();
			cc_startBox.DrawRect(
			startBox,
              fillColor: CCColor4B.White,
			borderWidth: 1,
				borderColor: CCColor4B.Red);
			//layer.AddChild(cc_startBox); */
			
            /*//add touch listener
			var touchListener = new CCEventListenerTouchAllAtOnce();
			touchListener.OnTouchesBegan = HandleInput; */
			
            /*//define action for DrawRect
			var distance = new CCPoint(196, 0); //82 to 278 = 278-82 = 196
			var distance2 = new CCPoint(0, 0); //0 as an x-value migth seem strange, but the reference value is the x-value of the object when it was first defined!
			float timeToTake = 5f;
			var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
			var sendAckAction = new CCMoveTo(timeToTake, distance2); //this action moves the object back to where it originally was
			var removeAction = new CCRemoveSelf(); //this action removes the object*/
            
			/*//apply action to object
			cc_startBox.AddAction(sendingAction);*/


			//define object
			float yPos = 15 + (65 * (28 - a)); //where the box !starts!
			var pp = new PPackage();
            pp.Position = new CCPoint(82,yPos);
            layer.AddChild(pp);

			//define action
            float timeToTake = 5f;
            var distance = new CCPoint(280, yPos); //82 to 278 = 278-82 = 196
            var distance2 = new CCPoint(82, yPos);
			var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
			var sendAckAction = new CCMoveTo(timeToTake, distance2); //this action moves the object back to where it originally was
			var removeAction = new CCRemoveSelf(); //this action removes the object*/

			//define sequence of actions 
			var cc_seq1 = new CCSequence(sendPackageAction, sendAckAction, removeAction);

            //apply sequence of actions to object
            //cc_startBox.RunAction(cc_seq1);
            pp.RunAction(cc_seq1);
            //pp2.RunAction(cc_seq1);
        }

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

                /*var label4 = new CCLabel(i.ToString(), "Arial", 20)
				{
					Position = new CCPoint(360, yPos),//layer.VisibleBoundsWorldspace.Center,
					Color = CCColor3B.Black,
					IsAntialiased = true,
					HorizontalAlignment = CCTextAlignment.Center,
					VerticalAlignment = CCVerticalTextAlignment.Center,
					IgnoreAnchorPointForPosition = true
				};
				layer.AddChild(label4);*/

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


//CLASS FOR SPRITE OBJECT
public class PPackage : CCNode
{
	CCSprite sprite;
    int id;
    static int count = 0;

    CCEventListenerTouchOneByOne touchListener;

	public PPackage() : base()
	{
        
		sprite = new CCSprite("square");
        id = count;
        count++;
		// Center the Sprite in this entity to simplify
		// centering the Ship on screen
		sprite.AnchorPoint = AnchorPoint = new CCPoint(0, 0);
        sprite.SpriteFrame = new CCSpriteFrame(new CCTexture2D("square"), new CCRect(50, 50, 40, 50));//x and y pos in the sprite image and size and heigth of the sprite
		this.AddChild(sprite);

        touchListener = new CCEventListenerTouchOneByOne();
        touchListener.OnTouchBegan = OnTouchBegan;
		AddEventListener(touchListener, this);

	}

    public void UpdateColor()
    {
        this.sprite.SpriteFrame = new CCSpriteFrame(new CCTexture2D("redX"), new CCRect(50, 50, 40, 50));
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
        else {
            UpdateColor();
            GetSpriteByID(1).SpriteFrame.Texture = new CCTexture2D("redX");
            CCTexture2D redX = new CCTexture2D("redX");
            sprite.SpriteFrame.Texture = new CCTexture2D("redX"); Debug.WriteLine("ID:"+ GetID() + " x:" + touchEvent.CurrentTarget.PositionX + " y:"+touchEvent.CurrentTarget.PositionY); return false; 

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
