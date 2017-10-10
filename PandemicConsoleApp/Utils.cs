using System;
using System.Collections.Generic;
using System.Linq;

namespace PandemicConsoleApp
{
    static class Utils
    {
        public static int[] Shuffle(int[] deck)
        {
            var rnd = new Random();
            return deck.OrderBy(y => rnd.Next()).ToArray();
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int parts)
        {
            var returnList = new List<List<T>>();

            var stack = new Stack<T>(array);

            for (; parts > 0; parts--)
            {
                double range = stack.Count / parts;
                var rounded = Math.Round(range);

                var subList = new List<T>();
                for (var j = 0; j < rounded; j++)
                {
                    subList.Add(stack.Pop());
                }
                returnList.Add(subList);
            }

            return returnList;
        }

        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, Int32 size)
        {
            for (var i = 0; i < (source.Count / size) + (source.Count % size > 0 ? 1 : 0); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }
}