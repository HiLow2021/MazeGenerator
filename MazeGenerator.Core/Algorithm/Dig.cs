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
    public class Dig : MazeGenerator
    {
        private readonly IList<Vector2> remainingPositions = new List<Vector2>();

        public Dig(Layer layer, bool isAnswerUsed = false) : base(layer, isAnswerUsed) { }

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
            await RecursiveDig(remainingPositions[0], 0);
            RaiseMazeGenerationCompleted(remainingPositions.Count, remainingPositions.Count);
        }

        private async Task RecursiveDig(Vector2 position, int remainingPositionCounter)
        {
            await DoInterval();
            Layer.Set(position, CellType.Road);
            RaiseMazeGenerationProgressChanged(remainingPositionCounter++, remainingPositions.Count);

            foreach (var direction in VectorUtility.GetRandomDirections())
            {
                var next = position + direction * 2;

                if (Layer.Get(next) == CellType.Wall)
                {
                    await DoInterval();
                    Layer.Set(position + direction, CellType.Road);
                    RaiseMazeGenerationProgressChanged(remainingPositionCounter++, remainingPositions.Count);
                    await RecursiveDig(next, remainingPositionCounter);
                }
            }
        }

        private void Initialize()
        {
            Layer.FillValues(CellType.Wall);
            Layer.BorderValues(CellType.Sentinel);

            remainingPositions.Clear();

            for (int y = 1; y < Layer.Height - 1; y += 2)
            {
                for (int x = 1; x < Layer.Width - 1; x += 2)
                {
                    remainingPositions.Add(new Vector2(x, y));
                }
            }

            remainingPositions.Shuffle(_rnd);
        }
    }
}
