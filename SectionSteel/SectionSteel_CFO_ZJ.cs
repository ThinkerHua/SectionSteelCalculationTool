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
 *  SectionSteel_CFO_ZJ.cs: 冷弯开口型钢（Cold forming open section steel）卷边Z型钢（含斜卷边Z型钢XZ）
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>冷弯开口型钢（Cold forming open section steel）卷边Z型钢（含斜卷边Z型钢XZ）。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CFO_ZJ_1"/>: <inheritdoc cref="Pattern_Collection.CFO_ZJ_1"/><br/>
    /// <see cref="Pattern_Collection.CFO_ZJ_2"/>: <inheritdoc cref="Pattern_Collection.CFO_ZJ_2"/><br/>
    /// </summary>
    public class SectionSteel_CFO_ZJ : SectionSteelBase {
        private double h, b1, c1, b2, c2, t;
        private GBData data;
        private static readonly GBData[] _gbDataSet = new GBData[] {
            new GBData("", new double[] { 100, 40, 20,2 }, 3.208, 0),
            new GBData("", new double[] { 100, 40, 20,2.5 }, 3.933, 0),
            new GBData("", new double[] { 120, 50, 20,2 }, 3.82, 0),
            new GBData("", new double[] { 120, 50, 20,2.5 }, 4.7, 0),
            new GBData("", new double[] { 120, 50, 20,3 }, 5.54, 0),
            new GBData("", new double[] { 140, 50, 20,2.5 }, 5.11, 0),
            new GBData("", new double[] { 140, 50, 20,3 }, 6.04, 0),
            new GBData("", new double[] { 160, 60, 20,2.5 }, 5.87, 0),
            new GBData("", new double[] { 160, 60, 20,3 }, 6.95, 0),
            new GBData("", new double[] { 160, 70, 20,2.5 }, 6.27, 0),
            new GBData("", new double[] { 160, 70, 20,3 }, 7.42, 0),
            new GBData("", new double[] { 180, 70, 20,2.5 }, 6.68, 0),
            new GBData("", new double[] { 180, 70, 20,3 }, 7.924, 0),
            new GBData("", new double[] { 230, 75, 25,3 }, 9.573, 0),
            new GBData("", new double[] { 230, 75, 25,4 }, 12.518, 0),
            new GBData("", new double[] { 250, 75, 25,3 }, 10.044, 0),
            new GBData("", new double[] { 250, 75, 25,4 }, 13.146, 0),
            new GBData("", new double[] { 300, 100, 30,4 }, 16.545, 0),
            new GBData("", new double[] { 300, 100, 30,6 }, 23.88, 0),
            new GBData("", new double[] { 400, 120, 40,8 }, 40.789, 0),
            new GBData("", new double[] { 400, 120, 40,10 }, 49.692, 0),
        };
        public override GBData[] GBDataSet => _gbDataSet;
        public SectionSteel_CFO_ZJ() { }
        public SectionSteel_CFO_ZJ(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.CFO_ZJ_1);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.CFO_ZJ_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException(e.NewText);

                double.TryParse(match.Groups["h"].Value, out h);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["c1"].Value, out c1);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["c2"].Value, out c2);
                double.TryParse(match.Groups["t"].Value, out t);

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
                    formula = $"{h}*2+{b1}*2+{b2}";
                    if (!exclude_topSurface)
                        formula += $"*2";
                } else {
                    formula = $"{h}*2+{b1}";
                    if (exclude_topSurface)
                        formula += "*3";
                    else
                        formula += "*4";
                }

                if (c2 != c1)
                    formula += $"+{c1}*2+{c2}*2";
                else
                    formula += $"+{c1}*4";
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{t}*8";
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
                    formula += $"+{c1}+{c2}";
                else
                    formula += $"+{c1}*2";

                formula += $"-{t}*4)*{t}*{DENSITY}";
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
