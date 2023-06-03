using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeGenerator.WinForms
{
    public partial class Form2 : Form
    {
        private MyAppSettings? _settings;

        public Form2(MyAppSettings settings, int formWidth, int formHeight, int pictureBoxWidth, int pictureBoxHeight)
        {
            InitializeComponent();

            Load += (sender, e) =>
            {
                _settings = settings;

                checkBox1.Checked = _settings.IsTopMost;
                checkBox2.Checked = _settings.IsFixedWindowPosition;
                checkBox3.Checked = _settings.IsFixedWindowSize;
                checkBox4.Checked = _settings.IsDisplayAnswerRoute;

                numericUpDown1.Value = _settings.MazeWidth;
                numericUpDown2.Value = _settings.MazeHeight;
                numericUpDown3.Value = pictureBoxWidth;
                numericUpDown4.Value = pictureBoxHeight;
                CalculateCellSize();
            };

            numericUpDown1.ValueChanged += (sender, e) => CalculateCellSize();
            numericUpDown2.ValueChanged += (sender, e) => CalculateCellSize();
            numericUpDown3.ValueChanged += (sender, e) => CalculateCellSize();
            numericUpDown4.ValueChanged += (sender, e) => CalculateCellSize();
            numericUpDown1.Validated += (sender, e) => CoerceSizeValue(sender, e);
            numericUpDown2.Validated += (sender, e) => CoerceSizeValue(sender, e);

            button1.Click += (sender, e) =>
            {
                if (_settings == null)
                {
                    return;
                }

                _settings.IsTopMost = checkBox1.Checked;
                _settings.IsFixedWindowPosition = checkBox2.Checked;
                _settings.IsFixedWindowSize = checkBox3.Checked;
                _settings.IsDisplayAnswerRoute = checkBox4.Checked;

                _settings.MazeWidth = (int)numericUpDown1.Value;
                _settings.MazeHeight = (int)numericUpDown2.Value;
                _settings.Width = (int)numericUpDown3.Value + (formWidth - pictureBoxWidth);
                _settings.Height = (int)numericUpDown4.Value + (formHeight - pictureBoxHeight);
            };
            button3.Click += (sender, e) =>
            {
                numericUpDown3.Value = MyAppSettings.DefaultPictureBoxWidth;
                numericUpDown4.Value = MyAppSettings.DefaultPictureBoxHeight;
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

        private void CalculateCellSize()
        {
            label9.Text = ((decimal)MathF.Round((float)numericUpDown3.Value / (int)numericUpDown1.Value, 1)).ToString();
            label11.Text = ((decimal)MathF.Round((float)numericUpDown4.Value / (int)numericUpDown2.Value, 1)).ToString();
        }
    }
}
