using System;
using CocosSharp;

namespace WertheApp.BS
{
    public class AllocationStrategiesScene : CCScene
    {
        //VARIABLES
		CCDrawNode circle;
        CCDrawNode cc_box;

        //CONSTRUCTOR
		public AllocationStrategiesScene(CCGameView gameView) : base(gameView)
        {
			//add a layer to draw on
            var layer = new CCLayer();
			this.AddLayer(layer);

            //draw the outlines of the memorybox
			var box = new CCRect(15, 1, 300, 70);
            cc_box = new CCDrawNode();
			cc_box.DrawRect(
                box,
				fillColor: CCColor4B.Transparent,
				borderWidth: 1,
                borderColor: CCColor4B.Gray);

            //add box to layer
            layer.AddChild(cc_box);

			circle = new CCDrawNode();
			layer.AddChild(circle);
			circle.DrawCircle(
				// The center to use when drawing the circle,
				// relative to the CCDrawNode:
				new CCPoint(0, 0),
				radius: 15,
				color: CCColor4B.White);
			circle.PositionX = 20;
			circle.PositionY = 50;
		}

        //METHODS
        void DrawMemoryBox(){ 

                                   }


    }
}
