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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SectionSteel {
    /// <summary>
    /// <para>冷弯空心型钢（Cold forming hollow section steel）圆管。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CFH_Y_1"/>: <inheritdoc cref="Pattern_Collection.CFH_Y_1"/><para></para>
    /// </summary>
    public class SectionSteel_CFH_Y : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double d, t;
        private GBDataBase data;
        public PIStyleEnum PIStyle { get; set; }
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public SectionSteel_CFH_Y() {

        }
        public SectionSteel_CFH_Y(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            d = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFH_Y_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["d"].Value, out d);
                double.TryParse(match.Groups["t"].Value, out t);

                data = GBData.SearchGBData(GBData.CFH_Y, new double[] { d, t });

                d *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                d = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现GBDATA（国标截面特性表格中无表面积数据）</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
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

        public string GetSiffenerProfileStr(bool truncatedRounding) {
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
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                formula = $"{(PIStyle == 0 ? "PI()" : "3.14")}*({d * 0.5}^2-{d * 0.5 - t}^2)*{GBData.DENSITY}";
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
