using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using My.Randomizer;

namespace Maze.Extensions
{
    public static class IListExtensions
    {
        public static T ElementAtRandomly<T>(this IList<T> list, XorShift randomizer)
        {
            return list[randomizer.Next(list.Count)];
        }

        public static IList<T> Shuffle<T>(this IList<T> list, XorShift randomizer)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T tmp = list[i];
                int Index = randomizer.Next(i, list.Count);
                list[i] = list[Index];
                list[Index] = tmp;
            }

            return list;
        }
    }
}
