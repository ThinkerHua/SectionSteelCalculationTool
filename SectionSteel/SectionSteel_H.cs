﻿/*==============================================================================
 *  SectionSteelCalculationTool - A tool that assists Excel in calculating 
 *  quantities of steel structures
 *
 *  Copyright © 2024 Huang YongXing.                 
 *
 *  This library is free software, licensed under the terms of the GNU 
 *  General Public License as published by the Free Software Foundation, 
 *  either version 3 of the License, or (at your option) any later version. 
 *  You should have received a copy of the GNU General Public License 
 *  along with this program. If not, see <http://www.gnu.org/licenses/>. 
 *==============================================================================
 *  SectionSteel_H.cs: H型钢
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>H型钢。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.H_1"/>: <inheritdoc cref="Pattern_Collection.H_1"/><br/>
    /// <see cref="Pattern_Collection.H_2"/>: <inheritdoc cref="Pattern_Collection.H_2"/><br/>
    /// <see cref="Pattern_Collection.H_3"/>: <inheritdoc cref="Pattern_Collection.H_3"/><br/>
    /// <see cref="Pattern_Collection.H_4"/>: <inheritdoc cref="Pattern_Collection.H_4"/><br/>
    /// <see cref="Pattern_Collection.H_5"/>: <inheritdoc cref="Pattern_Collection.H_5"/><br/>
    /// <see cref="Pattern_Collection.H_6"/>: <inheritdoc cref="Pattern_Collection.H_6"/><br/>
    /// <see cref="Pattern_Collection.H_7"/>: <inheritdoc cref="Pattern_Collection.H_7"/><br/>
    /// </summary>
    /// <remarks>当匹配到H3模式时，在国标截面特性表格中查找，同一型号名下有多项时按以下规则进行匹配：
    /// <list type="number">
    ///     <item>
    ///         匹配<b>非星标型号</b>，例如：<br/>
    ///         HW200*200型号名下，有H200*200*8*12和H200*204*12*12两种型号，其中第1种不带星标，第2种带星标。此时匹配第1种。
    ///     </item>
    ///     <item>
    ///         如均为星标型号，则匹配<b>参数与型号名一致的型号</b>，例如：<br/>
    ///         HM550*300型号名下，有H544*300*11*15和H550*300*11*18两种型号，均带星标，但第2种参数与型号名一致。此时匹配第2种。
    ///     </item>
    ///     <item>
    ///         如<b>均不符合</b>，则匹配<b>第1项</b>，例如：<br/>
    ///         HW500*500型号名下，有H492*465*15*20、H502*465*15*25、H502*470*20*25三种型号，均带星标，且三种参数均与型号名不一致。此时匹配第1种。
    ///     </item>
    /// </list>
    /// </remarks>
    public class SectionSteel_H : SectionSteelBase {
        private string type = string.Empty;
        private double h1, h2, b1, b2, s, t1, t2;
        private GBData data;
        private static readonly GBData[] _gbDataSet_HW = new GBData[] {
            new GBData("100*100", new double[] { 100, 100, 6,8 }, 16.9, 0.574),
            new GBData("125*125", new double[] { 125, 125, 6.5,9 }, 23.6, 0.723),
            new GBData("150*150", new double[] { 150, 150, 7,10 }, 31.1, 0.872),
            new GBData("175*175", new double[] { 175, 175, 7.5,11 }, 40.4, 1.01),
            new GBData("200*200", new double[] { 200, 200, 8,12 }, 49.9, 1.16),
            new GBData("", new double[] { 200, 204, 12,12 }, 56.2, 1.17),
            new GBData("", new double[] { 244, 252, 11,11 }, 63.8, 1.45),
            new GBData("250*250", new double[] { 250, 250, 9,14 }, 71.8, 1.46),
            new GBData("", new double[] { 250, 255, 14,14 }, 81.6, 1.47),
            new GBData("", new double[] { 294, 302, 12,12 }, 83.5, 1.75),
            new GBData("300*300", new double[] { 300, 300, 10,15 }, 93, 1.76),
            new GBData("", new double[] { 300, 305, 15,15 }, 105, 1.77),
            new GBData("", new double[] { 338, 351, 13,13 }, 105, 2.03),
            new GBData("", new double[] { 344, 348, 10,16 }, 113, 2.04),
            new GBData("", new double[] { 344, 354, 16,16 }, 129, 2.05),
            new GBData("350*350", new double[] { 350, 350, 12,19 }, 135, 2.05),
            new GBData("", new double[] { 350, 357, 19,19 }, 154, 2.07),
            new GBData("", new double[] { 388, 402, 15,15 }, 140, 2.32),
            new GBData("", new double[] { 394, 398, 11,18 }, 147, 2.32),
            new GBData("", new double[] { 394, 405, 18,18 }, 168, 2.33),
            new GBData("400*400", new double[] { 400, 400, 13,21 }, 172, 2.34),
            new GBData("", new double[] { 400, 408, 21,21 }, 197, 2.35),
            new GBData("", new double[] { 414, 405, 18,28 }, 232, 2.37),
            new GBData("", new double[] { 428, 407, 20,35 }, 283, 2.41),
            new GBData("", new double[] { 458, 417, 30,50 }, 415, 2.49),
            new GBData("", new double[] { 498, 432, 45,70 }, 604, 2.6),
            new GBData("500*500", new double[] { 492, 465, 15,20 }, 202, 2.78),
            new GBData("", new double[] { 502, 465, 15,25 }, 239, 2.8),
            new GBData("", new double[] { 502, 470, 20,25 }, 259, 2.81),
        };
        private static readonly GBData[] _gbDataSet_HM = new GBData[] {
            new GBData("150*100", new double[] { 148, 100, 6,9 }, 20.7, 0.67),
            new GBData("200*150", new double[] { 194, 150, 6,9 }, 29.9, 0.962),
            new GBData("250*175", new double[] { 244, 175, 7,11 }, 43.6, 1.15),
            new GBData("300*200", new double[] { 294, 200, 8,12 }, 55.8, 1.35),
            new GBData("", new double[] { 298, 201, 9,14 }, 64.4, 1.36),
            new GBData("350*250", new double[] { 340, 250, 9,14 }, 78.1, 1.64),
            new GBData("400*300", new double[] { 390, 300, 10,16 }, 105, 1.94),
            new GBData("450*300", new double[] { 440, 300, 11,18 }, 121, 2.04),
            new GBData("", new double[] { 482, 300, 11,15 }, 111, 2.12),
            new GBData("500*300", new double[] { 488, 300, 11,18 }, 125, 2.13),
            new GBData("", new double[] { 544, 300, 11,15 }, 116, 2.24),
            new GBData("550*300", new double[] { 550, 300, 11,18 }, 130, 2.26),
            new GBData("", new double[] { 582, 300, 12,17 }, 133, 2.32),
            new GBData("600*300", new double[] { 588, 300, 12,20 }, 147, 2.33),
            new GBData("", new double[] { 594, 302, 14,23 }, 170, 2.35),
        };
        private static readonly GBData[] _gbDataSet_HN = new GBData[] {
            new GBData("100*50", new double[] { 100, 50, 5,7 }, 9.3, 0.376),
            new GBData("125*60", new double[] { 125, 60, 6,8 }, 13.1, 0.464),
            new GBData("150*75", new double[] { 150, 75, 5,7 }, 14, 0.576),
            new GBData("175*90", new double[] { 175, 90, 5,8 }, 18, 0.686),
            new GBData("", new double[] { 198, 99, 4.5,7 }, 17.8, 0.769),
            new GBData("200*100", new double[] { 200, 100, 5.5,8 }, 20.9, 0.775),
            new GBData("", new double[] { 248, 124, 5,8 }, 25.1, 0.968),
            new GBData("250*125", new double[] { 250, 125, 6,9 }, 29, 0.974),
            new GBData("", new double[] { 298, 149, 5.5,8 }, 32, 1.16),
            new GBData("300*150", new double[] { 300, 150, 6.5,9 }, 36.7, 1.16),
            new GBData("", new double[] { 346, 174, 6,9 }, 41.2, 1.35),
            new GBData("350*175", new double[] { 350, 175, 7,11 }, 49.4, 1.36),
            new GBData("400*150", new double[] { 400, 150, 8,13 }, 55.2, 1.36),
            new GBData("", new double[] { 396, 199, 7,11 }, 56.1, 1.55),
            new GBData("400*200", new double[] { 400, 200, 8,13 }, 65.4, 1.56),
            new GBData("", new double[] { 446, 150, 7,12 }, 52.6, 1.46),
            new GBData("450*150", new double[] { 450, 151, 8,14 }, 60.8, 1.47),
            new GBData("", new double[] { 446, 199, 8,12 }, 65.1, 1.65),
            new GBData("450*200", new double[] { 450, 200, 9,14 }, 74.9, 1.66),
            new GBData("", new double[] { 470, 150, 7,13 }, 56.2, 1.5),
            new GBData("", new double[] { 475, 151.5, 8.5,15.5 }, 67.6, 1.52),
            new GBData("475*150", new double[] { 482, 153.5, 10.5,19 }, 83.5, 1.53),
            new GBData("", new double[] { 492, 150, 7,12 }, 55.1, 1.55),
            new GBData("", new double[] { 500, 152, 9,16 }, 72.4, 1.57),
            new GBData("500*150", new double[] { 504, 153, 10,18 }, 81.1, 1.58),
            new GBData("", new double[] { 496, 199, 9,14 }, 77.9, 1.75),
            new GBData("500*200", new double[] { 500, 200, 10,16 }, 88.1, 1.76),
            new GBData("", new double[] { 506, 201, 11,19 }, 102, 1.77),
            new GBData("", new double[] { 546, 199, 9,14 }, 81.5, 1.85),
            new GBData("550*200", new double[] { 550, 200, 10,16 }, 92, 1.86),
            new GBData("", new double[] { 596, 199, 10,15 }, 92.4, 1.95),
            new GBData("600*200", new double[] { 600, 200, 11,17 }, 103, 1.96),
            new GBData("", new double[] { 606, 201, 12,20 }, 118, 1.97),
            new GBData("", new double[] { 625, 198.5, 13.5,17.5 }, 118, 1.99),
            new GBData("625*200", new double[] { 630, 200, 15,20 }, 133, 2.01),
            new GBData("", new double[] { 638, 202, 17,24 }, 156, 2.03),
            new GBData("", new double[] { 646, 299, 12,18 }, 144, 2.43),
            new GBData("650*300", new double[] { 650, 300, 13,20 }, 159, 2.44),
            new GBData("", new double[] { 654, 301, 14,22 }, 173, 2.45),
            new GBData("", new double[] { 692, 300, 13,20 }, 163, 2.53),
            new GBData("700*300", new double[] { 700, 300, 13,24 }, 182, 2.54),
            new GBData("", new double[] { 734, 299, 12,16 }, 143, 2.61),
            new GBData("", new double[] { 742, 300, 13,20 }, 168, 2.63),
            new GBData("750*300", new double[] { 750, 300, 13,24 }, 187, 2.64),
            new GBData("", new double[] { 758, 303, 16,28 }, 224, 2.67),
            new GBData("", new double[] { 792, 300, 14,22 }, 188, 2.73),
            new GBData("800*300", new double[] { 800, 300, 14,26 }, 207, 2.74),
            new GBData("", new double[] { 834, 298, 14,19 }, 179, 2.8),
            new GBData("", new double[] { 842, 299, 15,23 }, 204, 2.82),
            new GBData("850*300", new double[] { 850, 300, 16,27 }, 229, 2.84),
            new GBData("", new double[] { 858, 301, 17,31 }, 255, 2.86),
            new GBData("", new double[] { 890, 299, 15,23 }, 210, 2.92),
            new GBData("900*300", new double[] { 900, 300, 16,28 }, 240, 2.94),
            new GBData("", new double[] { 912, 302, 18,34 }, 283, 2.97),
            new GBData("", new double[] { 970, 297, 16,21 }, 217, 3.07),
            new GBData("", new double[] { 980, 298, 17,26 }, 248, 3.09),
            new GBData("", new double[] { 990, 298, 17,31 }, 271, 3.11),
            new GBData("1000*300", new double[] { 1000, 300, 19,36 }, 310, 3.13),
            new GBData("", new double[] { 1008, 302, 21,40 }, 345, 3.15),
        };
        private static readonly GBData[] _gbDataSet_HT = new GBData[] {
            new GBData("", new double[] { 95, 48, 3.2,4.5 }, 5.98, 0.362),
            new GBData("100*50", new double[] { 97, 49, 4,5.5 }, 7.36, 0.368),
            new GBData("100*100", new double[] { 96, 99, 4.5,6 }, 12.7, 0.565),
            new GBData("", new double[] { 118, 58, 3.2,4.5 }, 7.26, 0.448),
            new GBData("125*60", new double[] { 120, 59, 4,5.5 }, 8.94, 0.454),
            new GBData("125*125", new double[] { 119, 123, 4.5,6 }, 15.8, 0.707),
            new GBData("", new double[] { 145, 73, 3.2,4.5 }, 9, 0.562),
            new GBData("150*75", new double[] { 147, 74, 4,5.5 }, 11.1, 0.568),
            new GBData("", new double[] { 139, 97, 3.2,4.5 }, 10.6, 0.646),
            new GBData("150*100", new double[] { 142, 99, 4.5,6 }, 14.3, 0.657),
            new GBData("", new double[] { 144, 148, 5,7 }, 21.8, 0.856),
            new GBData("150*150", new double[] { 147, 149, 6,8.5 }, 26.4, 0.864),
            new GBData("", new double[] { 168, 88, 3.2,4.5 }, 10.6, 0.668),
            new GBData("175*90", new double[] { 171, 89, 4,6 }, 13.8, 0.676),
            new GBData("", new double[] { 167, 173, 5,7 }, 26.2, 0.994),
            new GBData("175*175", new double[] { 172, 175, 6.5,9.5 }, 35, 1.01),
            new GBData("", new double[] { 193, 98, 3.2,4.5 }, 12, 0.758),
            new GBData("200*100", new double[] { 196, 99, 4,6 }, 15.5, 0.766),
            new GBData("200*150", new double[] { 188, 149, 4.5,6 }, 20.7, 0.949),
            new GBData("200*200", new double[] { 192, 198, 6,8 }, 34.3, 1.14),
            new GBData("250*125", new double[] { 244, 124, 4.5,6 }, 20.3, 0.961),
            new GBData("250*175", new double[] { 238, 173, 4.5,8 }, 30.7, 1.14),
            new GBData("300*150", new double[] { 294, 148, 4.5,6 }, 25, 1.15),
            new GBData("300*200", new double[] { 286, 198, 6,8 }, 38.7, 1.33),
            new GBData("350*175", new double[] { 340, 173, 4.5,6 }, 29, 1.34),
            new GBData("400*150", new double[] { 390, 148, 6,8 }, 37.3, 1.34),
            new GBData("400*200", new double[] { 390, 198, 6,8 }, 43.6, 1.54),
        };
        public override GBData[] GBDataSet {
            get {
                switch (type) {
                case "HW":
                    return _gbDataSet_HW;
                case "HM":
                    return _gbDataSet_HM;
                case "HN":
                    return _gbDataSet_HN;
                case "HT":
                    return _gbDataSet_HT;
                default:
                    if (data == null) return null;
                    if (FindGBData(_gbDataSet_HW, data.Parameters) != null) return _gbDataSet_HW;
                    if (FindGBData(_gbDataSet_HM, data.Parameters) != null) return _gbDataSet_HM;
                    if (FindGBData(_gbDataSet_HN, data.Parameters) != null) return _gbDataSet_HN;
                    if (FindGBData(_gbDataSet_HT, data.Parameters) != null) return _gbDataSet_HT;
                    return null;
                }
            }
        }

        public SectionSteel_H() { }
        public SectionSteel_H(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = (type, h1, h2, b1, b2, s, t1, t2, data);
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.H_1);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.H_2);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.H_4);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.H_5);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.H_6);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.H_7);
                if (match.Success) {
                    double.TryParse(match.Groups["h1"].Value, out h1);
                    double.TryParse(match.Groups["h2"].Value, out h2);
                    double.TryParse(match.Groups["b1"].Value, out b1);
                    double.TryParse(match.Groups["b2"].Value, out b2);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double.TryParse(match.Groups["t1"].Value, out t1);
                    double.TryParse(match.Groups["t2"].Value, out t2);

                    type = string.Empty;

                    if (!(h2 != 0 && h2 != h1
                        || b2 != 0 && b2 != b1
                        || t2 != 0 && t2 != t1))
                        data = FindGBData(type, h1, b1, s, t1);
                    else
                        data = null;
                } else {
                    match = Regex.Match(e.NewText, Pattern_Collection.H_3);
                    if (!match.Success)
                        throw new MismatchedProfileTextException(e.NewText);

                    type = match.Groups["TYPE"].Value;
                    var name = match.Groups["H"] + "*" + match.Groups["B"];
                    data = FindGBData(type, name);
                    if (data == null) {
                        double.TryParse(match.Groups["H"].Value, out double H);
                        double.TryParse(match.Groups["B"].Value, out double B);
                        data = FindGBData(type, H, B);
                    }
                    if (data == null)
                        throw new MismatchedProfileTextException(e.NewText);

                    h1 = data.Parameters[0];
                    b1 = data.Parameters[1];
                    s = data.Parameters[2];
                    t1 = data.Parameters[3];
                }

                if (h2 == 0) h2 = h1;
                if (b2 == 0) b2 = b1;
                if (t2 == 0) t2 = t1;

                h1 *= 0.001; h2 *= 0.001; b1 *= 0.001; b2 *= 0.001; s *= 0.001; t1 *= 0.001; t2 *= 0.001;
            } catch (MismatchedProfileTextException) {
                type = tmp.type; h1 = tmp.h1; h2 = tmp.h2;
                b1 = tmp.b1; b2 = tmp.b2; s = tmp.s;
                t1 = tmp.t1; t2 = tmp.t2; data = tmp.data;
                throw;
            }
        }
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                //byte key = 0b0000;
                //if (h2 != 0 && h2 != h1)
                //    key |= 0b0001;
                //if (b2 != 0 && b2 != b1)
                //    key |= 0b0010;
                //if (exclude_topSurface)
                //    key |= 0b0100;

                //switch (key) {
                //case 0b0000:
                //    formula = $"{h1}*2+{b1}*4";
                //    break;
                //case 0b0100:
                //    formula = $"{h1}*2+{b1}*3";
                //    break;
                //case 0b0010:
                //    formula = $"{h1}*2+{b1}*2+{b2}*2";
                //    break;
                //case 0b0110:
                //    formula = $"{h1}*2+{b1}+{b2}*2";
                //    break;
                //case 0b0001:
                //    formula = $"{h1}+{h2}+{b1}*4";
                //    break;
                //case 0b0101:
                //    formula = $"{h1}+{h2}+{b1}*3";
                //    break;
                //case 0b0011:
                //    formula = $"{h1}+{h2}+{b1}*2+{b2}*2";
                //    break;
                //case 0b0111:
                //    formula = $"{h1}+{h2}+{b1}+{b2}*2";
                //    break;
                //default:
                //    break;
                //}
                if (h2 != h1) {
                    formula = h1 + "+" + h2;
                } else {
                    formula = h1 + "*2";
                }
                if (b2 != b1) {
                    if (exclude_topSurface) {
                        formula += "+" + b1 + "+" + b2 + "*2";
                    } else {
                        formula += "+" + b1 + "*2+" + b2 + "*2";
                    }
                } else {
                    if (exclude_topSurface) {
                        formula += "+" + b1 + "*3";
                    } else {
                        formula += "+" + b1 + "*4";
                    }
                }
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{s}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    break;
                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b1}";
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                //byte key = 0b0000;
                //if(h2 != 0 && h2 != h1)
                //    key |= 0b0001;
                //if(b2 != 0 && b2 != b1)
                //    key |= 0b0010;
                //if(t2 != 0 && t2 != t1)
                //    key |= 0b0100;

                //switch(key) {
                //case 0b0000:
                //    formula = $"(({h1}-{t1}*2)*{s}+{b1}*{t1}*2)*{DENSITY}";
                //    break;
                //case 0b0001:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}*2)*{s}+{b1}*{t1}*2)*{DENSITY}";
                //    break;
                //case 0b0010:
                //    formula = $"(({h1}-{t1}*2)*{s}+({b1}+{b2})*{t1})*{DENSITY}";
                //    break;
                //case 0b0011:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}*2)*{s}+({b1}+{b2})*{t1})*{DENSITY}";
                //    break;
                //case 0b0100:
                //    formula = $"(({h1}-{t1}-{t2})*{s}+{b1}*({t1}+{t2}))*{DENSITY}";
                //    break;
                //case 0b0101:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}-{t2})*{s}+{b1}*({t1}+{t2}))*{DENSITY}";
                //    break;
                //case 0b0110:
                //    formula = $"(({h1}-{t1}-{t2})*{s}+{b1}*{t1}+{b2}*{t2})*{DENSITY}";
                //    break;
                //case 0b0111:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}-{t2})*{s}+{b1}*{t1}+{b2}*{t2})*{DENSITY}";
                //    break;
                //default:
                //    break;
                //}
                if (h2 != h1) {
                    formula = "((" + (h1 + h2) / 2;
                } else {
                    formula = "((" + h1;
                }
                if (t2 != t1) {
                    formula += "-" + t1 + "-" + t2;
                } else {
                    formula += "-" + t1 + "*2";
                }
                formula += ")*" + s;
                if (b2 != b1) {
                    if (t2 != t1) {
                        formula += "+" + b1 + "*" + t1 + "+" + b2 + "*" + t2 + ")";
                    } else {
                        formula += "+(" + b1 + "+" + b2 + ")*" + t1 + ")";
                    }
                } else {
                    if (t2 != t1) {
                        formula += "+" + b1 + "*(" + t1 + "+" + t2 + "))";
                    } else {
                        formula += "+" + b1 + "*" + t1 + "*2)";
                    }
                }
                formula += "*" + DENSITY;
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}";
                break;
            default:
                break;
            }

            return formula;
        }

        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = ((b1 + b2) * 0.5 - s) * 0.5;
            l = (h1 + h2) * 0.5 - t1 - t2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        private GBData FindGBData(string type, string byName) {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(byName)) {
                throw new ArgumentException($"“{nameof(byName)}”不能为 null 或空。", nameof(byName));
            }

            GBData data;
            switch (type) {
            case "HW":
                data = FindGBData(_gbDataSet_HW, byName);
                break;
            case "HM":
                data = FindGBData(_gbDataSet_HM, byName);
                break;
            case "HN":
                data = FindGBData(_gbDataSet_HN, byName);
                break;
            case "HT":
                data = FindGBData(_gbDataSet_HT, byName);
                break;
            case "H":
            default:
                data = FindGBData(_gbDataSet_HW, byName);
                if (data == null)
                    data = FindGBData(_gbDataSet_HM, byName);
                if (data == null)
                    data = FindGBData(_gbDataSet_HN, byName);
                if (data == null)
                    data = FindGBData(_gbDataSet_HT, byName);
                break;
            }
            return data;
        }
        private GBData FindGBData(string type, params double[] byParameters) {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (byParameters is null) {
                throw new ArgumentNullException(nameof(byParameters));
            }

            GBData data;
            switch (type) {
            case "HW":
                data = FindGBData(_gbDataSet_HW, byParameters);
                break;
            case "HM":
                data = FindGBData(_gbDataSet_HM, byParameters);
                break;
            case "HN":
                data = FindGBData(_gbDataSet_HN, byParameters);
                break;
            case "HT":
                data = FindGBData(_gbDataSet_HT, byParameters);
                break;
            case "H":
            default:
                data = FindGBData(_gbDataSet_HM, byParameters);
                if (data == null)
                    data = FindGBData(_gbDataSet_HW, byParameters);
                if (data == null)
                    data = FindGBData(_gbDataSet_HN, byParameters);
                if (data == null)
                    data = FindGBData(_gbDataSet_HT, byParameters);
                break;
            }
            return data;
        }
    }
}
