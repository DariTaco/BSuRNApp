using System;
namespace WertheApp.BS
{
    public class BuddySystemBlock
    {
        //VARIABLES
        private bool free;
        private int blockSize;
        private String processName;
        private int processSize;

        //CONTRUCTOR
        public BuddySystemBlock(int bsize)
        {
            free = true;
            blockSize = bsize;
            processName = null;
            processSize = 0;
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
    }
}
