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
 *  SectionSteel_PL.cs: 矩形板件
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
    /// <para>矩形板件。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.PL_1"/>: <inheritdoc cref="Pattern_Collection.PL_1"/><para></para>
    /// </summary>
    public class SectionSteel_PL : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double t, b, l;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_PL() {

        }
        public SectionSteel_PL(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            t = b = l = 0;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.PL_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException();

                Match subMatch;
                List<double> parameters = new List<double> { Capacity = 3 };
                bool[] isVariable = new bool[3] { false, false, false };
                double tmp1, tmp2;

                subMatch = Regex.Match(match.Groups["t"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[0] = true;
                } else {
                    double.TryParse(match.Groups["t"].Value, out tmp1);
                    parameters.Add(tmp1);
                }

                subMatch = Regex.Match(match.Groups["b"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[1] = true;
                } else {
                    double.TryParse(match.Groups["b"].Value, out tmp1);
                    parameters.Add(tmp1);
                }

                subMatch = Regex.Match(match.Groups["l"].Value, Pattern_Collection.VariableCrossSection);
                if (subMatch.Success) {
                    double.TryParse(subMatch.Groups["v1"].Value, out tmp1);
                    double.TryParse(subMatch.Groups["v2"].Value, out tmp2);
                    parameters.Add((tmp1 + tmp2) * 0.5);
                    isVariable[2] = true;
                } else {
                    double.TryParse(match.Groups["l"].Value, out tmp1);
                    parameters.Add(tmp1);
                }


                //  不应对 t 使用 '~' 符号，且 b 和 l 中只可以有一个使用 '~' 符号
                //  虽然可以实现对全部参数使用 '~' 符号的支持，但没什么意义，禁止这样操作以免后续产生意想不到的情况
                //  附带完成t, b, l的赋值
                int minParameterIndex;
                if (parameters[2] == 0) {
                    minParameterIndex = parameters.IndexOf(Math.Min(parameters[0], parameters[1]));

                    t = parameters[minParameterIndex];
                    b = minParameterIndex == 0 ? parameters[1] : parameters[0];
                } else {
                    minParameterIndex = parameters.IndexOf(parameters.Min());

                    parameters.Sort();
                    t = parameters[0]; b = parameters[1]; l = parameters[2];
                }

                if (isVariable[minParameterIndex]) {
                    //  t 使用了 '~' 符号
                    throw new MismatchedProfileTextException();
                } else {
                    isVariable[minParameterIndex] = true;
                    //  b, l 同时使用了 '~' 符号
                    if (isVariable[0] && isVariable[1] && isVariable[2])
                        throw new MismatchedProfileTextException();
                }


                t *= 0.001; b *= 0.001; l *= 0.001;
            } catch (MismatchedProfileTextException) {
                t = b = l = 0;
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
            if (b == 0) return formula;//实现使用中可能 t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (l == 0)
                    formula = $"{b}";
                else
                    formula = $"{b}*{l}";

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
            if (b == 0) return formula;//实际使用中可能t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (t == 0) {
                    formula = "0";
                } else {
                    if (l == 0)
                        formula = $"{b}*{t}*{GBData.DENSITY}";
                    else
                        formula = $"{b}*{l}*{t}*{GBData.DENSITY}";
                }
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
