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
 *  SectionSteel_I.cs: 工字钢
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>工字钢。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.I_1"/>: <inheritdoc cref="Pattern_Collection.I_1"/><br/>
    /// <see cref="Pattern_Collection.I_2"/>: <inheritdoc cref="Pattern_Collection.I_2"/><br/>
    /// </summary>
    /// <remarks>匹配到两种模式时，均在国标截面特性表格中查找。<br/>
    /// <see cref="Pattern_Collection.I_2"/> 模式下，当型号大于等于20号且无后缀时，按后缀为"a"处理。
    /// </remarks>
    public class SectionSteel_I : SectionSteelBase {
        private double h, b, s, t;
        private GBData data;
        private static readonly GBData[] _gbDataSet = new GBData[] {
            new GBData("10", new double[] { 100, 68, 4.5,7.6 }, 11.3, 0.432),
            new GBData("12", new double[] { 120, 74, 5,8.4 }, 14, 0.493),
            new GBData("12.6", new double[] { 126, 74, 5,8.4 }, 14.2, 0.505),
            new GBData("14", new double[] { 140, 80, 5.5,9.1 }, 16.9, 0.553),
            new GBData("16", new double[] { 160, 88, 6,9.9 }, 20.5, 0.621),
            new GBData("18", new double[] { 180, 94, 6.5,10.7 }, 24.1, 0.681),
            new GBData("20a", new double[] { 200, 100, 7,11.4 }, 27.9, 0.742),
            new GBData("20b", new double[] { 200, 102, 9,11.4 }, 31.1, 0.746),
            new GBData("22a", new double[] { 220, 110, 7.5,12.3 }, 33.1, 0.817),
            new GBData("22b", new double[] { 220, 112, 9.5,12.3 }, 36.5, 0.821),
            new GBData("24a", new double[] { 240, 116, 8,13 }, 37.5, 0.878),
            new GBData("24b", new double[] { 240, 118, 10,13 }, 41.2, 0.882),
            new GBData("25a", new double[] { 250, 116, 8,13 }, 38.1, 0.898),
            new GBData("25b", new double[] { 250, 118, 10,13 }, 42, 0.902),
            new GBData("27a", new double[] { 270, 122, 8.5,13.7 }, 42.8, 0.958),
            new GBData("27b", new double[] { 270, 124, 10.5,13.7 }, 47, 0.962),
            new GBData("28a", new double[] { 280, 122, 8.5,13.7 }, 43.5, 0.978),
            new GBData("28b", new double[] { 280, 124, 10.5,13.7 }, 47.9, 0.982),
            new GBData("30a", new double[] { 300, 126, 9,14.4 }, 48.1, 1.031),
            new GBData("30b", new double[] { 300, 128, 11,14.4 }, 52.8, 1.035),
            new GBData("30c", new double[] { 300, 130, 13,14.4 }, 57.5, 1.039),
            new GBData("32a", new double[] { 320, 130, 9.5,15 }, 52.7, 1.084),
            new GBData("32b", new double[] { 320, 132, 11.5,15 }, 57.7, 1.088),
            new GBData("32c", new double[] { 320, 134, 13.5,15 }, 62.7, 1.092),
            new GBData("36a", new double[] { 360, 136, 10,15.8 }, 60, 1.185),
            new GBData("36b", new double[] { 360, 138, 12,15.8 }, 65.7, 1.189),
            new GBData("36c", new double[] { 360, 140, 14,15.8 }, 71.3, 1.193),
            new GBData("40a", new double[] { 400, 142, 10.5,16.5 }, 67.6, 1.285),
            new GBData("40b", new double[] { 400, 144, 12.5,16.5 }, 73.8, 1.289),
            new GBData("40c", new double[] { 400, 146, 14.5,16.5 }, 80.1, 1.293),
            new GBData("45a", new double[] { 450, 150, 11.5,18 }, 80.4, 1.411),
            new GBData("45b", new double[] { 450, 152, 13.5,18 }, 87.4, 1.415),
            new GBData("45c", new double[] { 450, 154, 15.5,18 }, 94.5, 1.419),
            new GBData("50a", new double[] { 500, 158, 12,20 }, 93.6, 1.539),
            new GBData("50b", new double[] { 500, 160, 14,20 }, 101, 1.543),
            new GBData("50c", new double[] { 500, 162, 16,20 }, 109, 1.547),
            new GBData("55a", new double[] { 550, 166, 12.5,21 }, 105, 1.667),
            new GBData("55b", new double[] { 550, 168, 14.5,21 }, 114, 1.671),
            new GBData("55c", new double[] { 550, 170, 16.5,21 }, 123, 1.675),
            new GBData("56a", new double[] { 560, 166, 12.5,21 }, 106, 1.687),
            new GBData("56b", new double[] { 560, 168, 14.5,21 }, 115, 1.691),
            new GBData("56c", new double[] { 560, 170, 16.5,21 }, 124, 1.695),
            new GBData("63a", new double[] { 630, 176, 13,22 }, 121, 1.862),
            new GBData("63b", new double[] { 630, 178, 15,22 }, 131, 1.866),
            new GBData("63c", new double[] { 630, 180, 17,22 }, 141, 1.87),
        };
        public override GBData[] GBDataSet => _gbDataSet;

        public SectionSteel_I() { }
        public SectionSteel_I(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = (h, b, s, t, data);
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.I_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);
                    data = FindGBData(_gbDataSet, h, b, s);
                    if (data == null)
                        throw new MismatchedProfileTextException(e.NewText);

                    t = data.Parameters[3];
                } else {
                    match = Regex.Match(e.NewText, Pattern_Collection.I_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException(e.NewText);

                    double.TryParse(match.Groups["CODE"].Value, out double code);
                    var suffix = match.Groups["SUFFIX"].Value;
                    var name = match.Groups["NAME"].Value;
                    if (code >= 20 && string.IsNullOrEmpty(suffix))
                        name += "a";

                    data = FindGBData(_gbDataSet, name);
                    if (data == null)
                        throw new MismatchedProfileTextException(e.NewText);

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                h *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = tmp.h; b = tmp.b; s = tmp.s; t = tmp.t;
                data = tmp.data;
                throw;
            }
        }
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                formula = $"{h}*2+{b}";
                if (exclude_topSurface)
                    formula += $"*3";
                else
                    formula += $"*4";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{s}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                //构造成功必定给data字段分配值
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
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = (this.b - s) * 0.5;
            l = h - this.t * 2;
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
                formula = $"{data.Weight}";

            return formula;
        }
    }
}
