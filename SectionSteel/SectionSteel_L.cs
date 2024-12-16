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
 *  SectionSteel_L.cs: 角钢
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
    /// <para>角钢。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.L_1"/>: <inheritdoc cref="Pattern_Collection.L_1"/><para></para>
    /// <see cref="Pattern_Collection.L_2"/>: <inheritdoc cref="Pattern_Collection.L_2"/><para></para>
    /// <para>当匹配到L2模式时，在国标截面特性表格中查找，同一型号名下按第一个进行匹配。</para>
    /// </summary>
    public class SectionSteel_L : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h, b, t;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_L() {

        }
        public SectionSteel_L(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.L_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["t"].Value, out t);

                    if (b == 0)
                        b = h;
                    data = GBData.SearchGBData(GBData.L, new double[] { h, b, t });
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.L_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);

                    if (b == 0)
                        b = h;
                    h *= 10; b *= 10;
                    data = GBData.SearchGBData(GBData.L, new double[] { h, b });
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    t = data.Parameters[2];
                }

                h *= 0.001; b *= 0.001; t *= 0.001;
            } catch (MismatchedProfileTextException) {
                h = b = t = 0;
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
            if (h == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                if (b != h) {
                    formula = $"{h}*2+{b}";
                    if (!exclude_topSurface)
                        formula += $"*2";
                } else {
                    formula = $"{h}";
                    if (exclude_topSurface)
                        formula += $"*3";
                    else
                        formula += $"*4";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    return formula;

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
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = this.t;
            b = this.b - this.t;
            l = h - this.t;
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
        /// <para><b>在本类中：ROUGHLY, PRECISELY 均等效于 GBDATA</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;

            if (data != null)
                formula = $"{data.Weight}";

            return formula;
        }
    }
}
