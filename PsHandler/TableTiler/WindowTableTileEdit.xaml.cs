using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsHandler.PokerTypes;


namespace PsHandler.TableTiler
{
    /// <summary>
    /// Interaction logic for WindowTableTileEdit.xaml
    /// </summary>
    public partial class WindowTableTileEdit : Window
    {
        public TableTile TableTile;
        public bool Saved;

        public WindowTableTileEdit(Window owner, TableTile tableTile = null)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            TableTile = tableTile ?? new TableTile();

            TextBox_Name.Text = TableTile.Name;
            TextBoxHotkey_Hotkey.KeyCombination = TableTile.KeyCombination;
            CheckBox_SortByStartingTime.IsChecked = TableTile.SortByStartingHand;
            TextBox_IncludeAnd.Text = !TableTile.IncludeAnd.Any() ? "" : TableTile.IncludeAnd.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_IncludeOr.Text = !TableTile.IncludeOr.Any() ? "" : TableTile.IncludeOr.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_ExcludeAnd.Text = !TableTile.ExcludeAnd.Any() ? "" : TableTile.ExcludeAnd.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_ExcludeOr.Text = !TableTile.ExcludeOr.Any() ? "" : TableTile.ExcludeOr.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);

            StringBuilder sb = new StringBuilder();
            foreach (var xywh in TableTile.XYWHs) sb.Append(string.Format("{0} {1} {2} {3}{4}", xywh.X, xywh.Y, xywh.Width, xywh.Height, Environment.NewLine));
            TextBox_XYWH.Text = sb.ToString();

            TextBox_XYWH.TextChanged += (sender, args) => UCScreenPreview_Main_Update();
            CheckBox_SortByStartingTime.Checked += (sender, args) => UCScreenPreview_Main_Update();
            CheckBox_SortByStartingTime.Unchecked += (sender, args) => UCScreenPreview_Main_Update();

            Loaded += (sender, args) =>
            {
                string text = TextBox_XYWH.Text;
                TextBox_XYWH.Text = text + "1";
                TextBox_XYWH.Text = text;
            };
        }

        private void UCScreenPreview_Main_Update()
        {
            var config = GetXYWHs();
            TextBox_XYWH.Background = config == null ? new SolidColorBrush(Colors.MistyRose) : new SolidColorBrush(Colors.Honeydew);
            UCScreenPreview_Main.Update(config, CheckBox_SortByStartingTime.IsChecked == true);
        }

        private System.Drawing.Rectangle[] GetXYWHs()
        {
            string text = TextBox_XYWH.Text;
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

        private void Button_SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_Name.Text.Length == 0)
            {
                MessageBox.Show(string.Format("Invalid '{0}' input.", Label_Name.Content), "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int countNames = TableTileManager.TableTiles.Count(o => o.Name.ToLowerInvariant().Equals(TextBox_Name.Text.ToLowerInvariant()));
            if (countNames != 0)
            {
                MessageBox.Show("Cannot save. Duplicate names.", "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            System.Drawing.Rectangle[] xywhs = GetXYWHs();
            if (xywhs == null)
            {
                MessageBox.Show(string.Format("Invalid '{0}' input.", Label_XYWHs.Content), "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TableTile.Name = TextBox_Name.Text;
            TableTile.KeyCombination = TextBoxHotkey_Hotkey.KeyCombination;
            TableTile.SortByStartingHand = CheckBox_SortByStartingTime.IsChecked == true;
            TableTile.IncludeAnd = TextBox_IncludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            TableTile.IncludeOr = TextBox_IncludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            TableTile.ExcludeAnd = TextBox_ExcludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            TableTile.ExcludeOr = TextBox_ExcludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            TableTile.XYWHs = xywhs;

            Saved = true;
            Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Saved = false;
            Close();
        }
    }
}
