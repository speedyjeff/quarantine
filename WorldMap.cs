using System;

namespace quarantine
{
    class WorldMap : IMap
    {
        public WorldMap()
        {
            // sanity check the surface area
            var surfaceArea = 0;
            for (int i = 0; i < Regions.Length; i++)
            {
                surfaceArea += Regions[i].SurfaceArea;
            }

            if (surfaceArea != Countries.Length * Countries[0].Length)
            {
                throw new Exception($"Incorrect surface area: {surfaceArea} != {Countries.Length * Countries[0].Length}");
            }

            // initialize the space array
            Spaces = new Space[Countries.Length][];

            for (int ver = 0; ver < Spaces.Length; ver++)
            {
                Spaces[ver] = new Space[Countries[ver].Length];
                for (int hor = 0; hor < Spaces[ver].Length; hor++)
                {
                    int id = Countries[ver][hor];

                    // add the airport routes during construction
                    if (ver == 12 && hor == 9) Spaces[ver][hor] = new Space(id, Regions[id].Population / Regions[id].SurfaceArea, new Route[] { new Route(8, 26) }); // Califorina
                    else if (ver == 8 && hor == 26) Spaces[ver][hor] = new Space(id, Regions[id].Population / Regions[id].SurfaceArea, new Route[] { new Route(12, 9), new Route(28, 45) }); // England
                    else if (ver == 28 && hor == 45) Spaces[ver][hor] = new Space(id, Regions[id].Population / Regions[id].SurfaceArea, new Route[] { new Route(8, 26) }); // Australia
                    else if (id != 0 && id != 18) Spaces[ver][hor] = new Space(id, Regions[id].Population / Regions[id].SurfaceArea);
                    else Spaces[ver][hor] = null;
                }
            }
        }

        public override string[] Information(int id)
        {
            if (id <= 0 || id > Regions.Length - 1) throw new Exception("Access invalid item in Regions array");
            return Regions[id].RegionNames;
        }

