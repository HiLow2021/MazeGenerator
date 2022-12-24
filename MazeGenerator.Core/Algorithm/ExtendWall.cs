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
    public class ExtendWall : MazeGenerator
    {
        private readonly IList<Vector2> startPositions = new List<Vector2>();

        public ExtendWall(Layer layer, bool isAnswerUsed = false) : base(layer, isAnswerUsed) { }

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
            var currentRoutes = new List<Vector2>();

            do
            {
                var position = startPositions[i++];
                currentRoutes.Clear();

                if (Layer.Get(position) != CellType.Sentinel)
                {
                    var isSucceeded = await RecursiveExtendWall(position, currentRoutes, progress);
                    var cellType = isSucceeded ? CellType.Sentinel : CellType.Road;

                    foreach (var route in currentRoutes)
                    {
                        Layer.Set(route, cellType);

                        if (isSucceeded)
                        {
                            progress++;
                        }
                    }
                    if (!isSucceeded)
                    {
                        i--;
                    }
                }

                await DoInterval();
            }
            while (i < startPositions.Count);

            RaiseMazeGenerationCompleted(progress, startPositions.Count);
        }

        private async Task<bool> RecursiveExtendWall(Vector2 position, IList<Vector2> currentRoutes, int progress)
        {
            await DoInterval();
            Layer.Set(position, CellType.Wall);
            currentRoutes.Add(position);
            RaiseMazeGenerationProgressChanged(progress++, startPositions.Count);

            foreach (var direction in VectorUtility.GetRandomDirections())
            {
                var next = position + direction * 2;

                if (Layer.Get(next) != CellType.Wall)
                {
                    await DoInterval();
                    Layer.Set(position + direction, CellType.Wall);
                    currentRoutes.Add(position + direction);
                    RaiseMazeGenerationProgressChanged(progress++, startPositions.Count);

                    if (Layer.Get(next) == CellType.Road)
                    {
                        return await RecursiveExtendWall(next, currentRoutes, progress);
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void Initialize()
        {
            Layer.FillValues(CellType.Road);
            Layer.BorderValues(CellType.Sentinel);
            startPositions.Clear();

            for (int y = 1; y < Layer.Height / 2; y++)
            {
                for (int x = 1; x < Layer.Width / 2; x++)
                {
                    startPositions.Add(new Vector2(x * 2, y * 2));
                }
            }

            startPositions.Shuffle(_rnd);
        }
    }
}
