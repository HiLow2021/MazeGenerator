using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    public enum CellType
    {
        OutOfRange = -1,
        Sentinel,
        Wall,
        Road,
        Mark
    }
}
