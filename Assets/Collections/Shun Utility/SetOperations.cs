using System.Collections.Generic;
using System.Linq;

namespace Collections.Shun_Utility
{
    public static class SetOperations
    {
        // Union function to return the union of two lists
        public static List<T> Union<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            return list1.Union(list2).ToList();
        }

        // Set difference function to return the elements that are in list1 but not in list2
        public static List<T> SetDifference<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            return list1.Except(list2).ToList();
        }

        // Intersection function to return the common elements in both lists
        public static List<T> Intersection<T>(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            return list1.Intersect(list2).ToList();
        }
    }
}