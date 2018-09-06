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
		int ramSize; 
        int discSize;

        //CONSTRUCTOR
        public PageReplacementStrategiesScene(CCGameView gameView): base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);

            //Android bug: Background in Android is always black. Workaround: draw a white rect with the size of the layer
            if (Device.RuntimePlatform == Device.Android)
            {
                var cc_background = new CCDrawNode();
                var backgroundWorkAround = new CCRect(
                    0, 0, layer.VisibleBoundsWorldspace.MaxX, layer.VisibleBoundsWorldspace.MaxY);
                cc_background.DrawRect(backgroundWorkAround,
                    fillColor: CCColor4B.White);
                layer.AddChild(cc_background);
            }

            sequenceList = PageReplacementStrategies.SequenceList;
            strategy = PageReplacementStrategies.strategy;
            ramSize = PageReplacementStrategies.ramSize;
            discSize = PageReplacementStrategies.discSize;

            Debug.WriteLine("x: " + layer.VisibleBoundsWorldspace.MaxX);
            Debug.WriteLine("y: " + layer.VisibleBoundsWorldspace.MaxY);
		}

        //METHODS
        /**********************************************************************
        *********************************************************************/
        public static void Optimal(){
            
        }

        /**********************************************************************
        *********************************************************************/
        public static void Fifo(){
            
        }

        /**********************************************************************
        *********************************************************************/
        public static void FifoSecond(){
            
        }

        /**********************************************************************
        *********************************************************************/
        public static void Rnu(){
            
        }

        /**********************************************************************
        *********************************************************************/
        public static void RnuSecond(){
            
        }


    }
}