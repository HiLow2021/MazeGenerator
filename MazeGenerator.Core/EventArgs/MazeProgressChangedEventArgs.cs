using System;
using System.Collections.Generic;
using System.Text;

namespace Maze.EventArgs
{
    public class MazeProgressChangedEventArgs : MazeEventArgs
    {
        public int Progress { get; }
        public int Total { get; }
        public float Percentage => (float)Progress / Total;

        public MazeProgressChangedEventArgs(int progress, int total)
        {
            Progress = progress;
            Total = total;
        }
    }
}
