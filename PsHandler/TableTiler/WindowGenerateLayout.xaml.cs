using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsHandler.Custom;
using PsHandler.UI;

namespace PsHandler.TableTiler
{
    /// <summary>
    /// Interaction logic for WindowGenerateLayout.xaml
    /// </summary>
    public partial class WindowGenerateLayout : Window
    {
        private int WindowsBorderThicknessInPixelsLeft = 0;
        private int WindowsBorderThicknessInPixelsTop = 0;
        private int WindowsBorderThicknessInPixelsRight = 0;
        private int WindowsBorderThicknessInPixelsBottom = 0;
        private readonly System.Windows.Forms.Screen[] _screens = System.Windows.Forms.Screen.AllScreens;
        private System.Windows.Forms.Screen SelectedScreen
        {
            get
            {
                for (int i = 0; i < StackPanel_Screens.Children.Count; i++)
                {
                    var radioButtonCentered = StackPanel_Screens.Children[i] as RadioButtonCentered;
                    if (radioButtonCentered != null && radioButtonCentered.IsChecked == true)
                    {
                        return _screens[i];
                    }
                }
                return _screens[0];
            }
        }
        private bool AdjustByWidth { get { return RadioButtonCentered_AdjustByWidth.IsChecked == true; } }
        private bool AdjustByHeight { get { return RadioButtonCentered_AdjustByHeight.IsChecked == true; } }
        private bool FixedTableSize { get { return CheckBox_FixedTableSize.IsChecked == true; } }
        private bool HorizontalAlignmentLeft { get { return RadioButtonCentered_HorizontalAlignmentLeft.IsChecked == true; } }
        private bool HorizontalAlignmentCenter { get { return RadioButtonCentered_HorizontalAlignmentCenter.IsChecked == true; } }
        private bool HorizontalAlignmentRight { get { return RadioButtonCentered_HorizontalAlignmentRight.IsChecked == true; } }
        private bool HorizontalAlignmentStretch { get { return RadioButtonCentered_HorizontalAlignmentStretch.IsChecked == true; } }
        private bool VerticalAlignmentTop { get { return RadioButtonCentered_VerticalAlignmentTop.IsChecked == true; } }
        private bool VerticalAlignmentCenter { get { return RadioButtonCentered_VerticalAlignmentCenter.IsChecked == true; } }
        private bool VerticalAlignmentBottom { get { return RadioButtonCentered_VerticalAlignmentBottom.IsChecked == true; } }
        private bool VerticalAlignmentStretch { get { return RadioButtonCentered_VerticalAlignmentStretch.IsChecked == true; } }

