using System;
using CocosSharp;

namespace WertheApp.BS
{
    public class AllocationStrategiesScene : CCScene
    {
		CCDrawNode circle;
		public AllocationStrategiesScene(CCGameView gameView) : base(gameView)
        {
			var layer = new CCLayer();
			this.AddLayer(layer);
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
    }
}
