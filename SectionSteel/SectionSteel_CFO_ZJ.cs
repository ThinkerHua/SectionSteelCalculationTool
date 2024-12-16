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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SectionSteel {
    /// <summary>
    /// <para>冷弯开口型钢（Cold forming open section steel）卷边Z型钢（含斜卷边Z型钢XZ）。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CFO_ZJ_1"/>: <inheritdoc cref="Pattern_Collection.CFO_ZJ_1"/><para></para>
    /// <see cref="Pattern_Collection.CFO_ZJ_2"/>: <inheritdoc cref="Pattern_Collection.CFO_ZJ_2"/><para></para>
    /// </summary>
    public class SectionSteel_CFO_ZJ : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b1, c1, b2, c2, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CFO_ZJ() {

        }
        public SectionSteel_CFO_ZJ(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b1 = c1 = b2 = c2 = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFO_ZJ_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFO_ZJ_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["h"].Value, out h);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["c1"].Value, out c1);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["c2"].Value, out c2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (b2 == 0) b2 = b1;
                if (c2 == 0) c2 = c1;

                if (b2 == b1 && c2 == c1)
                    data = GBData.SearchGBData(GBData.CFO_ZJ, new double[] { h, b1, c1, t });

                h *= 0.001; b1 *= 0.001; c1 *= 0.001; b2 *= 0.001; c2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b1 = c1 = b2 = c2 = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：不实现 GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
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
        public string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
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

                formula += $"-{t}*4)*{t}*{GBData.DENSITY}";
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
