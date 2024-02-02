using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Utils {

    public static class ListUtils {

        public static T GetRandomElement<T>(this IReadOnlyList<T> source) {
            int count = source.Count;
            return count == 0 ? default : source[Random.Range(0, count)];
        }

    }

}
