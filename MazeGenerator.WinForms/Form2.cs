﻿using System;
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
        public Form2(MyAppSettings settings, int formWidth, int formHeight, int pictureBoxWidth, int pictureBoxHeight)
        {
            InitializeComponent();

            Load += (sender, e) =>
            {
                checkBox1.Checked = settings.IsTopMost;
                checkBox2.Checked = settings.IsFixedWindowPosition;
                checkBox3.Checked = settings.IsFixedWindowSize;
                checkBox4.Checked = settings.IsDisplayAnswerRoute;

                numericUpDown1.Value = settings.MazeWidth;
                numericUpDown2.Value = settings.MazeHeight;
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
                if (settings == null)
                {
                    return;
                }

                settings.IsTopMost = checkBox1.Checked;
                settings.IsFixedWindowPosition = checkBox2.Checked;
                settings.IsFixedWindowSize = checkBox3.Checked;
                settings.IsDisplayAnswerRoute = checkBox4.Checked;

                settings.MazeWidth = (int)numericUpDown1.Value;
                settings.MazeHeight = (int)numericUpDown2.Value;
                settings.Width = (int)numericUpDown3.Value + (formWidth - pictureBoxWidth);
                settings.Height = (int)numericUpDown4.Value + (formHeight - pictureBoxHeight);
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
