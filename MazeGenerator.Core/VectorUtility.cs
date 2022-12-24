using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using My.Randomizer;
using Maze.Extensions;

namespace Maze
{
    internal static class VectorUtility
    {
        private static readonly XorShift _rnd = new XorShift();

        public static readonly Vector2 Up = new Vector2(0, -1);
        public static readonly Vector2 Right = new Vector2(1, 0);
        public static readonly Vector2 Down = new Vector2(0, 1);
        public static readonly Vector2 Left = new Vector2(-1, 0);

        public static readonly Vector2[] Directions = new Vector2[] { Up, Right, Down, Left };
        public static readonly Vector2[] DirectionsWithoutUp = new Vector2[] { Right, Down, Left };

        public static Vector2[] GetRandomDirections() => (Vector2[])((Vector2[])Directions.Clone()).Shuffle(_rnd);
        public static Vector2[] GetRandomDirectionsWithoutUp() => (Vector2[])((Vector2[])DirectionsWithoutUp.Clone()).Shuffle(_rnd);
    }
}
