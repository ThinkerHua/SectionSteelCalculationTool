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
 *  SectionSteel_CHAN_BtB.cs: 槽钢，背对背双拼
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>槽钢，背对背双拼。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CHAN_BtB_1"/>: <inheritdoc cref="Pattern_Collection.CHAN_BtB_1"/><br/>
    /// <see cref="Pattern_Collection.CHAN_BtB_2"/>: <inheritdoc cref="Pattern_Collection.CHAN_BtB_2"/><br/>
    /// </summary>
    /// <remarks>匹配到两种模式时，均在国标截面特性表格中查找。<br/>
    /// <see cref="Pattern_Collection.CHAN_BtB_2"/> 模式下，当型号大于等于14号且无后缀时，按后缀为"a"处理。
    /// </remarks>
    public class SectionSteel_CHAN_BtB : SectionSteelBase {
        double h, b, s, t;
        GBData data;
        private static readonly GBData[] _gbDataSet = new GBData[] {
            new GBData("5", new double[] { 50, 37, 4.5,7 }, 5.44, 0.226),
            new GBData("6.3", new double[] { 63, 40, 4.8,7.5 }, 6.63, 0.262),
            new GBData("6.5", new double[] { 65, 40, 4.3,7.5 }, 6.51, 0.267),
            new GBData("8", new double[] { 80, 43, 5,8 }, 8.04, 0.307),
            new GBData("10", new double[] { 100, 48, 5.3,8.5 }, 10, 0.365),
            new GBData("12", new double[] { 120, 53, 5.5,9 }, 12.1, 0.423),
            new GBData("12.6", new double[] { 126, 53, 5.5,9 }, 12.3, 0.435),
            new GBData("14a", new double[] { 140, 58, 6,9.5 }, 14.5, 0.48),
            new GBData("14b", new double[] { 140, 60, 8,9.5 }, 16.7, 0.484),
            new GBData("16a", new double[] { 160, 63, 6.5,10 }, 17.2, 0.538),
            new GBData("16b", new double[] { 160, 65, 8.5,10 }, 19.8, 0.542),
            new GBData("18a", new double[] { 180, 68, 7,10.5 }, 20.2, 0.596),
            new GBData("18b", new double[] { 180, 70, 9,10.5 }, 23, 0.6),
            new GBData("20a", new double[] { 200, 73, 7,11 }, 22.6, 0.654),
            new GBData("20b", new double[] { 200, 75, 9,11 }, 25.8, 0.658),
            new GBData("22a", new double[] { 220, 77, 7,11.5 }, 25, 0.709),
            new GBData("22b", new double[] { 220, 79, 9,11.5 }, 28.5, 0.713),
            new GBData("24a", new double[] { 240, 78, 7,12 }, 26.9, 0.752),
            new GBData("24b", new double[] { 240, 80, 9,12 }, 30.6, 0.756),
            new GBData("24c", new double[] { 240, 82, 11,12 }, 34.4, 0.76),
            new GBData("25a", new double[] { 250, 78, 7,12 }, 27.4, 0.722),
            new GBData("25b", new double[] { 250, 80, 9,12 }, 31.3, 0.776),
            new GBData("25c", new double[] { 250, 82, 11,12 }, 35.3, 0.78),
            new GBData("27a", new double[] { 270, 82, 7.5,12.5 }, 30.8, 0.826),
            new GBData("27b", new double[] { 270, 84, 9.5,12.5 }, 35.1, 0.83),
            new GBData("27c", new double[] { 270, 86, 11.5,12.5 }, 39.3, 0.834),
            new GBData("28a", new double[] { 280, 82, 7.5,12.5 }, 31.4, 0.846),
            new GBData("28b", new double[] { 280, 84, 9.5,12.5 }, 35.8, 0.85),
            new GBData("28c", new double[] { 280, 86, 11.5,12.5 }, 40.2, 0.854),
            new GBData("30a", new double[] { 300, 85, 7.5,13.5 }, 34.5, 0.897),
            new GBData("30b", new double[] { 300, 87, 9.5,13.5 }, 39.2, 0.901),
            new GBData("30c", new double[] { 300, 89, 11.5,13.5 }, 43.9, 0.905),
            new GBData("32a", new double[] { 320, 88, 8,14 }, 38.1, 0.947),
            new GBData("32b", new double[] { 320, 90, 10,14 }, 43.1, 0.951),
            new GBData("32c", new double[] { 320, 92, 12,14 }, 48.1, 0.955),
            new GBData("36a", new double[] { 360, 96, 9,16 }, 47.8, 1.053),
            new GBData("36b", new double[] { 360, 98, 11,16 }, 53.5, 1.057),
            new GBData("36c", new double[] { 360, 100, 13,16 }, 59.1, 1.061),
            new GBData("40a", new double[] { 400, 100, 10.5,18 }, 58.9, 1.144),
            new GBData("40b", new double[] { 400, 102, 12.5,18 }, 65.2, 1.148),
            new GBData("40c", new double[] { 400, 104, 14.5,18 }, 71.5, 1.152),
        };
        public override GBData[] GBDataSet => _gbDataSet;
        public SectionSteel_CHAN_BtB() { }
        public SectionSteel_CHAN_BtB(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = (h, b, s, t, data);
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.CHAN_BtB_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);

                    data = FindGBData(_gbDataSet, h, b, s);
                    if (data == null)
                        throw new MismatchedProfileTextException(e.NewText);

                    t = data.Parameters[3];
                } else {
                    match = Regex.Match(e.NewText, Pattern_Collection.CHAN_BtB_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException(e.NewText);

                    double.TryParse(match.Groups["CODE"].Value, out double code);
                    var suffix = match.Groups["SUFFIX"].Value;
                    var name = match.Groups["NAME"].Value;
                    if (code >= 14 && string.IsNullOrEmpty(suffix))
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
                    formula += "*6";
                else
                    formula += "*8";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{s}*4";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    break;
                formula = $"{data.Area}*2-{h}*2";
                if (exclude_topSurface)
                    formula += $"-{b}*2";
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
            b = this.b - s;
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
                formula = $"{data.Weight}*2";

            return formula;
        }
    }
}
