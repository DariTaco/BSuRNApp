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
        static CCLayer layer;

        //variables for drawing the memory box
        static List<int> fragmentList; //used to draw the memory
        static int availableMemory; //sum of all fragment values
        static int numberOfFragments; //numberOfFragments-1 = number of parting lines(Size = 1) between fragments
        static int totalMemorySize; //totalMemorySize = availableMemory + numberOfFragments -1
        static float relativeFragmentSize; //rule of three -> 300px XYtotalMemorySize //I chose float instead of double because in order to draw a box with CCSharp you need a float value
        static CCDrawNode cc_box;

		//variables for adding function
		static String strategy; 
        public static int[,] memoryBlocks; //used to work with the memory and is constantly updated
        public static int pos; //memoryBlocks[pos]
        //+relativeFragmentsize
        //number of Blocks = memoryBlock.length();

        //lines and position of red arrow
        static CCDrawNode cc_arrow1;
        static CCDrawNode cc_arrow1a;
        static CCDrawNode cc_arrow1b;
        static float posArrow1;



        //lines and position of grey arrow
        static CCDrawNode cc_arrow2;
        static CCDrawNode cc_arrow2a;
        static CCDrawNode cc_arrow2b;
        static float posArrow2;

        //fill
        static CCDrawNode cc_fill;

        //CONSTRUCTOR
		public AllocationStrategiesScene(CCGameView gameView) : base(gameView)
        {
			//add a layer to draw on
			layer = new CCLayer();
			this.AddLayer(layer); 

            fragmentList = AllocationStrategies.fragmentList;
            strategy = AllocationStrategies.strategy;

			//create an array for all fragments=memoryblocks
            memoryBlocks = new int[fragmentList.Count, 2];
            for (int i = 0; i < memoryBlocks.GetLength(0); i++) // i < size of Array Dimension 0
			{
				memoryBlocks[i, 0] = fragmentList.ElementAt(i);
                memoryBlocks[i, 1] = 0;
			}

			/*for (int i = 0; i < memoryBlocks.GetLength(0); i++)
			{
				for (int j = 0; j < memoryBlocks.GetLength(1); j++)
				{
                    Debug.WriteLine(" " +memoryBlocks[i, j]);
				}
			}*/

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


        static void DrawMemory()
        {
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

            for (int i = 0; i < numberOfFragments - 1; i++)
            {
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

        public static void DrawRedArrow()
        {
            //red arrow
            float c = 0;
            for (int i = 0; i <= pos; i++)
            {
                if(i==pos){c += memoryBlocks[i, 1] * relativeFragmentSize + (memoryBlocks[i, 0] * relativeFragmentSize)/2; }
                else{c += memoryBlocks[i, 1] * relativeFragmentSize + memoryBlocks[i, 0] * relativeFragmentSize; }
                if (i > 0) { c += relativeFragmentSize; } //don't forget the parting lines with width 1
            }

            //Memory Box starts at 15 +1 linewidth ->15.5
            posArrow1 = 15.5f + c; 


			cc_arrow1 = new CCDrawNode();
			cc_arrow1a = new CCDrawNode();
			cc_arrow1b = new CCDrawNode();

			cc_arrow1.DrawLine(
				from: new CCPoint(posArrow1, 95),
				to: new CCPoint(posArrow1, 75),
				lineWidth: 1,
				color: CCColor4B.Red);
			cc_arrow1a.DrawLine(
				from: new CCPoint(posArrow1 + 0.5f, 75),
				to: new CCPoint(posArrow1 - 5, 80),
				lineWidth: 1,
				color: CCColor4B.Red);
			cc_arrow1b.DrawLine(
				from: new CCPoint(posArrow1 - 0.5f, 75),
				to: new CCPoint(posArrow1 + 5, 80),
				lineWidth: 1,
				color: CCColor4B.Red);

			layer.AddChild(cc_arrow1);
			layer.AddChild(cc_arrow1a);
			layer.AddChild(cc_arrow1b);
        }

        public static void ClearRedArrow()
        {
			if (cc_arrow1 != null && cc_arrow1a != null && cc_arrow1b != null)
			{
				cc_arrow1.Clear();
				cc_arrow1b.Clear();
				cc_arrow1a.Clear();
			}
        }

        public static void DrawGrayArrow()
        {
			//gray arrow
			posArrow2 = 16;
			cc_arrow2 = new CCDrawNode();
			cc_arrow2a = new CCDrawNode();
			cc_arrow2b = new CCDrawNode();

			cc_arrow2.DrawLine(
				from: new CCPoint(posArrow2, 16),
				to: new CCPoint(posArrow2, 0),
				lineWidth: 1,
				color: CCColor4B.Gray);
			cc_arrow2a.DrawLine(
				from: new CCPoint(posArrow2 + 0.5f, 16),
				to: new CCPoint(posArrow2 - 5, 12),
				lineWidth: 1,
				color: CCColor4B.Gray);
			cc_arrow2b.DrawLine(
				from: new CCPoint(posArrow2 - 0.5f, 16),
				to: new CCPoint(posArrow2 + 5, 12),
				lineWidth: 1,
				color: CCColor4B.Gray);

			layer.AddChild(cc_arrow2);
			layer.AddChild(cc_arrow2a);
			layer.AddChild(cc_arrow2b);
        }

        public static void ClearGrayArrow()
        {
			if (cc_arrow2 != null && cc_arrow2a != null && cc_arrow2b != null)
			{
				cc_arrow2.Clear();
				cc_arrow2b.Clear();
				cc_arrow2a.Clear();
			}
        }

        public static void DrawFill()
        {
            int request = AllocationStrategies.memoryRequest; 
			float start = 0;
			for (int i = 0; i <= pos; i++)
			{
				if (i == pos) 
                { 
                    start += memoryBlocks[i, 1] * relativeFragmentSize; 
                    Debug.WriteLine("request:" + request);
                    Debug.WriteLine("pos:"+pos);
                    Debug.WriteLine("memoryBlocksize:"+ memoryBlocks[i,0]);

                    memoryBlocks[i, 0] -= request;
                    memoryBlocks[i, 1] += request;
                }
				else 
                { 
                    start += memoryBlocks[i, 1] * relativeFragmentSize + memoryBlocks[i, 0] * relativeFragmentSize;
                }
				if (i > 0) 
                { 
                    start += relativeFragmentSize;
                } //don't forget the parting lines with width 1
			}

            //Memory Box starts at 15 +1 linewidth -> we have to start at 16
            float partingLineWidth = relativeFragmentSize / 2;
            float posXstart = 16f + start + +partingLineWidth;


            //var fill = new CCRect(posXstart, 22, posXend, 48);//CCRect(x,y,legth,width)
			cc_fill = new CCDrawNode();

            for (int i = 0; i < request; i++)
            {
                cc_fill.DrawLine(
                    from: new CCPoint(posXstart, 22),
                    to: new CCPoint(posXstart, 70),
                    lineWidth: partingLineWidth,
                    color: CCColor4B.Blue);
                posXstart += relativeFragmentSize;
            }
			layer.AddChild(cc_fill);

		}

        public static bool CheckIfFull()
        {
            bool check = true;
            for (int i = 0; i < memoryBlocks.GetLength(0); i++)
            {
                if (memoryBlocks[i, 0] != 0) { check = false; } //as long as one memory block with free space remains it's(the whole memory) not full!
            }
            return check;
        }

        public static void FirstFit(int memoryRequest)
        {
            //don't show previously drawn arrows
            ClearRedArrow();


            if (memoryBlocks[pos, 0] == 0)
            {
                    pos++;
                    FirstFit(memoryRequest);
            }
            else
            { 
                DrawRedArrow();
                AllocationStrategies.l_Free.Text = memoryBlocks[pos, 0].ToString();
                AllocationStrategies.l_Diff.Text = (memoryBlocks[pos, 0] - memoryRequest).ToString();
				if (pos == memoryBlocks.GetLength(0) - 1 && memoryRequest > memoryBlocks[pos, 0])
				{
					Debug.WriteLine("End is reached");
					AllocationStrategies.memoryRequestState = (WertheApp.BS.AllocationStrategies.myEnum)AllocationStrategies.myEnum.unsuccessfull;

				}
				else if (memoryRequest <= memoryBlocks[pos, 0]) //if it fits ->successfull
				{
					Debug.WriteLine("It fits in memory block: " + memoryBlocks[pos, 0]);
					AllocationStrategies.memoryRequestState = (WertheApp.BS.AllocationStrategies.myEnum)AllocationStrategies.myEnum.successfull;
				}
            }

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