        public WindowGenerateLayout()
        {
            InitializeComponent();

            for (int i = 0; i < _screens.Length; i++)
            {
                StackPanel_Screens.Children.Add(new RadioButtonCentered
                {
                    Text = string.Format("Screen #{0} ({1} x {2})", i, _screens[i].Bounds.Width, _screens[i].Bounds.Height),
                    Height = 22
                });
            }
            if (StackPanel_Screens.Children.OfType<RadioButtonCentered>().Any())
            {
                StackPanel_Screens.Children.OfType<RadioButtonCentered>().First().IsChecked = true;
            }

            TextBox_XYWidthHeight.TextChanged += (sender, args) => UCScreenPreview_Main_Update();
            SizeChanged += (sender, args) => UCScreenPreview_Main_Update();
            CheckBox_FixedTableSize.Checked += (sender, args) =>
            {
                Label_TableWidth.IsEnabled = true;
                Label_TableHeight.IsEnabled = true;
                TextBox_TableWidth.IsEnabled = true;
                TextBox_TableHeight.IsEnabled = true;
                RadioButtonCentered_AdjustByWidth.IsEnabled = false;
                RadioButtonCentered_AdjustByHeight.IsEnabled = false;
            };
            CheckBox_FixedTableSize.Unchecked += (sender, args) =>
            {
                Label_TableWidth.IsEnabled = false;
                Label_TableHeight.IsEnabled = false;
                TextBox_TableWidth.IsEnabled = false;
                TextBox_TableHeight.IsEnabled = false;
                RadioButtonCentered_AdjustByWidth.IsEnabled = true;
                RadioButtonCentered_AdjustByHeight.IsEnabled = true;
            };
            TextBox_TableWidth.TextChanged += (sender, args) =>
            {
                if (TextBox_TableWidth.IsFocused)
                {
                    try
                    {
                        var clientWidth = int.Parse(TextBox_TableWidth.Text) - WindowsBorderThicknessInPixelsLeft - WindowsBorderThicknessInPixelsRight;
                        var clientSize = PokerStarsThemeTable.GetClientSizeByWidth(clientWidth);
                        TextBox_TableHeight.Text = string.Format("{0}", clientSize.Height + WindowsBorderThicknessInPixelsTop + WindowsBorderThicknessInPixelsBottom);
                    }
                    catch (Exception)
                    {
                    }
                }
            };
            TextBox_TableHeight.TextChanged += (sender, args) =>
            {
                if (TextBox_TableHeight.IsFocused)
                {
                    try
                    {
                        var clientHeight = int.Parse(TextBox_TableHeight.Text) - WindowsBorderThicknessInPixelsTop - WindowsBorderThicknessInPixelsBottom;
                        var clientSize = PokerStarsThemeTable.GetClientSizeByHeight(clientHeight);
                        TextBox_TableWidth.Text = string.Format("{0}", clientSize.Width + WindowsBorderThicknessInPixelsLeft + WindowsBorderThicknessInPixelsRight);
                    }
                    catch (Exception)
                    {
                    }
                }
            };
            TextBox_TableWidth.LostFocus += (sender, args) =>
            {
                var clientWidth = int.Parse(TextBox_TableWidth.Text) - WindowsBorderThicknessInPixelsLeft - WindowsBorderThicknessInPixelsRight;
                if (clientWidth > PokerStarsThemeTable.WIDTH_MAX)
                {
                    TextBox_TableWidth.Text = string.Format("{0}", PokerStarsThemeTable.WIDTH_MAX + WindowsBorderThicknessInPixelsLeft + WindowsBorderThicknessInPixelsRight);
                    TextBox_TableHeight.Text = string.Format("{0}", PokerStarsThemeTable.HEIGHT_MAX + WindowsBorderThicknessInPixelsTop + WindowsBorderThicknessInPixelsBottom);
                }
                if (clientWidth < PokerStarsThemeTable.WIDTH_MIN)
                {
                    TextBox_TableWidth.Text = string.Format("{0}", PokerStarsThemeTable.WIDTH_MIN + WindowsBorderThicknessInPixelsLeft + WindowsBorderThicknessInPixelsRight);
                    TextBox_TableHeight.Text = string.Format("{0}", PokerStarsThemeTable.HEIGHT_MIN + WindowsBorderThicknessInPixelsTop + WindowsBorderThicknessInPixelsBottom);
                }
            };

            TextBox_TableHeight.LostFocus += (sender, args) =>
            {
                var clientHeight = int.Parse(TextBox_TableHeight.Text) - WindowsBorderThicknessInPixelsTop - WindowsBorderThicknessInPixelsBottom;
                if (clientHeight > PokerStarsThemeTable.HEIGHT_MAX)
                {
                    TextBox_TableWidth.Text = string.Format("{0}", PokerStarsThemeTable.WIDTH_MAX + WindowsBorderThicknessInPixelsLeft + WindowsBorderThicknessInPixelsRight);
                    TextBox_TableHeight.Text = string.Format("{0}", PokerStarsThemeTable.HEIGHT_MAX + WindowsBorderThicknessInPixelsTop + WindowsBorderThicknessInPixelsBottom);
                }
                if (clientHeight < PokerStarsThemeTable.HEIGHT_MIN)
                {
                    TextBox_TableWidth.Text = string.Format("{0}", PokerStarsThemeTable.WIDTH_MIN + WindowsBorderThicknessInPixelsLeft + WindowsBorderThicknessInPixelsRight);
                    TextBox_TableHeight.Text = string.Format("{0}", PokerStarsThemeTable.HEIGHT_MIN + WindowsBorderThicknessInPixelsTop + WindowsBorderThicknessInPixelsBottom);
                }
            };
            Grid_Main.MouseLeftButtonDown += (sender, args) =>
            {
                Grid_Main.Focus();
            };

            Loaded += (sender, args) =>
            {
                var clientRectangle = WinApi.GetClientRectangle(this.GetHandle());
                var windowRectangle = WinApi.GetWindowRectangle(this.GetHandle());

                WindowsBorderThicknessInPixelsLeft = clientRectangle.Left - windowRectangle.Left;
                WindowsBorderThicknessInPixelsTop = clientRectangle.Top - windowRectangle.Top;
                WindowsBorderThicknessInPixelsRight = windowRectangle.Right - clientRectangle.Right;
                WindowsBorderThicknessInPixelsBottom = windowRectangle.Bottom - clientRectangle.Bottom;
            };

            // seed

            RadioButtonCentered_AdjustByWidth.IsChecked = true;
            CheckBox_FixedTableSize.IsChecked = true;
            CheckBox_FixedTableSize.IsChecked = false;
            TextBox_TableWidth.Text = string.Format("{0}", PokerStarsThemeTable.WIDTH_DEFAULT + WindowsBorderThicknessInPixelsLeft + WindowsBorderThicknessInPixelsRight);
            TextBox_TableHeight.Text = string.Format("{0}", PokerStarsThemeTable.HEIGHT_DEFAULT + WindowsBorderThicknessInPixelsTop + WindowsBorderThicknessInPixelsBottom);
            RadioButtonCentered_HorizontalAlignmentStretch.IsChecked = true;
            RadioButtonCentered_VerticalAlignmentStretch.IsChecked = true;
            UCScreenPreview_Main_Update();
        }

