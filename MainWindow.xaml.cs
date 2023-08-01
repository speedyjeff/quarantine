using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace quarantine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // initialize
            UITimer = new DispatcherTimer();
            UITimer.Tick += new EventHandler(uiTimer_Tick);
            UITimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            NewsflashTimer = new DispatcherTimer();
            NewsflashTimer.Tick += new EventHandler(tickerTimer_Tick);
            NewsflashTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            Space_Grid.Visibility = System.Windows.Visibility.Collapsed;
            GrayBrush = new SolidColorBrush(Colors.Gray);
            GreenBrush = new SolidColorBrush(Colors.Green);
            BlackBrush = new SolidColorBrush(Colors.Black);
            RedBrush = new SolidColorBrush(Colors.Red);
            YellowBrush = new SolidColorBrush(Colors.Yellow);
            BlueBrush = new SolidColorBrush(Colors.Blue);
            GreyBrush = new SolidColorBrush(Colors.LightGray);
            NeutralBrush = new SolidColorBrush(Color.FromArgb(100, 150, 255, 150));
            CurrentDeterrent = DeterrentType.NONE;
            SumDays = 0;
            NumTries = 0;
            MinDays = Int32.MaxValue;
            MaxDays = Int32.MinValue;
            rPopulation = 0;
            Rand = new Random();
            UIChildren = new Dictionary<string, Rectangle>();

            // setup
            Start(restart: false);

            // configure for board
            BWidth = ((XEnd - XStart) / (double)(Spaces.Length));
            BHeight = ((YEnd - YStart) / (double)Spaces[0].Length);

            // create the game spaces
            var outerContainer = new Grid();

            // add the row definitions
            for (int ver = 0; ver < Spaces.Length; ver++)
            {
                var row = new RowDefinition()
                {
                    Height = new GridLength(1d, GridUnitType.Star)
                };
                outerContainer.RowDefinitions.Add(row);
            }

            // add the column definitions
            for (int hor = 0; hor < Spaces[0].Length; hor++)
            {
                var col = new ColumnDefinition()
                {
                    Width = new GridLength(1d, GridUnitType.Star)
                };
                outerContainer.ColumnDefinitions.Add(col);
            }
            LayoutRoot.Children.Add(outerContainer);

            // create the spaces
            for (int ver = 0; ver < Spaces.Length; ver++)
            {
                for (int hor = 0; hor < Spaces[ver].Length; hor++)
                {
                    if (Spaces[ver][hor] != null)
                    {
                        var rect = new Rectangle();
                        rect.Name = $"Space_{ver}x{hor}";

                        // set row and column
                        Grid.SetRow(rect, ver);
                        Grid.SetColumn(rect, hor);

                        // Set Rectangle's width and color
                        rect.StrokeThickness = 1;
                        if (Spaces[ver][hor].AirDestinations != null && Spaces[ver][hor].AirDestinations.Length > 0)
                        {
                            rect.StrokeThickness = 2;
                            rect.StrokeDashArray = new DoubleCollection() { 2 };
                        }
                        else rect.Stroke = BlueBrush;

                        // Fill rectangle with blue color
                        rect.Fill = NeutralBrush;

                        // setup the callbacks
                        rect.MouseEnter += new MouseEventHandler(rect_MouseEnter);
                        rect.MouseLeftButtonUp += new MouseButtonEventHandler(rect_MouseLeftButtonUp);

                        // Add Rectangle to the Grid
                        outerContainer.Children.Add(rect);
                        UIChildren.Add(rect.Name, rect);
                    }
                }
            }

            // start timer
            UITimer.Start();
            NewsflashTimer.Start();
        }

        #region private
        private QuarantineGame Quarantine;
        private SpaceInfo[][] Spaces;
        private DeterrentType CurrentDeterrent;

        private string NewsFlash = "";
        private const int NewsFlashLen = 400;
        private int NewsflashStart = 0;
        private int NewsflashEnd = 0;
        private int[] DeterrentCounts;
        private double SumDays;
        private double NumTries;
        private double rPopulation;
        private int MaxDays;
        private int MinDays;
        private Random Rand;
        private Dictionary<string, Rectangle> UIChildren;

        private string[] DeterrentTitles = new string[]
            {
                "Street Patrol",
                "Road block",
                "Terminal Screening",
                "Roving Vigilantes",
                "Medics",
                "",
                "Nerve Gas",
                "Nuclear Weapon",
                "Evacuation",
                "Add Infected",
                "Add Population"
            };
        private string[] DeterrentDescriptions = new string[]
            {
                "Official street police incarcerate the infected, and take them off the street before they can infect others.",
                "Stop the infected from crossing borders.  The border patrol take suspicious people off the road.",
                "Screening at all bus and airport terminals. All infected people are quarantined and not allowed to board.",
                "A hired set of vigilantes that are better with guns than with personal safety.",
                "Specialized group of doctors who can vaccinate against the infection.  All recently infected people have the chase to be cured.",
                "",
                "Disperse nerve gas to the general public killing everyone, including the infected.",
                "Drop a small nuclear weapon and wipe out an entire area, infected and all.",
                "Evacuate the healthy population from a city.",
                "Increase the infected to an area.",
                "Increase the population in an area."
            };
        private string[] DeterrentImages = new string[]
            {
                "/quarantine;component/media/streetpatrol.small.png",
                "/quarantine;component/media/roadblock.small.png",
                "/quarantine;component/media/terminal.small.png",
                "/quarantine;component/media/vigilanty.small.png",
                "/quarantine;component/media/hospital.small.png",
                "",
                "/quarantine;component/media/small.png",
                "/quarantine;component/media/large.png",
                "/quarantine;component/media/evaculation.png",
                "/quarantine;component/media/plusi.png",
                "/quarantine;component/media/plusp.png"
            };

        private DispatcherTimer UITimer;
        private DispatcherTimer NewsflashTimer;

        private SolidColorBrush GrayBrush;
        private SolidColorBrush BlueBrush;
        private SolidColorBrush GreyBrush;
        private SolidColorBrush BlackBrush;
        private SolidColorBrush RedBrush;
        private SolidColorBrush YellowBrush;
        private SolidColorBrush GreenBrush;
        private SolidColorBrush NeutralBrush;

        private const double XStart = -400;
        private const double XEnd = 400;
        private const double YStart = -250;
        private const double YEnd = 150;

        private double BWidth;
        private double BHeight;

        //
        // Timers
        //

        private void tickerTimer_Tick(object sender, EventArgs e)
        {
            // display the text
            NewsFlash_Label.Content = GetNewsFlash();
        }

        private void uiTimer_Tick(object sender, EventArgs e)
        {
            // stop the timer
            UITimer.Stop();

            // capture the board
            Spaces = Quarantine.Board;

            // apply auto defense if asked
            if (AutoDefense_Checkbox.IsChecked.Value)
            {
                // apply defenses at random
                for (int i = 0; i < DeterrentCounts.Length; i++)
                {
                    for (int j = 0; j < DeterrentCounts[i]; j++)
                    {
                        int hor, ver;

                        // select random space
                        do
                        {
                            ver = Rand.Next() % Spaces.Length;
                            hor = Rand.Next() % Spaces[0].Length;
                        }
                        while (Spaces[ver][hor] == null);

                        if ((DeterrentType)i != DeterrentType.LENGTH && (DeterrentType)i != DeterrentType.NONE && (DeterrentType)i != DeterrentType.AddPopulation && (DeterrentType)i != DeterrentType.AddInfected)
                        {
                            CurrentDeterrent = (DeterrentType)i;
                            TryApplyDeterrent(ver, hor);
                        }
                    }
                }
            }

            // set globals
            Population_Label.Content = Quarantine.Population.ToString("N").Replace(".00", "");
            Dead_Label.Content = Quarantine.Dead.ToString("N").Replace(".00", "");
            Infected_Label.Content = Quarantine.Infected.ToString("N").Replace(".00", "");
            Causualties_Label.Content = Quarantine.Casualties.ToString("N").Replace(".00", "");
            Day_Label.Content = Quarantine.Day;

            if (NumTries == 0d)
            {
                AverageDays_Label.Content = "0/0/0";
            }
            else
            {
                AverageDays_Label.Content = $"{(int)(SumDays / NumTries)}/{MinDays}/{MaxDays}";
            }

            for (int ver = 0; ver < Spaces.Length; ver++)
            {
                for (int hor = 0; hor < Spaces[ver].Length; hor++)
                {
                    if (Spaces[ver][hor] != null)
                    {
                        // calculate the infection rate
                        var pInfected = 100 * (double)Spaces[ver][hor].Infected / (double)(Spaces[ver][hor].Population + Spaces[ver][hor].Infected);
                        var pDead = 100 * (double)Spaces[ver][hor].Dead / (double)(Spaces[ver][hor].Population + Spaces[ver][hor].Dead);

                        // get the rectangle
                        if (!UIChildren.TryGetValue($"Space_{ver}x{hor}", out var rect)) throw new Exception("unable to find rectangle");

                        // set the neutral color
                        if (Spaces[ver][hor].Infected == 0 && Spaces[ver][hor].Dead == 0)
                        {
                            rect.Fill = NeutralBrush;
                        }

                        // clear the stroke
                        rect.Stroke = BlueBrush;

                        // get infected coloring
                        if (pInfected > 0.0 && pInfected < 1.0) rect.Fill = new SolidColorBrush(Color.FromArgb(100, 100, 0, 0));
                        else if (pInfected == 100.0 || Spaces[ver][hor].Population == 0) rect.Fill = new SolidColorBrush(Colors.Black);
                        else if (pInfected > 0.0) rect.Fill = new SolidColorBrush(Color.FromArgb(200, (byte)(155 + pInfected), 0, 0));
                        else if (pDead > 0.5) rect.Fill = new SolidColorBrush(Color.FromArgb(200, 0, 50, 50));
                        else if (pDead >= 1.0) rect.Fill = new SolidColorBrush(Colors.Black);

                        // check if there are deterrents
                        for (int i = 0; i < Spaces[ver][hor].Deterrents.Length; i++)
                        {
                            if (Spaces[ver][hor].Deterrents[i])
                            {
                                rect.Stroke = YellowBrush;
                                break;
                            }
                        }
                    }
                }
            }

            // check for auto next day
            if (Auto_Nextday.IsChecked.Value && Quarantine.Population > 0 && (Quarantine.Infected > 0 || Quarantine.Day == 0))
            {
                // run the next day
                NextDay();
            }

            if ((Quarantine.Population == 0 || Quarantine.Infected == 0) && Restart_Checkbox.IsChecked.Value)
            {
                Start(true);
            }
        }

        //
        // Game play
        //

        private string GetNewsFlash()
        {
            string display = "";

            lock (NewsFlash)
            {
                // get the subset
                if (NewsflashEnd < NewsflashStart)
                {
                    // there are two segments
                    display = NewsFlash.Substring(NewsflashStart, NewsFlash.Length - NewsflashStart);
                    display += NewsFlash.Substring(0, NewsflashEnd);
                }
                else
                {
                    display = NewsFlash.Substring(NewsflashStart, NewsflashEnd - NewsflashStart);
                }

                // increment
                NewsflashStart = (NewsflashStart + 1) % NewsFlash.Length;
                NewsflashEnd = (NewsflashEnd + 1) % NewsFlash.Length;
            }

            return display;
        }

        private void SetNewsFlash()
        {
            var newNews = "";

            // grab the new board
            Spaces = Quarantine.Board;

            switch (Quarantine.Day)
            {
                case 0:
                    GregorianCalendar g = new GregorianCalendar();
                    newNews = $"... Today is {DateTime.Now.DayOfWeek} {DateTime.Now}, outside is sunny and warm";
                    newNews += "... Defenses' are reported as a way the best way to stop the spread of the infection";
                    newNews += $"... The global population has hit a new high today of {Quarantine.Population.ToString("N").Replace(".00", "")}";
                    break;
                default:
                    var newInfection = new List<string>();
                    var epidemic = new List<string>();
                    var gone = new List<string>();
                    var unique = new List<string>();

                    if (Quarantine.Population == 0)
                    {
                        newNews = "... This is the last transmission before signing off, it is reported that the rest of the world has been wiped out by 'THEM', so long and good luck";
                    }
                    else if (Quarantine.Infected == 0)
                    {
                        newNews = "... The people that are left are flooding the streets around the globe in celebration of the end of the infection";
                        newNews += "... The 'cure' has taken and there are no new reports of infection in the world";
                        newNews += "... There are reports of people in ";
                        for (int ver = 0; ver < Spaces.Length; ver++)
                        {
                            for (int hor = 0; hor < Spaces[ver].Length; hor++)
                            {
                                if (Spaces[ver][hor] != null && Spaces[ver][hor].Population > 0)
                                {
                                    for (int i = 0; i < Spaces[ver][hor].Names.Length; i++)
                                        if (!unique.Contains(Spaces[ver][hor].Names[i]))
                                        {
                                            newNews += Spaces[ver][hor].Names[i] + ", ";
                                            unique.Add(Spaces[ver][hor].Names[i]);
                                        }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int ver = 0; ver < Spaces.Length; ver++)
                        {
                            for (int hor = 0; hor < Spaces[ver].Length; hor++)
                            {
                                if (Spaces[ver][hor] != null)
                                {
                                    if (Spaces[ver][hor].Infected > 0 && Spaces[ver][hor].Infected < 100)
                                    {
                                        for (int i = 0; i < Spaces[ver][hor].Names.Length; i++)
                                            if (!newInfection.Contains(Spaces[ver][hor].Names[i]))
                                                newInfection.Add(Spaces[ver][hor].Names[i]);
                                    }
                                    else if (Spaces[ver][hor].Population == 0)
                                    {
                                        for (int i = 0; i < Spaces[ver][hor].Names.Length; i++)
                                            if (!gone.Contains(Spaces[ver][hor].Names[i]))
                                                gone.Add(Spaces[ver][hor].Names[i]);
                                    }
                                    else if (Spaces[ver][hor].Infected > Spaces[ver][hor].Population || Spaces[ver][hor].Infected > 100)
                                    {
                                        for (int i = 0; i < Spaces[ver][hor].Names.Length; i++)
                                            if (!epidemic.Contains(Spaces[ver][hor].Names[i]))
                                                epidemic.Add(Spaces[ver][hor].Names[i]);
                                    }
                                }
                            }
                        }

                        // add news
                        var len = Math.Max(newInfection.Count, epidemic.Count);
                        len = Math.Max(len, gone.Count);

                        var index = 0;
                        newNews = "";
                        if (Quarantine.Day >= 3 && Quarantine.Day < 6) newNews += "... The infection is highly communitive and extremely dangerous, avoid those that are sick";
                        while (index < len)
                        {
                            if (index < newInfection.Count)
                            {
                                if (Quarantine.Day < 3) newNews += "... A strange infection has been reported in the following regions, the authorities advice to stay in doors... ";
                                else newNews += "... The infection has spread and is now reported in the following regions, the authorities advice to evacuate... ";
                                for (int i = index; i < (index + 5) && i < newInfection.Count; i++) newNews += $" {newInfection[i]}, ";
                            }
                            if (index < epidemic.Count)
                            {
                                newNews += "... The infection has over run the following regions: the authorities say 'GET OUT NOW'... ";
                                for (int i = index; i < (index + 5) && i < epidemic.Count; i++) newNews += $" {epidemic[i]}, ";
                            }
                            if (index < gone.Count)
                            {
                                newNews += "... The sick referred to as 'THEM' have devastated the following regions: everyone is dead... ";
                                for (int i = index; i < (index + 5) && i < gone.Count; i++) newNews += $" {gone[i]}, ";
                            }
                            index += 5;
                        }
                    }
                    break;
            }

            // pad the news if less than NewsFlashLen
            while (newNews.Length < NewsFlashLen) newNews += $"     {newNews}";

            // set newsFlash
            lock (NewsFlash)
            {
                // setup the newsflash
                NewsflashStart = 0;
                NewsflashEnd = newNews.Length - 1;
                NewsFlash = newNews;
                NewsFlash_Label.Content = NewsFlash;
            }
        }

        private bool TryApplyDeterrent(int ver, int hor)
        {
            if (CurrentDeterrent == DeterrentType.NONE) return false;

            // exit early if there are no remaining deterrents to apply
            if (DeterrentCounts[(int)CurrentDeterrent] <= 0) return false;

            // apply deterrent
            if (CurrentDeterrent != DeterrentType.NONE)
            {
                // apply the deterrent
                if (Quarantine.TryApplyDeterrent(ver, hor, CurrentDeterrent))
                {
                    // keep count
                    DeterrentCounts[(int)CurrentDeterrent]--;

                    return true;
                }
            }

            return false;
        }

        private SpaceInfo GetSpaceInfo(string name, out int ver, out int hor)
        {
            // Space_verxhor
            var parts = name.Split('_');
            if (parts.Length != 2) throw new Exception("invalid parts");
            parts = parts[1].Split('x');
            if (parts.Length != 2) throw new Exception("invalid parts");

            ver = Convert.ToInt32(parts[0]);
            hor = Convert.ToInt32(parts[1]);

            return Spaces[ver][hor];
        }

        private void NextDay()
        {
            // clear the selection
            CurrentDeterrent = DeterrentType.NONE;
            for (int i = 0; i < DeterrentCounts.Length; i++) DeterrentCounts[i] += ContainmentUIConstants.PerDeterrentIncrease;

            // advance to the next day
            Quarantine.NextDay();

            // grab the next days new headlines
            SetNewsFlash();

            // restart uiTimer
            UITimer.Start();
        }

        private void DisplayDeterrent(DeterrentType det)
        {
            DeteriantTitle_Label.Content = DeterrentTitles[(int)det];
            DeteriantDesc_TextBlock.Text = DeterrentDescriptions[(int)det];
            DeteriantIcon_Image.Source = new BitmapImage(new Uri(DeterrentImages[(int)det], UriKind.RelativeOrAbsolute));
            DeteriantItems_Label.Content = DeterrentCounts[(int)det];

            DeteriantPopulation_Label.Foreground = RedBrush;
            DeteriantInfected_Label.Foreground = RedBrush;

            if (det == DeterrentType.AddPopulation)
            {
                DeteriantPopulation_Label.Foreground = GreenBrush;
                DeteriantPopulation_Label.Content = "+" + QuarantineConstants.AdditionalPopulation.ToString();
                DeteriantInfected_Label.Content = 0;
            }
            else if (det == DeterrentType.AddInfected)
            {
                DeteriantPopulation_Label.Content = 0;
                DeteriantInfected_Label.Foreground = GreenBrush;
                DeteriantInfected_Label.Content = "+" + QuarantineConstants.AdditionalInfected.ToString();
            }
            else if (det == DeterrentType.Medics)
            {
                DeteriantPopulation_Label.Content = 0;
                DeteriantInfected_Label.Foreground = GreenBrush;
                DeteriantInfected_Label.Content = "+" + (int)(QuarantineConstants.InfectedBlocker[(int)det] * 100) + "%";
            }
            else
            {
                // all the rest
                DeteriantPopulation_Label.Content = "-" + (int)(QuarantineConstants.PopulationCapture[(int)det] * 100) + "%";
                DeteriantInfected_Label.Content = "-" + (int)(QuarantineConstants.InfectedCapture[(int)det] * 100) + "%";
            }

            DeteriantNoSpaces_Label.Content = QuarantineConstants.DeterrentProximity[(int)det].ToString();
        }

        private void SelectDeterrent(DeterrentType det)
        {
            // set the selection
            CurrentDeterrent = det;
        }

        private void Start(bool restart)
        {
            // collect stats
            if (restart)
            {
                NumTries++;
                SumDays += Quarantine.Day;
                MinDays = Math.Min(Quarantine.Day, MinDays);
                MaxDays = Math.Max(Quarantine.Day, MaxDays);
                rPopulation += Quarantine.Population;
            }

            // setup
            CurrentDeterrent = DeterrentType.NONE;
            Quarantine = new QuarantineGame();
            Spaces = Quarantine.Board;
            DeterrentCounts = new int[(int)DeterrentType.NONE];
            for (int i = 0; i < DeterrentCounts.Length; i++) DeterrentCounts[i] = ContainmentUIConstants.InitialDeterrent;

            // set defaults
            SelectDeterrent(DeterrentType.StreetPatrol);
            DisplayDeterrent(DeterrentType.StreetPatrol);

            // create the initial string for the news flash
            SetNewsFlash();

            if (restart)
            {
                UITimer.Start();
            }
        }

        //
        // Callbacks
        //
        private void rect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var rect = (Rectangle)sender;
            var space = GetSpaceInfo(rect.Name, out var ver, out var hor);

            if (TryApplyDeterrent(ver, hor))
            {
                // update the stats
                DisplayDeterrent(CurrentDeterrent);

                // update the details
                rect_MouseEnter((object)rect, e);

                // change the border color
                rect.Stroke = RedBrush;
            }
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Space_Grid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void rect_MouseEnter(object sender, MouseEventArgs e)
        {
            var rect = (Rectangle)sender;
            var space = GetSpaceInfo(rect.Name, out var ver, out var hor);

            // set the data
            SpaceName_Label.Content = "";
            for (int i = 0; i < space.Names.Length; i++)
            {
                SpaceName_Label.Content += space.Names[i];
                if (i < space.Names.Length - 1) SpaceName_Label.Content += ", ";
            }
            SpacePopulation_Label.Content = space.Population.ToString("N").Replace(".00", "");
            SpaceInfected_Label.Content = space.Infected.ToString("N").Replace(".00", "");
            SpaceDead_Label.Content = space.Dead.ToString("N").Replace(".00", "");
            SpaceAir_Label.Content = "";
            if (space.AirDestinations != null)
            {
                // change the label
                Airport_Label.Foreground = BlackBrush;

                // display the names
                for (int i = 0; i < space.AirDestinations.Length; i++)
                {
                    SpaceAir_Label.Content += space.AirDestinations[i];
                    if (i < space.AirDestinations.Length - 1) SpaceAir_Label.Content += ", ";
                }
            }
            else
            {
                // gray out the label
                Airport_Label.Foreground = GrayBrush;
            }

            // light-up for the deterrents
            SpaceStreet_Image.Visibility = System.Windows.Visibility.Collapsed;
            SpaceRoad_Image.Visibility = System.Windows.Visibility.Collapsed;
            SpaceVigilanty_Image.Visibility = System.Windows.Visibility.Collapsed;
            SpaceMedic_Image.Visibility = System.Windows.Visibility.Collapsed;
            SpaceAir_Image.Visibility = System.Windows.Visibility.Collapsed;
            for (int i = 0; i < space.Deterrents.Length; i++)
            {
                if (space.Deterrents[i])
                {
                    switch ((DeterrentType)i)
                    {
                        case DeterrentType.StreetPatrol:
                            SpaceStreet_Image.Visibility = System.Windows.Visibility.Visible;
                            break;
                        case DeterrentType.RoadBlock:
                            SpaceRoad_Image.Visibility = System.Windows.Visibility.Visible;
                            break;
                        case DeterrentType.RovingVigilantly:
                            SpaceVigilanty_Image.Visibility = System.Windows.Visibility.Visible;
                            break;
                        case DeterrentType.Medics:
                            SpaceMedic_Image.Visibility = System.Windows.Visibility.Visible;
                            break;
                        case DeterrentType.TerminalScreening:
                            SpaceAir_Image.Visibility = System.Windows.Visibility.Visible;
                            break;
                        default:
                            throw new Exception("Unexpected input: " + (DeterrentType)i);
                    }
                }
            }

            // position the grid
            var point = rect.TranslatePoint(new Point(0d, 0d), Application.Current.MainWindow);
            var trans = new TranslateTransform()
            {
                X = point.X + BWidth,
                Y = point.Y + BHeight
            };
            Space_Grid.RenderTransform = trans;

            // make it visible
            Space_Grid.Visibility = System.Windows.Visibility.Visible;
        }

        private void NextDay_Button_Click(object sender, RoutedEventArgs e)
        {
            NextDay();
        }

        private void Hide_Button_Click(object sender, RoutedEventArgs e)
        {
            // show the settings
            Settings_Grid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void StreetPatrol_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.StreetPatrol);
            DisplayDeterrent(DeterrentType.StreetPatrol);
        }

        private void RoadBlock_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.RoadBlock);
            DisplayDeterrent(DeterrentType.RoadBlock);
        }

        private void Vigilantly_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.RovingVigilantly);
            DisplayDeterrent(DeterrentType.RovingVigilantly);
        }

        private void Medic_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.Medics);
            DisplayDeterrent(DeterrentType.Medics);
        }

        private void AirSearch_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.TerminalScreening);
            DisplayDeterrent(DeterrentType.TerminalScreening);
        }

        private void SmallTerminate_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.TerminateSmall);
            DisplayDeterrent(DeterrentType.TerminateSmall);
        }

        private void LargeTerminate_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.TerminateLarge);
            DisplayDeterrent(DeterrentType.TerminateLarge);
        }

        private void Evacuation_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.Evacuate);
            DisplayDeterrent(DeterrentType.Evacuate);
        }

        private void PlusInfected_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.AddInfected);
            DisplayDeterrent(DeterrentType.AddInfected);
        }

        private void PlusPopulation_Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectDeterrent(DeterrentType.AddPopulation);
            DisplayDeterrent(DeterrentType.AddPopulation);
        }

        private void Auto_Nextday_Checked(object sender, RoutedEventArgs e)
        {
            // start the game play
            UITimer.Interval = new TimeSpan(0, 0, 1);
            UITimer.Start();
        }

        private void Auto_Nextday_Unchecked(object sender, RoutedEventArgs e)
        {
            // speed up the timer again
            UITimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
        }

        private void Restart_Button_Click(object sender, RoutedEventArgs e)
        {
            Start(true);
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            // setting defaults
            SettingsInitial_Slider.Value = ContainmentUIConstants.InitialDeterrent;
            SettingsInitial_Label.Content = ContainmentUIConstants.InitialDeterrent;
            SettingsPerDay_Slider.Value = ContainmentUIConstants.PerDeterrentIncrease;
            SettingsPerDay_Label.Content = ContainmentUIConstants.PerDeterrentIncrease;
            SettingsDaysToDeath_Slider.Value = QuarantineConstants.DaysToDeath;
            SettingsDaysToDeath_Label.Content = QuarantineConstants.DaysToDeath;
            SettingsSPPop_Slider.Value = QuarantineConstants.PopulationCapture[(int)DeterrentType.StreetPatrol];
            SettingsSPPop_Label.Content = (int)(QuarantineConstants.PopulationCapture[(int)DeterrentType.StreetPatrol] * 100) + "%";
            SettingsSPInf_Slider.Value = QuarantineConstants.InfectedCapture[(int)DeterrentType.StreetPatrol];
            SettingsSPInf_Label.Content = (int)(QuarantineConstants.InfectedCapture[(int)DeterrentType.StreetPatrol] * 100) + "%";
            SettingsRBPop_Slider.Value = QuarantineConstants.PopulationCapture[(int)DeterrentType.RoadBlock];
            SettingsRBPop_Label.Content = (int)(QuarantineConstants.PopulationCapture[(int)DeterrentType.RoadBlock] * 100) + "%";
            SettingsRBInf_Slider.Value = QuarantineConstants.InfectedCapture[(int)DeterrentType.RoadBlock];
            SettingsRBInf_Label.Content = (int)(QuarantineConstants.InfectedCapture[(int)DeterrentType.RoadBlock] * 100) + "%";
            SettingsRVPop_Slider.Value = QuarantineConstants.PopulationCapture[(int)DeterrentType.RovingVigilantly];
            SettingsRVPop_Label.Content = (int)(QuarantineConstants.PopulationCapture[(int)DeterrentType.RovingVigilantly] * 100) + "%";
            SettingsRVInf_Slider.Value = QuarantineConstants.InfectedCapture[(int)DeterrentType.RovingVigilantly];
            SettingsRVInf_Label.Content = (int)(QuarantineConstants.InfectedCapture[(int)DeterrentType.RovingVigilantly] * 100) + "%";
            SettingsMInf_Slider.Value = QuarantineConstants.InfectedBlocker[(int)DeterrentType.Medics];
            SettingsMInf_Label.Content = (int)(QuarantineConstants.InfectedBlocker[(int)DeterrentType.Medics] * 100) + "%";
            SettingsTSPop_Slider.Value = QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminalScreening];
            SettingsTSPop_Label.Content = (int)(QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminalScreening] * 100) + "%";
            SettingsTSInf_Slider.Value = QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminalScreening];
            SettingsTSInf_Label.Content = (int)(QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminalScreening] * 100) + "%";
            SettingsNGPop_Slider.Value = QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminateSmall];
            SettingsNGPop_Label.Content = (int)(QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminateSmall] * 100) + "%";
            SettingsNGInf_Slider.Value = QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminateSmall];
            SettingsNGInf_Label.Content = (int)(QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminateSmall] * 100) + "%";
            SettingsNWPop_Slider.Value = QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminateLarge];
            SettingsNWPop_Label.Content = (int)(QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminateLarge] * 100) + "%";
            SettingsNWInf_Slider.Value = QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminateLarge];
            SettingsNWInf_Label.Content = (int)(QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminateLarge] * 100) + "%";
            SettingsEPop_Slider.Value = QuarantineConstants.PopulationCapture[(int)DeterrentType.Evacuate];
            SettingsEPop_Label.Content = (int)(QuarantineConstants.PopulationCapture[(int)DeterrentType.Evacuate] * 100) + "%";
            SettingsAIInf_Slider.Value = QuarantineConstants.AdditionalInfected;
            SettingsAIInf_Label.Content = QuarantineConstants.AdditionalInfected;
            SettingsAPPop_Slider.Value = QuarantineConstants.AdditionalPopulation;
            SettingsAPPop_Label.Content = QuarantineConstants.AdditionalPopulation;
            SettingsInfRegions_Slider.Value = QuarantineConstants.InitialSpaces;
            SettingsInfRegions_Label.Content = QuarantineConstants.InitialSpaces;
            SettingsInfPRegion_Slider.Value = QuarantineConstants.InitialInfected;
            SettingsInfPRegion_Label.Content = QuarantineConstants.InitialInfected;

            // show the settings
            Settings_Grid.Visibility = System.Windows.Visibility.Visible;
        }

        private void SettingsInitial_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsInitial_Label.Content = (int)SettingsInitial_Slider.Value;
            ContainmentUIConstants.InitialDeterrent = (int)SettingsInitial_Slider.Value;
        }

        private void SettingsPerDay_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsPerDay_Label.Content = (int)SettingsPerDay_Slider.Value;
            ContainmentUIConstants.PerDeterrentIncrease = (int)SettingsPerDay_Slider.Value;
        }

        private void SettingsDaysToDeath_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SettingsDaysToDeath_Slider == null || SettingsDaysToDeath_Label == null) return;
            SettingsDaysToDeath_Label.Content = (int)SettingsDaysToDeath_Slider.Value;
            QuarantineConstants.DaysToDeath = (int)SettingsDaysToDeath_Slider.Value;
        }

        private void SettingsSPPop_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsSPPop_Label.Content = (int)(SettingsSPPop_Slider.Value * 100) + "%";
            QuarantineConstants.PopulationCapture[(int)DeterrentType.StreetPatrol] = SettingsSPPop_Slider.Value;
        }

        private void SettingsSPInf_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsSPInf_Label.Content = (int)(SettingsSPInf_Slider.Value * 100) + "%";
            QuarantineConstants.InfectedCapture[(int)DeterrentType.StreetPatrol] = SettingsSPInf_Slider.Value;
        }

        private void SettingsRBPop_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsRBPop_Label.Content = (int)(SettingsRBPop_Slider.Value * 100) + "%";
            QuarantineConstants.PopulationCapture[(int)DeterrentType.RoadBlock] = SettingsRBPop_Slider.Value;
        }

        private void SettingsRBInf_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsRBInf_Label.Content = (int)(SettingsRBInf_Slider.Value * 100) + "%";
            QuarantineConstants.InfectedCapture[(int)DeterrentType.RoadBlock] = SettingsRBInf_Slider.Value;
        }

        private void SettingsRVPop_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsRVPop_Label.Content = (int)(SettingsRVPop_Slider.Value * 100) + "%";
            QuarantineConstants.PopulationCapture[(int)DeterrentType.RovingVigilantly] = SettingsRVPop_Slider.Value;
        }

        private void SettingsRVInf_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsRVInf_Label.Content = (int)(SettingsRVInf_Slider.Value * 100) + "%";
            QuarantineConstants.InfectedCapture[(int)DeterrentType.RovingVigilantly] = SettingsRVInf_Slider.Value;
        }

        private void SettingsMInf_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsMInf_Label.Content = (int)(SettingsMInf_Slider.Value * 100) + "%";
            QuarantineConstants.InfectedBlocker[(int)DeterrentType.Medics] = SettingsMInf_Slider.Value;
        }

        private void SettingsTSPop_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsTSPop_Label.Content = (int)(SettingsTSPop_Slider.Value * 100) + "%";
            QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminalScreening] = SettingsTSPop_Slider.Value;
        }

        private void SettingsTSInf_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsTSInf_Label.Content = (int)(SettingsTSInf_Slider.Value * 100) + "%";
            QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminalScreening] = SettingsTSInf_Slider.Value;
        }

        private void SettingsNGPop_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsNGPop_Label.Content = (int)(SettingsNGPop_Slider.Value * 100) + "%";
            QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminateSmall] = SettingsNGPop_Slider.Value;
        }

        private void SettingsNGInf_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsNGInf_Label.Content = (int)(SettingsNGInf_Slider.Value * 100) + "%";
            QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminateSmall] = SettingsNGInf_Slider.Value;
        }

        private void SettingsNWPop_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsNWPop_Label.Content = (int)(SettingsNWPop_Slider.Value * 100) + "%";
            QuarantineConstants.PopulationCapture[(int)DeterrentType.TerminateLarge] = SettingsNWPop_Slider.Value;
        }

        private void SettingsNWInf_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsNWInf_Label.Content = (int)(SettingsNWInf_Slider.Value * 100) + "%";
            QuarantineConstants.InfectedCapture[(int)DeterrentType.TerminateLarge] = SettingsNWInf_Slider.Value;
        }

        private void SettingsEPop_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsEPop_Label.Content = (int)(SettingsEPop_Slider.Value * 100) + "%";
            QuarantineConstants.PopulationCapture[(int)DeterrentType.Evacuate] = SettingsEPop_Slider.Value;
        }

        private void SettingsAIInf_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SettingsAIInf_Slider == null) return;
            SettingsAIInf_Label.Content = (int)(SettingsAIInf_Slider.Value);
            QuarantineConstants.AdditionalInfected = (int)SettingsAIInf_Slider.Value;
        }

        private void SettingsAPPop_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SettingsAPPop_Label.Content = (int)(SettingsAPPop_Slider.Value);
            QuarantineConstants.AdditionalPopulation = (int)SettingsAPPop_Slider.Value;
        }

        private void SettingsInfRegions_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SettingsInfRegions_Slider == null || SettingsInfRegions_Label == null) return;
            SettingsInfRegions_Label.Content = (int)(SettingsInfRegions_Slider.Value);
            QuarantineConstants.InitialSpaces = (int)(SettingsInfRegions_Slider.Value);
        }

        private void SettingsInfPRegion_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SettingsInfPRegion_Slider == null || SettingsInfPRegion_Label == null) return;
            SettingsInfPRegion_Label.Content = (int)(SettingsInfPRegion_Slider.Value);
            QuarantineConstants.InitialInfected = (int)(SettingsInfPRegion_Slider.Value);
        }
        #endregion
    }
}