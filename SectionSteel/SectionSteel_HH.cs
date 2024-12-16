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
 *  SectionSteel_HH.cs: 十字交叉焊接H型钢
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
    /// <para>十字交叉焊接H型钢。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.HH_1"/>: <inheritdoc cref="Pattern_Collection.HH_1"/><para></para>
    /// <see cref="Pattern_Collection.HH_2"/>: <inheritdoc cref="Pattern_Collection.HH_2"/><para></para>
    /// </summary>
    public class SectionSteel_HH : SectionSteelBase, ISectionSteel {
        private string _profileText;
        private double h1, h2, b1, b2, s1, s2, t1, t2;
        private GBDataBase data1, data2;
        public string ProfileText {
            get => _profileText;
            set {
                _profileText = value;
                SetFieldsValue();
            }
        }
        public PIStyleEnum PIStyle { get; set; }
        public SectionSteel_HH() {

        }
        public SectionSteel_HH(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue() {
            h1 = h2 = b1 = b2 = s1 = s2 = t1 = t2 = 0;
            data1 = data2 = null;
            try {
                if (string.IsNullOrEmpty(ProfileText))
                    throw new MismatchedProfileTextException();

                Match match = Regex.Match(ProfileText, Pattern_Collection.HH_1);
                if (!match.Success)
                    match = Regex.Match(ProfileText, Pattern_Collection.HH_2);
                if (!match.Success)
                    throw new MismatchedProfileTextException();
                double.TryParse(match.Groups["h1"].Value, out h1);
                double.TryParse(match.Groups["b1"].Value, out b1);
                double.TryParse(match.Groups["s1"].Value, out s1);
                double.TryParse(match.Groups["t1"].Value, out t1);
                double.TryParse(match.Groups["h2"].Value, out h2);
                double.TryParse(match.Groups["b2"].Value, out b2);
                double.TryParse(match.Groups["s2"].Value, out s2);
                double.TryParse(match.Groups["t2"].Value, out t2);

                double[] parameters1 = { h1, b1, s1, t1 };
                data1 = GetGBData(parameters1);
                if (h2 == 0 && b2 == 0 && s2 == 0 && t2 == 0) {
                    h2 = h1; b2 = b1; s2 = s1; t2 = t1;
                }
                if (h2 == h1 && b2 == b1 && s2 == s1 && t2 == t1)
                    data2 = data1;
                else {
                    double[] parameters2 = { h2, b2, s2, t2 };
                    data2 = GetGBData(parameters2);
                }

                h1 *= 0.001; b1 *= 0.001; s1 *= 0.001; t1 *= 0.001;
                h2 *= 0.001; b2 *= 0.001; s2 *= 0.001; t2 *= 0.001;
            } catch (MismatchedProfileTextException) {
                h1 = h2 = b1 = b2 = s1 = s2 = t1 = t2 = 0;
                //data1 = data2 = null;
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
        public string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (h1 == 0) return formula;

            switch (accuracy) {
            case FormulaAccuracyEnum.ROUGHLY:
                if (h2 != h1) {
                    formula = h1 + "*2+" + h2 + "*2+";
                } else {
                    formula = h1 + "*4+";
                }
                if (b2 != b1) {
                    if (exclude_topSurface) {
                        formula += b1 + "*3+" + b2 + "*4";
                    } else {
                        formula += "(" + b1 + b2 + ")*4";
                    }
                } else {
                    if (exclude_topSurface) {
                        formula += b1 + "*7";
                    } else {
                        formula += b1 + "*8";
                    }
                }
                break;
            case FormulaAccuracyEnum.PRECISELY:
                formula = this.GetAreaFormula(FormulaAccuracyEnum.ROUGHLY, exclude_topSurface);
                if (s2 != s1) {
                    formula += "-" + s1 + "*4-" + s2 + "*4";
                } else {
                    formula += "-" + s1 + "*8";
                }
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data2 == null || data1 == null)
                    break;
                if (data2.Equals(data1)) {
                    formula = data1.Area + "*2";
                } else {
                    formula = data1.Area + "+" + data2.Area;
                }
                if (s2 != s1) {
                    formula += "-" + s1 + "*2-" + s2 + "*2";
                } else {
                    formula += "-" + s1 + "*4";
                }
                if (exclude_topSurface)
                    formula += "-" + b1;
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
            t = Math.Max(s1, s2);
            b = (h2 - t2 * 2 - s1) * 0.5;
            l = (h1 - t1 * 2 - s2) * 0.5;
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
                if (h2 != h1) {
                    if (s2 != s1) {
                        formula = "((" + h1 + "-" + t1 + "*2)*" + s1 + "+(" + h2 + "-" + t2 + "*2)*" + s2;
                    } else {
                        if (t2 != t1) {
                            formula = "((" + h1 + "-" + t1 + "*2+" + h2 + "-" + t2 + "*2)*" + s1;
                        } else {
                            formula = "((" + h1 + "+" + h2 + "-" + t1 + "*4)*" + s1;
                        }
                    }
                } else {
                    formula = "((" + h1;
                    if (t2 != t1) {
                        formula += "-" + t1 + "-" + t2 + ")*";
                        if (s2 != s1) {
                            formula += "(" + s1 + "+" + s2 + ")";
                        } else {
                            formula += s1 + "*2";
                        }
                    } else {
                        formula += "-" + t1 + "*2)*";
                        if (s2 != s1) {
                            formula += "(" + s1 + "+" + s2 + ")";
                        } else {
                            formula += s1 + "*2";
                        }
                    }
                }
                if (s2 != s1) {
                    formula += "-" + s1 + "*" + s2;
                } else {
                    formula += "-" + s1 + "*" + s1;
                }
                if (b2 != b1) {
                    if (t2 != t1) {
                        formula += "+(" + b1 + "*" + t1 + "+" + b2 + "*" + t2 + ")*2)";
                    } else {
                        formula += "+(" + b1 + "+" + b2 + ")*" + t1 + "*2)";
                    }
                } else {
                    if (t2 != t1) {
                        formula += "+" + b1 + "*(" + t1 + "+" + t2 + ")*2)";
                    } else {
                        formula += "+" + b1 + "*" + t1 + "*4)";
                    }
                }
                formula += "*" + GBData.DENSITY;
                break;
            case FormulaAccuracyEnum.GBDATA:
                if (data2 == null || data1 == null)
                    break;
                if (data2.Equals(data1)) {
                    formula = data1.Weight + "*2";
                } else {
                    formula = data1.Weight + "+" + data2.Weight;
                }
                if (s2 != s1) {
                    formula += "-" + s1 + "*" + s2 + "*" + GBData.DENSITY;
                } else {
                    formula += "-" + s1 + "*" + s1 + "*" + GBData.DENSITY;
                }
                break;
            default:
                break;
            }
            return formula;
        }
        private GBDataBase GetGBData(double[] byParameters) {
            GBDataBase data;
            data = GBData.SearchGBData(GBData.HW, byParameters);
            if (data == null)
                data = GBData.SearchGBData(GBData.HM, byParameters);
            if (data == null)
                data = GBData.SearchGBData(GBData.HN, byParameters);
            if (data == null)
                data = GBData.SearchGBData(GBData.HT, byParameters);
            return data;
        }
    }
}
