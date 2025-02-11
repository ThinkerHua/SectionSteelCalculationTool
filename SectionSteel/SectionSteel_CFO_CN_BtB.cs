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
 *  SectionSteel_CFO_CN_BtB.cs: 冷弯开口型钢（Cold forming open section steel）内卷边槽钢，背对背双拼
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>冷弯开口型钢（Cold forming open section steel）内卷边槽钢，背对背双拼。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CFO_CN_BtB_1"/>: <inheritdoc cref="Pattern_Collection.CFO_CN_BtB_1"/><br/>
    /// <see cref="Pattern_Collection.CFO_CN_BtB_2"/>: <inheritdoc cref="Pattern_Collection.CFO_CN_BtB_2"/><br/>
    /// </summary>
    public class SectionSteel_CFO_CN_BtB : SectionSteelBase {
        private double h, b1, c1, b2, c2, t;
        private GBData? data;
        private static readonly GBData[] _gbDataSet = new GBData[] {
            new GBData("", new double[] { 60, 30, 10,2.5 }, 2.363, 0),
            new GBData("", new double[] { 60, 30, 10,3 }, 2.743, 0),
            new GBData("", new double[] { 80, 40, 15,2 }, 2.72, 0),
            new GBData("", new double[] { 100, 50, 15,2.5 }, 4.11, 0),
            new GBData("", new double[] { 100, 50, 20,2.5 }, 4.325, 0),
            new GBData("", new double[] { 100, 50, 20,3 }, 5.098, 0),
            new GBData("", new double[] { 120, 50, 20,2.5 }, 4.7, 0),
            new GBData("", new double[] { 120, 60, 20,3 }, 6.01, 0),
            new GBData("", new double[] { 140, 50, 20,2 }, 4.14, 0),
            new GBData("", new double[] { 140, 50, 20,2.5 }, 5.09, 0),
            new GBData("", new double[] { 140, 60, 20,2.5 }, 5.503, 0),
            new GBData("", new double[] { 140, 60, 20,3 }, 6.511, 0),
            new GBData("", new double[] { 160, 60, 20,2 }, 4.76, 0),
            new GBData("", new double[] { 160, 60, 20,2.5 }, 5.87, 0),
            new GBData("", new double[] { 160, 70, 20,3 }, 7.42, 0),
            new GBData("", new double[] { 180, 60, 20,3 }, 7.453, 0),
            new GBData("", new double[] { 180, 70, 20,3 }, 7.924, 0),
            new GBData("", new double[] { 180, 70, 20,2 }, 5.39, 0),
            new GBData("", new double[] { 180, 70, 20,2.5 }, 6.66, 0),
            new GBData("", new double[] { 200, 60, 20,3 }, 7.924, 0),
            new GBData("", new double[] { 200, 70, 20,2 }, 5.71, 0),
            new GBData("", new double[] { 200, 70, 20,2.5 }, 7.05, 0),
            new GBData("", new double[] { 200, 70, 20,3 }, 8.395, 0),
            new GBData("", new double[] { 220, 75, 20,2 }, 6.18, 0),
            new GBData("", new double[] { 220, 75, 20,2.5 }, 7.64, 0),
            new GBData("", new double[] { 250, 40, 15,3 }, 7.924, 0),
            new GBData("", new double[] { 300, 40, 15,3 }, 9.102, 0),
            new GBData("", new double[] { 400, 50, 15,3 }, 11.928, 0),
            new GBData("", new double[] { 450, 70, 30,6 }, 28.092, 0),
            new GBData("", new double[] { 450, 70, 30,8 }, 36.421, 0),
            new GBData("", new double[] { 500, 100, 40,6 }, 34.176, 0),
            new GBData("", new double[] { 500, 100, 40,8 }, 44.533, 0),
            new GBData("", new double[] { 500, 100, 40,10 }, 54.372, 0),
            new GBData("", new double[] { 550, 120, 50,8 }, 51.397, 0),
            new GBData("", new double[] { 550, 120, 50,10 }, 62.952, 0),
            new GBData("", new double[] { 550, 120, 50,12 }, 73.99, 0),
            new GBData("", new double[] { 600, 150, 60,12 }, 86.158, 0),
            new GBData("", new double[] { 600, 150, 60,14 }, 97.395, 0),
            new GBData("", new double[] { 600, 150, 60,16 }, 109.025, 0),
        };
        public override GBData[]? GBDataSet => _gbDataSet;
        public SectionSteel_CFO_CN_BtB() { }
        public SectionSteel_CFO_CN_BtB(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.CFO_CN_BtB_1);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.CFO_CN_BtB_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException(e.NewText);

                _ = double.TryParse(match.Groups["h"].Value, out h);
                _ = double.TryParse(match.Groups["b1"].Value, out b1);
                _ = double.TryParse(match.Groups["c1"].Value, out c1);
                _ = double.TryParse(match.Groups["b2"].Value, out b2);
                _ = double.TryParse(match.Groups["c2"].Value, out c2);
                _ = double.TryParse(match.Groups["t"].Value, out t);

                if (b2 == 0) b2 = b1;
                if (c2 == 0) c2 = c1;

                if (b2 == b1 && c2 == c1)
                    data = FindGBData(_gbDataSet, h, b1, c1, t);
                else
                    data = null;

                h *= 0.001; b1 *= 0.001; c1 *= 0.001; b2 *= 0.001; c2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {

                throw;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：不实现 GBDATA（国标截面特性表格中无表面积数据）</b></para>
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
                if (b2 != b1) {
                    formula = $"{h}*2+{b1}*4+{b2}";
                    if (exclude_topSurface)
                        formula += "*2";
                    else
                        formula += "*4";
                } else {
                    formula = $"{h}*2+{b1}";
                    if (exclude_topSurface)
                        formula += "*6";
                    else
                        formula += "*8";
                }

                if (c2 != c1)
                    formula += $"+{c1}*4+{c2}*4";
                else
                    formula += $"+{c1}*8";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{t}*16";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// <para><b>本类不实现此方法。</b></para>
        /// </summary>
        /// <param name="truncatedRounding"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
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
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (b2 != b1)
                    formula = $"({h}+{b1}+{b2}";
                else
                    formula = $"({h}+{b1}*2";

                if (c2 != c1)
                    formula += $"+{c1}+{c2}-{t}*4)*{t}*{DENSITY}*2";
                else
                    formula += $"+{c1}*2-{t}*4)*{t}*{DENSITY}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data != null)
                    formula = $"{data.Weight}*2";
                break;
            default:
                break;
            }

            return formula;
        }
    }
}
