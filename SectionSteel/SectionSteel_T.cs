/*==============================================================================
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
 *  SectionSteel_T.cs: 剖分T型钢
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>剖分T型钢。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.T_1"/>: <inheritdoc cref="Pattern_Collection.T_1"/><br/>
    /// <see cref="Pattern_Collection.T_2"/>: <inheritdoc cref="Pattern_Collection.T_2"/><br/>
    /// <see cref="Pattern_Collection.T_3"/>: <inheritdoc cref="Pattern_Collection.T_3"/><br/>
    /// </summary>
    /// <remarks>当匹配到 <see cref="Pattern_Collection.T_3"/> 模式时，
    /// 在国标截面特性表格中查找，同一型号名下有多项时按最接近项匹配。
    /// </remarks>
    public class SectionSteel_T : SectionSteelBase {
        private string type = string.Empty;
        private double h1, h2, b, s, t;
        private GBData data;
        private static readonly GBData[] _gbDataSet_TW = new GBData[] {
            new GBData("50*100", new double[] { 50, 100, 6,8 }, 8.47, 0.293),
            new GBData("62.5*125", new double[] { 62.5, 125, 6.5,9 }, 11.8, 0.368),
            new GBData("75*150", new double[] { 75, 150, 7,10 }, 15.6, 0.443),
            new GBData("87.5*175", new double[] { 87.5, 175, 7.5,11 }, 20.2, 0.514),
            new GBData("100*200", new double[] { 100, 200, 8,12 }, 24.9, 0.589),
            new GBData("", new double[] { 100, 204, 12,12 }, 28.1, 0.597),
            new GBData("125*250", new double[] { 125, 250, 9,14 }, 35.9, 0.739),
            new GBData("", new double[] { 125, 255, 14,14 }, 40.8, 0.749),
            new GBData("", new double[] { 147, 302, 12,12 }, 41.7, 0.887),
            new GBData("150*300", new double[] { 150, 300, 10,15 }, 46.5, 0.889),
            new GBData("", new double[] { 150, 305, 15,15 }, 52.4, 0.899),
            new GBData("", new double[] { 172, 348, 10,16 }, 56.5, 1.03),
            new GBData("175*350", new double[] { 175, 350, 12,19 }, 67.5, 1.04),
            new GBData("", new double[] { 194, 402, 15,15 }, 70, 1.17),
            new GBData("", new double[] { 197, 398, 11,18 }, 73.3, 1.17),
            new GBData("200*400", new double[] { 200, 400, 13,21 }, 85.8, 1.18),
            new GBData("", new double[] { 200, 408, 21,21 }, 98.4, 1.2),
            new GBData("", new double[] { 207, 405, 18,28 }, 116, 1.21),
            new GBData("", new double[] { 214, 407, 20,35 }, 142, 1.22),
        };
        private static readonly GBData[] _gbDataSet_TM = new GBData[] {
            new GBData("75*100", new double[] { 74, 100, 6,9 }, 10.3, 0.341),
            new GBData("100*150", new double[] { 97, 150, 6,9 }, 15, 0.487),
            new GBData("125*175", new double[] { 122, 175, 7,11 }, 21.8, 0.583),
            new GBData("", new double[] { 147, 200, 8,12 }, 27.9, 0.683),
            new GBData("150*200", new double[] { 149, 201, 9,14 }, 32.2, 0.689),
            new GBData("175*250", new double[] { 170, 250, 9,14 }, 39.1, 0.829),
            new GBData("200*300", new double[] { 195, 300, 10,16 }, 52.3, 0.979),
            new GBData("225*300", new double[] { 220, 300, 11,18 }, 60.4, 1.03),
            new GBData("", new double[] { 241, 300, 11,15 }, 55.4, 1.07),
            new GBData("250*300", new double[] { 244, 300, 11,18 }, 62.5, 1.08),
            new GBData("", new double[] { 272, 300, 11,15 }, 58.1, 1.13),
            new GBData("275*300", new double[] { 275, 300, 11,18 }, 65.2, 1.14),
            new GBData("", new double[] { 291, 300, 12,17 }, 66.4, 1.17),
            new GBData("", new double[] { 294, 300, 12,20 }, 73.5, 1.18),
            new GBData("300*300", new double[] { 297, 302, 14,23 }, 85.2, 1.19),
        };
        private static readonly GBData[] _gbDataSet_TN = new GBData[] {
            new GBData("50*50", new double[] { 50, 50, 5,7 }, 4.65, 0.193),
            new GBData("62.5*60", new double[] { 62.5, 60, 6,8 }, 6.55, 0.238),
            new GBData("75*75", new double[] { 75, 75, 5,7 }, 7, 0.293),
            new GBData("", new double[] { 85.5, 89, 4,6 }, 6.9, 0.342),
            new GBData("87.5*90", new double[] { 87.5, 90, 5,8 }, 8.98, 0.348),
            new GBData("", new double[] { 99, 99, 4.5,7 }, 8.9, 0.389),
            new GBData("100*100", new double[] { 100, 100, 5.5,8 }, 10.5, 0.393),
            new GBData("", new double[] { 124, 124, 5,8 }, 12.6, 0.489),
            new GBData("125*125", new double[] { 125, 125, 6,9 }, 14.5, 0.493),
            new GBData("", new double[] { 149, 149, 5.5,8 }, 16, 0.585),
            new GBData("150*150", new double[] { 150, 150, 6.5,9 }, 18.4, 0.589),
            new GBData("", new double[] { 173, 174, 6,9 }, 20.6, 0.683),
            new GBData("175*175", new double[] { 175, 175, 7,11 }, 24.7, 0.689),
            new GBData("", new double[] { 198, 199, 7,11 }, 28, 0.783),
            new GBData("200*200", new double[] { 200, 200, 8,13 }, 32.7, 0.789),
            new GBData("", new double[] { 223, 150, 7,12 }, 26.3, 0.735),
            new GBData("225*150", new double[] { 225, 151, 8,14 }, 30.4, 0.741),
            new GBData("", new double[] { 223, 199, 8,12 }, 32.6, 0.833),
            new GBData("225*200", new double[] { 225, 200, 9,14 }, 37.5, 0.839),
            new GBData("", new double[] { 235, 150, 7,13 }, 28.1, 0.759),
            new GBData("237.5*150", new double[] { 237.5, 151.5, 8.5,15.5 }, 33.8, 0.767),
            new GBData("", new double[] { 241, 153.5, 10.5,19 }, 41.8, 0.778),
            new GBData("", new double[] { 246, 150, 7,12 }, 27.6, 0.781),
            new GBData("250*150", new double[] { 250, 152, 9,16 }, 36.2, 0.793),
            new GBData("", new double[] { 252, 153, 10,18 }, 40.6, 0.799),
            new GBData("", new double[] { 248, 199, 9,14 }, 39, 0.883),
            new GBData("250*200", new double[] { 250, 200, 10,16 }, 44.1, 0.889),
            new GBData("", new double[] { 253, 201, 11,19 }, 50.8, 0.897),
            new GBData("", new double[] { 273, 199, 9,14 }, 40.7, 0.933),
            new GBData("275*200", new double[] { 275, 200, 10,16 }, 46, 0.939),
            new GBData("", new double[] { 298, 199, 10,15 }, 46.2, 0.983),
            new GBData("300*200", new double[] { 300, 200, 11,17 }, 51.7, 0.989),
            new GBData("", new double[] { 303, 201, 12,20 }, 58.8, 0.997),
            new GBData("312.5*200", new double[] { 312.5, 198.5, 13.5,17.5 }, 59.1, 1.01),
            new GBData("", new double[] { 315, 200, 15,20 }, 66.7, 1.02),
            new GBData("", new double[] { 319, 202, 17,24 }, 78, 1.03),
            new GBData("", new double[] { 323, 299, 12,18 }, 72.1, 1.23),
            new GBData("325*300", new double[] { 325, 300, 13,20 }, 79.3, 1.23),
            new GBData("", new double[] { 327, 301, 14,22 }, 86.59, 1.24),
            new GBData("", new double[] { 346, 300, 13,20 }, 81.5, 1.28),
            new GBData("350*300", new double[] { 350, 300, 13,24 }, 90.9, 1.28),
            new GBData("", new double[] { 396, 300, 14,22 }, 94, 1.38),
            new GBData("400*300", new double[] { 400, 300, 14,26 }, 103, 1.38),
            new GBData("", new double[] { 445, 299, 15,23 }, 105, 1.47),
            new GBData("450*300", new double[] { 450, 300, 16,28 }, 120, 1.48),
            new GBData("", new double[] { 456, 302, 18,34 }, 141, 1.5),
        };

        public override GBData[] GBDataSet {
            get {
                switch (type) {
                case "TW":
                    return _gbDataSet_TW;
                case "TM":
                    return _gbDataSet_TM;
                case "TN":
                    return _gbDataSet_TN;
                default:
                    if (data == null) return null;
                    if (FindGBData(_gbDataSet_TW, data.Parameters) != null) return _gbDataSet_TW;
                    if (FindGBData(_gbDataSet_TM, data.Parameters) != null) return _gbDataSet_TM;
                    if (FindGBData(_gbDataSet_TN, data.Parameters) != null) return _gbDataSet_TN;
                    return null;
                }
            }
        }

        public SectionSteel_T() { }
        public SectionSteel_T(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = (type, h1, h2, b, s, t, data);
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.T_1);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.T_2);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.T_4);
                if (match.Success) {
                    double.TryParse(match.Groups["h1"].Value, out h1);
                    double.TryParse(match.Groups["h2"].Value, out h2);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double.TryParse(match.Groups["t"].Value, out t);

                    if (h2 == 0 || h2 == h1) {
                        data = FindGBData(string.Empty, h1, b, s, t);
                    }
                } else {
                    match = Regex.Match(e.NewText, Pattern_Collection.T_3);
                    if (!match.Success)
                        throw new MismatchedProfileTextException(e.NewText);

                    type = match.Groups["TYPE"].Value;
                    string name = match.Groups["H"].Value + "*" + match.Groups["B"].Value;
                    data = FindGBData(type, name);
                    if (data == null) {
                        double.TryParse(match.Groups["H"].Value, out double H);
                        double.TryParse(match.Groups["B"].Value, out double B);
                        data = FindGBData(type, H, B);
                    }
                    if (data == null)
                        throw new MismatchedProfileTextException(e.NewText);

                    h1 = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                if (h2 == 0) h2 = h1;
                h1 *= 0.001; h2 *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                type = tmp.type; h1 = tmp.h1; h2 = tmp.h2;
                b = tmp.b; s = tmp.s; t = tmp.t;
                data = tmp.data;
                throw;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1) {
                    formula = $"{(h1 + h2) * 0.5}";
                } else {
                    formula = $"{h1}";
                }
                formula += $"*2+{b}";
                if (!exclude_topSurface)
                    formula += $"*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    break;
                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b}";
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
            b = (this.b - s) * 0.5;
            l = (h1 + h2) * 0.5 - this.t;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">ROUGHLY 等效于 PRECISELY</param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1) {
                    formula = $"(({(h1 + h2) * 0.5}-{t})*{s}+{b}*{t})*{DENSITY}";
                } else {
                    formula = $"(({h1}-{t})*{s}+{b}*{t})*{DENSITY}";
                }
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
        private GBData FindGBData(string type, string byName) {
            if (type is null) {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(byName)) {
                throw new ArgumentException($"“{nameof(byName)}”不能为 null 或空。", nameof(byName));
            }

            GBData data;
            switch (type) {
            case "TW":
                data = FindGBData(_gbDataSet_TW, byName);
                break;
            case "TM":
                data = FindGBData(_gbDataSet_TM, byName);
                break;
            case "TN":
                data = FindGBData(_gbDataSet_TN, byName);
                break;
            case "T":
            default:
                data = FindGBData(_gbDataSet_TW, byName);
                if (data == null)
                    data = FindGBData(_gbDataSet_TM, byName);
                if (data == null)
                    data = FindGBData(_gbDataSet_TN, byName);
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
            case "TW":
                data = FindGBData(_gbDataSet_TW, byParameters);
                break;
            case "TM":
                data = FindGBData(_gbDataSet_TM, byParameters);
                break;
            case "TN":
                data = FindGBData(_gbDataSet_TN, byParameters);
                break;
            case "T":
            default:
                data = FindGBData(_gbDataSet_TW, byParameters);
                if (data == null)
                    data = FindGBData(_gbDataSet_TM, byParameters);
                if (data == null)
                    data = FindGBData(_gbDataSet_TN, byParameters);
                break;
            }
            return data;
        }
    }
}
