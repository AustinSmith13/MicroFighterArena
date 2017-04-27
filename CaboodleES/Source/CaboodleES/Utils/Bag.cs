using System;
using System.Collections;
using System.Collections.Generic;

namespace CaboodleES.Utils
{
    public class Bag<T> : IList<T>
    {
        private List<T> elements;
        private int next;

        public int Count { get { return next; } }

        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public Boolean IsFixedSize
        {
            get
            {
                return false;
            }
        }


        public Boolean IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Bag()
        {
            elements = new List<T>(32);
            next = 0;
        }

        
        public T this[int key]
        {
            get
            {
                return elements[key];
            }
            set
            {
                elements[key] = value;
            } 
        }

       // public void Grow()
       // {
        //    var old = elements;
        //    elements = new List<T>(elements.Count * 2);
        //    for(int i = 0; i < old.Count; i++)
        //        elements[i] = old[i];
            
       // }

        public void Clear()
        {
            elements = new List<T>(32);
            next = 0;
        }

        public void RemoveAt(Int32 index)
        {
            if (index >= next - 1)
            {
                elements[index] = default(T);
                next--;
                return;
            }

            var temp = elements[--next];
            elements[index] = temp;
        }

        public void CopyTo(Array array, Int32 index)
        {
            for(int i = index; i < array.Length && i < elements.Count; i++)
            {
                array.SetValue(elements[i], i);
            }
        }

        public Int32 IndexOf(T item)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (item.Equals(elements[i]))
                    return i;
            }
            return -1;
        }

        public void Insert(Int32 index, T item)
        {
            elements[index] = item;
        }

        public void Add(T item)
        {
            //if (next >= elements.Count)
            //    Grow();
            // elements[next++] =  item;
            elements.Add(item);
            next++;
        }

        public Boolean Contains(T item)
        {
            //for (int i = 0; i < elements.Count; i++)
            //{
            //    if (item.Equals(elements[i]))
            //        return true;
            // }
            //return false;
            return elements.Contains(item);
        }

        public void CopyTo(T[] array, Int32 arrayIndex)
        {
            elements.CopyTo(array, arrayIndex);
        }

        public Boolean Remove(T item)
        {
            Int32 i = this.IndexOf(item);
            if (i == -1) return false;
            RemoveAt(i);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return elements.GetEnumerator();
        }
    }
}
