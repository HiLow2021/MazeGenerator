using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Maze.Extensions;

namespace Maze.Algorithm
{
    public class Cluster : MazeGenerator
    {
        private readonly IList<Vector2> startPositions = new List<Vector2>();
        private readonly IList<int> cluster = new List<int>();

        public Cluster(Layer layer, bool isAnswerUsed = false) : base(layer, isAnswerUsed) { }

        public override async Task Start()
        {
            if (IsRunning)
            {
                return;
            }

            try
            {
                IsRunning = true;
                _TokenSource = new CancellationTokenSource();
                _Token = _TokenSource.Token;

                Initialize();
                await DoInterval();
                await Generate();

                if (!IsAnswerUsed)
                {
                    return;
                }

                await DoInterval();
                SetEntrance(out Vector2 e1, out Vector2 e2);
                await SearchAnswer(e1, e2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async Task Generate()
        {
            var i = 0;
            var progress = 1;

            do
            {
                var position = startPositions[i++];

                foreach (var item in VectorUtility.GetRandomDirections())
                {
                    var candidate = position + item;

                    if (Layer.Get(candidate) == CellType.Wall)
                    {
                        var index1 = cluster[GetClusterIndex(position)];
                        var index2 = cluster[GetClusterIndex(position + item * 2)];
                        var id1 = GetClusterId(index1);
                        var id2 = GetClusterId(index2);

                        if (id1 != id2)
                        {
                            await DoInterval();
                            Layer.Set(candidate, CellType.Road);
                            RaiseMazeGenerationProgressChanged(progress++, startPositions.Count);

                            if (id1 < id2)
                            {
                                cluster[id2] = id1;
                            }
                            else
                            {
                                cluster[id1] = id2;
                            }
                        }
                    }
                }
            }
            while (i < startPositions.Count);

            RaiseMazeGenerationCompleted(i, startPositions.Count);
        }

        private int GetClusterIndex(Vector2 position)
        {
            var x = (int)position.X / 2;
            var y = (int)position.Y / 2;
            var w = Layer.Width / 2;

            return y * w + x;
        }

        private int GetClusterId(int i)
        {
            while (i != cluster[i])
            {
                i = cluster[i];
            }

            return i;
        }

        private void Initialize()
        {
            Layer.FillValues(CellType.Wall);
            Layer.BorderValues(CellType.Sentinel);

            startPositions.Clear();
            cluster.Clear();

            var i = 0;

            for (int y = 1; y < Layer.Height - 1; y += 2)
            {
                for (int x = 1; x < Layer.Width - 1; x += 2)
                {
                    Layer.Set(new Vector2(x, y), CellType.Road);
                    startPositions.Add(new Vector2(x, y));
                    cluster.Add(i++);
                }
            }

            startPositions.Shuffle(_rnd);
        }
    }
}
