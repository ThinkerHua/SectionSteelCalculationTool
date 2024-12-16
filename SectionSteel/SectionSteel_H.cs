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
 *  SectionSteel_H.cs: H型钢
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
    /// <para>H型钢。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.H_1"/>: <inheritdoc cref="Pattern_Collection.H_1"/><para></para>
    /// <see cref="Pattern_Collection.H_2"/>: <inheritdoc cref="Pattern_Collection.H_2"/><para></para>
    /// <see cref="Pattern_Collection.H_3"/>: <inheritdoc cref="Pattern_Collection.H_3"/><para></para>
    /// <see cref="Pattern_Collection.H_4"/>: <inheritdoc cref="Pattern_Collection.H_4"/><para></para>
    /// <see cref="Pattern_Collection.H_5"/>: <inheritdoc cref="Pattern_Collection.H_5"/><para></para>
    /// <see cref="Pattern_Collection.H_6"/>: <inheritdoc cref="Pattern_Collection.H_6"/><para></para>
    /// <see cref="Pattern_Collection.H_7"/>: <inheritdoc cref="Pattern_Collection.H_7"/><para></para>
    /// <para>当匹配到H3模式时，在国标截面特性表格中查找，同一型号名下有多项时按以下规则进行匹配：</para>
    /// <list type="number">
    ///     <item>
    ///         <para>匹配<b>非星标型号</b>，例如：</para>
    ///         <para>HW200*200型号名下，有H200*200*8*12和H200*204*12*12两种型号，其中第1种不带星标，第2种带星标。此时匹配第1种。</para>
    ///     </item>
    ///     <item>
    ///         <para>如均为星标型号，则匹配<b>参数与型号名一致的型号</b>，例如：</para>
    ///         <para>HM550*300型号名下，有H544*300*11*15和H550*300*11*18两种型号，均带星标，但第2种参数与型号名一致。此时匹配第2种。</para>
    ///     </item>
    ///     <item>
    ///         <para>如<b>均不符合</b>，则匹配<b>第1项</b>，例如：</para>
    ///         <para>HW500*500型号名下，有H492*465*15*20、H502*465*15*25、H502*470*20*25三种型号，均带星标，且三种参数均与型号名不一致。此时匹配第1种。</para>
    ///     </item>
    /// </list>
    /// </summary>
    public class SectionSteel_H : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private string type;
        private double h1, h2, b1, b2, s, t1, t2;
        private GBDataBase data;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_H() {
        }
        public SectionSteel_H(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            type = string.Empty;
            h1 = h2 = b1 = b2 = s = t1 = t2 = 0;
            data = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.H_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_2);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_4);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_5);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_6);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.H_7);
                if (match.Success) {
                    double.TryParse(match.Groups["h1"].Value, out h1);
                    double.TryParse(match.Groups["h2"].Value, out h2);
                    double.TryParse(match.Groups["b1"].Value, out b1);
                    double.TryParse(match.Groups["b2"].Value, out b2);
                    double.TryParse(match.Groups["s"].Value, out s);
                    double.TryParse(match.Groups["t1"].Value, out t1);
                    double.TryParse(match.Groups["t2"].Value, out t2);

                    if (!(h2 != 0 && h2 != h1
                        || b2 != 0 && b2 != b1
                        || t2 != 0 && t2 != t1))
                        data = GetGBData(new double[] { h1, b1, s, t1 });
                } else {
                    match = Regex.Match(ProfileText, Pattern_Collection.H_3);
                    if (!match.Success)
                        throw new MismatchedProfileTextException();

                    type = match.Groups["TYPE"].Value;
                    string name = match.Groups["H"] + "*" + match.Groups["B"];
                    data = GetGBData(name);
                    if (data == null) {
                        double.TryParse(match.Groups["H"].Value, out double H);
                        double.TryParse(match.Groups["B"].Value, out double B);
                        data = GetGBData(new double[] { H, B });
                    }
                    if (data == null)
                        throw new MismatchedProfileTextException();

                    h1 = data.Parameters[0];
                    b1 = data.Parameters[1];
                    s = data.Parameters[2];
                    t1 = data.Parameters[3];
                }

                if (h2 == 0) h2 = h1;
                if (b2 == 0) b2 = b1;
                if (t2 == 0) t2 = t1;

                h1 *= 0.001; h2 *= 0.001; b1 *= 0.001; b2 *= 0.001; s *= 0.001; t1 *= 0.001; t2 *= 0.001;
            } catch (MismatchedProfileTextException) {
                type = null;
                h1 = h2 = b1 = b2 = s = t1 = t2 = 0;
                data = null;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[1]"/>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc cref="ISectionSteel.GetAreaFormula(FormulaAccuracyEnum, bool)" path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public virtual string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                //byte key = 0b0000;
                //if (h2 != 0 && h2 != h1)
                //    key |= 0b0001;
                //if (b2 != 0 && b2 != b1)
                //    key |= 0b0010;
                //if (exclude_topSurface)
                //    key |= 0b0100;

                //switch (key) {
                //case 0b0000:
                //    formula = $"{h1}*2+{b1}*4";
                //    break;
                //case 0b0100:
                //    formula = $"{h1}*2+{b1}*3";
                //    break;
                //case 0b0010:
                //    formula = $"{h1}*2+{b1}*2+{b2}*2";
                //    break;
                //case 0b0110:
                //    formula = $"{h1}*2+{b1}+{b2}*2";
                //    break;
                //case 0b0001:
                //    formula = $"{h1}+{h2}+{b1}*4";
                //    break;
                //case 0b0101:
                //    formula = $"{h1}+{h2}+{b1}*3";
                //    break;
                //case 0b0011:
                //    formula = $"{h1}+{h2}+{b1}*2+{b2}*2";
                //    break;
                //case 0b0111:
                //    formula = $"{h1}+{h2}+{b1}+{b2}*2";
                //    break;
                //default:
                //    break;
                //}
                if (h2 != h1) {
                    formula = h1 + "+" + h2;
                } else {
                    formula = h1 + "*2";
                }
                if (b2 != b1) {
                    if (exclude_topSurface) {
                        formula += "+" + b1 + "+" + b2 + "*2";
                    } else {
                        formula += "+" + b1 + "*2+" + b2 + "*2";
                    }
                } else {
                    if (exclude_topSurface) {
                        formula += "+" + b1 + "*3";
                    } else {
                        formula += "+" + b1 + "*4";
                    }
                }
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                formula += $"-{s}*2";
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data == null)
                    break;
                formula = $"{data.Area}";
                if (exclude_topSurface)
                    formula += $"-{b1}";
                break;
            default:
                break;
            }

            return formula;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc cref="ISectionSteel.GetWeightFormula(FormulaAccuracyEnum)" path="/param[1]"/>
        /// <para><b>在本类中：ROUGHLY 等效于 PRECISELY</b></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public virtual string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
            case FormulaAccuracyEnum.PRECISELY:
                //byte key = 0b0000;
                //if(h2 != 0 && h2 != h1)
                //    key |= 0b0001;
                //if(b2 != 0 && b2 != b1)
                //    key |= 0b0010;
                //if(t2 != 0 && t2 != t1)
                //    key |= 0b0100;

                //switch(key) {
                //case 0b0000:
                //    formula = $"(({h1}-{t1}*2)*{s}+{b1}*{t1}*2)*{GBData.DENSITY}";
                //    break;
                //case 0b0001:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}*2)*{s}+{b1}*{t1}*2)*{GBData.DENSITY}";
                //    break;
                //case 0b0010:
                //    formula = $"(({h1}-{t1}*2)*{s}+({b1}+{b2})*{t1})*{GBData.DENSITY}";
                //    break;
                //case 0b0011:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}*2)*{s}+({b1}+{b2})*{t1})*{GBData.DENSITY}";
                //    break;
                //case 0b0100:
                //    formula = $"(({h1}-{t1}-{t2})*{s}+{b1}*({t1}+{t2}))*{GBData.DENSITY}";
                //    break;
                //case 0b0101:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}-{t2})*{s}+{b1}*({t1}+{t2}))*{GBData.DENSITY}";
                //    break;
                //case 0b0110:
                //    formula = $"(({h1}-{t1}-{t2})*{s}+{b1}*{t1}+{b2}*{t2})*{GBData.DENSITY}";
                //    break;
                //case 0b0111:
                //    formula = $"(({(h1 + h2) * 0.5}-{t1}-{t2})*{s}+{b1}*{t1}+{b2}*{t2})*{GBData.DENSITY}";
                //    break;
                //default:
                //    break;
                //}
                if (h2 != h1) {
                    formula = "((" + (h1 + h2) / 2;
                } else {
                    formula = "((" + h1;
                }
                if (t2 != t1) {
                    formula += "-" + t1 + "-" + t2;
                } else {
                    formula += "-" + t1 + "*2";
                }
                formula += ")*" + s;
                if (b2 != b1) {
                    if (t2 != t1) {
                        formula += "+" + b1 + "*" + t1 + "+" + b2 + "*" + t2 + ")";
                    } else {
                        formula += "+(" + b1 + "+" + b2 + ")*" + t1 + ")";
                    }
                } else {
                    if (t2 != t1) {
                        formula += "+" + b1 + "*(" + t1 + "+" + t2 + "))";
                    } else {
                        formula += "+" + b1 + "*" + t1 + "*2)";
                    }
                }
                formula += "*" + GBData.DENSITY;
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

        public virtual string GetSiffenerProfileStr(bool truncatedRounding) {
            string stifProfileText = string.Empty;
            if (h1 == 0) return stifProfileText;

            double t, b, l;
            t = s;
            b = ((b1 + b2) * 0.5 - s) * 0.5;
            l = (h1 + h2) * 0.5 - t1 - t2;
            t *= 1000; b *= 1000; l *= 1000;
            if (truncatedRounding) {
                t = Math.Truncate(t);
                b = Math.Truncate(b);
                l = Math.Truncate(l);
            }
            stifProfileText = $"PL{t}*{b}*{l}";

            return stifProfileText;
        }
        private GBDataBase GetGBData(string byName) {
            GBDataBase data;
            switch (type) {
            case "HW":
                data = GBData.SearchGBData(GBData.HW, byName);
                break;
            case "HM":
                data = GBData.SearchGBData(GBData.HM, byName);
                break;
            case "HN":
                data = GBData.SearchGBData(GBData.HN, byName);
                break;
            case "HT":
                data = GBData.SearchGBData(GBData.HT, byName);
                break;
            case "H":
            default:
                data = GBData.SearchGBData(GBData.HW, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HM, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HN, byName);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HT, byName);
                break;
            }
            return data;
        }
        private GBDataBase GetGBData(double[] byParameters) {
            GBDataBase data;
            switch (type) {
            case "HW":
                data = GBData.SearchGBData(GBData.HW, byParameters);
                break;
            case "HM":
                data = GBData.SearchGBData(GBData.HM, byParameters);
                break;
            case "HN":
                data = GBData.SearchGBData(GBData.HN, byParameters);
                break;
            case "HT":
                data = GBData.SearchGBData(GBData.HT, byParameters);
                break;
            case "H":
            default:
                data = GBData.SearchGBData(GBData.HW, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HM, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HN, byParameters);
                if (data == null)
                    data = GBData.SearchGBData(GBData.HT, byParameters);
                break;
            }
            return data;
        }
    }
}
