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
 *  SectionSteel_CFH_Y.cs: 冷弯空心型钢（Cold forming hollow section steel）圆管
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>冷弯空心型钢（Cold forming hollow section steel）圆管。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CFH_Y_1"/>: <inheritdoc cref="Pattern_Collection.CFH_Y_1"/>
    /// </summary>
    public class SectionSteel_CFH_Y : SectionSteelBase {
        private double d, t;
        private GBData? data;
        private static readonly GBData[] _gbDataSet = new GBData[] {
            new GBData("", new double[] { 21.3, 1.2 }, 0.59, 0),
            new GBData("", new double[] { 21.3, 1.5 }, 0.73, 0),
            new GBData("", new double[] { 21.3, 1.75 }, 0.84, 0),
            new GBData("", new double[] { 21.3, 2 }, 0.95, 0),
            new GBData("", new double[] { 21.3, 2.5 }, 1.16, 0),
            new GBData("", new double[] { 21.3, 3 }, 1.35, 0),
            new GBData("", new double[] { 26.8, 1.2 }, 0.76, 0),
            new GBData("", new double[] { 26.8, 1.5 }, 0.94, 0),
            new GBData("", new double[] { 26.8, 1.75 }, 1.08, 0),
            new GBData("", new double[] { 26.8, 2 }, 1.22, 0),
            new GBData("", new double[] { 26.8, 2.5 }, 1.5, 0),
            new GBData("", new double[] { 26.8, 3 }, 1.76, 0),
            new GBData("", new double[] { 33.5, 1.5 }, 1.18, 0),
            new GBData("", new double[] { 33.5, 2 }, 1.55, 0),
            new GBData("", new double[] { 33.5, 2.5 }, 1.91, 0),
            new GBData("", new double[] { 33.5, 3 }, 2.26, 0),
            new GBData("", new double[] { 33.5, 3.5 }, 2.59, 0),
            new GBData("", new double[] { 33.5, 4 }, 2.91, 0),
            new GBData("", new double[] { 42.3, 1.5 }, 1.51, 0),
            new GBData("", new double[] { 42.3, 2 }, 1.99, 0),
            new GBData("", new double[] { 42.3, 2.5 }, 2.45, 0),
            new GBData("", new double[] { 42.3, 3 }, 2.91, 0),
            new GBData("", new double[] { 42.3, 4 }, 3.78, 0),
            new GBData("", new double[] { 48, 1.5 }, 1.72, 0),
            new GBData("", new double[] { 48, 2 }, 2.27, 0),
            new GBData("", new double[] { 48, 2.5 }, 2.81, 0),
            new GBData("", new double[] { 48, 3 }, 3.33, 0),
            new GBData("", new double[] { 48, 4 }, 4.34, 0),
            new GBData("", new double[] { 48, 5 }, 5.3, 0),
            new GBData("", new double[] { 60, 2 }, 2.86, 0),
            new GBData("", new double[] { 60, 2.5 }, 3.55, 0),
            new GBData("", new double[] { 60, 3 }, 4.22, 0),
            new GBData("", new double[] { 60, 4 }, 5.52, 0),
            new GBData("", new double[] { 60, 5 }, 6.78, 0),
            new GBData("", new double[] { 75.5, 2.5 }, 4.5, 0),
            new GBData("", new double[] { 75.5, 3 }, 5.36, 0),
            new GBData("", new double[] { 75.5, 4 }, 7.05, 0),
            new GBData("", new double[] { 75.5, 5 }, 8.69, 0),
            new GBData("", new double[] { 88.5, 3 }, 6.33, 0),
            new GBData("", new double[] { 88.5, 4 }, 8.34, 0),
            new GBData("", new double[] { 88.5, 5 }, 10.3, 0),
            new GBData("", new double[] { 88.5, 6 }, 12.21, 0),
            new GBData("", new double[] { 114, 4 }, 10.85, 0),
            new GBData("", new double[] { 114, 5 }, 13.44, 0),
            new GBData("", new double[] { 114, 6 }, 15.98, 0),
            new GBData("", new double[] { 140, 4 }, 13.42, 0),
            new GBData("", new double[] { 140, 5 }, 16.65, 0),
            new GBData("", new double[] { 140, 6 }, 19.83, 0),
            new GBData("", new double[] { 165, 4 }, 15.88, 0),
            new GBData("", new double[] { 165, 5 }, 19.73, 0),
            new GBData("", new double[] { 165, 6 }, 23.53, 0),
            new GBData("", new double[] { 165, 8 }, 30.97, 0),
            new GBData("", new double[] { 219.1, 5 }, 26.4, 0),
            new GBData("", new double[] { 219.1, 6 }, 31.53, 0),
            new GBData("", new double[] { 219.1, 8 }, 41.6, 0),
            new GBData("", new double[] { 219.1, 10 }, 51.6, 0),
            new GBData("", new double[] { 273, 5 }, 33, 0),
            new GBData("", new double[] { 273, 6 }, 39.5, 0),
            new GBData("", new double[] { 273, 8 }, 52.3, 0),
            new GBData("", new double[] { 273, 10 }, 64.9, 0),
            new GBData("", new double[] { 325, 5 }, 39.5, 0),
            new GBData("", new double[] { 325, 6 }, 47.2, 0),
            new GBData("", new double[] { 325, 8 }, 62.5, 0),
            new GBData("", new double[] { 325, 10 }, 77.7, 0),
            new GBData("", new double[] { 325, 12 }, 92.6, 0),
            new GBData("", new double[] { 355.6, 6 }, 51.7, 0),
            new GBData("", new double[] { 355.6, 8 }, 68.6, 0),
            new GBData("", new double[] { 355.6, 10 }, 85.2, 0),
            new GBData("", new double[] { 355.6, 12 }, 101.7, 0),
            new GBData("", new double[] { 406.4, 8 }, 78.6, 0),
            new GBData("", new double[] { 406.4, 10 }, 97.8, 0),
            new GBData("", new double[] { 406.4, 12 }, 116.7, 0),
            new GBData("", new double[] { 457, 8 }, 88.6, 0),
            new GBData("", new double[] { 457, 10 }, 110, 0),
            new GBData("", new double[] { 457, 12 }, 131.7, 0),
            new GBData("", new double[] { 508, 8 }, 98.6, 0),
            new GBData("", new double[] { 508, 10 }, 123, 0),
            new GBData("", new double[] { 508, 12 }, 146.8, 0),
            new GBData("", new double[] { 610, 8 }, 118.8, 0),
            new GBData("", new double[] { 610, 10 }, 148, 0),
            new GBData("", new double[] { 610, 12.5 }, 184.2, 0),
            new GBData("", new double[] { 610, 16 }, 234.4, 0),
        };
        public override GBData[]? GBDataSet => _gbDataSet;
        public SectionSteel_CFH_Y() { }
        public SectionSteel_CFH_Y(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.CFH_Y_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException(e.NewText);

                _ = double.TryParse(match.Groups["d"].Value, out d);
                _ = double.TryParse(match.Groups["t"].Value, out t);

                data = FindGBData(_gbDataSet, d, t);

                d *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {

                throw;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"{(PIStyle == 0 ? "PI()" : "3.14")}*{d}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }

        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (this.d == 0) return stifProfileText;

            double t, d;
            t = this.t;
            d = this.d - this.t * 2;
            t *= 1000; d *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                d = Math.Truncate(d);
            }
            stifProfileText = $"PLD{t}*{d}";

            return stifProfileText;
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
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"{(PIStyle == 0 ? "PI()" : "3.14")}*({d * 0.5}^2-{d * 0.5 - t}^2)*{DENSITY}";
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
    }
}
