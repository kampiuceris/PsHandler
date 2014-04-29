using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PsHandler.Hud
{
    public class Symbol
    {
        public string SymbolText;
        public Bmp Bmp;
    }

    public class LobbyTime
    {
        private readonly List<Symbol> _symbolsBlack;
        private readonly List<Symbol> _symbolsClassic;

        public LobbyTime()
        {
            _symbolsBlack = new List<Symbol>();
            _symbolsClassic = new List<Symbol>();
            List<string> list = new List<string>();
            for (int i = 0; i < 10; i++) list.Add(i.ToString(CultureInfo.InvariantCulture));

            foreach (string s in list)
            {
                Bmp bmp = new Bmp(Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.Lobby.Black.{0}.png", s)));
                MakeBlackWhite(ref bmp, 255, 255, 255, 50, 50, 50);
                _symbolsBlack.Add(new Symbol { SymbolText = s.Equals("separator") ? ":" : s, Bmp = bmp });

                bmp = new Bmp(Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.Lobby.Classic.{0}.png", s)));
                MakeBlackWhite(ref bmp, 231, 201, 106, 100, 100, 100);
                _symbolsClassic.Add(new Symbol { SymbolText = s.Equals("separator") ? ":" : s, Bmp = bmp });
            }
        }

        private static void MakeBlackWhite(ref Bmp bmp, int r, int g, int b, int diffR, int diffG, int diffB)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp[x, y] = (Math.Abs(bmp.GetPixelR(x, y) - r) < diffR && Math.Abs(bmp.GetPixelG(x, y) - g) < diffG && Math.Abs(bmp.GetPixelB(x, y) - b) < diffB) ? new byte[] { 255, 255, 255, 255 } : new byte[] { 255, 0, 0, 0 };
                }
            }
        }

        private string GetLetter(Bmp bmp, int x, int y, bool themeClassic)
        {
            List<Symbol> _symbols = themeClassic ? _symbolsClassic : _symbolsBlack;

            Symbol bestSymbol = null;
            double compatibility = 0;

            foreach (Symbol s in _symbols)
            {
                bool enoughWidthAndHeight = (x + s.Bmp.Width < bmp.Width) && (y + s.Bmp.Height < bmp.Height);
                if (enoughWidthAndHeight)
                {
                    int matchSum = 0;
                    for (int sy = 0; sy < s.Bmp.Height; sy++)
                    {
                        for (int sx = 0; sx < s.Bmp.Width; sx++)
                        {
                            if (bmp.GetPixelR(x + sx, y + sy) == s.Bmp.GetPixelR(sx, sy)
                                && bmp.GetPixelG(x + sx, y + sy) == s.Bmp.GetPixelG(sx, sy)
                                && bmp.GetPixelB(x + sx, y + sy) == s.Bmp.GetPixelB(sx, sy))
                            {
                                matchSum++;
                            }
                        }
                    }
                    double avg = (double)matchSum / (s.Bmp.Width * s.Bmp.Height);
                    if (avg > compatibility)
                    {
                        bestSymbol = s;
                        compatibility = avg;
                    }
                }
            }

            double compMax = themeClassic ? 0.9 : 0.95;
            if (compatibility < compMax) bestSymbol = null;
            //if (bestSymbol != null) Debug.WriteLine(bestSymbol.SymbolText + " " + compatibility);

            return bestSymbol == null ? "" : bestSymbol.SymbolText;
        }

        public string GetText(Bmp bmp, bool themeClassic)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    sb.Append(GetLetter(bmp, x, y, themeClassic));
                }
            }
            return sb.ToString();
        }
    }
}
