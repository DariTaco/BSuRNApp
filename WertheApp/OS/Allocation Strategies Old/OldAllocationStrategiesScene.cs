using System;
using CocosSharp;
using System.Collections.Generic;
using System.Linq; //fragmentList.ElementAt(i);
using Xamarin.Forms; //Messaging Center
using System.Diagnostics;

//just gonna say: whoever has to read or even work with this code. I'm genuinly sorry. I'm sure there is a way better solution for everything...
namespace WertheApp.OS
{
    public class OldAllocationStrategiesScene : CCScene
    {
        //VARIABLES
        static CCLayer layer;

        //variables for drawing the memory box
        static List<int> fragmentList; //used to draw the memory
        static List<int> indexSequenceCF;
        static int availableMemory; //sum of all fragment values
        static int numberOfFragments; //numberOfFragments-1 = number of parting lines(Size = 1) between fragments
        static int totalMemorySize; //totalMemorySize = availableMemory + numberOfFragments -1
        static float relativeFragmentSize; //rule of three -> 300px XYtotalMemorySize //I chose float instead of double because in order to draw a box with CCSharp you need a float value
        static CCDrawNode cc_box;
        static CCColor4B color_blue, color_red;

        //variables for adding function
        static String strategy;
        public static int[,] memoryBlocks; //2D array used to work with the memory and is constantly updated
        public static int pos; //memoryBlocks[pos]
        public static int suc; //the latest successfull block that was filled //NEXTFIT
        public static int end; // marks the end of search for NEXTFIT
        public static int besValue;
        public static int besPos;
        public static int indexCFLastAllocated;

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
        public OldAllocationStrategiesScene(CCGameView gameView) : base(gameView)
        {
            //add a layer to draw on
            layer = new CCLayer();
            this.AddLayer(layer);

            pos = 0;
            suc = 0;  //the very first request...
            indexCFLastAllocated = 0;
            indexSequenceCF = new List<int>();

            besValue = 0;
            besPos = -1;

            fragmentList = OldAllocationStrategies.fragmentList;
            strategy = OldAllocationStrategies.strategy;

            //create an array for all fragments=memoryblocks
            memoryBlocks = new int[fragmentList.Count, 2];
            for (int i = 0; i < memoryBlocks.GetLength(0); i++) // i < size of Array Dimension 0
            {
                memoryBlocks[i, 0] = fragmentList.ElementAt(i);
                memoryBlocks[i, 1] = 0;
            }
            end = memoryBlocks.GetLength(0)-1;
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

            CalculateNeededVariables();
            DrawMemory();

        }

        //METHODS

        /**********************************************************************
        *********************************************************************/
        //Calculates availableMemory, numerOfFragments, totalMemorySize, relativeFragmentSize
        void CalculateNeededVariables()
        {
            int sizeOfList = fragmentList.Count();
            availableMemory = 0;
            numberOfFragments = sizeOfList;
            for (int i = 0; i < sizeOfList; i++)
            {
                availableMemory += fragmentList.ElementAt(i);
            }
            totalMemorySize = availableMemory + numberOfFragments - 1;
            //300 because memorybox is set to be 300px
            relativeFragmentSize = float.Parse("300") / float.Parse(totalMemorySize.ToString());//nicht gerade elegant, ich weiß

            color_blue = new CCColor4B(67, 110, 238, 204);
            color_red = new CCColor4B(170, 0, 60);
        }


        /**********************************************************************
        *********************************************************************/
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
                borderColor: color_red);
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

