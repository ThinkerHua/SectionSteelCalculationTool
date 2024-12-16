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
 *  SectionSteel_PL_Circular.cs: 圆形板
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
    /// <para>圆形板。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.PL_O_1"/>: <inheritdoc cref="Pattern_Collection.PL_O_1"/><para></para>
    /// </summary>
    public class SectionSteel_PL_Circular : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double t, d;
        public PIStyleEnum PIStyle { get; set; }
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public SectionSteel_PL_Circular() {

        }
        public SectionSteel_PL_Circular(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            t = d = 0;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.PL_O_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["t"].Value, out t);
                double.TryParse(match.Groups["d"].Value, out d);

                if (d < t) { (t, d) = (d, t); }

                t *= 0.001; d *= 0.001;
            } catch (MismatchedProfileTextException) {
                t = d = 0;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (d == 0) return formula;//实际使用中可能 t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                var PI = PIStyle == 0 ? "PI()" : "3.14";
                formula = $"{PI}*{d * 0.5}^2";
                if (!exclude_topSurface)
                    formula += "*2";
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
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (d == 0) return formula;//实际使用中可能 t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                var PI = PIStyle == 0 ? "PI()" : "3.14";
                if (t == 0)
                    formula = "0";
                else
                    formula = $"{PI}*{d * 0.5}^2*{t}*{GBData.DENSITY}";
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