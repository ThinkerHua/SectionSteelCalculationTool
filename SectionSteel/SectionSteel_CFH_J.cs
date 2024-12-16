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
 *  SectionSteel_CFH_J.cs: 冷弯空心型钢（Cold forming hollow section steel）方管和矩管
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
    /// <para>冷弯空心型钢（Cold forming hollow section steel）方管和矩管。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CFH_J_1"/>: <inheritdoc cref="Pattern_Collection.CFH_J_1"/><para></para>
    /// <see cref="Pattern_Collection.CFH_J_2"/>: <inheritdoc cref="Pattern_Collection.CFH_J_2"/><para></para>
    /// <see cref="Pattern_Collection.CFH_J_3"/>: <inheritdoc cref="Pattern_Collection.CFH_J_3"/><para></para>
    /// </summary>
    public class SectionSteel_CFH_J : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h1, b1, h2, b2, t;
        GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CFH_J() {

        }
        public SectionSteel_CFH_J(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h1 = b1 = h2 = b2 = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CFH_J_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFH_J_2);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.CFH_J_3);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                double.TryParse(match.Groups["h1"].Value, out h1);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["h2"].Value, out h2);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (b1 == 0) b1 = h1;
                if (h2 == 0) h2 = h1;
                if (b2 == 0) b2 = b1;

                if (h2 == h1 && b2 == b1)
                    data = GBData.SearchGBData(GBData.CFH_J, new double[] { h1, b1, t });

                h1 *= 0.001; b1 *= 0.001; h2 *= 0.001; b2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h1 = b1 = h2 = b2 = t = 0;
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
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (b1 == h1 && b2 == h2 && h2 == h1) {
                    if (exclude_topSurface)
                        formula = $"{h1}*3";
                    else
                        formula = $"{h1}*4";

                    break;
                }

                if (h2 != h1)
                    formula = $"{h1}+{h2}";
                else
                    formula = $"{h1}*2";

                if (b2 != b1) {
                    if (exclude_topSurface)
                        formula += $"+{(b1 + b2) * 0.5}";
                    else
                        formula += $"+{b1}+{b2}";
                } else {
                    if (exclude_topSurface)
                        formula += $"+{b1}";
                    else
                        formula += $"+{b1}*2";
                }
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
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = this.t;
            b = (b1 + b2) * 0.5 - this.t * 2;
            l = (h1 + h2) * 0.5 - this.t * 2;
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
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 == h1 && b2 == b1 && b1 == h1)
                    formula = $"({h1}*4";
                else {
                    if (h2 != h1)
                        formula = $"({h1}+{h2}";
                    else
                        formula = $"({h1}*2";

                    if (b2 != b1)
                        formula += $"+{b1}+{b2}";
                    else
                        formula += $"+{b1}*2";
                }

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