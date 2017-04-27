using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace CaboodleES.Utils
{
    /// <summary>
    /// Collection of bits.
    /// </summary>
    public class BitMask
    {
        public int Count
        {
            get
            {
                return mask.Length * 8;
            }
        }

        private byte[] mask;

        public BitMask()
        {
            mask = new byte[4];
        }

        /// <summary>
        /// Sets a bit value in the array.
        /// </summary>
        /// <param name="i">position of bit.</param>
        /// <param name="value">value to set the bit too.</param>
        public void Set(int i, bool value)
        {
            if(i >= Count - 1)
                Grow(i);

            if (value)
                mask[i/8] = (byte)(mask[i / 8] | (1 << (i % 8)));
            else
                mask[i / 8] = (byte)(mask[i / 8] & ~(1 << (i % 8)));
        }

        /// <summary>
        /// Gets the bits value at the given position.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool Get(int i)
        {
            return ((mask[i / 8] & (1 << (i % 8))) != 0);
        }

        public void Clear()
        {
            mask = new byte[4];
        }

        public static bool operator ==(BitMask right, BitMask left)
        {
            //if (right.Count != left.Count)
            //    return false;

            for(int i = 0; i < right.Count && i < left.Count; i++)
                if (right.Get(i) != left.Get(i))
                    return false;
            
            return true;
        }

        public static bool operator !=(BitMask right, BitMask left)
        {
            return !(right == left);
        }

        public static bool operator >=(BitMask right, BitMask left)
        {
            for (int i = 0; i < left.Count; i++)
                if (left.Get(i) == true)
                    if(right.Get(i) != true)
                        return false;
            return true;
        }

        public static bool operator <=(BitMask right, BitMask left)
        {
            if (left.Count != right.Count)
                return false;

            for (int i = 0; i < right.Count; i++)
                if (right.Get(i) == true)
                    if (left.Get(i) != true)
                        return false;
            return true;
        }

        private void Grow(int min)
        {
            int mult = 2;
            while ((mask.Length * mult) < min)
                mult += 2;

            var oldMask = mask;
            mask = new byte[mask.Length * mult];
            Array.Copy(oldMask, mask, oldMask.Length);
        }
    }
}
