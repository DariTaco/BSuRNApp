using System;
namespace WertheApp.OS.AllocationStrategies
{
    public class FragmentBlock
    {
        private int size;
        private int startIndex;
        private int endIndex;
        private bool free;


        public FragmentBlock(int p_size, bool p_free, int p_startIndex)
        {
            this.size = p_size;
            this.free = p_free;
            this.startIndex = p_startIndex;
            this.endIndex = this.startIndex + ( this.size - 1 );
            /* example: 
            |o|o|x|x|x|o|o|o|o|o|
             0 1 2 3 4 5 6 7 8 9

            size = 3
            free = true
            startIndex = 2
            endIndex = 2 + (3 - 1) = 4
             */
        }

        public int GetSize()
        {
            return this.size;
        }
        public void AddToSize(int addedSize)
        {
            this.size += addedSize;
            this.endIndex += addedSize;
        }
        public bool IsFree()
        {
            return this.free;
        }
        public int GetStartIndex()
        {
            return this.startIndex;
        }
        public int GetEndIndex()
        {
            return this.endIndex;
        }
        public void Use()
        {
            this.free = false;
        }
        public int Use(int p_size)
        {
            this.size = p_size;
            this.free = false;
            this.endIndex = this.startIndex + (p_size - 1);

            int newStartIndex = endIndex + 1;
            return newStartIndex;
        }
    }
}
