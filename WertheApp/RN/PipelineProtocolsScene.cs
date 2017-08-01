using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class PipelineProtocolsScene : CCScene
    {
		//VARIABLES
		CCLayer layer;

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

            DrawTest();
        }

		//METHODS
        void DrawLabels()
        {
           
        }


        //draw everything. Begginging from the Top
		void DrawTest()
		{
            //draw first line without boxes

            //draw 31 lines of boxes
            float yPos =800;
            for (int i = 0; i < 31; i++)
            {
                //draw the box on the left side
                var leftBox = new CCRect(15, yPos,20,20);
                CCDrawNode cc_leftBox = new CCDrawNode();
				cc_leftBox.DrawRect(
                leftBox,
				fillColor: CCColor4B.White,
				borderWidth: 1,
				borderColor: CCColor4B.Gray);
                layer.AddChild(cc_leftBox);

                //draw the box on the right side
                var rightBox = new CCRect(302, yPos,20,20);
				CCDrawNode cc_rightBox = new CCDrawNode();
				cc_rightBox.DrawRect(
				rightBox,
				fillColor: CCColor4B.White,
				borderWidth: 1,
				borderColor: CCColor4B.Gray);
				layer.AddChild(cc_rightBox);
                //set yPos 
                yPos -= 40;

            }


			// CCLabel Bitmap Font - No need to pass a CCLabelFormat because the default for this constructor is BitmapFont
			var label2 = new CCLabel("Hello Bitmap Font", "Arial",44);
            label2.Position = new CCPoint(90, 800);
            label2.Color = CCColor3B.Black;
            //label2.IgnoreAnchorPointForPosition = true;
			// CCLabel using the system font Arial
			var label3 = new CCLabel("Hello System Font", "Arial", 20);

            layer.AddChild(label2);

            label3.AnchorPoint = new CCPoint(21, 34); layer.AddChild(label3);
            var label4 = new CCLabel("Bildschirm berühren!", "Arial", 44)
            {
                Position = new CCPoint(100, 100),//layer.VisibleBoundsWorldspace.Center,
				Color = CCColor3B.Black,
				IsAntialiased = true,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				IgnoreAnchorPointForPosition = true
			};
            layer.AddChild(label4);
            // CCLabel using the MorrisRoman-Black.ttf font included as content in the fonts folder
           // var label4 = new CCLabel("Hello MorrisRoman-Black", "fonts/MorrisRoman-Black.ttf", size, CCLabelFormat.SystemFont);

		}
    }
}

