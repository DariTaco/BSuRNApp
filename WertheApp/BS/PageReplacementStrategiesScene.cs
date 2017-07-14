using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class PageReplacementStrategiesScene : CCScene
    {
		//VARIABLES
		CCLayer layer;
		List<int> sequenceList { get; set; }
		String strategy;
		int ram; 
        int disc;

        //CONSTRUCTOR
        public PageReplacementStrategiesScene(CCGameView gameView): base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);

            sequenceList = PageReplacementStrategies.sequenceList;
            strategy = PageReplacementStrategies.strategy;
            ram = PageReplacementStrategies.ram;
            disc = PageReplacementStrategies.disc;
		}

        //METHODS
    }
}