        #region private
        private Region[] Regions = new Region[]
        {
            // id, surfaceArea, population (in millions), names
            new Region(0, 1781, 0, new string[] {""}),
            new Region(1, 46, 306.8, new string[] {"United States"}),
            new Region(2, 53, 33.7, new string[] {"Canada"}),
            new Region(3, 5, 92.2, new string[] {"Philippines"}),
            new Region(4, 25, 0.05, new string[] {"Greenland"}),
            new Region(5, 8, 0.3+7.3+14.0+7.5+109.6+5.7, new string[] {"Belize", "El Salvador", "Guatemala", "Honduras", "Mexico", "Nicaragua"}),
            new Region(6, 3, 4.5+3.5, new string[] {"Costa rica", "Panama"}),
            new Region(7, 5, 45.1, new string[] {"Colombia"}),
            new Region(8, 1, 13.6, new string[] {"Ecuador"}),
            new Region(9, 5, 29.2, new string[] {"Peru"}),
            new Region(10, 4, 28.4, new string[] {"Venezuela"}),
            new Region(11, 34, 191.5, new string[] {"Brazil"}),
            new Region(12, 2, 0.8, new string[] {"Guyana"}),
            new Region(13, 1, 0.2+0.5, new string[] {"French Guiana", "Suriname"}),
            new Region(14, 4, 9.9, new string[] {"Bolivia"}),
            new Region(15, 1, 6.3, new string[] {"Paraguay"}),
            new Region(16, 17, 40.3+17.0, new string[] {"Argentina", "Chile"}),
            new Region(17, 1, 3.4, new string[] {"Uruguay"}),
            new Region(18, 114, 0, new string[] {"Antarctic"}),
            new Region(19, 1, 10.6, new string[] {"Portugal"}),
            new Region(20, 3, 46.9, new string[] {"Spain"}),
            new Region(21, 3, 62.6, new string[] {"France"}),
            new Region(22, 4, 4.5+61.8, new string[] {"Ireland", "United Kingdom"}),
            new Region(23, 1, 10.8+16.5, new string[] {"Belgium", "Netherlands"}),
            new Region(24, 2, 82.0, new string[] {"Germany"}),
            new Region(25, 2, 60.3, new string[] {"Italy"}),
            new Region(26, 1, 10.0+38.1+5.4, new string[] {"Hungary", "Poland", "Slovakia"}),
            new Region(27, 1, 8.4+7.8+10.0, new string[] {"Austria", "Switzerland", "Czech Republic"}),
            new Region(28, 1, 3.8, new string[] {"Bosnia-Herzegovina"}),
            new Region(29, 1, 3.2+11.3, new string[] {"Albania", "Greece"}),
            new Region(30, 1, 7.6, new string[] {"Bulgaria"}),
            new Region(31, 1, 21.5+4.1, new string[] {"Romania", "Moldova"}),
            new Region(32, 1, 9.7, new string[] {"Belarus"}),
            new Region(33, 1, 1.3+2.3+3.3, new string[] {"Estonia", "Latvia", "Lithuania"}),
            new Region(34, 8, 5.3+4.8+9.3, new string[] {"Finland", "Norway", "Sweden"}),
            new Region(35, 1, 5.5, new string[] {"Denmark"}),
            new Region(36, 93, 141.8, new string[] {"Russia"}),
            new Region(37, 6, 127.6, new string[] {"Japan"}),
            new Region(38, 1, 22.7, new string[] {"North Korea"}),
            new Region(39, 1, 48.7, new string[] {"South Korea"}),
            new Region(40, 7, 2.7, new string[] {"Mongolia"}),
            new Region(41, 35, 1331.4+7.0+0.6, new string[] {"China"}),
            new Region(42, 11, 15.9, new string[] {"Kazakhstan"}),
            new Region(43, 3, 5.1+27.6, new string[] {"Turkmenistan", "Uzbekistan"}),
            new Region(44, 1, 5.3+7.5, new string[] {"Kyrgyzstan", "Tajikistan"}),
            new Region(45, 1, 3.1+8.8+4.6, new string[] {"Armenia", "Azerbaijan", "Georgia"}),
            new Region(46, 3, 74.8, new string[] {"Turkey"}),
            new Region(47, 2, 46.0, new string[] {"Ukraine"}),
            new Region(48, 1, 1.1+3.9, new string[] {"Cyprus", "Lebanon"}),
            new Region(49, 1, 7.6+5.9+21.9+3.9, new string[] {"Israel", "Jordan", "Syria", "Palestinian Territory"}),
            new Region(50, 2, 30.0, new string[] {"Iraq"}),
            new Region(51, 6, 73.2, new string[] {"Iran"}),
            new Region(52, 8, 1.2+3.0+1.4+5.1, new string[] {"Bahrain", "Kuwait", "Qatar", "United Arab Emirates"}),
            new Region(53, 5, 3.1+22.9, new string[] {"Oman", "Yemen"}),
            new Region(54, 2, 28.4, new string[] {"Afghanistan"}),
            new Region(55, 5, 180.8, new string[] {"Pakistan"}),
            new Region(56, 12, 1171.0+20.5, new string[] {"India", "Sri Lanka"}),
            new Region(57, 1, 27.5, new string[] {"Nepal"}),
            new Region(58, 2, 162.2+0.7, new string[] {"Bangladesh", "Bhutan"}),
            new Region(59, 3, 50.0+67.8, new string[] {"Myanmar", "Thailand"}),
            new Region(60, 1, 6.3, new string[] {"Laos"}),
            new Region(61, 3, 14.8+67.8, new string[] {"Cambodia", "Thailand"}),
            new Region(62, 2, 87.3, new string[] {"Vietnam"}),
            new Region(63, 1, 31.5, new string[] {"Morocco"}),
            new Region(64, 1, 0.5, new string[] {"Western Sahara"}),
            new Region(65, 5, 3.3, new string[] {"Mauritania"}),
            new Region(66, 9, 35.4, new string[] {"Algeria"}),
            new Region(67, 4, 13.0+15.8, new string[] {"Mali", "Burkina Faso"}),
            new Region(68, 1, 12.5+1.6, new string[] {"Senegal", "Gambia"}),
            new Region(69, 2, 10.1+1.6+5.7, new string[] {"Guinea", "Guinan-Bissau", "Sierra Leone"}),
            new Region(70, 1, 4.0, new string[] {"Liberia"}),
            new Region(71, 2, 23.8+6.6, new string[] {"Ghana", "Togo"}),
            new Region(72, 8, 6.3+10.4, new string[] {"Libya", "Tunisia"}),
            new Region(73, 2, 78.6, new string[] {"Egypt"}),
            new Region(74, 5, 15.3+8.9, new string[] {"Niger", "Benin"}),
            new Region(75, 3, 152.6, new string[] {"Nigeria"}),
            new Region(76, 4, 10.3, new string[] {"Chad"}),
            new Region(77, 8, 42.3, new string[] {"Sudan"}),
            new Region(78, 4, 5.1+82.8, new string[] {"Eritrea", "Ethiopia"}),
            new Region(79, 4, 0.9+9.1, new string[] {"Djibouti", "Somalia"}),
            new Region(80, 2, 39.1, new string[] {"Kenya"}),
            new Region(81, 2, 8.3+9.9+30.7, new string[] {"Burundi", "Rwanda", "Uganda"}),
            new Region(82, 2, 4.5, new string[] {"Central African Republic"}),
            new Region(83, 2, 18.9, new string[] {"Cameroon"}),
            new Region(84, 1, 1.5, new string[] {"Gabon"}),
            new Region(85, 3, 19.5, new string[] {"Madagascar"}),
            new Region(86, 8, 3.7+68.7, new string[] {"Congo", "Democratic Republic of the Congo"}),
            new Region(87, 5, 43.7, new string[] {"Tanzania"}),
            new Region(88, 1, 14.2+22.0, new string[] {"Malawi", "Mozambique"}),
            new Region(89, 3, 12.6, new string[] {"Zambia"}),
            new Region(90, 6, 17.1, new string[] {"Angola"}),
            new Region(91, 4, 2.2, new string[] {"Namibia"}),
            new Region(92, 2, 2.0, new string[] {"Botswana"}),
            new Region(93, 1, 12.5, new string[] {"Zimbabwe"}),
            new Region(94, 8, 2.1+50.7+1.2, new string[] {"Lesotho", "South Africa", "Swaziland"}),
            new Region(95, 31, 21.9, new string[] {"Australia"}),
            new Region(96, 1, 23.1, new string[] {"Taiwan"}),
            new Region(97, 13, 243.3+28.3+5.1, new string[] {"Indonesia", "Malaysia", "Singapore"}),
        };

