using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using PsHandler.Custom;
using PsHandler.Import;
using PsHandler.PokerTypes;

namespace PsHandler.Hud
{
    public class TableHud : IDisposable
    {
        private readonly Thread _thread;
        public TableSize TableSize = TableSize.Default;
        public long TournamentNumber = -1;
        public long LastHandNumber = -1;
        public decimal Sb, Bb, Ante;
        private PokerType _pokerType;
        //
        public Table Table;
        public WindowTimer WindowTimer;
        public WindowBigBlind WindowBigBlind;
        //
        private static readonly Regex _regexSbBb = new Regex(@".+Blinds .{0,1}(?<sb>[\d\.]+)\/.{0,1}(?<bb>[\d\.]+) - Tournament (?<tournament_number>\d+).+Logged In as (?<hero>.+)");
        private static readonly Regex _regexSbBbAnte = new Regex(@".+Blinds .{0,1}(?<sb>[\d\.]+)\/.{0,1}(?<bb>[\d\.]+) Ante .{0,1}(?<ante>[\d\.]+) - Tournament (?<tournament_number>\d+).+Logged In as (?<hero>.+)");


        public TableHud(Table table)
        {
            Methods.UiInvoke(() =>
            {
                Table = table;
                WindowTimer = new WindowTimer(Table);
                WindowTimer.Show();
                WindowBigBlind = new WindowBigBlind(Table);
                WindowBigBlind.Show();
            });

            _thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        string title = Table.Title, className = Table.ClassName, textboxBigBlindContent = Config.BigBlindHHNotFound, textboxTimerContent = Config.TimerHHNotFound, hero = "";
                        decimal sb = decimal.MinValue, bb = decimal.MinValue, ante = decimal.MinValue, bigBlindMValue = 0;
                        bool bigBlindMIsSet = false;
                        Match match = _regexSbBb.Match(title);
                        if (!match.Success)
                        {
                            match = _regexSbBbAnte.Match(title);
                            if (match.Success)
                            {
                                ante = decimal.Parse(match.Groups["ante"].Value);
                            }
                        }
                        if (match.Success)
                        {
                            sb = decimal.Parse(match.Groups["sb"].Value);
                            bb = decimal.Parse(match.Groups["bb"].Value);
                            long toutnamentNumber = long.Parse(match.Groups["tournament_number"].Value);
                            hero = match.Groups["hero"].Value;

                            Tournament tournament = App.HandHistoryManager.GetTournament(toutnamentNumber);
                            if (tournament != null)
                            {
                                #region BigBlind

                                TableSize tableSize = tournament.GetLastHandTableSize();
                                decimal latestStack = tournament.GetLatestStack(hero);
                                if (latestStack != decimal.MinValue)
                                {
                                    if (!Config.BigBlindShowTournamentM)
                                    {
                                        bigBlindMIsSet = true;
                                        bigBlindMValue = Math.Round(latestStack / bb, Config.BigBlindDecimals);
                                    }
                                    else
                                    {
                                        if (Config.BigBlindMByPlayerCount)
                                        {
                                            bigBlindMIsSet = true;
                                            bigBlindMValue = Math.Round(latestStack / (sb + bb + ante * tournament.GetLastHandPlayerCountAfterHand()), Config.BigBlindDecimals);
                                        }
                                        else if (Config.BigBlindMByTableSize)
                                        {
                                            bigBlindMIsSet = true;
                                            bigBlindMValue = Math.Round(latestStack / (sb + bb + ante * (decimal)tableSize), Config.BigBlindDecimals);
                                        }
                                    }
                                    string decimalFormat = "0:0.";
                                    for (int i = 0; i < Config.BigBlindDecimals; i++) decimalFormat += "0";
                                    textboxBigBlindContent = Config.BigBlindPrefix + string.Format("{" + decimalFormat + "}", bigBlindMValue) + Config.BigBlindPostfix;
                                }

                                #endregion

                                #region Timer

                                int pokerTypeErrors = -1;
                                if (_pokerType == null)
                                {
                                    _pokerType = PokerTypeManager.GetPokerType(title, className, out pokerTypeErrors);
                                    if (pokerTypeErrors != 0)
                                    {
                                        _pokerType = null;
                                    }
                                }
                                if (_pokerType != null)
                                {
                                    DateTime dateTimeNow = DateTime.Now.AddSeconds(-Config.TimerDiff);
                                    DateTime dateTimeNextLevel = tournament.GetFirstHandTimestamp();
                                    while (dateTimeNextLevel < dateTimeNow) dateTimeNextLevel = dateTimeNextLevel.AddSeconds(_pokerType.LevelLength.TotalSeconds);
                                    TimeSpan timeSpan = dateTimeNextLevel - dateTimeNow;
                                    textboxTimerContent = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                                    Methods.UiInvoke(() =>
                                    {
                                        WindowTimer.UCLabel_Main.SetToolTip(_pokerType.Name);
                                    });
                                }
                                else
                                {
                                    if (pokerTypeErrors == 1)
                                    {
                                        textboxTimerContent = Config.TimerPokerTypeNotFound;
                                    }
                                    else if (pokerTypeErrors == 2)
                                    {
                                        textboxTimerContent = Config.TimerMultiplePokerTypes;
                                    }
                                    else
                                    {
                                        textboxTimerContent = string.Format("Unknown Error");
                                    }
                                }

                                #endregion
                            }
                        }

                        #region GUI Update

                        Methods.UiInvoke(() =>
                        {
                            // default client size: 792 x 546

                            WindowTimer.UCLabel_Main.SetText(textboxTimerContent);
                            WindowTimer.Left = Table.RectangleClient.X + (double)Table.RectangleClient.Width * TableManager.GetHudTimerLocationX(TableSize, this);
                            WindowTimer.Top = Table.RectangleClient.Y + (double)Table.RectangleClient.Height * TableManager.GetHudTimerLocationY(TableSize, this);
                            WindowTimer.UCLabel_Main.SetBackground(Config.HudTimerBackground);
                            WindowTimer.UCLabel_Main.SetForeground(Config.HudTimerForeground);
                            WindowTimer.UCLabel_Main.SetFontFamily(Config.HudTimerFontFamily);
                            WindowTimer.UCLabel_Main.SetFontWeight(Config.HudTimerFontWeight);
                            WindowTimer.UCLabel_Main.SetFontStyle(Config.HudTimerFontStyle);
                            WindowTimer.UCLabel_Main.SetFontSize(Config.HudTimerFontSize, Table.RectangleClient.Height / 546.0);
                            WindowTimer.UCLabel_Main.SetMargin(Config.HudTimerMargin, Table.RectangleClient.Height / 546.0);
                            WindowTimer.Opacity = TableManager.EnableHudTimer ? 1 : 0;

                            WindowBigBlind.UCLabel_Main.SetText(textboxBigBlindContent);
                            WindowBigBlind.Left = Table.RectangleClient.X + (double)Table.RectangleClient.Width * TableManager.GetHudBigBlindLocationX(TableSize, this);
                            WindowBigBlind.Top = Table.RectangleClient.Y + (double)Table.RectangleClient.Height * TableManager.GetHudBigBlindLocationY(TableSize, this);
                            WindowBigBlind.UCLabel_Main.SetBackground(Config.HudBigBlindBackground);
                            WindowBigBlind.UCLabel_Main.SetForeground(ColorByValue.GetColorByValue(Config.HudBigBlindForeground, bigBlindMValue, bigBlindMIsSet ? Config.HudBigBlindColorsByValue : new List<ColorByValue>()));
                            WindowBigBlind.UCLabel_Main.SetFontFamily(Config.HudBigBlindFontFamily);
                            WindowBigBlind.UCLabel_Main.SetFontWeight(Config.HudBigBlindFontWeight);
                            WindowBigBlind.UCLabel_Main.SetFontStyle(Config.HudBigBlindFontStyle);
                            WindowBigBlind.UCLabel_Main.SetFontSize(Config.HudBigBlindFontSize, Table.RectangleClient.Height / 546.0);
                            WindowBigBlind.UCLabel_Main.SetMargin(Config.HudBigBlindMargin, Table.RectangleClient.Height / 546.0);
                            WindowBigBlind.Opacity = TableManager.EnableHudBigBlind ? 1 : 0;
                        });

                        #endregion

                        Thread.Sleep(250);

                        // Ensure visibility
                        Methods.UiInvoke(() =>
                        {
                            WindowTimer.Visibility = TableManager.EnableHudTimer ? Visibility.Visible : Visibility.Collapsed;
                            WindowBigBlind.Visibility = TableManager.EnableHudBigBlind ? Visibility.Visible : Visibility.Collapsed;
                        });
                    }
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
                        Methods.DisplayException(e);
                    }
                }
#endif
                finally
                {
                    Methods.UiInvoke(() =>
                    {
                        WindowTimer.Close();
                        WindowBigBlind.Close();
                    });
                }
            });
            _thread.Start();
        }

        public void Dispose()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
            }
        }
    }
}
