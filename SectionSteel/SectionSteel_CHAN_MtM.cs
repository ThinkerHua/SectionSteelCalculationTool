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
 *  SectionSteel_CHAN_MtM.cs: 槽钢，口对口双拼
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
    /// <para>槽钢，口对口双拼。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.CHAN_MtM_1"/>: <inheritdoc cref="Pattern_Collection.CHAN_MtM_1"/><para></para>
    /// <see cref="Pattern_Collection.CHAN_MtM_2"/>: <inheritdoc cref="Pattern_Collection.CHAN_MtM_2"/><para></para>
    /// <para>匹配到两种模式时，均在国标截面特性表格中查找。</para>
    /// <para>CHAN_MtM_2模式下，当型号大于等于14号且无后缀时，按后缀为"a"处理。</para>
    /// </summary>
    public class SectionSteel_CHAN_MtM : SectionSteelBase, ISectionSteel {
        private string _profileText;
        double h, b, s, t;
        GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_CHAN_MtM() {

        }
        public SectionSteel_CHAN_MtM(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h = b = s = t = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.CHAN_MtM_1);
                if (match.Success) {
                    double.TryParse(match.Groups["h"].Value, out h);
                    double.TryParse(match.Groups["b"].Value, out b);
                    double.TryParse(match.Groups["s"].Value, out s);

                    data = GBData.SearchGBData(GBData.CHAN, new double[] { h, b, s });
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    t = data.Parameters[3];
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.CHAN_MtM_2);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    string name, suffix;
                    double.TryParse(match.Groups["CODE"].Value, out double code);
                    suffix = match.Groups["SUFFIX"].Value;
                    name = match.Groups["NAME"].Value;

                    if (code >= 14 && string.IsNullOrEmpty(suffix))
                        name += "a";

                    data = GBData.SearchGBData(GBData.CHAN, name);
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h = data.Parameters[0];
                    b = data.Parameters[1];
                    s = data.Parameters[2];
                    t = data.Parameters[3];
                }

                h *= 0.001; b *= 0.001; s *= 0.001; t *= 0.001;
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
        /// <para><b>在本类中：PRECISELY, GBDATA 均等效于 ROUGHLY</b></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h == 0) return formula;

            formula = $"{h}*2+{b}";
            if (exclude_topSurface)
                formula += "*2";
            else
                formula += "*4";

            return formula;
        }

        public string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = (this.b - s) * 2;
            l = h - this.t * 2;
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
                formula = $"{data.Weight}*2";

            return formula;
        }
    }
}
