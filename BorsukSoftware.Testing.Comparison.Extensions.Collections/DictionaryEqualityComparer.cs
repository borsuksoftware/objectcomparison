using System;
using System.Collections.Generic;
using System.Text;

namespace BorsukSoftware.Testing.Comparison.Extensions.Collections
{
    /// <summary>
    /// Class used to act as the equality comparer for using dictionaries as keys for other dictionaries
    /// </summary>
    internal class DictionaryEqualityComparer : IEqualityComparer<IReadOnlyDictionary<string, object>>
    {
        public bool Equals(IReadOnlyDictionary<string, object> x, IReadOnlyDictionary<string, object> y)
        {
            if (x == null && y == null)
                return true;

            if (x == null || y == null || x.Count != y.Count)
                return false;

            foreach (var xPair in x)
            {
                if (!y.TryGetValue(xPair.Key, out var yValue))
                    return false;

                if (xPair.Value == null && yValue == null)
                    continue;

                if (xPair.Value == null || yValue == null)
                    return false;

                if (!xPair.Value.Equals(yValue))
                    return false;
            }

            return true;
        }

        public int GetHashCode(IReadOnlyDictionary<string, object> obj)
        {
            int hashCode = 0;
            foreach (var pair in obj)
            {
                hashCode ^= pair.Key.GetHashCode();
                if (pair.Value != null)
                    hashCode ^= pair.Value.GetHashCode();
            }

            return hashCode;
        }
    }
}
