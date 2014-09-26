using System.Drawing;
using System.Windows;
using System.Windows.Input;
using PsHandler.Custom;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for WindowTimer.xaml
    /// </summary>
    public partial class WindowTimer : Window
    {
        public Table Table;

        public WindowTimer(Table table)
        {
            Table = table;
            InitializeComponent();

            Loaded += (sender, args) => WinApi.SetWindowLong(this.GetHandle(), -8, Table.Handle.ToInt32());

            // drag by right mouse click
            System.Windows.Point startPosition = new System.Windows.Point();
            PreviewMouseRightButtonDown += (sender, e) =>
            {
                startPosition = e.GetPosition(this);
            };
            PreviewMouseMove += (sender, e) =>
            {
                if (!TableManager.HudTimerLocationLocked && e.RightButton == MouseButtonState.Pressed)
                {
                    System.Windows.Point endPosition = e.GetPosition(this);
                    Vector vector = endPosition - startPosition;
                    Left += vector.X;
                    Top += vector.Y;

                    Rectangle cr = WinApi.GetClientRectangle(Table.Handle);
                    double x = (Left - cr.Left) / cr.Width;
                    double y = (Top - cr.Top) / cr.Height;
                    TableManager.SetHudTimerLocationX(Table.TableHud.TableSize, (float)x, this);
                    TableManager.SetHudTimerLocationY(Table.TableHud.TableSize, (float)y, this);
                }
            };
        }
    }
}
