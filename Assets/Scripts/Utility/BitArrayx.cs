using UnityEngine;
using System.Collections;

namespace Extensions
{
    public static class BitArrayx
    {
        const int k_minSize = 1024;
        static int[] _worker;
        static int[] worker
        {
            get
            {
                if (_worker == null)
                {
                    _worker = new int[k_minSize];
                }
                return _worker;
            }
        }

        public static bool Any(this BitArray ba)
        {
            ba.CopyTo(worker, 0);
            int end = ba.Count / 32;
            int shift = 32 - ba.Count % 32;
            worker[end] = worker[end] >> shift << shift;
            int result = 0;
            for (int i = 0; i < end; i++)
            {
                result = result & worker[i];
            }
            return result != 0;
        }

    }
}