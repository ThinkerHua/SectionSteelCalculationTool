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
 *  SectionSteel_L_BtB.cs: 角钢，背对背双拼
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>角钢，背对背双拼。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.L_BtB_1"/>: <inheritdoc cref="Pattern_Collection.L_BtB_1"/><br/>
    /// <see cref="Pattern_Collection.L_BtB_2"/>: <inheritdoc cref="Pattern_Collection.L_BtB_2"/><br/>
    /// </summary>
    /// <remarks>当匹配到 <see cref="Pattern_Collection.L_BtB_2"/> 模式时，
    /// 在国标截面特性表格中查找，同一型号名下按第一个进行匹配。
    /// </remarks>
    public class SectionSteel_L_BtB : SectionSteelBase {
        private double h, b, t;
        private GBData data;
        private static readonly GBData[] _gbDataSet = new GBData[] {
            new GBData("2", new double[] { 20, 20, 3, }, 0.889, 0.078),
            new GBData("", new double[] { 20, 20, 4, }, 1.15, 0.077),
            new GBData("2.5", new double[] { 25, 25, 3, }, 1.12, 0.098),
            new GBData("", new double[] { 25, 25, 4, }, 1.46, 0.097),
            new GBData("3", new double[] { 30, 30, 3, }, 1.37, 0.117),
            new GBData("", new double[] { 30, 30, 4, }, 1.79, 0.117),
            new GBData("3.6", new double[] { 36, 36, 3, }, 1.66, 0.141),
            new GBData("", new double[] { 36, 36, 4, }, 2.16, 0.141),
            new GBData("", new double[] { 36, 36, 5, }, 2.65, 0.141),
            new GBData("4", new double[] { 40, 40, 3, }, 1.85, 0.157),
            new GBData("", new double[] { 40, 40, 4, }, 2.42, 0.157),
            new GBData("", new double[] { 40, 40, 5, }, 2.98, 0.156),
            new GBData("4.5", new double[] { 45, 45, 3, }, 2.09, 0.177),
            new GBData("", new double[] { 45, 45, 4, }, 2.74, 0.177),
            new GBData("", new double[] { 45, 45, 5, }, 3.37, 0.176),
            new GBData("", new double[] { 45, 45, 6, }, 3.99, 0.176),
            new GBData("5", new double[] { 50, 50, 3, }, 2.33, 0.197),
            new GBData("", new double[] { 50, 50, 4, }, 3.06, 0.197),
            new GBData("", new double[] { 50, 50, 5, }, 3.77, 0.196),
            new GBData("", new double[] { 50, 50, 6, }, 4.46, 0.196),
            new GBData("5.6", new double[] { 56, 56, 3, }, 2.624, 0.221),
            new GBData("", new double[] { 56, 56, 4, }, 3.446, 0.22),
            new GBData("", new double[] { 56, 56, 5, }, 4.251, 0.22),
            new GBData("", new double[] { 56, 56, 6, }, 5.04, 0.22),
            new GBData("", new double[] { 56, 56, 7, }, 5.81, 0.219),
            new GBData("", new double[] { 56, 56, 8, }, 6.568, 0.219),
            new GBData("6", new double[] { 60, 60, 5, }, 4.58, 0.236),
            new GBData("", new double[] { 60, 60, 6, }, 5.43, 0.235),
            new GBData("", new double[] { 60, 60, 7, }, 6.26, 0.235),
            new GBData("", new double[] { 60, 60, 8, }, 7.08, 0.235),
            new GBData("6.3", new double[] { 63, 63, 4, }, 3.91, 0.248),
            new GBData("", new double[] { 63, 63, 5, }, 4.82, 0.248),
            new GBData("", new double[] { 63, 63, 6, }, 5.72, 0.247),
            new GBData("", new double[] { 63, 63, 7, }, 6.6, 0.247),
            new GBData("", new double[] { 63, 63, 8, }, 7.47, 0.247),
            new GBData("", new double[] { 63, 63, 10, }, 9.15, 0.246),
            new GBData("7", new double[] { 70, 70, 4, }, 4.37, 0.275),
            new GBData("", new double[] { 70, 70, 5, }, 5.4, 0.275),
            new GBData("", new double[] { 70, 70, 6, }, 6.41, 0.275),
            new GBData("", new double[] { 70, 70, 7, }, 7.4, 0.275),
            new GBData("", new double[] { 70, 70, 8, }, 8.37, 0.274),
            new GBData("7.5", new double[] { 75, 75, 5, }, 5.82, 0.295),
            new GBData("", new double[] { 75, 75, 6, }, 6.91, 0.294),
            new GBData("", new double[] { 75, 75, 7, }, 7.98, 0.294),
            new GBData("", new double[] { 75, 75, 8, }, 9.03, 0.294),
            new GBData("", new double[] { 75, 75, 9, }, 10.1, 0.294),
            new GBData("", new double[] { 75, 75, 10, }, 11.1, 0.293),
            new GBData("8", new double[] { 80, 80, 5, }, 6.21, 0.315),
            new GBData("", new double[] { 80, 80, 6, }, 7.38, 0.314),
            new GBData("", new double[] { 80, 80, 7, }, 8.53, 0.314),
            new GBData("", new double[] { 80, 80, 8, }, 9.66, 0.314),
            new GBData("", new double[] { 80, 80, 9, }, 10.8, 0.314),
            new GBData("", new double[] { 80, 80, 10, }, 11.9, 0.313),
            new GBData("9", new double[] { 90, 90, 6, }, 8.35, 0.354),
            new GBData("", new double[] { 90, 90, 7, }, 9.66, 0.354),
            new GBData("", new double[] { 90, 90, 8, }, 10.9, 0.353),
            new GBData("", new double[] { 90, 90, 9, }, 12.2, 0.353),
            new GBData("", new double[] { 90, 90, 10, }, 13.5, 0.353),
            new GBData("", new double[] { 90, 90, 12, }, 15.9, 0.352),
            new GBData("10", new double[] { 100, 100, 6, }, 9.37, 0.393),
            new GBData("", new double[] { 100, 100, 7, }, 10.8, 0.393),
            new GBData("", new double[] { 100, 100, 8, }, 12.3, 0.393),
            new GBData("", new double[] { 100, 100, 9, }, 13.7, 0.392),
            new GBData("", new double[] { 100, 100, 10, }, 15.1, 0.392),
            new GBData("", new double[] { 100, 100, 12, }, 17.9, 0.391),
            new GBData("", new double[] { 100, 100, 14, }, 20.6, 0.391),
            new GBData("", new double[] { 100, 100, 16, }, 23.3, 0.39),
            new GBData("11", new double[] { 110, 110, 7, }, 11.9, 0.433),
            new GBData("", new double[] { 110, 110, 8, }, 13.5, 0.433),
            new GBData("", new double[] { 110, 110, 10, }, 16.7, 0.432),
            new GBData("", new double[] { 110, 110, 12, }, 19.8, 0.431),
            new GBData("", new double[] { 110, 110, 14, }, 22.8, 0.431),
            new GBData("12.5", new double[] { 125, 125, 8, }, 15.5, 0.492),
            new GBData("", new double[] { 125, 125, 10, }, 19.1, 0.491),
            new GBData("", new double[] { 125, 125, 12, }, 22.7, 0.491),
            new GBData("", new double[] { 125, 125, 14, }, 26.2, 0.49),
            new GBData("", new double[] { 125, 125, 16, }, 29.6, 0.489),
            new GBData("14", new double[] { 140, 140, 10, }, 21.5, 0.551),
            new GBData("", new double[] { 140, 140, 12, }, 25.5, 0.551),
            new GBData("", new double[] { 140, 140, 14, }, 29.5, 0.55),
            new GBData("", new double[] { 140, 140, 16, }, 33.4, 0.549),
            new GBData("15", new double[] { 150, 150, 8, }, 18.6, 0.592),
            new GBData("", new double[] { 150, 150, 10, }, 23.1, 0.591),
            new GBData("", new double[] { 150, 150, 12, }, 27.4, 0.591),
            new GBData("", new double[] { 150, 150, 14, }, 31.7, 0.59),
            new GBData("", new double[] { 150, 150, 15, }, 33.8, 0.59),
            new GBData("", new double[] { 150, 150, 16, }, 35.9, 0.589),
            new GBData("16", new double[] { 160, 160, 10, }, 24.7, 0.63),
            new GBData("", new double[] { 160, 160, 12, }, 29.4, 0.63),
            new GBData("", new double[] { 160, 160, 14, }, 34, 0.629),
            new GBData("", new double[] { 160, 160, 16, }, 38.5, 0.629),
            new GBData("18", new double[] { 180, 180, 12, }, 33.2, 0.71),
            new GBData("", new double[] { 180, 180, 14, }, 38.4, 0.709),
            new GBData("", new double[] { 180, 180, 16, }, 43.5, 0.709),
            new GBData("", new double[] { 180, 180, 18, }, 48.6, 0.708),
            new GBData("20", new double[] { 200, 200, 14, }, 42.9, 0.788),
            new GBData("", new double[] { 200, 200, 16, }, 48.7, 0.788),
            new GBData("", new double[] { 200, 200, 18, }, 54.4, 0.787),
            new GBData("", new double[] { 200, 200, 20, }, 60.1, 0.787),
            new GBData("", new double[] { 200, 200, 24, }, 71.2, 0.785),
            new GBData("22", new double[] { 220, 220, 16, }, 53.9, 0.866),
            new GBData("", new double[] { 220, 220, 18, }, 60.3, 0.866),
            new GBData("", new double[] { 220, 220, 20, }, 66.5, 0.865),
            new GBData("", new double[] { 220, 220, 22, }, 72.8, 0.865),
            new GBData("", new double[] { 220, 220, 24, }, 78.9, 0.864),
            new GBData("", new double[] { 220, 220, 26, }, 85, 0.864),
            new GBData("25", new double[] { 250, 250, 18, }, 69, 0.985),
            new GBData("", new double[] { 250, 250, 20, }, 76.2, 0.984),
            new GBData("", new double[] { 250, 250, 22, }, 83.3, 0.983),
            new GBData("", new double[] { 250, 250, 24, }, 90.4, 0.983),
            new GBData("", new double[] { 250, 250, 26, }, 97.5, 0.982),
            new GBData("", new double[] { 250, 250, 28, }, 104, 0.982),
            new GBData("", new double[] { 250, 250, 30, }, 111, 0.981),
            new GBData("", new double[] { 250, 250, 32, }, 118, 0.981),
            new GBData("", new double[] { 250, 250, 35, }, 128, 0.98),
            new GBData("2.5/1.6", new double[] { 25, 16, 3, }, 0.91, 0.08),
            new GBData("", new double[] { 25, 16, 4, }, 1.18, 0.079),
            new GBData("3.2/2", new double[] { 32, 20, 3, }, 1.17, 0.102),
            new GBData("", new double[] { 32, 20, 4, }, 1.52, 0.101),
            new GBData("4/2.5", new double[] { 40, 25, 3, }, 1.48, 0.127),
            new GBData("", new double[] { 40, 25, 4, }, 1.94, 0.127),
            new GBData("4.5/2.8", new double[] { 45, 28, 3, }, 1.69, 0.143),
            new GBData("", new double[] { 45, 28, 4, }, 2.2, 0.143),
            new GBData("5/3.2", new double[] { 50, 32, 3, }, 1.91, 0.161),
            new GBData("", new double[] { 50, 32, 4, }, 2.49, 0.16),
            new GBData("5.6/3.6", new double[] { 56, 36, 3, }, 2.15, 0.181),
            new GBData("", new double[] { 56, 36, 4, }, 2.82, 0.18),
            new GBData("", new double[] { 56, 36, 5, }, 3.47, 0.18),
            new GBData("6.3/4", new double[] { 63, 40, 4, }, 3.19, 0.202),
            new GBData("", new double[] { 63, 40, 5, }, 3.92, 0.202),
            new GBData("", new double[] { 63, 40, 6, }, 4.64, 0.201),
            new GBData("", new double[] { 63, 40, 7, }, 5.34, 0.201),
            new GBData("7/4.5", new double[] { 70, 45, 4, }, 3.57, 0.226),
            new GBData("", new double[] { 70, 45, 5, }, 4.4, 0.225),
            new GBData("", new double[] { 70, 45, 6, }, 5.22, 0.225),
            new GBData("", new double[] { 70, 45, 7, }, 6.01, 0.225),
            new GBData("7.5/5", new double[] { 75, 50, 5, }, 4.81, 0.245),
            new GBData("", new double[] { 75, 50, 6, }, 5.7, 0.245),
            new GBData("", new double[] { 75, 50, 8, }, 7.43, 0.244),
            new GBData("", new double[] { 75, 50, 10, }, 9.1, 0.244),
            new GBData("8/5", new double[] { 80, 50, 5, }, 5, 0.255),
            new GBData("", new double[] { 80, 50, 6, }, 5.93, 0.255),
            new GBData("", new double[] { 80, 50, 7, }, 6.85, 0.255),
            new GBData("", new double[] { 80, 50, 8, }, 7.75, 0.254),
            new GBData("9/5.6", new double[] { 90, 56, 5, }, 5.66, 0.287),
            new GBData("", new double[] { 90, 56, 6, }, 6.72, 0.286),
            new GBData("", new double[] { 90, 56, 7, }, 7.76, 0.286),
            new GBData("", new double[] { 90, 56, 8, }, 8.78, 0.286),
            new GBData("10/6.3", new double[] { 100, 63, 6, }, 7.55, 0.32),
            new GBData("", new double[] { 100, 63, 7, }, 8.72, 0.32),
            new GBData("", new double[] { 100, 63, 8, }, 9.88, 0.319),
            new GBData("", new double[] { 100, 63, 10, }, 12.1, 0.319),
            new GBData("10/8", new double[] { 100, 80, 6, }, 8.35, 0.354),
            new GBData("", new double[] { 100, 80, 7, }, 9.66, 0.354),
            new GBData("", new double[] { 100, 80, 8, }, 10.9, 0.353),
            new GBData("", new double[] { 100, 80, 10, }, 13.5, 0.353),
            new GBData("11/7", new double[] { 110, 70, 6, }, 8.35, 0.354),
            new GBData("", new double[] { 110, 70, 7, }, 9.66, 0.354),
            new GBData("", new double[] { 110, 70, 8, }, 10.9, 0.353),
            new GBData("", new double[] { 110, 70, 10, }, 13.5, 0.353),
            new GBData("12.5/8", new double[] { 125, 80, 7, }, 11.1, 0.403),
            new GBData("", new double[] { 125, 80, 8, }, 12.6, 0.403),
            new GBData("", new double[] { 125, 80, 10, }, 15.5, 0.402),
            new GBData("", new double[] { 125, 80, 12, }, 18.3, 0.402),
            new GBData("9/14", new double[] { 140, 90, 8, }, 14.2, 0.453),
            new GBData("", new double[] { 140, 90, 10, }, 17.5, 0.452),
            new GBData("", new double[] { 140, 90, 12, }, 20.7, 0.451),
            new GBData("", new double[] { 140, 90, 14, }, 23.9, 0.451),
            new GBData("9/15", new double[] { 150, 90, 8, }, 14.8, 0.473),
            new GBData("", new double[] { 150, 90, 10, }, 18.3, 0.472),
            new GBData("", new double[] { 150, 90, 12, }, 21.7, 0.471),
            new GBData("", new double[] { 150, 90, 14, }, 25, 0.471),
            new GBData("", new double[] { 150, 90, 15, }, 25.7, 0.471),
            new GBData("", new double[] { 150, 90, 16, }, 28.3, 0.47),
            new GBData("10/16", new double[] { 160, 100, 10, }, 19.9, 0.512),
            new GBData("", new double[] { 160, 100, 12, }, 23.6, 0.511),
            new GBData("", new double[] { 160, 100, 14, }, 27.2, 0.51),
            new GBData("", new double[] { 160, 100, 16, }, 30.8, 0.51),
            new GBData("11/18", new double[] { 180, 110, 10, }, 22.3, 0.571),
            new GBData("", new double[] { 180, 110, 12, }, 26.5, 0.571),
            new GBData("", new double[] { 180, 110, 14, }, 30.6, 0.57),
            new GBData("", new double[] { 180, 110, 16, }, 34.6, 0.569),
            new GBData("20/12.5", new double[] { 200, 125, 12, }, 29.8, 0.641),
            new GBData("", new double[] { 200, 125, 14, }, 34.4, 0.64),
            new GBData("", new double[] { 200, 125, 16, }, 39, 0.639),
            new GBData("", new double[] { 200, 125, 18, }, 43.6, 0.639),
        };
        public override GBData[] GBDataSet => _gbDataSet;

        public SectionSteel_L_BtB() { }
        public SectionSteel_L_BtB(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = (h, b, t, data);
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.L_BtB_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["t"].Value, out t);

                    if (b == 0) b = h;
                    data = FindGBData(_gbDataSet, h, b, t);
                } else {
                    match = Regex.Match(e.NewText, Pattern_Collection.L_BtB_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException(e.NewText);

                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    if (b == 0) b = h;
                    h *= 10; b *= 10;
                    data = FindGBData(_gbDataSet, h, b);
                    if (data == null)
                        throw new MismatchedProfileTextException(e.NewText);

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    t = data.Parameters[2];
                }

                h *= 0.001; b *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = tmp.h; b = tmp.b; t = tmp.t;
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
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (b != h) {
                    formula = $"{h}*2";
                    if (exclude_topSurface)
                        formula += $"+{b}*2";
                    else
                        formula += $"+{b}*4";
                } else {
                    if (exclude_topSurface)
                        formula += $"{h}*4";
                    else
                        formula += $"{h}*6";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    return formula;

                formula = $"{data.Area}*2";
                if (b != h) {
                    formula += $"-{h}*2";
                    if (exclude_topSurface)
                        formula += $"-{b}*2";
                } else {
                    if (exclude_topSurface)
                        formula += $"-{h}*4";
                    else
                        formula += $"-{h}*2";
                }
                break;
            default:
                break;
            }

            return formula;
        }

        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = this.t;
            b = this.b - this.t;
            l = h - this.t;
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
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY, PRECISELY 均等效于 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (data != null)
                formula = $"{data.Weight}*2";

            return formula;
        }
    }
}
