using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Utility.WPF.Panels;


namespace Utility.Structs
{
    public struct Index : IIndex
    {
        private readonly int[] collection;
        public Index(params int[] indexes)
        {
            collection = indexes;
        }

        public int? this[int key]
        {
            get
            {
                if (collection.Length > key)
                    return collection[key];
                return null;
            }
        }

        int IReadOnlyList<int>.this[int index] => collection[index];

        public bool IsEmpty => collection.Length == 0;

        public int Local => collection.Last();

        public int Count => collection.Length;

        public int CompareTo(IIndex? other)
        {
            if (other == null) return 1;

            int minLength = Math.Min(this.Count, other.Count);
            for (int i = 0; i < minLength; i++)
            {
                int comparison = collection[i].CompareTo(other[i]);
                if (comparison != 0) return comparison;
            }
            return this.Count.CompareTo(other.Count);
        }

        public int CompareTo(object? obj)
        {
            if (!(obj is Index index)) return 1;
            return CompareTo(index);
        }

        public bool Equals(IIndex? other)
        {
            if (this.Count != other?.Count)
                return false;
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] != other?[i]) return false;
            }
            return true;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return collection.AsEnumerable().GetEnumerator();
        }

        public static explicit operator Index(int[] b) => new Index(b);

        public static explicit operator Index(string b) => new Index(ParseKeyToPath(b));

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (collection.Length == 0)
                return string.Empty;
            foreach (var item in collection)
            {
                stringBuilder.Append(item);
                stringBuilder.Append('.');
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            return stringBuilder.ToString();
        }
        public static int[] ParseKeyToPath(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return Array.Empty<int>();

            try
            {
                return key.Split('.').Select(part => int.Parse(part.Trim(), CultureInfo.InvariantCulture)).ToArray();
            }
            catch
            {
                return Array.Empty<int>();
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}