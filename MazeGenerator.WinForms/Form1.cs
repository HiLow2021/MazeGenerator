using System.Drawing.Imaging;
using System.Numerics;
using My.Extensions;
using My.IO;
using My.Security;
using Maze;
using Maze.Algorithm;
using Generator = Maze.Algorithm.MazeGenerator;

namespace MazeGenerator.WinForms
{
    public partial class Form1 : Form
    {
        private const int RATE = 100;
        private MyAppSettings _settings = new();
        private Generator? _generator;
        private Layer? _layer;

        public Form1()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;

            Load += (sender, e) =>
            {
                LoadConfigFile();
                DisplaySizeMessage();
                _generator = CreateMazeGenerator();
                DisplaySizeMessage();
            };
            Shown += (sender, e) => TopMost = _settings.IsTopMost;
            FormClosed += (sender, e) => SaveConfigFile();
            SizeChanged += (sender, e) => RefreshMaze();
            exitToolStripMenuItem.Click += (sender, e) => Application.Exit();
            startToolStripMenuItem.Click += async (sender, e) =>
            {
                if (_generator != null && !_generator.IsRunning)
                {
                    await Start();
                }
            };
            cancelToolStripMenuItem.Click += (sender, e) =>
            {
                if (_generator != null && _generator.IsRunning)
                {
                    Stop();
                }
            };
            snapshotToolStripMenuItem.Click += (sender, e) =>
            {
                saveFileDialog1.FileName = string.IsNullOrEmpty(saveFileDialog1.FileName) ? "maze.png" : Path.GetFileName(saveFileDialog1.FileName);
                saveFileDialog1.Filter = "PNG形式|*.png|JPG形式|*.jpg|GIF形式|*.gif";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    var fileName = saveFileDialog1.FileName;
                    var extension = Path.GetExtension(fileName).ToLower();
                    var imageFormat = extension switch
                    {
                        ".jpg" => ImageFormat.Jpeg,
                        ".jpeg" => ImageFormat.Jpeg,
                        ".png" => ImageFormat.Png,
                        ".gif" => ImageFormat.Gif,
                        _ => ImageFormat.Png,
                    };
                    var width = pictureBox1.ClientSize.Width;
                    var height = pictureBox1.ClientSize.Height;
                    using (var bitmap = new Bitmap(width, height))
                    {
                        pictureBox1.DrawToBitmap(bitmap, new Rectangle(0, 0, width, height));
                        bitmap.Save(fileName, imageFormat);
                    }
                }
            };
            exportToolStripMenuItem.Click += (sender, e) =>
            {
                if (_layer == null)
                {
                    return;
                }

                saveFileDialog2.FileName = string.IsNullOrEmpty(saveFileDialog2.FileName) ? "maze.csv" : Path.GetFileName(saveFileDialog2.FileName);
                saveFileDialog2.Filter = "CSV形式|*.csv";

                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    var lines = new List<IList<string>>();

                    for (int y = 0; y < _layer.Height; y++)
                    {
                        var line = new List<string>();

                        for (int x = 0; x < _layer.Width; x++)
                        {
                            var position = new Vector2(x, y);
                            var value = (int)_layer.Get(position);

                            line.Add(value.ToString());
                        }

                        lines.Add(line.ToArray());
                    }

                    var text = string.Join(Environment.NewLine, lines.Select(line => string.Join(",", line)));
                    File.WriteAllText(saveFileDialog2.FileName, text);
                }
            };
            settingsToolStripMenuItem.Click += (sender, e) =>
            {
                _settings.MazeWidth = (int)numericUpDown1.Value;
                _settings.MazeHeight = (int)numericUpDown2.Value;

                using var form2 = new Form2(_settings, Width, Height, pictureBox1.Width, pictureBox1.Height);
                form2.TopMost = TopMost;

                if (form2.ShowDialog(this) == DialogResult.OK)
                {
                    Width = _settings.Width;
                    Height = _settings.Height;
                    TopMost = _settings.IsTopMost;
                    numericUpDown1.Value = _settings.MazeWidth;
                    numericUpDown2.Value = _settings.MazeHeight;
                }
            };

            button1.Click += async (sender, e) =>
            {
                if (_generator == null)
                {
                    return;
                }
                if (!_generator.IsRunning)
                {
                    await Start();
                }
                else
                {
                    Stop();
                }
            };
            numericUpDown1.Validated += (sender, e) => CoerceSizeValue(sender, e);
            numericUpDown2.Validated += (sender, e) => CoerceSizeValue(sender, e);
            trackBar1.ValueChanged += (sender, e) =>
            {
                if (_generator == null)
                {
                    return;
                }

                _generator.IntervalMilliseconds = (int)(1 / (float)trackBar1.Value * RATE);
            };

            pictureBox1.SizeChanged += (sender, e) => DisplaySizeMessage();
            pictureBox1.Paint += (sender, e) =>
            {
                if (_layer == null)
                {
                    return;
                }

                var (cellWidth, cellHeight) = GetCellSize();

                for (int y = 0; y < _layer.Height; y++)
                {
                    for (int x = 0; x < _layer.Width; x++)
                    {
                        var position = new Vector2(x, y);

                        if (_layer.Get(position) == CellType.Sentinel)
                        {
                            e.Graphics.FillRectangle(Brushes.Black, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                        }
                        else if (_layer.Get(position) == CellType.Wall)
                        {
                            e.Graphics.FillRectangle(Brushes.Brown, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                        }
                        else if (_layer.Get(position) == CellType.Road)
                        {
                            e.Graphics.FillRectangle(Brushes.White, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                        }
                        else if (_layer.Get(position) == CellType.Mark)
                        {
                            e.Graphics.FillRectangle(Brushes.GreenYellow, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                        }
                    }
                }
            };

            static void CoerceSizeValue(object? sender, EventArgs e)
            {
                if (sender is not NumericUpDown numericUpDown)
                {
                    return;
                }
                if (numericUpDown.Value % 2 == 0)
                {
                    numericUpDown.Value--;
                }
            }
        }

        private async Task Start()
        {
            EnableControls(false);
            button1.Text = "キャンセル";
            DisplayStateMessage("実行中");
            DisplaySizeMessage();

            _generator = CreateMazeGenerator();
            await _generator.Start();
            RefreshMaze();

            DisplayStateMessage("完了");
            DisplaySizeMessage();
            button1.Text = "スタート";
            EnableControls(true);
        }

        private void Stop()
        {
            _generator?.Stop();
        }

        private Generator CreateMazeGenerator()
        {
            var index = comboBox1.SelectedIndex;
            var isAnswerUsed = _settings.IsDisplayAnswerRoute;

            _layer = new Layer((int)numericUpDown1.Value, (int)numericUpDown2.Value);
            _generator = index switch
            {
                0 => new PullBar(_layer, isAnswerUsed),
                1 => new Dig(_layer, isAnswerUsed),
                2 => new ExtendWall(_layer, isAnswerUsed),
                3 => new Cluster(_layer, isAnswerUsed),
                _ => throw new NotImplementedException(),
            };
            _generator.IntervalMilliseconds = (int)(1 / (float)trackBar1.Value * RATE);
            _generator.MazeGenerationProgressChanged += (sender, e) =>
            {
                this.InvokeIfRequired(() => RefreshMaze());
            };
            _generator.SearchAnswerProgressChanged += (sender, e) =>
            {
                this.InvokeIfRequired(() => RefreshMaze());
            };

            return _generator;
        }

        private void EnableControls(bool flag)
        {
            if (flag)
            {
                comboBox1.Enabled = true;
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                startToolStripMenuItem.Enabled = true;
                cancelToolStripMenuItem.Enabled = false;
                toolsToolStripMenuItem.Enabled = true;


            }
            else
            {
                comboBox1.Enabled = false;
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                startToolStripMenuItem.Enabled = false;
                cancelToolStripMenuItem.Enabled = true;
                toolsToolStripMenuItem.Enabled = false;
            }
        }

        private void RefreshMaze() => pictureBox1.Refresh();

        private void DisplayStateMessage(string message) => toolStripStatusLabel1.Text = message;

        private void DisplaySizeMessage()
        {
            var (cellWidth, cellHeight) = GetCellSize();

            toolStripStatusLabel2.Text = $"全体:{pictureBox1.Width}×{pictureBox1.Height}  セルサイズ:{cellWidth}×{cellHeight}";
        }

        private (float, float) GetCellSize()
        {
            if (_layer == null)
            {
                return (1, 1);
            }

            var cellWidth = MathF.Round((float)pictureBox1.Width / _layer.Width, 1);
            var cellHeight = MathF.Round((float)pictureBox1.Height / _layer.Height, 1);

            return (cellWidth, cellHeight);
        }

        private void LoadConfigFile()
        {
            if (FileAdvanced.Exists(MyAppSettings.ConfigPath))
            {
                var bs = FileAdvanced.LoadFromBinaryFile<byte[]>(MyAppSettings.ConfigPath);

                _settings = Cryptography.Decrypt<MyAppSettings>(bs);

                if (_settings.IsFixedWindowPosition)
                {
                    Left = _settings.Left;
                    Top = _settings.Top;
                }
                if (_settings.IsFixedWindowSize)
                {
                    Width = _settings.Width;
                    Height = _settings.Height;
                }

                comboBox1.SelectedIndex = _settings.MazeAlgorithmMethodType;
                numericUpDown1.Value = _settings.MazeWidth;
                numericUpDown2.Value = _settings.MazeHeight;
                trackBar1.Value = _settings.MazeGenerationMilliseconds;
            }
        }

        private void SaveConfigFile()
        {
            if (WindowState == FormWindowState.Normal)
            {
                _settings.Left = Left;
                _settings.Top = Top;
            }

            _settings.Width = Width;
            _settings.Height = Height;

            _settings.MazeAlgorithmMethodType = comboBox1.SelectedIndex;
            _settings.MazeWidth = (int)numericUpDown1.Value;
            _settings.MazeHeight = (int)numericUpDown2.Value;
            _settings.MazeGenerationMilliseconds = trackBar1.Value;

            FileAdvanced.SaveToBinaryFile(MyAppSettings.ConfigPath, Cryptography.Encrypt(_settings));
        }
    }
}