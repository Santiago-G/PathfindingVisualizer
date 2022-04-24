using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathfindingVisualizer
{
    public class BinaryHeap<T>
    {
        public T[] array = new T[0];
        public int count { get; private set; }
        IComparer<T> Comparer;
        //Use IComparer
        //Pass in the comparer and have the comparer be based off final distance

        public BinaryHeap(IComparer<T> comparer)
        {
            Comparer = comparer;
        }

        public void Insert(T value)
        {
            if (array.Length == 0)
            {
                T[] newArray = new T[array.Length + 1];
                newArray[0] = value;

                array = newArray;
                count++;
            }
            else if (!DupeChecker(value))
            {
                count++;

                if (count >= array.Length)
                {
                    T[] newArray = new T[array.Length * 2];
                    Array.Copy(array, newArray, array.Length);

                    array = newArray;
                }

                array[count - 1] = value;

                HeapifyUp(count - 1);
            }
        }

        int childIndex(int index, bool rightChild)
        {
            if (rightChild)
            {
                return index * 2 + 2;
            }

            return index * 2 + 1;
        }

        int parentIndex(int index)
        {
            return (index - 1) / 2;
        }

        public T Pop()
        {
            if (count == 0)
            {
                throw new Exception("No items to pop.");
            }

            T rootVal = array[0];

            array[0] = array[count - 1];
            count--;

            HeapifyDown(0);

            return rootVal;
        }

        void HeapifyUp(int index)
        {
            //index * 2 
            if (index == 0)
            {
                return;
            }


            int parIndex = parentIndex(index);
            T temp;

            if (Comparer.Compare(array[parIndex], array[index]) > 0)
            {
                temp = array[parIndex];

                array[parIndex] = array[index];
                array[index] = temp;
            }
            else
            {
                return;
            }
            HeapifyUp(parIndex);
        }

        void HeapifyDown(int index)
        {
            if (index >= count)
            {
                return;
            }

            int rightChildIndex = childIndex(index, true);
            int leftChildIndex = childIndex(index, false);

            int theChildIndex;
            T temp;

            if (rightChildIndex >= count && leftChildIndex >= count)
            {
                return;
            }
            else
            {
                if (leftChildIndex < count && rightChildIndex < count)
                {
                    //normal

                    

                    if (Comparer.Compare(array[leftChildIndex], array[rightChildIndex]) > 0) //array[leftChildIndex].CompareTo(array[rightChildIndex]) > 0
                    {
                        theChildIndex = rightChildIndex;
                    }
                    else
                    {
                        theChildIndex = leftChildIndex;
                    }
                }
                else if (leftChildIndex >= count)
                {
                    theChildIndex = rightChildIndex;
                }
                else
                {
                    theChildIndex = leftChildIndex;
                }
            }

            if (Comparer.Compare(array[theChildIndex], array[index]) < 0) //array[theChildIndex].CompareTo(array[index]) < 0
            {
                temp = array[index];

                array[index] = array[theChildIndex];
                array[theChildIndex] = temp;
            }
            else
            {
                return;
            }

            HeapifyDown(theChildIndex);
        }

        public bool DupeChecker(T val)
        {
            foreach (var item in array)
            {
                if (item == null)
                {
                    return false;
                }

                if (item.Equals(val))
                {
                    return true;
                }
            }

            return false;
            //dumb dumb heap tree i never want to use these again
        }

        public void Clear()
        {
            array = new T[0];
            count = 0;
        }

        public void TestHeap()
        {
          //  BinaryHeap<int> b = new BinaryHeap<int>()
        }
    }
}
