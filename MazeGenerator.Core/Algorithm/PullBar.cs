using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maze.Algorithm
{
    public class PullBar : MazeGenerator
    {
        public PullBar(Layer layer, bool isAnswerUsed = false) : base(layer, isAnswerUsed) { }

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
            var i = 1;
            var total = (Layer.Height / 2) * (Layer.Width / 2);

            for (int y = 2; y < Layer.Height - 1; y += 2)
            {
                for (int x = 2; x < Layer.Width - 1; x += 2)
                {
                    await PutWall(new Vector2(x, y));
                    RaiseMazeGenerationProgressChanged(i++, total);
                }
            }

            RaiseMazeGenerationCompleted(i, total);
        }

        private async Task PutWall(Vector2 position)
        {
            await DoInterval();
            Layer.Set(position, CellType.Wall);

            var directions = position.Y == 2 ? VectorUtility.GetRandomDirections() : VectorUtility.GetRandomDirectionsWithoutUp();

            foreach (var direction in directions)
            {
                var candidate = position + direction;

                if (Layer.Get(candidate) != CellType.Wall)
                {
                    await DoInterval();
                    Layer.Set(candidate, CellType.Wall);

                    break;
                }
            }
        }

        private void Initialize()
        {
            Layer.FillValues(CellType.Road);
            Layer.BorderValues(CellType.Sentinel);
        }
    }
}
