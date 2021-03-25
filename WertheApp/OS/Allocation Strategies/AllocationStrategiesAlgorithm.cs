using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace WertheApp.OS.AllocationStrategies
{
    public class AllocationStrategiesAlgorithm
    {
        private static List<int> fragmentsList; // memory fragments
        private static String strategy; // chosen strategy for memory allocation
        private static int totalMemorySize; // sum of all fragments

        //TODO: status. vll noch enum draus machen
        private static int status; //0-> initial memory frag, start, 1-> searching, 2->search succ, ...

        // algorithm
        private static int indexLastAllocated;

        public AllocationStrategiesAlgorithm(String p_Strategy, List<int> p_FragmentsList)
        {
            strategy = p_Strategy;
            fragmentsList = p_FragmentsList;
            totalMemorySize = p_FragmentsList.Sum();

            //TODO: status.
            status = 0;
        }


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
        public List<int> GetFragmentsList()
        {
            return fragmentsList;
        }
        public int GetStatus()
        {
            return status;
        }
    }
}

