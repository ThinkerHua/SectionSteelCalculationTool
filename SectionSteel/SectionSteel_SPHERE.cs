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
 *  SectionSteel_SPHERE.cs: 球体
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>球体。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.SPHERE_1"/>: <inheritdoc cref="Pattern_Collection.SPHERE_1"/>
    /// </summary>
    public class SectionSteel_SPHERE : SectionSteelBase {
        private double d;

        public override GBData[]? GBDataSet => null;

        public SectionSteel_SPHERE() { }
        public SectionSteel_SPHERE(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = d;
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.SPHERE_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException(e.NewText);

                _ = double.TryParse(match.Groups["d"].Value, out d);

                d *= 0.001;
            } catch (MismatchedProfileTextException) {
                d = tmp;
                throw;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy"><inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface"><inheritdoc path="/param[2]"/></param>
        /// <returns><inheritdoc/></returns>
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                var PI = PIStyle == 0 ? "PI()" : "3.14";
                formula = $"4*{PI}*{d * 0.5}^2";
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
        /// <param name="accuracy"><inheritdoc/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (d == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                var PI = PIStyle == 0 ? "PI()" : "3.14";
                formula = $"4/3*{PI}*{d * 0.5}^3*{DENSITY}";
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
    }
}
