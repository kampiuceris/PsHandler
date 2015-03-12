// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsHandler.Custom;

namespace PsHandler.Randomizer
{
    /// <summary>
    /// Interaction logic for WindowRandomizer.xaml
    /// </summary>
    public partial class WindowRandomizer : Window
    {
        private Thread _thread;

        public WindowRandomizer(double value)
        {
            InitializeComponent();

            for (int i = 0; i < 5; i++)
            {
                Grid_Info.Children.Add(new Label
                {
                    Width = 100,
                    Height = 16,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Foreground = Brushes.White,
                    Content = string.Format("{0:0%} chance", value),
                    Padding = new Thickness(0),
                    Effect = new DropShadowEffect
                    {
                        ShadowDepth = 0,
                        Color = Colors.Black,
                        Opacity = 1,
                        BlurRadius = 5,
                        RenderingBias = RenderingBias.Quality
                    }
                });
            }

            Border_Yes.Width = value * 100;
            Border_No.Width = (1.0 - value) * 100;

            double randomNumber = Methods.GetRandomNumber(0, 100) / 100.0;
            bool yes;
            if (randomNumber < value)
            {
                // yes
                yes = true;
                Border_No.Opacity = 0.3;
            }
            else
            {
                // no
                yes = false;
                Border_Yes.Opacity = 0.3;
            }

            System.Drawing.Point mp = System.Windows.Forms.Control.MousePosition;
            Left = mp.X - 50;
            Top = mp.Y + 20;

            Loaded += (sender, args) =>
            {
                _thread = new Thread(() =>
                {
                    try
                    {
                        if (yes)
                        {
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Sound.EmbeddedResources.yes.wav"));
                            player.Play();
                        }
                        else
                        {
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Sound.EmbeddedResources.no.wav"));
                            player.Play();
                        }

                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        while (sw.Elapsed.TotalSeconds < 1)
                        {
                            System.Drawing.Point mousePosition = System.Windows.Forms.Control.MousePosition;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Left = mousePosition.X - 50;
                                Top = mousePosition.Y + 20;
                            });
                            Thread.Sleep(10);
                        }
                        sw.Stop();
                    }
#if (DEBUG)
                    catch (ThreadInterruptedException)
                    {
                    }
#else
                    catch (Exception e)
                    {
                        if (!(e is ThreadInterruptedException))
                        {
                            Methods.DisplayException(e, App.WindowMain, WindowStartupLocation.CenterOwner);
                        }
                    }
#endif
                    finally
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Close();
                        });
                    }
                });
                _thread.Start();
            };

            Closing += (sender, args) =>
            {
                if (_thread != null)
                {
                    _thread.Interrupt();
                }
            };
        }
    }
}
