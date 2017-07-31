using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using CocosDenshion;

using Xamarin.Forms;

namespace WertheApp.BS
{
    public class BuddySystemScene : CCScene
    {
		//VARIABLES
		static CCLayer layer;
        int absoluteMemorySize;
        static CCDrawNode cc_box;

        public static List<int> freeBlocksList; //Process names

		//CONSTRUCTOR
		public BuddySystemScene(CCGameView gameView): base(gameView)
        {
            
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer);


            absoluteMemorySize = BuddySystem.absoluteMemorySize;

            DrawTest();
		}

        //METHODS
        static void DrawTest()
        {
            var box = new CCRect(15, 21, 50, 302);
            cc_box = new CCDrawNode();
			cc_box.DrawRect(
	
                box,
	
                fillColor: CCColor4B.White,
	
                borderWidth: 1,
	
                borderColor: CCColor4B.Gray);
            layer.AddChild(cc_box);
        }

        //draws the current Memory
        static void DrawMemory()
        {
            
        }

		//Finds a free block of size 2k and marks it as occupied
        static void Allocate(int k)
        {
            
        }

		//Marks the previously allocated block B as free and may merge it with others to form a larger free block
        static void Dellocate(String b)
        {
            
        }

        //Merges to blocks
        static void MergeBlocks()
        {
            
        }

	}
}

