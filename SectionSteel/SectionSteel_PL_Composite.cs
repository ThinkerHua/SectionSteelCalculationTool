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
 *  SectionSteel_PL_Composite.cs: 复合板件
 *  written by Huang YongXing - thinkerhua@hotmail.com
 *==============================================================================*/
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SectionSteel {
    /// <summary>
    /// <para>复合板件。在以下模式中尝试匹配：</para>
    /// <see cref="Pattern_Collection.PL_CMP_1"/>: <inheritdoc cref="Pattern_Collection.PL_CMP_1"/>
    /// </summary>
    public class SectionSteel_PL_Composite : SectionSteelBase {
        protected struct SubPlate {
            public double num;//数量
            public SectionSteelBase plate;//子板件
        }
        private readonly List<SubPlate> subPlates = new List<SubPlate>();

        public override GBData[] GBDataSet => throw new System.NotImplementedException();

        public SectionSteel_PL_Composite() { }
        public SectionSteel_PL_Composite(string profileText) {
            this.ProfileText = profileText;
        }
        protected override void SetFieldsValue(SectionSteelBase sender, ProfileTextChangingEventArgs e) {
            var tmp = subPlates.ToArray();
            subPlates.Clear();
            try {
                if (string.IsNullOrEmpty(e.NewText))
                    throw new MismatchedProfileTextException(e.NewText);

                Match match = Regex.Match(e.NewText, Pattern_Collection.PL_CMP_1);
                if (!match.Success)
                    throw new MismatchedProfileTextException(e.NewText);

                var tempText = e.NewText.Replace("-", "+-");
                var profileTexts = tempText.Split('+');

                SubPlate subplate;
                foreach (var profileText in profileTexts) {
                    match = Regex.Match(profileText, Pattern_Collection.PL_CMP_SUB_PL);
                    if (match.Success) {
                        subplate.plate = new SectionSteel_PL(match.Groups["main"].Value);
                        _ = double.TryParse(match.Groups["num"].Value, out subplate.num);
                        if (subplate.num == 0) {
                            subplate.num = match.Groups["num"].Value == "-" ? -1 : 1;
                        }
                        subPlates.Add(subplate);

                        continue;
                    }
                    match = Regex.Match(profileText, Pattern_Collection.PL_CMP_SUB_PLT);
                    if (match.Success) {
                        subplate.plate = new SectionSteel_PL_Triangle(match.Groups["main"].Value);
                        _ = double.TryParse(match.Groups["num"].Value, out subplate.num);
                        if (subplate.num == 0) {
                            subplate.num = match.Groups["num"].Value == "-" ? -1 : 1;
                        }
                        subPlates.Add(subplate);

                        continue;
                    }
                    match = Regex.Match(profileText, Pattern_Collection.PL_CMP_SUB_PLO);
                    if (match.Success) {
                        subplate.plate = new SectionSteel_PL_Circular(match.Groups["main"].Value);
                        _ = double.TryParse(match.Groups["num"].Value, out subplate.num);
                        if (subplate.num == 0) {
                            subplate.num = match.Groups["num"].Value == "-" ? -1 : 1;
                        }
                        subPlates.Add(subplate);

                        continue;
                    }

                    throw new MismatchedProfileTextException(profileText);
                }

            } catch (MismatchedProfileTextException) {
                subPlates.Clear();
                subPlates.AddRange(tmp);
                throw;
            }
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="accuracy">
        /// <inheritdoc path="/param[1]"/>
        /// <para>具体说明参见：</para>
        /// <para><see cref="SectionSteel_PL.GetAreaFormula(FormulaAccuracyEnum, bool)"/></para>
        /// <para><see cref="SectionSteel_PL_Triangle.GetAreaFormula(FormulaAccuracyEnum, bool)"/></para>
        /// <para><see cref="SectionSteel_PL_Circular.GetAreaFormula(FormulaAccuracyEnum, bool)"/></para>
        /// </param>
        /// <param name="exclude_topSurface">
        /// <inheritdoc path="/param[2]"/>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetAreaFormula(FormulaAccuracyEnum accuracy, bool exclude_topSurface) {
            string formula = string.Empty;
            if (subPlates.Count == 0 || accuracy == FormulaAccuracyEnum.GBDATA) return formula;

            foreach (var subplate in subPlates) {
                if (subplate.num == -1)
                    formula += $"-{subplate.plate.GetAreaFormula(accuracy, exclude_topSurface)}";
                else if (subplate.num < 0)
                    formula += $"{subplate.num}*{subplate.plate.GetAreaFormula(accuracy, exclude_topSurface)}";
                else if (subplate.num == 1)
                    formula += $"+{subplate.plate.GetAreaFormula(accuracy, exclude_topSurface)}";
                else
                    formula += $"+{subplate.num}*{subplate.plate.GetAreaFormula(accuracy, exclude_topSurface)}";
            }
            if (formula[0] == '+') formula = formula.Remove(0, 1);

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
        /// <para>具体说明参见：</para>
        /// <para><see cref="SectionSteel_PL.GetWeightFormula(FormulaAccuracyEnum)"/></para>
        /// <para><see cref="SectionSteel_PL_Triangle.GetWeightFormula(FormulaAccuracyEnum)"/></para>
        /// <para><see cref="SectionSteel_PL_Circular.GetWeightFormula(FormulaAccuracyEnum)"/></para>
        /// </param>
        /// <returns><inheritdoc/></returns>
        public override string GetWeightFormula(FormulaAccuracyEnum accuracy) {
            string formula = string.Empty;
            if (subPlates.Count == 0 || accuracy == FormulaAccuracyEnum.GBDATA) return formula;

            //foreach(var subplate in subPlates) {
            //    if(subplate.num < 0)
            //        formula += $"{subplate.num}*{subplate.plate.GetWeightFormula(accuracy)}";
            //    else if(subplate.num == 1)
            //        formula += $"{subplate.plate.GetWeightFormula(accuracy)}";
            //    else
            //        formula += $"+{subplate.num}*{subplate.plate.GetWeightFormula(accuracy)}";
            //}
            //if(formula.IndexOf('+') == 0) formula = formula.Remove(0, 1);

            if (subPlates.Count == 1) goto NoIdenticalItems;

            List<string> weights = new List<string>();
            foreach (var subplate in subPlates) {
                weights.Add(subplate.plate.GetWeightFormula(accuracy));
            }

            string pattern1 = @"\*\d+\.?\d*\*" + DENSITY + "$";
            string pattern2 = @"\*" + DENSITY + "$";
            Match match = Regex.Match(weights[0], pattern1);
            string value = match.Value;
            int i;
            for (i = 1; i < weights.Count; i++) {
                if (!weights[i].EndsWith(value))
                    break;
            }
            if (i < weights.Count) {
                match = Regex.Match(weights[0], pattern2);
                value = match.Value;
                for (i = 1; i < weights.Count; i++) {
                    if (!weights[i].EndsWith(value))
                        break;
                }
            }
            //无同类项返回
            if (i < weights.Count) {
                goto NoIdenticalItems;
            }

            //合并同类项返回
            for (i = 0; i < weights.Count; i++) {
                var item = weights[i];
                weights[i] = item.Remove(item.Length - value.Length, value.Length);
            }
            for (i = 0; i < subPlates.Count; i++) {
                if (subPlates[i].num == -1)
                    formula += $"-{weights[i]}";
                else if (subPlates[i].num < 0)
                    formula += $"{subPlates[i].num}*{weights[i]}";
                else if (subPlates[i].num == 1)
                    formula += $"+{weights[i]}";
                else
                    formula += $"+{subPlates[i].num}*{weights[i]}";
            }
            if (formula[0] == '+') formula = formula.Remove(0, 1);
            formula = $"({formula}){value}";

            return formula;

        NoIdenticalItems:
            foreach (var subplate in subPlates) {
                if (subplate.num == -1)
                    formula += $"-{subplate.plate.GetWeightFormula(accuracy)}";
                if (subplate.num < 0)
                    formula += $"{subplate.num}*{subplate.plate.GetWeightFormula(accuracy)}";
                else if (subplate.num == 1)
                    formula += $"+{subplate.plate.GetWeightFormula(accuracy)}";
                else
                    formula += $"+{subplate.num}*{subplate.plate.GetWeightFormula(accuracy)}";
            }
            if (formula[0] == '+') formula = formula.Remove(0, 1);

            return formula;
        }
    }
}
