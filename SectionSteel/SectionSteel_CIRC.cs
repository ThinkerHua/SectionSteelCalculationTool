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
 *  SectionSteel_CIRC.cs: 圆钢或圆管
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>圆钢或圆管。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CIRC_1"/>: <inheritdoc cref="Pattern_Collection.CIRC_1"/><br/>
    /// <see cref="Pattern_Collection.CIRC_2"/>: <inheritdoc cref="Pattern_Collection.CIRC_2"/><br/>
    /// <see cref="Pattern_Collection.CIRC_3"/>: <inheritdoc cref="Pattern_Collection.CIRC_3"/><br/>
    /// </summary>
    public class SectionSteel_CIRC : SectionSteelBase {
        private double d1, r1, d2, r2, t;

        public override GBData[] GBDataSet => null;

        public SectionSteel_CIRC() { }
        public SectionSteel_CIRC(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.CIRC_1);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.CIRC_2);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.CIRC_3);
                if (!match.Success)
                    throw new MismatchedProfileTextException(e.NewText);

                double.TryParse(match.Groups["d1"].Value, out d1);
                double.TryParse(match.Groups["r1"].Value, out r1);
                double.TryParse(match.Groups["d2"].Value, out d2);
                double.TryParse(match.Groups["r2"].Value, out r2);
                double.TryParse(match.Groups["t"].Value, out t);

                if (r1 == 0) r1 = d1;
                if (d2 == 0) d2 = d1;
                if (r2 == 0) r2 = d2;
                d1 *= 0.001; r1 *= 0.001; d2 *= 0.001; r2 *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {

                throw;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY，不实现GBDATA</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/>
        /// <para>椭圆周长采用估算公式：h=(a-b)^2/(a+b)^2, p=π(a+b)(1+3h/(10+(4-3h)^0.5))</para>
        /// </returns>
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            string PI, c1, c2;

            if (d1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                PI = PIStyle == 0 ? "PI()" : "3.14";

                if (r1 == d1)
                    c1 = $"{PI}*{d1}";
                else
                    c1 = EllipseCircumference(d1 * 0.5, r1 * 0.5).ToString();

                if (r2 == d2)
                    c2 = $"{PI}*{d2}";
                else
                    c2 = EllipseCircumference(d2 * 0.5, r2 * 0.5).ToString();

                if (c2.Equals(c1))
                    formula = $"{c1}";
                else {
                    if (r1 == d1 && r2 == d2)
                        formula = $"{PI}*({c1.Remove(0, PI.Length + 1)}+{c2.Remove(0, PI.Length + 1)})*0.5";
                    else
                        formula = $"({c1}+{c2})*0.5";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// 根据椭圆的长、短半轴估算周长，估算公式 h=(a-b)^2/(a+b)^2, p=π(a+b)(1+3h/(10+(4-3h)^0.5))
        /// </summary>
        /// <param name="a">长半轴</param>
        /// <param name="b">短半轴</param>
        /// <returns>椭圆周长的估算值，保留三位小数。</returns>
        protected double EllipseCircumference(double a, double b) {
            double h, result = 0;

            if (a + b == 0) return result;
            h = Math.Pow(a - b, 2) / Math.Pow(a + b, 2);
            result = Math.PI * (a + b) * (1 + 3 * h / (10 + Math.Sqrt(4 - 3 * h)));

            return Math.Round(result, 3);
        }

        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (d1 == 0) return stifProfileText;

            double t, d;
            t = this.t;
            d = (d1 + r1 + d2 + r2) * 0.25 - t * 2;
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
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            string PI, s1, s2;

            if (d1 == 0) return formula;
            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                PI = PIStyle == 0 ? "PI()" : "3.14";
                if (r1 == d1) {
                    if (t == 0)
                        s1 = $"{d1 * 0.5}^2";
                    else
                        s1 = $"{d1 * 0.5}^2-{d1 * 0.5 - t}^2";
                } else {
                    if (t == 0)
                        s1 = $"{d1 * 0.5}*{r1 * 0.5}";
                    else
                        s1 = $"{d1 * 0.5}*{r1 * 0.5}-{d1 * 0.5 - t}*{r1 * 0.5 - t}";
                }

                if (r2 == d2) {
                    if (t == 0)
                        s2 = $"{d2 * 0.5}^2";
                    else
                        s2 = $"{d2 * 0.5}^2-{d2 * 0.5 - t}^2";
                } else {
                    if (t == 0)
                        s2 = $"{d2 * 0.5}*{r2 * 0.5}";
                    else
                        s2 = $"{d2 * 0.5}*{r2 * 0.5}-{d2 * 0.5 - t}*{r2 * 0.5 - t}";
                }

                if (s2.Equals(s1)) {
                    if (t == 0)
                        formula = $"{PI}*{s1}*{DENSITY}";
                    else
                        formula = $"{PI}*({s1})*{DENSITY}";
                } else
                    formula = $"{PI}*({s1}+{s2})*0.5*{DENSITY}";
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
