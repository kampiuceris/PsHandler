using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PsHandler.Replayer
{
    class UcButton : Grid
    {
        public int Size = -1;

        public static Image GetButtonImage(int size)
        {
            return new Image
            {
                Source = new BitmapImage(new Uri(string.Format(@"/PsHandler;component/Images/Resources/Replayer/Chips/{0}/chip-d.png", size), UriKind.Relative)),
                Width = Converter.DEFAULT_CHIPS_SIZES[size].X,
                Height = Converter.DEFAULT_CHIPS_SIZES[size].Y,
            };
        }
    }
}
