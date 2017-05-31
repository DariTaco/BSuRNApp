using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace WertheApp.BS
{
    public class AllocationStrategiesScene : CCScene
    {
        //VARIABLES
        List<int> fragmentList;
        String strategy;

        int availableMemory; //sum of all fragment values
        int numberOfFragments; //numberOfFragments-1 = number of parting lines(Size = 1) between fragments
        int totalMemorySize; //totalMemorySize = availableMemory + numberOfFragments -1
        float relativeFragmentSize { get; set; } //rule of three -> 300px XYtotalMemorySize

        CCDrawNode cc_box;

        //CONSTRUCTOR
		public AllocationStrategiesScene(CCGameView gameView) : base(gameView)
        {
            fragmentList = AllocationStrategies.fragmentList;
            strategy = AllocationStrategies.strategy;

            Debug.WriteLine(strategy);
            Debug.WriteLine("##############");
            Debug.WriteLine(fragmentList.ElementAt(0));
          

			//add a layer to draw on
            var layer = new CCLayer();
			this.AddLayer(layer);

            //draw the outlines of the memorybox
			var box = new CCRect(15, 1, 300, 70);
            cc_box = new CCDrawNode();
			cc_box.DrawRect(
                box,
                fillColor: CCColor4B.White,
				borderWidth: 1,
                borderColor: CCColor4B.Gray);

            //add box to layer
            layer.AddChild(cc_box);
		}

        //METHODS


    }
}
