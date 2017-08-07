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

        static CCDrawNode cc_startBox;
        static CCRect startBox;
        static CCColor4B col1 = CCColor4B.Gray;

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
            //define object
            float yPos = 15 + (65 * (28-a)); //where the box !starts!
			startBox = new CCRect(82, yPos, 40, 50); //x,y,length, width 82
			cc_startBox = new CCDrawNode();
			cc_startBox.DrawRect(
			startBox,
				fillColor: col1,
			borderWidth: 1,
				borderColor: CCColor4B.Red);
			layer.AddChild(cc_startBox);


			//add touch listener
			var touchListener = new CCEventListenerTouchAllAtOnce();
			touchListener.OnTouchesMoved = HandleInput;
            layer.AddEventListener(touchListener, cc_startBox);

            //define action
            var distance = new CCPoint(196, 0); //82 to 278 = 278-82 = 196
            var distance2 = new CCPoint(0, 0); //0 as an x-value migth seem strange, but the reference value is the x-value of the object when it was first defined!
            float timeToTake = 5f;
            var sendPackageAction = new CCMoveTo(timeToTake, distance); //this action moves the object 196 in x-direction within 5 seconds
            var sendAckAction = new CCMoveTo(timeToTake, distance2); //this action moves the object back to where it originally was
			var removeAction = new CCRemoveSelf(); //this action removes the object

            //apply action to object
            //cc_startBox.AddAction(sendingAction);
           
            //define sequence of actions 
            var cc_seq1 = new CCSequence(sendPackageAction, sendAckAction, removeAction);

            //apply sequence of actions to object
            cc_startBox.RunAction(cc_seq1);
        }

        private static void HandleInput(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count > 0)
			{
				CCTouch firstTouch = touches[0];

                cc_startBox.PositionX = firstTouch.Location.X;
				cc_startBox.PositionY = firstTouch.Location.Y;
			}

            cc_startBox.Color = CCColor3B.Magenta;
            Debug.WriteLine("clicked ");
            col1 = CCColor4B.Magenta;
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

