using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategiesAlgorithm
    {

        //VARIABLES
        private static List<int> freeFragmentsList; // list representing the free memory fragments - algorithm
        private static bool[] memoryArray; 
        private static String strategy; // chosen strategy for memory allocation
        private static int totalMemorySize; // sum of all fragments (free and used)

        //TODO: status. vll noch enum draus machen
        private static int status; //0-> initial memory frag, start, 1-> searching, 2->search succ, ...

        // algorithm
        private static int indexLastAllocated;

        //CONSTRUCTOR
        public AllocationStrategiesAlgorithm(String p_Strategy, List<int> p_FreeFragmentsList)
        {
            strategy = p_Strategy;
            freeFragmentsList = new List<int>(p_FreeFragmentsList); // copy List without reference to be able to alter it without affecting the original
      
            totalMemorySize = freeFragmentsList.Sum() + freeFragmentsList.Count - 1;
            memoryArray = CreateInitialMemoryArray(freeFragmentsList, totalMemorySize);
            //TODO: status.
            status = 0;
        }

        //METHODS
        public void Next()
        {
            switch (strategy)
            {
                case "First Fit":
                    FirstFit();
                    break;
                case "Next Fit":
                    NextFit();
                    break;
                case "Best Fit":
                    BestFit();
                    break;
                case "Worst Fit":
                    WorstFit();
                    break;
                case "Combined Fit":
                    CombinedFit();
                    break;
            }
        }
        public void Start(int p_MemoryRequest)
        {
            //TODO: status.
            status = 1;
            switch (strategy)
            {
                case "First Fit":
                    //TODO
                    break;
                case "Next Fit":
                    //TODO
                    break;
                case "Best Fit":
                    //TODO
                    break;
                case "Worst Fit":
                    //TODO
                    break;
                case "Combined Fit":
                    //TODO
                    break;
            }
        }
        public void FirstFit() { }
        public void NextFit() { }
        public void BestFit() { }
        public void WorstFit() { }
        public void CombinedFit() { }

        public int GetTotalMemorySize()
        {
            return totalMemorySize;
        }
        public List<int> GetFreeFragmentsList()
        {
            List<int> freeFragmentsListCopy = new List<int>(freeFragmentsList); // copy List without reference to be able to alter it without affecting the original
            return freeFragmentsListCopy;
        }
        /*public bool[] GetMemoryArray()
        {
            bool[] memoryArrayCopy = new List<bool>(memoryArray).ToArray();
            return memoryArrayCopy;
        }*/

        /**********************************************************************
        ***********************************************************************
        creates a List of all fragments (free and used) using the current memoryArray*/
        public List<FragmentBlock> CreateAllFragmentsList()
        {
            List<FragmentBlock> allFragmentsList = new List<FragmentBlock>(); 
            int fragmentBlockSize = 1;


            for (int i = 0; i < memoryArray.Length; i++)
            {
                //if last element
                if(i == memoryArray.Length - 1)
                {
                    allFragmentsList.Add(new FragmentBlock(fragmentBlockSize, memoryArray[i]));
                    fragmentBlockSize = 1;
                }
                else if(memoryArray[i] == memoryArray[i + 1])
                {
                    fragmentBlockSize++;
                }
                else
                {
                    allFragmentsList.Add(new FragmentBlock(fragmentBlockSize, memoryArray[i]));
                    fragmentBlockSize = 1;
                }
            }

            return allFragmentsList;
        }
        public int GetStatus()
        {
            return status;
        }

        /**********************************************************************
        ***********************************************************************
        IMPORTANT: Only call once in the beginning !!!! creates a bool array representing the memory from a list of free fragments.
        The memory will be the size of the free fragments(true)
        and the used (false) fragments // array representing the memory, size = free(true) and used(false) blocks - drawing*/
        private static bool[] CreateInitialMemoryArray(List<int> p_freeFragmentsList, int p_totalMemorySize)
        {
            //note: be careful and do not alter the list since it's not a copy
            bool[] memoryArray = new bool[p_totalMemorySize];
            int memoryIndex = 0;
            int fragmentsListItem = 1;
            int fragemntsListLastItem = p_freeFragmentsList.Count;
            foreach (int fragment in p_freeFragmentsList)
            {

                // add as many free blocks as the size of each fragment
                for(int i = 0; i < fragment; i++)
                {

                    memoryArray[memoryIndex] = true;
                    memoryIndex++;

                }
                // for every comma, add a used block
                if (fragmentsListItem != fragemntsListLastItem)
                {

                    memoryArray[memoryIndex] = false;
                    memoryIndex++;
                }
                fragmentsListItem++;
            }
            return memoryArray;
        }
    }
}

/*            String s = "memory: ";
            for(int i = 0; i < memoryArray.GetLength(0); i++)
            {

                s += memoryArray[i] + ",";
            }
            Debug.WriteLine(s);
*/