        /**********************************************************************
        *********************************************************************/
        public static void DrawRedArrow()
        {
            //red arrow
            float c = 0;
            for (int i = 0; i <= pos; i++)
            {
                if (i == pos) { c += memoryBlocks[i, 1] * relativeFragmentSize + (memoryBlocks[i, 0] * relativeFragmentSize) / 2; }
                else { c += memoryBlocks[i, 1] * relativeFragmentSize + memoryBlocks[i, 0] * relativeFragmentSize; }
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
                color: color_red);
            cc_arrow1a.DrawLine(
                from: new CCPoint(posArrow1 + 0.5f, 75),
                to: new CCPoint(posArrow1 - 5, 80),
                lineWidth: 1,
                color: color_red);
            cc_arrow1b.DrawLine(
                from: new CCPoint(posArrow1 - 0.5f, 75),
                to: new CCPoint(posArrow1 + 5, 80),
                lineWidth: 1,
                color: color_red);

            layer.AddChild(cc_arrow1);
            layer.AddChild(cc_arrow1a);
            layer.AddChild(cc_arrow1b);
        }

        /**********************************************************************
        *********************************************************************/
        public static void ClearRedArrow()
        {
            if (cc_arrow1 != null && cc_arrow1a != null && cc_arrow1b != null)
            {
                cc_arrow1.Clear();
                cc_arrow1b.Clear();
                cc_arrow1a.Clear();
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void DrawGrayArrow()
        {
            //gray arrow

            float c = 0;
            for (int i = 0; i <= besPos; i++)
            {
                if (i == besPos) { c += memoryBlocks[i, 1] * relativeFragmentSize + (memoryBlocks[i, 0] * relativeFragmentSize) / 2; }
                else { c += memoryBlocks[i, 1] * relativeFragmentSize + memoryBlocks[i, 0] * relativeFragmentSize; }
                if (i > 0) { c += relativeFragmentSize; } //don't forget the parting lines with width 1
            }


            //Memory Box starts at 15 +1 linewidth ->15.5
            posArrow2 = 15.5f + c;

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

        /**********************************************************************
        *********************************************************************/
        public static void ClearGrayArrow()
        {
            
            if (cc_arrow2 != null && cc_arrow2a != null && cc_arrow2b != null)
            {
                cc_arrow2.Clear();
                cc_arrow2b.Clear();
                cc_arrow2a.Clear();
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void DrawFill()
        {
            int request = OldAllocationStrategies.memoryRequest;
            float start = 0;
            for (int i = 0; i <= pos; i++)
            {
                if (i == pos)
                {
                    start += memoryBlocks[i, 1] * relativeFragmentSize;
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
                    color: color_blue);
                posXstart += relativeFragmentSize;
            }
            layer.AddChild(cc_fill);

        }

        /**********************************************************************
        *********************************************************************/
        public static bool CheckIfFull()
        {
            bool check = true;
            for (int i = 0; i < memoryBlocks.GetLength(0); i++)
            {
                if (memoryBlocks[i, 0] != 0) { check = false; } //as long as one memory block with free space remains it's(the whole memory) not full!
            }
            return check;
        }

        /**********************************************************************
        *********************************************************************/
        public static bool FollowingFull()
        {
            bool check = true;
            //works different for strategy next fit
            if (strategy == "Next Fit" || strategy == "Combined Fit")
            {
                if (pos < suc)
                {
                    for (int i = pos + 1; i < suc; i++)
                    {
                        if (memoryBlocks[i, 0] != 0) { check = false; } //as long as one memory block with free space remains it's(the whole memory) not full!
                    }
                }
                else
                {
                    for (int i = pos + 1; i < memoryBlocks.GetLength(0); i++)
                    {
                        if (memoryBlocks[i, 0] != 0) { check = false; } //as long as one memory block with free space remains it's(the whole memory) not full!
                    }
                    for (int i = 0; i < suc; i++)
                    {
                        if (memoryBlocks[i, 0] != 0) { check = false; } //as long as one memory block with free space remains it's(the whole memory) not full!
                    }

                }

            }
            else
            {
                for (int i = pos + 1; i < memoryBlocks.GetLength(0); i++)
                {
                    if (memoryBlocks[i, 0] != 0) { check = false; } //as long as one memory block with free space remains it's(the whole memory) not full!
                }
            }
            return check;
        }

        /**********************************************************************
        *********************************************************************/
        public static void FirstFit(int memoryRequest)
        {
            //don't show previously drawn arrows
            ClearRedArrow();

            //if the memory block is already full
            if (memoryBlocks[pos, 0] == 0)
            {
                pos++;
                FirstFit(memoryRequest);
            }
            else //if there is still space left in the memory block
            {
                DrawRedArrow();
                OldAllocationStrategies.l_Free.Text = memoryBlocks[pos, 0].ToString();
                OldAllocationStrategies.l_Diff.Text = (memoryBlocks[pos, 0] - memoryRequest).ToString();

                //if the search was unsuccessfull
                if ((pos == memoryBlocks.GetLength(0) - 1 || FollowingFull()) && memoryRequest > memoryBlocks[pos, 0])
                {
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.unsuccessfull;

                }
                //if the search was successfull
                else if (memoryRequest <= memoryBlocks[pos, 0]) //if it fits ->successfull
                {
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
                }
            }

        }


        /**********************************************************************
        *********************************************************************/
        public static void NextFit(int memoryRequest)
        {

            //don't show previously drawn arrows
            ClearRedArrow();

            //&&for the special case suc was not set to the former successfull
            //but instead pos+1 position because it fit in perfectly
            if (FollowingFull() && memoryRequest > memoryBlocks[pos, 0])
            {
                DrawRedArrow();
                OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.unsuccessfull;
            }
            else if (memoryBlocks[pos, 0] == 0)
            {
                if (pos == memoryBlocks.GetLength(0) - 1)
                {
                    pos = 0;
                }
                else { pos++; }

                NextFit(memoryRequest);
            }
            else //if there is still space left in the memory block
            {
                DrawRedArrow();
                OldAllocationStrategies.l_Free.Text = memoryBlocks[pos, 0].ToString();
                OldAllocationStrategies.l_Diff.Text = (memoryBlocks[pos, 0] - memoryRequest).ToString();

                if (memoryRequest <= memoryBlocks[pos, 0]) //if it fits ->successfull
                {
                    //if also the end is reached and memory request fits perfectly
                    if ((pos == memoryBlocks.GetLength(0) - 1 && memoryRequest == memoryBlocks[pos, 0] || (FollowingFull()) && memoryRequest == memoryBlocks[pos, 0]))
                    {
                        int j = 0;
                        for (int i = 0; i < memoryBlocks.GetLength(0); i++)
                        {
                            if (memoryBlocks[i, 0] != 0)
                            {
                                j = i;
                                i = memoryBlocks.GetLength(0);
                            }
                        }
                        //suc = erstes freies Stück
                        suc = j;
                    }
                    //if it fits perfectly and the following block has space left
                    else if (memoryRequest == memoryBlocks[pos, 0])
                    {
                        suc = pos + 1;
                    }
                    else
                    {
                        suc = pos;
                    } //the latest successfull block, that was filled
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;

                }
                //if the end is reached and wasn't successfull start from the beginning
                else if (pos == memoryBlocks.GetLength(0) - 1)
                {
                    pos = -1; //-1 becuase pos++ in button click method in allocationstrategies
                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void BestFit(int memoryRequest)
        {
            //don't show previously drawn arrows
            ClearRedArrow();


            //if the memory block is already full 
            if (memoryBlocks[pos, 0] == 0)
            {
                pos++;
                BestFit(memoryRequest);

            }
            else //if there is still space left in the memory block
            {
                DrawRedArrow();
                OldAllocationStrategies.l_Free.Text = memoryBlocks[pos, 0].ToString();
                OldAllocationStrategies.l_Diff.Text = (memoryBlocks[pos, 0] - memoryRequest).ToString();

                //if it fits perfectly -> search was successfull
                if (memoryRequest == memoryBlocks[pos, 0])
                {
                    ClearGrayArrow();
                    besPos = pos;
                    besValue = memoryBlocks[pos, 0];
                    DrawGrayArrow();
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
                }
                else
                {
                    //if free space was found and it's bigger than the last free space that has been found 
                    if (memoryRequest <= memoryBlocks[pos, 0] && memoryBlocks[pos, 0] < besValue)
                    {
                        ClearGrayArrow();
                        besPos = pos;
                        besValue = memoryBlocks[pos, 0];
                        DrawGrayArrow();
                        OldAllocationStrategies.l_Best.Text = (besValue - memoryRequest).ToString();

                    }
                }
                //if also the end is reached or every follwoing block is already full 
                if (pos == memoryBlocks.GetLength(0) - 1 || FollowingFull())
                {
                    if (besValue == Int32.MaxValue)
                    {
                        OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.unsuccessfull;
                    }
                    else
                    {
                        OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
                        pos = besPos;
                    }

                }
            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void WorstFit(int memoryRequest)
        {
            // don't show previously drawn arrows

            ClearRedArrow();


            //if the memory block is already full 
            if (memoryBlocks[pos, 0] == 0)
            {
                pos++;
                WorstFit(memoryRequest);

            }
            else //if there is still space left in the memory block
            {
                DrawRedArrow();
                OldAllocationStrategies.l_Free.Text = memoryBlocks[pos, 0].ToString();
                OldAllocationStrategies.l_Diff.Text = (memoryBlocks[pos, 0] - memoryRequest).ToString();

                //if free space was found and it's bigger than the last free space that has been found 
                if (memoryRequest <= memoryBlocks[pos, 0] && memoryBlocks[pos, 0] > besValue)
                {
                    ClearGrayArrow();
                    besPos = pos;
                    besValue = memoryBlocks[pos, 0];
                    DrawGrayArrow();
                    OldAllocationStrategies.l_Best.Text = (besValue - memoryRequest).ToString();

                }
            }
            //if also the end is reached or every follwoing block is already full 
            if (pos == memoryBlocks.GetLength(0) - 1 || FollowingFull())
            {
                if (besValue == 0)
                {
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.unsuccessfull;
                }
                else
                {
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
                    pos = besPos;
                }

            }
        }

        /**********************************************************************
        *********************************************************************/
        public static void TailoringBestFit(int memoryRequest)
        {
            //don't show previously drawn arrows
            ClearRedArrow();


            //if the memory block is already full 
            if (memoryBlocks[pos, 0] == 0)
            {
                pos++;
                TailoringBestFit(memoryRequest);

            }
            else //if there is still space left in the memory block
            {
                DrawRedArrow();
                OldAllocationStrategies.l_Free.Text = memoryBlocks[pos, 0].ToString();
                OldAllocationStrategies.l_Diff.Text = (memoryBlocks[pos, 0] - memoryRequest).ToString();

                //if it fits perfectly -> search was successfull
                if (memoryRequest == memoryBlocks[pos, 0])
                {
                    ClearGrayArrow();
                    besPos = pos;
                    besValue = memoryBlocks[pos, 0];
                    DrawGrayArrow();
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
                }
                else
                {
                    //if free space was found and it's bigger than the last free space that has been found 
                    if (memoryRequest <= memoryBlocks[pos, 0] && memoryBlocks[pos, 0] > besValue)
                    {
                        ClearGrayArrow();
                        besPos = pos;
                        besValue = memoryBlocks[pos, 0];
                        DrawGrayArrow();
                        OldAllocationStrategies.l_Best.Text = (besValue - memoryRequest).ToString();

                    }
                }
                //if also the end is reached or every follwoing block is already full 
                if (pos == memoryBlocks.GetLength(0) - 1 || FollowingFull())
                {
                    if (besValue == 0)
                    {
                        OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.unsuccessfull;
                    }
                    else
                    {
                        OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
                        pos = besPos;
                    }

                }
            }
        }

        public static void CombinedFit(int memoryRequest)
        {
            if(indexSequenceCF.Any())
            {
                pos = indexSequenceCF.First();
                //don't show previously drawn arrows
                ClearRedArrow();

                DrawRedArrow();
                OldAllocationStrategies.l_Free.Text = memoryBlocks[pos, 0].ToString();
                OldAllocationStrategies.l_Diff.Text = (memoryBlocks[pos, 0] - memoryRequest).ToString();

                //if it fits perfectly -> search was successfull
                if (memoryRequest == memoryBlocks[pos, 0])
                {
                    Debug.WriteLine("fits perfectly!");

                    ClearGrayArrow();
                    besPos = pos;
                    besValue = memoryBlocks[pos, 0];
                    indexSequenceCF.Remove(indexSequenceCF.First());
                    indexCFLastAllocated = pos;
                    suc = indexCFLastAllocated;
                    DrawGrayArrow();
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;

                }
                //if free big enough space was found 
                else if (memoryRequest <= memoryBlocks[pos, 0] )
                {
                    //and it's bigger than the last free space that has been found 
                    if (memoryBlocks[pos, 0] > besValue)
                    {
                        Debug.WriteLine("best pos found!");
                        ClearGrayArrow();
                        besPos = pos;
                        besValue = memoryBlocks[pos, 0];
                        indexSequenceCF.Remove(indexSequenceCF.First());
                        DrawGrayArrow();
                        OldAllocationStrategies.l_Best.Text = (besValue - memoryRequest).ToString();
                    }
                    else
                    {
                        
                        Debug.WriteLine(memoryBlocks[pos, 0] + " unfortunately not bigger than " + besValue);

                        indexSequenceCF.Remove(indexSequenceCF.First());
                    }
                    

                }
                else {
                    Debug.WriteLine("Space not big enough!");

                    indexSequenceCF.Remove(indexSequenceCF.First());
                     }

                Debug.WriteLine("current pos = " + pos);
                Debug.Write("current best pos = " + besPos);
                Debug.WriteLine(" bes value = " + besValue);
                //Debug.WriteLine("suc = " + suc);
            }
            //if end of ring search is reached
            else
            {
                Debug.WriteLine("end of ring search reached!");

                if (besValue == 0)
                {
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.unsuccessfull;
                }
                else
                {

                    ClearRedArrow();
                    pos = besPos;
                    indexCFLastAllocated = pos;
                    OldAllocationStrategies.memoryRequestState = (WertheApp.OS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;

                    //DrawRedArrow();
                }
            }
            
           
        }

        /**********************************************************************
        *********************************************************************/
        public static void RequestNew(int memoryRequest)
        {

            switch (strategy)
            {
                case "First Fit":
                    pos = 0; //every new request begins at the very start
                    FirstFit(memoryRequest);
                    break;
                case "Next Fit":
                    pos = suc; // every new request starts where the previous successfull search ended
                    NextFit(memoryRequest);
                    break;
                case "Best Fit":
                    pos = 0; //every new request begins at the very start
                    if (memoryBlocks[0, 0] - memoryRequest >= 0)
                    {
                        besPos = 0;
                        besValue = memoryBlocks[0, 0];
                        OldAllocationStrategies.l_Best.Text = (memoryBlocks[0, 0] - memoryRequest).ToString();
                        ClearGrayArrow();
                        DrawGrayArrow();
                    }
                    //Int32.MaxValue since in BestFit(): if(memoryRequest <= memoryBlocks[pos, 0] && memoryBlocks[pos, 0] < besValue)
                    else { besPos = -1; besValue = Int32.MaxValue; OldAllocationStrategies.l_Best.Text = "-"; } //reset variables
                    BestFit(memoryRequest);
                    break;
                case "Worst Fit":
       
                    
                    pos = 0; //every new request begins at the very start
                    if (memoryBlocks[0, 0] - memoryRequest >= 0)
                    {
                        besPos = 0;
                        besValue = memoryBlocks[0, 0];
                        OldAllocationStrategies.l_Best.Text = (memoryBlocks[0, 0] - memoryRequest).ToString();
                        ClearGrayArrow();
                        DrawGrayArrow();
                    }
                    //Int32.MaxValue since in BestFit(): if(memoryRequest <= memoryBlocks[pos, 0] && memoryBlocks[pos, 0] < besValue)
                    else { besPos = -1; besValue = 0; OldAllocationStrategies.l_Best.Text = "-"; } //reset variables
                    WorstFit(memoryRequest);
                    break;
                    
                case "Tailoring Best Fit":
                    pos = 0; //every new request begins at the very start
                    if (memoryBlocks[0, 0] - memoryRequest >= 0)
                    {
                        besPos = 0;
                        besValue = memoryBlocks[0, 0];
                        OldAllocationStrategies.l_Best.Text = (memoryBlocks[0, 0] - memoryRequest).ToString();
                        ClearGrayArrow();
                        DrawGrayArrow();
                    }
                    //Int32.MaxValue since in BestFit(): if(memoryRequest <= memoryBlocks[pos, 0] && memoryBlocks[pos, 0] < besValue)
                    else { besPos = -1; besValue = 0; OldAllocationStrategies.l_Best.Text = "-"; } //reset variables
                    TailoringBestFit(memoryRequest);
                    break;


                case "Combined Fit":
                    
                    indexSequenceCF.Clear();
                    //create index sequence
                    for(int i = indexCFLastAllocated; i < numberOfFragments; i++)
                    {
                        indexSequenceCF.Add(i);
                    }
                    for(int i  = 0; i < indexCFLastAllocated; i++)
                    {
                        indexSequenceCF.Add(i);
                    }
                    // remove indexes where fragments are full
                    for(int i = 0; i < numberOfFragments; i++)
                    {
                        //if its full 
                        if (memoryBlocks[i, 0] == 0)
                        {
                            indexSequenceCF.Remove(i);//then remove
                        }
                    }
                    //print list
                    Debug.Write("Allocation Sequence: ");
                    foreach (int indexCF in indexSequenceCF)
                    {
                        Debug.Write(indexCF + ",");
                    }
                  
                    Debug.WriteLine("");

                    besPos = -1;
                    besValue = 0;
                    break;
                    //ClearGrayArrow();
                    //DrawGrayArrow();
                    //pos = indexSequenceCF.First();

                    CombinedFit(memoryRequest);
                    /*
                    //TODO: hier liegt der Fehler???
                    pos = suc; // every new request starts where the previous successfull search ended
                    if (memoryBlocks[0, 0] - memoryRequest >= 0)
                    {
                        besPos = suc;
                        besValue = memoryBlocks[0, 0];
                        OldAllocationStrategies.l_Best.Text = (memoryBlocks[0, 0] - memoryRequest).ToString();
                        ClearGrayArrow();
                        DrawGrayArrow();
                    }
                    //Int32.MaxValue since in BestFit(): if(memoryRequest <= memoryBlocks[pos, 0] && memoryBlocks[pos, 0] < besValue)
                    else { besPos = -1; besValue = 0; OldAllocationStrategies.l_Best.Text = "-"; } //reset variables
                    CombinedFit(memoryRequest);
                    
                    break;
                    */
            }
        }
    }
}

/*
public static void CombinedFit2(int memoryRequest)
{
    //TODO: Fehler bei Eingabe 4, 17, 2
    //Fehler bei 9, 11
    Debug.WriteLine("current pos = " + pos);
    Debug.WriteLine("current best pos = " + besPos);
    Debug.WriteLine("suc = " + suc);
    Debug.WriteLine("Flag = " + flag);

    //don't show previously drawn arrows
    ClearRedArrow();

    //&&for the special case suc was not set to the former successfull
    //but instead pos+1 position because it fit in perfectly
    if (FollowingFull() && memoryRequest > memoryBlocks[pos, 0])
    {
        if (besPos == -1)
        {
            Debug.WriteLine("1");

            DrawRedArrow();
            flag = false;
            OldAllocationStrategies.memoryRequestState = (WertheApp.BS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.unsuccessfull;
        }
        else
        {
            Debug.WriteLine("2");

            DrawRedArrow();
            Debug.WriteLine("best position " + besPos);
            pos = besPos;
            flag = false;
            OldAllocationStrategies.memoryRequestState = (WertheApp.BS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
        }

    }
    else if (memoryBlocks[pos, 0] == 0)
    {
        //ring search
        if (pos == memoryBlocks.GetLength(0) - 1)
        {
            Debug.WriteLine("3");

            pos = 0;
        }
        else
        {
            Debug.WriteLine("4");
            pos++;
        }

        CombinedFit(memoryRequest);
    }
    else //if there is still space left in the memory block
    {
        Debug.WriteLine("a");

        DrawRedArrow();
        OldAllocationStrategies.l_Free.Text = memoryBlocks[pos, 0].ToString();
        OldAllocationStrategies.l_Diff.Text = (memoryBlocks[pos, 0] - memoryRequest).ToString();

        if (memoryRequest <= memoryBlocks[pos, 0]) //if it fits ->successfull
        {
            //if also the end is reached and memory request fits perfectly
            if ((pos == memoryBlocks.GetLength(0) - 1 && memoryRequest == memoryBlocks[pos, 0] || (FollowingFull()) && memoryRequest == memoryBlocks[pos, 0]))
            {
                Debug.WriteLine("4");

                int j = 0;
                for (int i = 0; i < memoryBlocks.GetLength(0); i++)
                {
                    if (memoryBlocks[i, 0] != 0)
                    {
                        j = i;
                        i = memoryBlocks.GetLength(0);
                    }
                }
                //suc = erstes freies Stück
                suc = j;

                ClearGrayArrow();
                besPos = pos;
                besValue = memoryBlocks[pos, 0];
                DrawGrayArrow();
                flag = false;
                OldAllocationStrategies.memoryRequestState = (WertheApp.BS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
            }
            //if it fits perfectly and the following block has space left
            else if (memoryRequest == memoryBlocks[pos, 0])
            {
                Debug.WriteLine("5");

                suc = pos + 1;

                ClearGrayArrow();
                besPos = pos;
                besValue = memoryBlocks[pos, 0];
                DrawGrayArrow();
                flag = false;
                OldAllocationStrategies.memoryRequestState = (WertheApp.BS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
            }
            else
            {
                //if free space was found and it's bigger than the last free space that has been found 
                if (memoryRequest <= memoryBlocks[pos, 0] && memoryBlocks[pos, 0] > besValue)
                {
                    Debug.WriteLine("6");

                    ClearGrayArrow();
                    besPos = pos;
                    besValue = memoryBlocks[pos, 0];
                    DrawGrayArrow();
                    OldAllocationStrategies.l_Best.Text = (besValue - memoryRequest).ToString();

                }
                //if the end is reached and wasn't successfull start from the beginning
                else if (pos == suc && flag || FollowingFull())
                {
                    Debug.WriteLine("7");
                    //TODO
                    if (besValue == 0)
                    {
                        flag = false;
                        OldAllocationStrategies.memoryRequestState = (WertheApp.BS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.unsuccessfull;
                    }
                    else
                    {
                        ClearGrayArrow();
                        pos = besPos;
                        besValue = memoryBlocks[pos, 0];
                        DrawGrayArrow();
                        flag = false;
                        suc = pos;
                        OldAllocationStrategies.memoryRequestState = (WertheApp.BS.OldAllocationStrategies.MyEnum)OldAllocationStrategies.MyEnum.successfull;
                    }

                }
                else if (pos == memoryBlocks.GetLength(0) - 1)
                {
                    Debug.WriteLine("8");
                    Debug.WriteLine("end reached pos = -1");
                    pos = -1; //-1 becuase pos++ in button click method in allocationstrategies
                }
                else
                {
                    Debug.WriteLine("8");
                    flag = true;
                }


            }


        }
        //if the end is reached and wasn't successfull start from the beginning
        else if (pos == memoryBlocks.GetLength(0) - 1)
        {
            Debug.WriteLine("9");
            Debug.WriteLine("end reached pos = -1");
            pos = -1; //-1 becuase pos++ in button click method in allocationstrategies
        }
    }
}
*/