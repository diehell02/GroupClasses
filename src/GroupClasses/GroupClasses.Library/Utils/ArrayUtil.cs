using System;
using System.Collections.Generic;

namespace GroupClasses.Library.Utils
{
    class ArrayUtil
    {
        public static T[][] SplitArray<T>(T[] data, int size)
        {
            int length = data.Length / size;
            int remainder = data.Length % size;

            T[][] list = new T[length][];

            for (int i = 0; i < length; i++)
            {
                T[] r = null;

                if (i < remainder)
                {
                    r = new T[size + 1];
                }
                else
                {
                    r = new T[size];
                }

                Array.Copy(data, i * size, r, 0, size);
                list[i] = r;
            }

            if (remainder != 0)
            {
                T[] r = new T[remainder];
                Array.Copy(data, data.Length - remainder, r, 0, remainder);

                for (int i = 0; i < r.Length; i++)
                {
                    var item = list[i];

                    item[item.Length - 1] = r[i];
                }
            }

            return list;
        }

        public static List<T[]> SplitArray<T>(List<T> data, int size)
        {
            int length = data.Count / size;
            int remainder = data.Count % size;

            List<T[]> list = new List<T[]>(length);

            for (int i = 0; i < length; i++)
            {
                T[] r;
                if (i < remainder)
                {
                    r = new T[size + 1];
                }
                else
                {
                    r = new T[size];
                }

                data.CopyTo(i * size, r, 0, size);
                list.Add(r);
            }

            if (data.Count % size != 0)
            {
                T[] r = new T[remainder];
                data.CopyTo(data.Count - remainder, r, 0, remainder);

                for (int i = 0; i < r.Length; i++)
                {
                    var item = list[i];

                    item[item.Length - 1] = r[i];
                }
            }

            return list;
        }

        public static T[] MergeArray<T>(List<T[]> arraies)
        {
            List<T> list = new List<T>();
            foreach (T[] item in arraies)
            {
                for (int i = 0; i < item.Length; i++) list.Add(item[i]);
            }
            T[] r = new T[list.Count];
            int n = 0;
            foreach (T x in list)
            {
                r[n++] = x;
            }
            return r;
        }
    }
}
