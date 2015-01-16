using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PsHandler.Replayer
{
    public class Converter
    {
        public static Point DEFAULT_TABLE_SIZE = new Point(792, 396);
        public static Point DEFAULT_PLAYER_SIZE = new Point(140, 50);
        public static Point DEFAULT_PLAYER_AVATAR_SIZE = new Point(50, 50);
        public static Point[] DEFAULT_PLAYER_XY =
        {
            new Point(485, 11), 
            new Point(634, 92), 
            new Point(644, 186), 
            new Point(634, 274), 
            new Point(434, 335), 
            new Point(217, 335), 
            new Point(16, 275), 
            new Point(6, 186), 
            new Point(16, 92), 
            new Point(166, 11), 
        };
        public static Point DEFAULT_POT_XY = new Point(397, 36);
        public static Point[] DEFAULT_BUTTON_XY =
        {
            new Point(486, 77), 
            new Point(593, 101), 
            new Point(644, 171), 
            new Point(629, 260), 
            new Point(529, 305), 
            new Point(321, 307), 
            new Point(215, 297), 
            new Point(161, 230), 
            new Point(154, 155), 
            new Point(238, 84), 
        };
        public static Point[] DEFAULT_BET_XY =
        {
            new Point(503, 116), 
            new Point(584, 143), 
            new Point(597, 229), 
            new Point(577, 272), 
            new Point(473, 293), 
            new Point(322, 293), 
            new Point(215, 272), 
            new Point(195, 229), 
            new Point(208, 143), 
            new Point(293, 116), 
        };
        public static Point[] DEFAULT_COMMUNITY_CARDS_XY =
        {
            new Point(268, 144),
            new Point(322, 144),
            new Point(376, 144),
            new Point(430, 144),
            new Point(484, 144),
        };
        public static double[] DEFAULT_MAX_TABLE_WIDTH_SIZES =
        {
            474,
            518,
            614,
            728,
            867,
            1029,
            1220,
        };
        public static double[] DEFAULT_CHIPS_GAP =
        {
            2,
            2,
            3,
            3,
            4,
            5,
            6,
            7,
        };
        public static Point[] DEFAULT_CHIPS_SIZES =
        {
            new Point(11,10),
            new Point(16,15),
            new Point(18,17),
            new Point(20,18),
            new Point(22,20),
            new Point(26,24),
            new Point(32,30),
            new Point(38,35),
        };
        public static double DEFAULT_PLAYER_RECTANGLE_RADIUSX = 3;
        public static double DEFAULT_PLAYER_RECTANGLE_RADIUSY = 3;
        public static double DEFAULT_PLAYER_RECTANGLE_STROKE_THICKNESS = 1.5;

        //

        public static Point GetPlayerSize(double tableWidth, double tableHeigh)
        {
            return new Point(DEFAULT_PLAYER_SIZE.X / DEFAULT_TABLE_SIZE.X * tableWidth, DEFAULT_PLAYER_SIZE.Y / DEFAULT_TABLE_SIZE.Y * tableHeigh);
        }

        public static Point GetPlayerTextSize(double tableWidth, double tableHeigh)
        {
            return new Point((DEFAULT_PLAYER_SIZE.X - DEFAULT_PLAYER_AVATAR_SIZE.X) / DEFAULT_TABLE_SIZE.X * tableWidth, DEFAULT_PLAYER_SIZE.Y / DEFAULT_TABLE_SIZE.Y * tableHeigh);
        }

        public static Point GetPlayerAvatarSize(double tableWidth, double tableHeigh)
        {
            return new Point(DEFAULT_PLAYER_AVATAR_SIZE.X / DEFAULT_TABLE_SIZE.X * tableWidth, DEFAULT_PLAYER_AVATAR_SIZE.Y / DEFAULT_TABLE_SIZE.Y * tableHeigh);
        }

        public static Point GetPlayerXY(double tableWidth, double tableHeigh, int playerIndex)
        {
            return new Point(DEFAULT_PLAYER_XY[playerIndex].X / DEFAULT_TABLE_SIZE.X * tableWidth, DEFAULT_PLAYER_XY[playerIndex].Y / DEFAULT_TABLE_SIZE.Y * tableHeigh);
        }

        public static Point GetPotXY(double tableWidth, double tableHeigh)
        {
            return new Point(DEFAULT_POT_XY.X / DEFAULT_TABLE_SIZE.X * tableWidth, DEFAULT_POT_XY.Y / DEFAULT_TABLE_SIZE.Y * tableHeigh);
        }

        public static Point GetButtonXY(double tableWidth, double tableHeigh, int playerIndex)
        {
            return new Point(DEFAULT_BUTTON_XY[playerIndex].X / DEFAULT_TABLE_SIZE.X * tableWidth, DEFAULT_BUTTON_XY[playerIndex].Y / DEFAULT_TABLE_SIZE.Y * tableHeigh);
        }

        public static Point GetBetXY(double tableWidth, double tableHeigh, int playerIndex)
        {
            return new Point(DEFAULT_BET_XY[playerIndex].X / DEFAULT_TABLE_SIZE.X * tableWidth, DEFAULT_BET_XY[playerIndex].Y / DEFAULT_TABLE_SIZE.Y * tableHeigh);
        }

        public static int GetSize(double tableWidth)
        {
            for (int i = 0; i < DEFAULT_MAX_TABLE_WIDTH_SIZES.Length; i++)
            {
                if (tableWidth <= DEFAULT_MAX_TABLE_WIDTH_SIZES[i])
                {
                    return i;
                }
            }
            return 7;
        }

        public static double GetChipsGap(int size)
        {
            return DEFAULT_CHIPS_GAP[size];
        }

        public static double GetPlayerRectangleRadiusX(double tableWidth)
        {
            return DEFAULT_PLAYER_RECTANGLE_RADIUSX / DEFAULT_TABLE_SIZE.X * tableWidth;
        }

        public static double GetPlayerRectangleRadiusY(double tableWidth)
        {
            return DEFAULT_PLAYER_RECTANGLE_RADIUSY / DEFAULT_TABLE_SIZE.X * tableWidth;
        }

        public static double GetPlayerRectangleStrokeThickness(double tableWidth)
        {
            return DEFAULT_PLAYER_RECTANGLE_STROKE_THICKNESS / DEFAULT_TABLE_SIZE.X * tableWidth;
        }
    }
}
