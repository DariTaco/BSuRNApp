using System;
using System.Collections.Generic;

namespace WertheApp.BS
{
    public class BuddySystemBlock
    {
        //VARIABLES
        private bool free;
        private int blockSize;
        private String processName;
        private int processSize;
        private int buddyNo; //indicates if block is the first(1) or second(2) buddy or has no buddy(0)
        private List<int> buddyNoList; //-1 means no parent

        //CONTRUCTOR
        public BuddySystemBlock(int bSize, int bNo)
        {
            this.free = true;
            this.blockSize = bSize;
            this.buddyNo = bNo;
            //if very first block without buddy
            if(bNo == 0){
                this.buddyNoList = new List<int>();
                this.buddyNoList.Add(bNo);
            }
            this.processName = null;
            this.processSize = 0;
        }

        //METHODS
        public void OccupyBlock(String pname, int psize){
            this.free = false;
            this.processName = pname;
            this.processSize = psize;
        }

        public void FreeBlock(){
            this.free = true;
            this.processName = null;
            this.processSize = 0;
        }

        public bool GetFree(){
            return this.free;
        }

        public int GetBlockSize(){
            return this.blockSize;
        }

        public int GetProcessSize(){
            return this.processSize;
        }

        public String GetProcessName(){
            return this.processName;
        }

        public int GetBuddyNo(){
            return this.buddyNo;
        }

        public List<int> GetBuddyNoList(){
            return this.buddyNoList;
        }

        public void SetBuddyNoList(List<int> l){
            this.buddyNoList = new List<int>(l);
            this.buddyNoList.Add(this.GetBuddyNo());
        }

        public void PrintBuddyNoList()
        {
            //Debug.WriteLine("############## Buddy No List");
            String s = "";
            for (int i = 0; i < this.buddyNoList.Count; i++)
            {
                String value = this.buddyNoList[i].ToString();
                s += value + " ";
            }
            //Debug.WriteLine(s);
        }
    }
}