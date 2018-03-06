using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class CongestionAvoidanceScene : CCScene
    {
		//VARIABLES
		CCLayer layer;
		int errorTreshold;
		int treshold;
		bool reno;
		bool tahoe;

        //CONSTRUCTOR
        public CongestionAvoidanceScene(CCGameView gameView) : base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);

            errorTreshold = CongestionAvoidance.errorTreshold;
            treshold = CongestionAvoidance.treshold;
            reno = CongestionAvoidance.reno;
            tahoe = CongestionAvoidance.tahoe;
        }

        //METHODS
    }
}

