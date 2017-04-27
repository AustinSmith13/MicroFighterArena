using System;
using System.Collections;
using System.Collections.Generic;

namespace CaboodleES.Utils
{
    /// <summary>
    /// Table collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Table<T>
    {
        private T[] elements;
        private int count;

        public int Count { get { return count; } }

        public Table()
        {
            elements = new T[32];
            this.count = 0;
        }

        public T Get(int i)
        {
            if (i > elements.Length - 1) return default(T);
            return elements[i];
        }

        public void Set(int i, T element)
        {
            if(i > elements.Length - 1)
            {
                Grow(i);
            }

            count++;
            elements[i] = element;
        }

        public bool Has(int i)
        {
            if (i >= elements.Length) return false;
            return elements[i] != null;
        }

        public void Remove(int i)
        {
            if (elements[i] != null)
            {
                elements[i] = default(T);
                count--;
            }
        }

        public void Clear()
        {
            elements = new T[32];
            count = 0;
        }

        private void Grow(int min)
        {
            int mult = 2;
            while((elements.Length * mult) < min)
                mult += 2;
            
            var old = elements;
            elements = new T[elements.Length * mult];
            Array.Copy(old, elements, old.Length);
        }
    }
}
