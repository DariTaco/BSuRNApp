using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq; //fragmentList.ElementAt(i);
using System.Diagnostics;
using Xamarin.Forms; //Messaging Center

namespace WertheApp.BS
{
    public class AllocationStrategiesScene : CCScene
    {
        //VARIABLES
        CCLayer layer;
        static List<int> fragmentList; //used to draw the memory
		public static int[] memoryBlocks; //used to work with the memory
		static String strategy;
        public static int pos;

        int availableMemory; //sum of all fragment values
        int numberOfFragments; //numberOfFragments-1 = number of parting lines(Size = 1) between fragments
        int totalMemorySize; //totalMemorySize = availableMemory + numberOfFragments -1
        float relativeFragmentSize; //rule of three -> 300px XYtotalMemorySize //I chose float instead of double because in order to draw a box with CCSharp you need a float value


        CCDrawNode cc_box;

        //CONSTRUCTOR
		public AllocationStrategiesScene(CCGameView gameView) : base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer); 

            fragmentList = AllocationStrategies.fragmentList;
            strategy = AllocationStrategies.strategy;

			//create an array for all fragments=memoryblocks
			memoryBlocks = new int[fragmentList.Count];
			for (int i = 0; i < memoryBlocks.Length; i++)
			{
				memoryBlocks[i] = fragmentList.ElementAt(i);
			}

            CalculateNeededVariables();
            DrawMemory();
		}

        //METHODS
        //Calculates availableMemory, numerOfFragments, totalMemorySize, relativeFragmentSize
        void CalculateNeededVariables(){
            int sizeOfList = fragmentList.Count();
            availableMemory = 0;
            numberOfFragments = sizeOfList;
            for (int i = 0; i < sizeOfList; i++)
            {
                availableMemory += fragmentList.ElementAt(i);
            }
            totalMemorySize = availableMemory + numberOfFragments - 1;
            //300 because memorybox is set to be 300px
            relativeFragmentSize = float.Parse("300") /float.Parse(totalMemorySize.ToString());//nicht gerade elegant, ich weiß
        }


        void DrawMemory(){
			//the problem with drawing lines is, that they don't start at a point and have a witdth in only one direction. 
			//The width actually spreads in both directions. wich makes it kinda difficult

			//draw the outlines of the memorybox
			//since the border width is 1, in order to achieve exactly 300 entities space, we have to add 2 entities
			var box = new CCRect(15, 21, 302, 50);//CCRect(x,y,legth,width)
			cc_box = new CCDrawNode();
			cc_box.DrawRect(
				box,
				fillColor: CCColor4B.White,
				borderWidth: 1,
				borderColor: CCColor4B.Gray);
			//add box to layer
			layer.AddChild(cc_box);

            //draw fragmentation of memorybox //it's only necessary to draw the gaps/parting lines
            int fragmentSize;
            float sizeInBox;
			float partingLineWidth = relativeFragmentSize / 2;
            float startX = 16; //kinda fixed value, since I set the starting point of the whole memorybox to 15// also add the border with and start after that

            for (int i = 0; i < numberOfFragments-1; i++){
                fragmentSize = fragmentList.ElementAt(i);
				sizeInBox = fragmentSize * relativeFragmentSize;
                startX += sizeInBox + partingLineWidth;

                //draw the line
				CCDrawNode cc_partingLine = new CCDrawNode();
				cc_partingLine.DrawLine(
					from: new CCPoint(startX, 22),
					to: new CCPoint(startX, 70),
					lineWidth: partingLineWidth,
                    color: CCColor4B.Gray);
				layer.AddChild(cc_partingLine);
                startX += partingLineWidth;
 
            }
        }

		public static void FirstFit(int memoryRequest) 
        {
            //if end is reached and nothing did fit in -> unsuccessfull
            if (pos == memoryBlocks.Length-1 && memoryRequest > memoryBlocks[pos])
            { 
                Debug.WriteLine("End is reached");
                AllocationStrategies.memoryRequestState = (WertheApp.BS.AllocationStrategies.myEnum)3;

			}
            else if(memoryRequest <= memoryBlocks[pos]) //if it fits ->successfull
            {
                Debug.WriteLine("It fits in memory block: " + memoryBlocks[pos]);
                AllocationStrategies.memoryRequestState = (WertheApp.BS.AllocationStrategies.myEnum)2;
            }

            //if end is reached and it did fit in -> successfull -> bei nächstem button click draw methode aufrufen
        }

		public static void NextFit(int memoryRequest) 
        { 
        
        }

        public static void BestFit(int memoryRequest) 
        { 
        
        }

		public static void WorstFit(int memoryRequest) 
        { 
        
        }

		public static void TailoringBestFit(int memoryRequest) 
        { 
        
        }

		public static void RequestNew(int memoryRequest)
		{
            pos = 0;
			switch (strategy)
			{
				case "First Fit":
					FirstFit(memoryRequest);
					break;
				case "Next Fit":
					NextFit(memoryRequest);
					break;
				case "Best Fit":
					BestFit(memoryRequest);
					break;
				case "Worst Fit":
					WorstFit(memoryRequest);
					break;
				case "Tailoring Best Fit":
					TailoringBestFit(memoryRequest);
					break;
			}
            /*
            Debug.WriteLine("total memorysize = "+totalMemorySize);
            Debug.WriteLine("relative fragment size = "+ relativeFragmentSize);
            Debug.WriteLine("total memorysize *relativefragmentsize = " +totalMemorySize*relativeFragmentSize);


            Debug.WriteLine("##############");
            Debug.WriteLine(availableMemory);
            Debug.WriteLine(numberOfFragments);
            Debug.WriteLine(totalMemorySize);
            Debug.WriteLine(relativeFragmentSize);*/

			//test for 2 Fragments with size 1 
			/*
            bool colorS = true;
            for (int i = 0; i < 3; i++)
            {
                if (colorS)
                {
                    CCDrawNode cc_partingLine = new CCDrawNode();
                    cc_partingLine.DrawLine(
                        from: new CCPoint(startX, 2),
                        to: new CCPoint(startX, 70),
                        lineWidth: partingLineWidth,
                        color: CCColor4B.Blue);
                    layer.AddChild(cc_partingLine);
					startX += relativeFragmentSize;
                    colorS = false;
                    continue;
                }
                else if(!colorS)
                {
                    CCDrawNode cc_partingLine = new CCDrawNode();
                    cc_partingLine.DrawLine(
                        from: new CCPoint(startX, 2),
                        to: new CCPoint(startX, 70),
                        lineWidth: partingLineWidth,
                        color: CCColor4B.Orange);
                    layer.AddChild(cc_partingLine);
                    startX += relativeFragmentSize;
                    colorS = true;
                    continue;
                }
            }*/

        }


    }
}
