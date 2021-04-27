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
        public enum Status
        {
            start = 0,
            searching = 1,
            successfull = 2,
            unsuccessfull = 3,
            merged = 4,
            undefined = 9
        }
        private static Status status;
        // algorithm
        private static String strategy; // chosen strategy for memory allocation
        private static int totalMemorySize; // sum of all fragments (free and used)
        private static List<FragmentBlock> allFragmentsList; // list representing the free and used memory fragments - algorithm
        private static List<int> freeFragmentsIndexSequenceList;
        private static int currentIndex;
        private static int indexLastAllocated; // list index (all fragmentslist) of fragment which was allocated in the most recent successfull memory allocation
        private static int mostPromisingIndex; // most promising fragment (all fragmentslist) block index found by the algorithm so far
        private static int memoryRequest; // current memory request made by the user

        //CONSTRUCTOR
        public AllocationStrategiesAlgorithm(String p_Strategy, List<FragmentBlock> p_AllFragmentsList)
        {
            strategy = p_Strategy;
            status = Status.undefined; // no request made yet

            // copy List without reference to be able to alter it without affecting the original
            allFragmentsList = new List<FragmentBlock>();
            foreach (FragmentBlock p_fb in p_AllFragmentsList)
            {
                FragmentBlock fb = new FragmentBlock(p_fb.GetSize(), p_fb.IsFree(), p_fb.GetStartIndex());
                allFragmentsList.Add(fb);
            }
            freeFragmentsIndexSequenceList = new List<int>(); //empty list
            indexLastAllocated = 0; // start at the beginnning
            currentIndex = -1; // nothing yet
            mostPromisingIndex = -1; //nothing yet
            memoryRequest = -1; //nothing yet

            //calculate total memory size
            totalMemorySize = 0;
            foreach (FragmentBlock fb in allFragmentsList)
            {
                totalMemorySize += fb.GetSize();
            }
        }

        //METHODS
        public static void Next()
        {
            status = Status.searching;

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
        public static void Start(int p_MemoryRequest)
        {
            status = Status.start;
            mostPromisingIndex = -1;
            if(strategy == "Next Fit" || strategy == "Combined Fit")
            {
                currentIndex = indexLastAllocated;
            }
            else
            {
                indexLastAllocated = 0; // start at the beginnning
                currentIndex = -1; // nothing yet
            }
            memoryRequest = p_MemoryRequest;
            UpdateFreeFragmentsIndexSequenceList(allFragmentsList);

        }

        /**********************************************************************
        ***********************************************************************/
        public static void FirstFit()
        {
            // end of search not yet reached
            if (freeFragmentsIndexSequenceList.Any())
            {

                currentIndex = freeFragmentsIndexSequenceList.First();
                Debug.WriteLine("current index: " + currentIndex);

                FragmentBlock fb = allFragmentsList.ElementAt(currentIndex);
                //if it fits perfectly or but free big enough space was found-> search was successfull
                if (memoryRequest == fb.GetSize() || memoryRequest <= fb.GetSize())
                {
                    Debug.WriteLine("fits!");
                    indexLastAllocated = allFragmentsList.IndexOf(fb);
                    status = Status.successfull;
                }
                // if it does not fit at all
                else
                {
                    Debug.WriteLine("Space not big enough!");
                }
                freeFragmentsIndexSequenceList.RemoveAt(0);
            }
            // end of ring search reached
            else
            {
                Debug.WriteLine("end of search reached!");

                // nothing found -> search was unsuccessfull
                status = Status.unsuccessfull;
            }
        }
        /**********************************************************************
        ***********************************************************************/
        public static void NextFit()
        {
            FirstFit();
        }
        /**********************************************************************
        ***********************************************************************/
        public static void BestFit()
        {
            Debug.WriteLine("BEST FIT");

            // end of search not yet reached
            if (freeFragmentsIndexSequenceList.Any())
            {

                currentIndex = freeFragmentsIndexSequenceList.First();
                Debug.WriteLine("current index: " + currentIndex);

                FragmentBlock fb = allFragmentsList.ElementAt(currentIndex);
                //if it fits perfectly -> search was successfull
                if (memoryRequest == fb.GetSize())
                {
                    Debug.WriteLine("fits perfectly!");
                    mostPromisingIndex = allFragmentsList.IndexOf(fb);
                    indexLastAllocated = mostPromisingIndex;
                    status = Status.successfull;
                }
                //if it doesn't fit perfectly but free big enough space was found
                else if (memoryRequest <= fb.GetSize())
                {
                    int mostPromisingSize = 99999;
                    if (mostPromisingIndex != -1) { mostPromisingSize = allFragmentsList.ElementAt(mostPromisingIndex).GetSize(); }

                    // and it's smaller than the last free space that has been found 
                    if (fb.GetSize() < mostPromisingSize)
                    {
                        Debug.WriteLine(fb.GetSize() + " smaller than " + mostPromisingSize);
                        mostPromisingIndex = allFragmentsList.IndexOf(fb);
                    }
                    // but it's not smaller
                    else
                    {
                        Debug.WriteLine(fb.GetSize() + " unfortunately not smaller than " + allFragmentsList.ElementAt(mostPromisingIndex).GetSize());
                    }
                }
                // if it does not fit at all
                else
                {
                    Debug.WriteLine("Space not big enough!");
                }
                freeFragmentsIndexSequenceList.RemoveAt(0);
            }
            // end of ring search reached
            else
            {
                Debug.WriteLine("end of search reached!");

                // nothing found -> search was unsuccessfull
                if (mostPromisingIndex == -1)
                {
                    status = Status.unsuccessfull;
                }
                // fragment block found -> search was successfull
                else
                {
                    indexLastAllocated = mostPromisingIndex;
                    currentIndex = mostPromisingIndex;
                    status = Status.successfull;
                }
            }
        }
        /**********************************************************************
        ***********************************************************************/
        public static void WorstFit()
        {
            Debug.WriteLine("WORST FIT");

            // end of search not yet reached
            if (freeFragmentsIndexSequenceList.Any())
            {

                currentIndex = freeFragmentsIndexSequenceList.First();
                Debug.WriteLine("current index: " + currentIndex);

                FragmentBlock fb = allFragmentsList.ElementAt(currentIndex);

                //if free big enough space was found
                if (memoryRequest <= fb.GetSize())
                {
                    int mostPromisingSize = 0;
                    if (mostPromisingIndex != -1) { mostPromisingSize = allFragmentsList.ElementAt(mostPromisingIndex).GetSize(); }

                    // and it's bigger than the last free space that has been found 
                    if (fb.GetSize() > mostPromisingSize)
                    {
                        Debug.WriteLine(fb.GetSize() + " bigger than " + mostPromisingSize);
                        mostPromisingIndex = allFragmentsList.IndexOf(fb);
                    }
                    // but it's not bigger
                    else
                    {
                        Debug.WriteLine(fb.GetSize() + " unfortunately not bigger than " + allFragmentsList.ElementAt(mostPromisingIndex).GetSize());
                    }
                }
                // if it does not fit at all
                else
                {
                    Debug.WriteLine("Space not big enough!");
                }
                freeFragmentsIndexSequenceList.RemoveAt(0);
            }
            // end of ring search reached
            else
            {
                Debug.WriteLine("end of search reached!");

                // nothing found -> search was unsuccessfull
                if (mostPromisingIndex == -1)
                {
                    status = Status.unsuccessfull;
                }
                // fragment block found -> search was successfull
                else
                {
                    indexLastAllocated = mostPromisingIndex;
                    currentIndex = mostPromisingIndex;
                    status = Status.successfull;
                }
            }
        }

        /**********************************************************************
        ***********************************************************************/
        public static void CombinedFit()
        {
            Debug.WriteLine("COMBINED FIT");

            // end of ring search not yet reached
            if (freeFragmentsIndexSequenceList.Any())
            {

                currentIndex = freeFragmentsIndexSequenceList.First();
                Debug.WriteLine("current index: " + currentIndex);

                FragmentBlock fb = allFragmentsList.ElementAt(currentIndex);
                //if it fits perfectly -> search was successfull
                if (memoryRequest == fb.GetSize())
                {
                    Debug.WriteLine("fits perfectly!");
                    mostPromisingIndex = allFragmentsList.IndexOf(fb);
                    indexLastAllocated = mostPromisingIndex;
                    status = Status.successfull;
                }
                //if it doesn't fit perfectly but free big enough space was found
                else if (memoryRequest <= fb.GetSize())
                {
                    int mostPromisingSize = 0;
                    if (mostPromisingIndex != -1) { mostPromisingSize = allFragmentsList.ElementAt(mostPromisingIndex).GetSize(); }

                    // and it's bigger than the last free space that has been found 
                    if (fb.GetSize() > mostPromisingSize)
                    {
                        Debug.WriteLine(fb.GetSize() + " bigger than " + mostPromisingSize);
                        mostPromisingIndex = allFragmentsList.IndexOf(fb);
                    }
                    // but it's not bigger
                    else
                    {
                        Debug.WriteLine(fb.GetSize() + " unfortunately not bigger than " + allFragmentsList.ElementAt(mostPromisingIndex).GetSize());
                    }
                }
                // if it does not fit at all
                else
                {
                    Debug.WriteLine("Space not big enough!");
                }
                freeFragmentsIndexSequenceList.RemoveAt(0);
            }
            // end of ring search reached
            else
            {
                Debug.WriteLine("end of ring search reached!");

                // nothing found -> search was unsuccessfull
                if (mostPromisingIndex == -1)
                {
                    status = Status.unsuccessfull;
                }
                // fragment block found -> search was successfull
                else
                {
                    indexLastAllocated = mostPromisingIndex;
                    currentIndex = mostPromisingIndex;
                    status = Status.successfull;
                }
            }
        }
        /**********************************************************************
        ***********************************************************************/
        public static void UpdateAllFragmentsList()
        {
            Debug.WriteLine("UPDATE ALL FRAGMENTSLIST");
            Debug.WriteLine("old fragemnts list");
            mostPromisingIndex = -1;
            foreach (FragmentBlock fb in allFragmentsList)
            {
                Debug.Write(" |" + fb.IsFree() + " " + fb.GetSize());
            }
            FragmentBlock allocatedFb = allFragmentsList.ElementAt(indexLastAllocated);
            int remainingFragmentSize = allocatedFb.GetSize() - memoryRequest;

            if(remainingFragmentSize == 0)
            {
                allFragmentsList.ElementAt(indexLastAllocated).Use();
            }
            else
            {
                int startIndex = allFragmentsList.ElementAt(indexLastAllocated).Use(memoryRequest);
                FragmentBlock newFb = new FragmentBlock(remainingFragmentSize, true, startIndex);
                allFragmentsList.Insert(indexLastAllocated + 1, newFb);
            }
            Debug.WriteLine("");
            Debug.WriteLine("new fragemnts list");
            foreach(FragmentBlock fb in allFragmentsList)
            {
                Debug.Write(" |" + fb.IsFree() + " " + fb.GetSize());
            }

            MergeUsedFragments(0);
            Debug.WriteLine("");
            Debug.WriteLine("merged fragemnts list");
            foreach (FragmentBlock fb in allFragmentsList)
            {
                Debug.Write(" |" + fb.IsFree() + " " + fb.GetSize());
            }
        }
        /**********************************************************************
        ***********************************************************************/
        public static void MergeUsedFragments(int index)
        {
            int i = index;
            if(i >= allFragmentsList.Count())
            {
                status = Status.merged;
            }
            else if(allFragmentsList.ElementAt(i).IsFree())
            {
                i++;
                MergeUsedFragments(i);
            }
            else
            {
                FragmentBlock savedFragmentBlock = allFragmentsList.ElementAt(i);
                i++;
                //if two or more used fragments are next to each other add the sizes of the following block to the leading block and then delete them
                    while (i < allFragmentsList.Count() && !allFragmentsList.ElementAt(i).IsFree())
                    {
                        savedFragmentBlock.AddToSize(allFragmentsList.ElementAt(i).GetSize());
                        allFragmentsList.RemoveAt(i);
                    }

                    MergeUsedFragments(i);
            }
        }

        /**********************************************************************
        ***********************************************************************
        creates a list of all free fragment indexes in the order they are going to be processed by the algorithm*/
        public static List<int> UpdateFreeFragmentsIndexSequenceList(List<FragmentBlock> p_AllFragmentsList)
        {
            int numberOfFragments = p_AllFragmentsList.Count();
            freeFragmentsIndexSequenceList.Clear();


            // go from index i to end of list
            for (int i = indexLastAllocated; i < numberOfFragments; i++)
            {
                FragmentBlock fb = p_AllFragmentsList.ElementAt(i);
                if (fb.IsFree())
                {
                    freeFragmentsIndexSequenceList.Add(i);
                }
            }
            // go from start of list to index i
            for (int i = 0; i < indexLastAllocated; i++)
            {
                FragmentBlock fb = p_AllFragmentsList.ElementAt(i);
                if (fb.IsFree())
                {
                    freeFragmentsIndexSequenceList.Add(i);
                }
            }

            return freeFragmentsIndexSequenceList;
        }


        /**********************************************************************
        ***********************************************************************/
        public static int GetTotalMemorySize()
        {
            return totalMemorySize;
        }

        public static Status GetStatus()
        {
            return status;
        }

        public static int GetMostPromisingIndex()
        {
            return mostPromisingIndex;
        }

        public static int GetCurrentIndex()
        {
            return currentIndex;
        }
        public static List<FragmentBlock> GetAllFragmentsList()
        {
            return allFragmentsList;
        }

        public static bool MemoryIsFull()
        {
            bool isFull = true;
            foreach (FragmentBlock fb in allFragmentsList)
            {
                if (fb.IsFree()) { isFull = false; }
            }
            return isFull;
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

/*public bool[] GetMemoryArray()
{
    bool[] memoryArrayCopy = new List<bool>(memoryArray).ToArray();
    return memoryArrayCopy;
}*/

/**********************************************************************
***********************************************************************
IMPORTANT: Only call once in the beginning !!!! creates a bool array representing the memory from a list of free fragments.
The memory will be the size of the free fragments(true)
and the used (false) fragments // array representing the memory, size = free(true) and used(false) blocks - drawing
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
}*/


/**********************************************************************
 ************************************************************************
       creates a List of all fragments (free and used) using the current memoryArray
public List<FragmentBlock> CreateAllFragmentsList()
{
    List<FragmentBlock> allFragmentsList = new List<FragmentBlock>();
    int fragmentBlockSize = 1;


    for (int i = 0; i < memoryArray.Length; i++)
    {
        //if last element
        if (i == memoryArray.Length - 1)
        {
            allFragmentsList.Add(new FragmentBlock(fragmentBlockSize, memoryArray[i]));
            fragmentBlockSize = 1;
        }
        else if (memoryArray[i] == memoryArray[i + 1])
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
*/