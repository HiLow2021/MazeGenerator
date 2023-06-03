using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.WinForms
{
    [Serializable]
    public class MyAppSettings
    {
        public static string ConfigPath { get; } = "config.dat";
        public static int DefaultPictureBoxWidth { get; } = 800;
        public static int DefaultPictureBoxHeight { get; } = 600;
        public static int DefaultPictureBoxMinWidth { get; } = 320;
        public static int DefaultPictureBoxMinHeight { get; } = 320;

        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsTopMost { get; set; } = false;
        public bool IsFixedWindowPosition { get; set; } = false;
        public bool IsFixedWindowSize { get; set; } = false;

        public bool IsDisplayAnswerRoute { get; set; } = false;
        public int MazeWidth { get; set; }
        public int MazeHeight { get; set; }

        public int MazeAlgorithmMethodType { get; set; }
        public int MazeGenerationMilliseconds { get; set; }
    }
}
