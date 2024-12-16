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
 *  SectionSteel_T.cs: T型钢
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
    /// <para>T型钢。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.T_1"/>: <inheritdoc cref="Pattern_Collection.T_1"/><para></para>
    /// <see cref="Pattern_Collection.T_2"/>: <inheritdoc cref="Pattern_Collection.T_2"/><para></para>
    /// <see cref="Pattern_Collection.T_3"/>: <inheritdoc cref="Pattern_Collection.T_3"/><para></para>
    /// <para>当匹配到T3模式时，在国标截面特性表格中查找，同一型号名下有多项时按最接近项匹配。</para>
    /// </summary>
    public class SectionSteel_T : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private string type;
        private double h1, h2, b, s, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_T() {

        }
        public SectionSteel_T(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            type = string.Empty;
            h1 = h2 = b = s = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.T_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.T_2);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.T_4);
                if (match.Success) {
                    double.TryParse(match.Groups["h1"].Value, out h1);
                    double.TryParse(match.Groups["h2"].Value, out h2);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double.TryParse(match.Groups["t"].Value, out t);

                    if (h2 == 0 || h2 == h1) {
                        double[] parameters = { h1, b, s, t };
                        data = GetGBData(parameters);
                    }
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.T_3);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    type = match.Groups["TYPE"].Value;
                    string name = match.Groups["H"].Value + "*" + match.Groups["B"].Value;
                    data = GetGBData(name);
                    if (data == null) {
                        double.TryParse(match.Groups["H"].Value, out double H);
                        double.TryParse(match.Groups["B"].Value, out double B);
                        data = GetGBData(new double[] { H, B });
                    }
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h1 = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                if (h2 == 0) h2 = h1;
                h1 *= 0.001; h2 *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                type = null;
                h1 = h2 = b = s = t = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// <para><b>在本类中：PRECISELY 等效于 ROUGHLY</b></para>
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
                if (h2 != h1) {
                    formula = $"{(h1 + h2) * 0.5}";
                } else {
                    formula = $"{h1}";
                }
                formula += $"*2+{b}";
                if (!exclude_topSurface)
                    formula += $"*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    break;
                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b}";
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
            t = s;
            b = (this.b - s) * 0.5;
            l = (h1 + h2) * 0.5 - this.t;
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
        /// <param name="accuracy">ROUGHLY 等效于 PRECISELY</param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (h2 != h1) {
                    formula = $"(({(h1 + h2) * 0.5}-{t})*{s}+{b}*{t})*{GBData.DENSITY}";
                } else {
                    formula = $"(({h1}-{t})*{s}+{b}*{t})*{GBData.DENSITY}";
                }
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
        private GBDataBase GetGBData(string byName) {
            GBDataBase data;
            switch (type) {
            case "TW":
                data = GBData.SearchGBData(GBData.TW, byName);
                break;
            case "TM":
                data = GBData.SearchGBData(GBData.TM, byName);
                break;
            case "TN":
                data = GBData.SearchGBData(GBData.TN, byName);
                break;
            case "T":
            default:
                data = GBData.SearchGBData(GBData.TW, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.TM, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.TN, byName);
                break;
            }
            return data;
        }
        private GBDataBase GetGBData(double[] byParameters) {
            GBDataBase data;
            switch (type) {
            case "TW":
                data = GBData.SearchGBData(GBData.TW, byParameters);
                break;
            case "TM":
                data = GBData.SearchGBData(GBData.TM, byParameters);
                break;
            case "TN":
                data = GBData.SearchGBData(GBData.TN, byParameters);
                break;
            case "T":
            default:
                data = GBData.SearchGBData(GBData.TW, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.TM, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.TN, byParameters);
                break;
            }
            return data;
        }
    }
}