        private System.Drawing.Rectangle[] GetXYWHs()
        {
            string text = TextBox_XYWidthHeight.Text;
            string[] lines = text.Split(new[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
            List<System.Drawing.Rectangle> config = new List<System.Drawing.Rectangle>();
            foreach (string line in lines)
            {
                string[] words = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length != 4) return null;
                int[] values = new int[4]; ;
                for (int i = 0; i < 4; i++) if (!int.TryParse(words[i], out values[i])) return null;
                if (values[2] <= 0 || values[3] <= 0) return null;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(values[0], values[1], values[2], values[3]);
                config.Add(rect);
            }
            return config.ToArray();
        }

        private void UCScreenPreview_Main_Update()
        {
            var config = GetXYWHs();
            TextBox_XYWidthHeight.Background = config == null ? new SolidColorBrush(Colors.MistyRose) : new SolidColorBrush(Colors.Honeydew);
            try
            {
                UCScreenPreview_Main.Update(config, true);
            }
            catch
            {
            }
        }

        private void Button_GenerateLayout_Click(object sender, RoutedEventArgs e)
        {
            GenerateLayout();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GenerateLayout()
        {
            System.Drawing.Rectangle workingArea = SelectedScreen.WorkingArea;
            bool adjustByWidth = AdjustByWidth;
            bool adjustByHeight = AdjustByHeight;
            bool fixedTableSize = FixedTableSize;
            int columns = 1;
            int rows = 1;
            bool horizontalAlignmentLeft = HorizontalAlignmentLeft;
            bool horizontalAlignmentCenter = HorizontalAlignmentCenter;
            bool horizontalAlignmentRight = HorizontalAlignmentRight;
            bool horizontalAlignmentStretch = HorizontalAlignmentStretch;
            bool verticalAlignmentTop = VerticalAlignmentTop;
            bool verticalAlignmentCenter = VerticalAlignmentCenter;
            bool verticalAlignmentBottom = VerticalAlignmentBottom;
            bool verticalAlignmentStretch = VerticalAlignmentStretch;

            // parse data

            try
            {
                columns = int.Parse(TextBox_TableColumns.Text);
                if (columns <= 0) throw new NotSupportedException();
            }
            catch (Exception)
            {
                WindowMessage.ShowDialog("Invalid input: columns", "Error", WindowMessageButtons.OK, WindowMessageImage.Error, this, WindowStartupLocation.CenterOwner, WindowMessageTextType.TextBlock);
                return;
            }
            try
            {
                rows = int.Parse(TextBox_TableRows.Text);
                if (rows <= 0) throw new NotSupportedException();
            }
            catch (Exception)
            {
                WindowMessage.ShowDialog("Invalid input: rows", "Error", WindowMessageButtons.OK, WindowMessageImage.Error, this, WindowStartupLocation.CenterOwner, WindowMessageTextType.TextBlock);
                return;
            }
            // get wanted table size

            var adjustedWindowSize = new System.Windows.Size(0, 0);
            if (!fixedTableSize)
            {
                var wantedWindowSize = new System.Windows.Size((double)workingArea.Width / columns, (double)workingArea.Height / rows);
                var wantedClientSize = new System.Windows.Size(wantedWindowSize.Width - WindowsBorderThicknessInPixelsLeft - WindowsBorderThicknessInPixelsRight, wantedWindowSize.Height - WindowsBorderThicknessInPixelsTop - WindowsBorderThicknessInPixelsBottom);

                // validate table size

                if (wantedClientSize.Width < PokerStarsThemeTable.WIDTH_MIN) wantedClientSize.Width = (int)PokerStarsThemeTable.WIDTH_MIN;
                if (wantedClientSize.Height < PokerStarsThemeTable.HEIGHT_MIN) wantedClientSize.Height = (int)PokerStarsThemeTable.HEIGHT_MIN;
                if (wantedClientSize.Width > PokerStarsThemeTable.WIDTH_MAX) wantedClientSize.Width = (int)PokerStarsThemeTable.WIDTH_MAX;
                if (wantedClientSize.Height > PokerStarsThemeTable.HEIGHT_MAX) wantedClientSize.Height = (int)PokerStarsThemeTable.HEIGHT_MAX;

                var adjustedClientSize = new System.Drawing.Size(0, 0);
                if (adjustByWidth) adjustedClientSize = PokerStarsThemeTable.GetClientSizeByWidth((int)Math.Round(wantedClientSize.Width));
                if (adjustByHeight) adjustedClientSize = PokerStarsThemeTable.GetClientSizeByHeight((int)Math.Round(wantedClientSize.Height));
                if (adjustedClientSize.IsEmpty) throw new NotSupportedException();

                adjustedWindowSize = new System.Windows.Size(adjustedClientSize.Width + WindowsBorderThicknessInPixelsLeft + WindowsBorderThicknessInPixelsRight, adjustedClientSize.Height + WindowsBorderThicknessInPixelsTop + WindowsBorderThicknessInPixelsBottom);
            }
            else
            {
                try
                {
                    adjustedWindowSize = new System.Windows.Size(int.Parse(TextBox_TableWidth.Text), int.Parse(TextBox_TableHeight.Text));
                    var adjustedClientSize = new System.Windows.Size(adjustedWindowSize.Width - WindowsBorderThicknessInPixelsLeft - WindowsBorderThicknessInPixelsRight, adjustedWindowSize.Height - WindowsBorderThicknessInPixelsTop - WindowsBorderThicknessInPixelsBottom);
                    if (adjustedClientSize.Width < PokerStarsThemeTable.WIDTH_MIN ||
                        adjustedClientSize.Width > PokerStarsThemeTable.WIDTH_MAX ||
                        adjustedClientSize.Height < PokerStarsThemeTable.HEIGHT_MIN ||
                        adjustedClientSize.Height > PokerStarsThemeTable.HEIGHT_MAX)
                    {
                        throw new NotSupportedException();
                    }
                }
                catch (Exception)
                {
                    WindowMessage.ShowDialog("Invalid input: table width/height", "Error", WindowMessageButtons.OK, WindowMessageImage.Error, this, WindowStartupLocation.CenterOwner, WindowMessageTextType.TextBlock);
                    return;
                }
            }

            // place tables

            double axisHorizontalLeft = workingArea.Left + (double)adjustedWindowSize.Width / 2;
            double axisHorizontalRight = workingArea.Right - (double)adjustedWindowSize.Width / 2;
            double axisVerticalTop = workingArea.Top + (double)adjustedWindowSize.Height / 2;
            double axisVerticalBottom = workingArea.Bottom - (double)adjustedWindowSize.Height / 2;

            // adjust by alignments

            bool overlappedHorizontal = adjustedWindowSize.Width * columns > workingArea.Width;
            bool overlappedVertical = adjustedWindowSize.Height * rows > workingArea.Height;

            if (!overlappedHorizontal)
            {
                if (horizontalAlignmentLeft)
                {
                    axisHorizontalLeft = workingArea.Left + adjustedWindowSize.Width / 2;
                    axisHorizontalRight = axisHorizontalLeft + adjustedWindowSize.Width * (columns - 1);
                }
                if (horizontalAlignmentRight)
                {
                    axisHorizontalRight = workingArea.Right - adjustedWindowSize.Width / 2;
                    axisHorizontalLeft = axisHorizontalRight - adjustedWindowSize.Width * (columns - 1);
                }
                if (horizontalAlignmentCenter)
                {
                    axisHorizontalLeft = (workingArea.Left + workingArea.Width / 2) - (adjustedWindowSize.Width * (columns - 1) / 2);
                    axisHorizontalRight = (workingArea.Left + workingArea.Width / 2) + (adjustedWindowSize.Width * (columns - 1) / 2);
                }
            }

            if (verticalAlignmentTop)
            {
                axisVerticalTop = workingArea.Top + adjustedWindowSize.Height / 2;
                axisVerticalBottom = axisVerticalTop + adjustedWindowSize.Height * (rows - 1);
            }

            if (verticalAlignmentBottom)
            {
                axisVerticalBottom = workingArea.Bottom - adjustedWindowSize.Height / 2;
                axisVerticalTop = axisVerticalBottom - adjustedWindowSize.Height * (rows - 1);
            }
            if (verticalAlignmentCenter)
            {
                axisVerticalTop = (workingArea.Top + workingArea.Height / 2) - (adjustedWindowSize.Height * (rows - 1) / 2);
                axisVerticalBottom = (workingArea.Top + workingArea.Height / 2) + (adjustedWindowSize.Height * (rows - 1) / 2);
            }

            // create table center points

            double axisColumnWidth = 0;
            double axisRowHeight = 0;
            if (columns > 1) axisColumnWidth = (axisHorizontalRight - axisHorizontalLeft) / (columns - 1);
            if (rows > 1) axisRowHeight = (axisVerticalBottom - axisVerticalTop) / (rows - 1);

            var pointsCentered = new List<System.Windows.Point>();
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    pointsCentered.Add(new System.Windows.Point
                    (
                        axisHorizontalLeft + axisColumnWidth * column,
                        axisVerticalTop + axisRowHeight * row
                    ));
                }
            }

            // create rectangles

            var rects = new List<System.Drawing.Rectangle>();
            foreach (var pointCentered in pointsCentered)
            {
                rects.Add(new System.Drawing.Rectangle
                (
                    (int)Math.Round(pointCentered.X - adjustedWindowSize.Width / 2),
                    (int)Math.Round(pointCentered.Y - adjustedWindowSize.Height / 2),
                    (int)Math.Round(adjustedWindowSize.Width),
                    (int)Math.Round(adjustedWindowSize.Height)
                ));
            }

            // printout

            TextBox_TableWidth.Text = string.Format("{0}", adjustedWindowSize.Width);
            TextBox_TableHeight.Text = string.Format("{0}", adjustedWindowSize.Height);

            var sb = new StringBuilder();
            foreach (var rect in rects)
            {
                sb.AppendLine(string.Format("{0} {1} {2} {3}", rect.Left, rect.Top, rect.Width, rect.Height));
            }
            TextBox_XYWidthHeight.Text = sb.ToString();
        }
    }
}
