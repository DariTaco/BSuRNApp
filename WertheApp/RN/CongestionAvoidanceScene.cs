﻿using System;
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

        //CONSTRUCTOR
        public CongestionAvoidanceScene(CCGameView gameView) : base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);
        }

        //METHODS
    }
}

