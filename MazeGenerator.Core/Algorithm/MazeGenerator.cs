using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using My.Randomizer;
using Maze.EventArgs;
using Maze.Extensions;

namespace Maze.Algorithm
{
    public abstract class MazeGenerator
    {
        protected CancellationTokenSource _TokenSource;
        protected CancellationToken _Token;
        protected XorShift _rnd = new XorShift();

        public Layer Layer { get; }
        public bool IsAnswerUsed { get; set; }
        public bool IsRunning { get; protected set; }
        public int IntervalMilliseconds { get; set; } = 10;

        public event EventHandler<MazeProgressChangedEventArgs>? MazeGenerationProgressChanged;
        public event EventHandler<MazeProgressChangedEventArgs>? MazeGenerationCompleted;
        public event EventHandler<MazeEventArgs>? SearchAnswerProgressChanged;
        public event EventHandler<MazeEventArgs>? SearchAnswerCompleted;

        public MazeGenerator(Layer layer, bool isAnswerUsed = false)
        {
            Layer = layer;
            IsAnswerUsed = isAnswerUsed;
            _TokenSource = new CancellationTokenSource();
            _Token = _TokenSource.Token;
        }

        public abstract Task Start();

        public void Stop()
        {
            _TokenSource?.Cancel();
        }

        protected virtual void SetEntrance(out Vector2 entrance1, out Vector2 entrance2)
        {
            var positions = new Vector2[]
            {
                new Vector2(_rnd.Next(1, Layer.Width - 1), 0),
                new Vector2(Layer.Width - 1, _rnd.Next(1, Layer.Height - 1)),
                new Vector2(_rnd.Next(1, Layer.Width - 1), Layer.Height - 1),
                new Vector2(0 , _rnd.Next(1, Layer.Height - 1))
            }.Shuffle(_rnd);

            entrance1 = DoOddExceptBorder(positions[0]);
            entrance2 = DoOddExceptBorder(positions[1]);
            Layer.Set(entrance1, CellType.Road);
            Layer.Set(entrance2, CellType.Road);

            Vector2 DoOddExceptBorder(Vector2 p)
            {
                if (p.X != 0 && p.X != Layer.Width - 1 && p.X % 2 == 0) { p.X--; }
                if (p.Y != 0 && p.Y != Layer.Height - 1 && p.Y % 2 == 0) { p.Y--; }

                return p;
            }
        }

        protected virtual async Task<bool> SearchAnswer(Vector2 entrance1, Vector2 entrance2)
        {
            await DoInterval();
            Layer.Set(entrance1, CellType.Mark);
            RaiseSearchAnswerProgressChanged();

            if (entrance1.Equals(entrance2))
            {
                return true;
            }

            foreach (var direction in VectorUtility.GetRandomDirections())
            {
                var candidate = entrance1 + direction;

                if (Layer.Get(candidate) == CellType.Road)
                {
                    if (await SearchAnswer(candidate, entrance2))
                    {
                        return true;
                    }
                    else
                    {
                        Layer.Set(candidate, CellType.Road);
                    }
                }
            }

            return false;
        }

        protected virtual async Task DoInterval()
        {
            _Token.ThrowIfCancellationRequested();

            if (IntervalMilliseconds > 0)
            {
                await Task.Delay(IntervalMilliseconds);
            }
        }

        protected virtual void RaiseMazeGenerationProgressChanged(int progress, int total)
        {
            if (IntervalMilliseconds > 0)
            {
                MazeGenerationProgressChanged?.Invoke(this, new MazeProgressChangedEventArgs(progress, total));
            }
        }

        protected virtual void RaiseMazeGenerationCompleted(int progress, int total) => MazeGenerationCompleted?.Invoke(this, new MazeProgressChangedEventArgs(progress, total));

        protected virtual void RaiseSearchAnswerProgressChanged()
        {
            if (IntervalMilliseconds > 0)
            {
                SearchAnswerProgressChanged?.Invoke(this, new MazeEventArgs());
            }
        }

        protected virtual void RaiseSearchAnswerCompleted() => SearchAnswerCompleted?.Invoke(this, new MazeEventArgs());
    }
}
