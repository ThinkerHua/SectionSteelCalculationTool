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
 *  SectionSteel_RECT.cs: 焊接矩形管
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>焊接矩形管。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.RECT_1"/>: <inheritdoc cref="Pattern_Collection.RECT_1"/><br/>
    /// <see cref="Pattern_Collection.RECT_2"/>: <inheritdoc cref="Pattern_Collection.RECT_2"/><br/>
    /// <see cref="Pattern_Collection.RECT_3"/>: <inheritdoc cref="Pattern_Collection.RECT_3"/><br/>
    /// <see cref="Pattern_Collection.RECT_4"/>: <inheritdoc cref="Pattern_Collection.RECT_4"/><br/>
    /// <see cref="Pattern_Collection.RECT_5"/>: <inheritdoc cref="Pattern_Collection.RECT_5"/><br/>
    /// <see cref="Pattern_Collection.RECT_6"/>: <inheritdoc cref="Pattern_Collection.RECT_6"/><br/>
    /// </summary>
    public class SectionSteel_RECT : SectionSteelBase {
        private double h1, h2, b1, b2, s, t;

        public override GBData[] GBDataSet => null;

        public SectionSteel_RECT() { }
        public SectionSteel_RECT(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = (h1, h2, b1, b2, s, t);
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.RECT_1);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.RECT_2);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.RECT_3);
                if (!match.Success)
                    match = Regex.Match(e.NewText, Pattern_Collection.RECT_4);
                if (match.Success) {
                    double.TryParse(match.Groups["h1"].Value, out h1);
                    double.TryParse(match.Groups["h2"].Value, out h2);
                    double.TryParse(match.Groups["b1"].Value, out b1);
                    double.TryParse(match.Groups["b2"].Value, out b2);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double.TryParse(match.Groups["t"].Value, out t);
                } else {
                    match = Regex.Match(e.NewText, Pattern_Collection.RECT_5);
                    if (match.Success) {
                        double.TryParse(match.Groups["h1"].Value, out h1);
                        double.TryParse(match.Groups["h2"].Value, out h2);
                        double.TryParse(match.Groups["s"].Value, out s);

                        b1 = h1; b2 = h2;
                    } else {
                        match = Regex.Match(e.NewText, Pattern_Collection.RECT_6);
                        if (!match.Success)
                            throw new MismatchedProfileTextException(e.NewText);

                        double.TryParse(match.Groups["H1"].Value, out double H1);
                        double.TryParse(match.Groups["H2"].Value, out double H2);
                        double.TryParse(match.Groups["h1"].Value, out h1);
                        double.TryParse(match.Groups["h2"].Value, out h2);
                        double.TryParse(match.Groups["b1"].Value, out b1);
                        double.TryParse(match.Groups["s"].Value, out s);

                        h1 = (H1 + h1) * 0.5;
                        h2 = (H2 + h2) * 0.5;
                    }
                }

                if (b1 == 0) b1 = h1;
                if (h2 == 0) h2 = h1;
                if (b2 == 0) b2 = b1;
                if (t == 0) t = s;

                h1 *= 0.001; h2 *= 0.001; b1 *= 0.001; b2 *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h1 = tmp.h1; h2 = tmp.h2; b1 = tmp.b1; b2 = tmp.b2;
                s = tmp.s; t = tmp.t;
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
                        formula += $"+({b1}+{b2})*0.5";
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

        public override string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = (b1 + b2) * 0.5 - s * 2;
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
        /// <inheritdoc path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY，不实现GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1)
                    formula = $"(({(h1 + h2) * 0.5}-{t}*2)*{s}";
                else
                    formula = $"(({h1}-{t}*2)*{s}";

                if (b2 != b1)
                    formula += $"+{(b2 + b1) * 0.5}*{t})*2";
                else
                    formula += $"+{b1}*{t})*2";

                formula += $"*{DENSITY}";
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
