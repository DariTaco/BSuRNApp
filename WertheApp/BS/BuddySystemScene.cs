using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystemScene : CCScene
    {
		//VARIABLES
		CCLayer layer;
        int absoluteMemorySize;

        //CONSTRUCTOR
        public BuddySystemScene(CCGameView gameView): base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);

            absoluteMemorySize = BuddySystem.absoluteMemorySize;
		}

        //METHODS
    }
}

