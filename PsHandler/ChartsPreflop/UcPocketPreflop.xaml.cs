using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsHandler.ChartsPreflop
{
    /// <summary>
    /// Interaction logic for UcPocketPreflop.xaml
    /// </summary>
    public partial class UcPocketPreflop : UserControl
    {
        private readonly Brush _brushBackgroundSelected = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6bb5ff"));
        private readonly Brush _brushBackgroundDefault;
        public string PocketStr;
        public int PocketInt;
        private readonly UcRangePreflop _ucRangePreflop;
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (_isSelected)
                {
                    Rectangle_Main.Fill = _brushBackgroundSelected;
                }
                else
                {
                    Rectangle_Main.Fill = _brushBackgroundDefault;
                }
                _ucRangePreflop.WindowRangePreflop.UpdateInfo();
            }
        }

        public UcPocketPreflop(UcRangePreflop ucRangePreflop, string pocketStr)
        {
            InitializeComponent();
            PocketStr = pocketStr;
            PocketInt = RangeConstructor.Parse(PocketStr);
            _ucRangePreflop = ucRangePreflop;

            if (pocketStr.Length == 2)
            {
                // pair
                _brushBackgroundDefault = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cedec6"));
            }
            else
            {
                if (pocketStr[2] == 's')
                {
                    // suited
                    _brushBackgroundDefault = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffe7b5"));
                }
                else
                {
                    // offsuited
                    _brushBackgroundDefault = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e7eff7"));
                }
            }
            Rectangle_Main.Fill = _brushBackgroundDefault;
            Label_Pocket.Content = pocketStr;

            MouseLeftButtonDown += (sender, args) =>
            {
                if (!_ucRangePreflop.IsReadOnly)
                {
                    Focus();

                    _ucRangePreflop.UcPocketPreFlopPressed = this;

                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        _ucRangePreflop.MouseDownCtrl(pocketStr, IsSelected);
                    }
                    else
                    {
                        IsSelected = !IsSelected;
                    }
                }
            };

            MouseEnter += (sender, args) =>
            {
                if (!_ucRangePreflop.IsReadOnly && args.LeftButton == MouseButtonState.Pressed && _ucRangePreflop.UcPocketPreFlopPressed != null)
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        _ucRangePreflop.MouseDownCtrl(pocketStr, !_ucRangePreflop.UcPocketPreFlopPressed.IsSelected);
                    }
                    else
                    {
                        IsSelected = _ucRangePreflop.UcPocketPreFlopPressed.IsSelected;
                    }
                }
            };
        }
    }
}
