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
using System.Linq;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>矩形板件。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.PL_1"/>: <inheritdoc cref="Pattern_Collection.PL_1"/>
    /// </summary>
    public class SectionSteel_PL : SectionSteelBase {
        private double t, b, l;

        public override GBData[]? GBDataSet => null;

        public SectionSteel_PL() { }
        public SectionSteel_PL(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = (t, b, l);
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.PL_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException(e.NewText);

                var paramsStr = new string[3];
                var paramsValue = new double[3];
                var isVariable = new bool[3] { false, false, false };

                paramsStr[0] = match.Groups["t"].Value;
                paramsStr[1] = match.Groups["b"].Value;
                paramsStr[2] = match.Groups["l"].Value;

                for (int i = 0; i < 3; i++) {
                    var subMatch = Regex.Match(paramsStr[i], Pattern_Collection.VariableCrossSection);
                    if (subMatch.Success) {
                        isVariable[i] = true;
                        _ = double.TryParse(subMatch.Groups["v1"].Value, out double tmp1);
                        _ = double.TryParse(subMatch.Groups["v2"].Value, out double tmp2);
                        paramsValue[i] = (tmp1 + tmp2) * 0.5;
                    } else {
                        _ = double.TryParse(paramsStr[i], out paramsValue[i]);
                    }
                }

                var query = paramsValue.Select((v, i) => (v, i)).OrderBy(item => item.v).ToArray();
                if (query[0].v == 0.0) {
                    //PLt*b情形，对t参数使用了'~'
                    if (isVariable[query[1].i])
                        throw new MismatchedProfileTextException(e.NewText);

                    t = query[1].v; b = query[2].v; l = 0;
                } else {
                    //PLt*b*l情形，对t参数使用了'~'，或同时对b、l参数使用了'~'
                    if (isVariable[query[0].i] || isVariable[query[1].i] && isVariable[query[2].i])
                        throw new MismatchedProfileTextException(e.NewText);

                    t = query[0].v; b = query[1].v; l = query[2].v;
                }

                t *= 0.001; b *= 0.001; l *= 0.001;
            } catch (MismatchedProfileTextException) {
                t = tmp.t; b = tmp.b; l = tmp.l;
                throw;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现 GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
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
        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            return string.Empty;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (b == 0) return formula;//实际使用中可能t == 0

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (t == 0) {
                    formula = "0";
                } else {
                    if (l == 0)
                        formula = $"{b}*{t}*{DENSITY}";
                    else
                        formula = $"{b}*{l}*{t}*{DENSITY}";
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
