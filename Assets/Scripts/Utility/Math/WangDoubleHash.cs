/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * The Initial Developer of the Original Code is Rune Skovbo Johansen.
 * Portions created by the Initial Developer are Copyright (C) 2015
 * the Initial Developer. All Rights Reserved.
 */

using System;

namespace Extensions
{
    public class WangDoubleHash : HashFunction
    {
        private uint seed;

        public WangDoubleHash(int seedValue)
        {
            seed = GetHashOfInt((uint)seedValue);
        }

        public override uint GetHash(int data)
        {
            return GetHashOfInt(seed ^ (uint)data);
        }

		public override uint GetHash(int x, int y)
        {
            uint result = GetHashOfInt(seed ^ (uint)x);
            return GetHashOfInt(result ^ (uint)y);
        }

        public override uint GetHash(params int[] data)
        {
            uint val = seed;
            for (int i = 0; i < data.Length; i++)
                val = GetHashOfInt(val ^ (uint)data[i]);
            return val;
        }

        private uint GetHashOfInt(uint data)
        {
            uint val = data;

            // Based on Thomas Wangâ€™s integer hash functions.
            // Applied twice.

            val = (val ^ 61) ^ (val >> 16);
            val *= 9;
            val = val ^ (val >> 4);
            val *= 0x27d4eb2d;
            val = val ^ (val >> 15);

            val = (val ^ 61) ^ (val >> 16);
            val *= 9;
            val = val ^ (val >> 4);
            val *= 0x27d4eb2d;
            val = val ^ (val >> 15);

            return val;
        }
    };
}
