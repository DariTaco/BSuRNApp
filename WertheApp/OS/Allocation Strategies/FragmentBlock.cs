using System;
namespace WertheApp.OS.AllocationStrategies
{
    public class FragmentBlock
    {
        private int size;
        private bool free;

        public FragmentBlock(int p_size, bool p_free)
        {
            this.size = p_size;
            this.free = p_free;
        }

        public int GetSize()
        {
            return this.size;
        }
        public bool IsFree()
        {
            return this.free;
        }
    }
}