        private int[][] Countries = new int[][] {
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 0, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,36,36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0,36,36,36,36,36,36,36, 0,36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 4, 4, 4, 4, 0, 0, 0,34,34,34, 0,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 2, 2, 0, 0, 2, 0, 0, 4, 0, 0, 4, 0, 0,34,34, 0,34,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 1, 1, 0, 2, 2, 2, 2, 2, 2, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0,34,34, 0,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36,36, 0,36, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0,22,22, 0, 0,35,33,36,36,36,36,36,36,36,36,36,36,36,36,36,36, 0, 0,36, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0,22,22,23,24,26,32,47,36,36,36,42,42,42,36,36,36,36,36,41,36,36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 2, 2, 2, 0, 2, 0, 0, 0, 0, 0, 0,21,21,24,27,31,47,36,36,42,42,42,42,42,40,40,40,40,41,41,36,37, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0,20,20,21,25,28,30, 0,36,36,42,43,42,42,41,41,40,40,40,41,41,36,37, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,19,20, 0,25, 0,29,46,46,45, 0,43,43,44,41,41,41,41,41,41,41,38,37,37, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,66, 0, 0, 0,46,48,50,51,51,54,55,41,41,41,41,41,41, 0,39,37,37, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,63,66,66,72, 0, 0, 0,49,50,51,51,54,55,56,41,41,41,41,41,41, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 5, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,66,66,66,72,72,72,73,52,52,51,51,55,55,56,57,41,41,41,41,41, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 5, 5, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0,64,65,66,66,72,72,72,73,52,52,52, 0,55,56,56,56,58,59,41,41,41,96, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,65,65,67,66,74,76,72,77, 0,52,52,53, 0,56,56,56,58,59,60,41, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,65,65,67,74,74,76,77,77, 0,52,53,53, 0, 0,56,56, 0,59,61, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,68,67,67,74,74,76,77,77,78,53,53, 0, 0, 0,56, 0, 0, 0,61,62, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 6,10,10, 0, 0, 0, 0, 0, 0, 0,69,69,71,75,75,76,77,77,78, 0,79, 0, 0, 0,56, 0, 0, 0,61,62, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7,10,12, 0, 0, 0, 0, 0, 0, 0,70,71,75,83,82,82,77,78,78,79, 0, 0, 0, 0, 0, 0, 0,97, 0,97, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7,10,12,13, 0, 0, 0, 0, 0, 0, 0, 0, 0,83,86,86,81,80,79,79, 0, 0, 0, 0, 0, 0, 0,97, 0,97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 7,11,11,11, 0, 0, 0, 0, 0, 0, 0, 0, 0,84,86,86,81,80, 0, 0, 0, 0, 0, 0, 0, 0, 0,97,97,97,97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9,11,11,11,11,11,11,11, 0, 0, 0, 0, 0, 0,86,86,86,87,87, 0, 0, 0, 0, 0, 0, 0, 0, 0,97,97, 0,97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9,11,11,11,11,11,11,11, 0, 0, 0, 0, 0, 0,90,90,86,87,87, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,97,97, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9, 9,14,11,11,11,11, 0, 0, 0, 0, 0, 0, 0, 0,90,90,89,87, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,95, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9,14,11,11,11,11, 0, 0, 0, 0, 0, 0, 0,90,90,89,89,88,85, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,95,95,95, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,14,14,11,11,11, 0, 0, 0, 0, 0, 0, 0,91,91,92,93, 0,85, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,95,95,95,95, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,15,11,11, 0, 0, 0, 0, 0, 0, 0, 0, 0,91,92,94, 0,85, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,95,95,95,95,95,95, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,16,11,11, 0, 0, 0, 0, 0, 0, 0, 0, 0,91,94,94, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,95,95,95,95,95,95, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,16,11,11, 0, 0, 0, 0, 0, 0, 0, 0, 0,94,94,94, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,95,95,95,95,95,95, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,16,17, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,94,94, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,95, 0, 0,95,95, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,16,16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,95,95, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16,16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,18, 0, 0,18, 0, 0,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,18, 0, 0, 0, 0, 0, 0,18, 0,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,18,18,18,18,18,18,18,18, 0, 0, 0,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,18,18,18,18,18,18,18,18,18, 0, 0,18, 0,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18,18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        };
        #endregion
    }
}
